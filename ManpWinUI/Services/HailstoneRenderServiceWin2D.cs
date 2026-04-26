using ManpWinUI.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.UI;

namespace ManpWinUI.Services;

/// <summary>
/// Win2D-based Hailstone sequence renderer using graphics abstraction layer.
/// Renders everything (graphics + text) directly on bitmap - no Canvas overlays needed!
/// </summary>
public class HailstoneRenderServiceWin2D
{
    private const double PaddingPercent = 0.15; // 15% padding around content

    // Default fixed viewport bounds (standard view for Hailstone sequences)
    private const int DefaultMinX = -40;
    private const int DefaultMaxX = 40;
    private const int DefaultMinY = -30;
    private const int DefaultMaxY = 30;

    /// <summary>
    /// Renders a Hailstone sequence using Win2D graphics abstraction.
    /// All elements (grid, trajectory, points, text labels) rendered directly on bitmap.
    /// </summary>
    /// <param name="result">The calculated Hailstone sequence.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="showAxes">Whether to draw coordinate axes and grid lines.</param>
    /// <param name="showPoints">Whether to draw dots at trajectory points.</param>
    /// <param name="showLabels">Whether to draw point labels directly on bitmap.</param>
    /// <param name="useFixedViewport">If true, uses fixed viewport bounds; if false, auto-scales to data.</param>
    /// <returns>A HailstoneRenderResult containing the bitmap and transform parameters.</returns>
    public async Task<HailstoneRenderResult> RenderSequenceAsync(
        HailstoneResult result,
        int width,
        int height,
        bool showAxes,
        bool showPoints,
        bool showLabels,
        bool useFixedViewport = false)
    {
        var stopwatch = Stopwatch.StartNew();

        Debug.WriteLine($"=== Win2D Hailstone Render ===");
        Debug.WriteLine($"Sequence points: {result.Sequence.Count}");
        Debug.WriteLine($"Toggles: Axes={showAxes}, Points={showPoints}, Labels={showLabels}");

        // Determine bounds
        int minX, maxX, minY, maxY;
        if (useFixedViewport)
        {
            minX = DefaultMinX;
            maxX = DefaultMaxX;
            minY = DefaultMinY;
            maxY = DefaultMaxY;
            Debug.WriteLine($"Using FIXED viewport: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
        }
        else
        {
            minX = result.MinX;
            maxX = result.MaxX;
            minY = result.MinY;
            maxY = result.MaxY;
            Debug.WriteLine($"Using AUTO-SCALE: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
        }

        // Calculate world-to-screen transform
        var (scaleX, scaleY, offsetX, offsetY) = CalculateTransform(
            minX, maxX, minY, maxY, width, height, useFixedViewport);

        Debug.WriteLine($"Transform: scaleX={scaleX:F2}, scaleY={scaleY:F2}, offsetX={offsetX:F2}, offsetY={offsetY:F2}");

        // Render on background thread
        WriteableBitmap? bitmap = await Task.Run(() =>
        {
            try
            {
                // Create Win2D renderer
                using var renderer = GraphicsRendererFactory.Create(width, height, GraphicsBackend.Win2D);

                // 1. Clear background
                renderer.Clear(Colors.Black);

                // 2. Set up coordinate transform (matches GDI+ approach in NumericalVisualizations)
                // This allows drawing in world coordinates while Win2D handles screen conversion
                SetupCoordinateTransform(renderer, width, height, minX, maxX, minY, maxY, 
                    scaleX, scaleY, offsetX, offsetY);

                // 3. Draw grid and axes (if enabled) - now in world coordinates!
                if (showAxes)
                {
                    DrawGridAndAxesWithTransform(renderer, minX, maxX, minY, maxY);
                }

                // 4. Draw sequence trajectory path - now in world coordinates!
                DrawSequencePathWithTransform(renderer, result.Sequence);

                // 5. Draw points at each position (if enabled) - now in world coordinates!
                if (showPoints)
                {
                    DrawPointsWithTransform(renderer, result.Sequence);
                }

                // 6. Reset transform for screen-space rendering (labels and info)
                renderer.ResetTransform();

                // 7. Draw point labels directly on bitmap (if enabled) - screen coordinates
                if (showLabels)
                {
                    DrawPointLabels(renderer, result.Sequence, scaleX, scaleY, offsetX, offsetY);
                }

                // 8. Draw info text in corner - screen coordinates
                DrawInfoText(renderer, result);

                // Convert to WriteableBitmap
                return renderer.ToWriteableBitmap();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in Win2D render: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
                return null;
            }
        });

        stopwatch.Stop();
        Debug.WriteLine($"=== Win2D Hailstone Render Complete ({stopwatch.ElapsedMilliseconds}ms) ===");

        if (bitmap == null)
        {
            throw new InvalidOperationException("Win2D rendering failed");
        }

        return new HailstoneRenderResult
        {
            Bitmap = bitmap,
            SequenceResult = result,
            ScaleX = scaleX,
            ScaleY = scaleY,
            OffsetX = offsetX,
            OffsetY = offsetY
        };
    }

    /// <summary>
    /// Draws grid lines and coordinate axes with tick marks.
    /// </summary>
    private void DrawGridAndAxes(IGraphicsRenderer renderer, int width, int height,
        int minX, int maxX, int minY, int maxY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Colors (very subtle gray grid, slightly brighter axes - matches NumVis aesthetic)
        var gridColor = Color.FromArgb(40, 50, 50, 50);      // More subtle (was 80)
        var axesColor = Color.FromArgb(150, 100, 100, 100);  // Distinguishable

        int range = Math.Max(maxX - minX, maxY - minY);
        int tickSpacing = CalculateTickSpacing(range);

        Debug.WriteLine($"Grid: tickSpacing={tickSpacing}, range={range}");

        // Draw vertical grid lines
        int startX = (minX / tickSpacing) * tickSpacing;
        if (startX > minX) startX -= tickSpacing;
        int endX = (maxX / tickSpacing) * tickSpacing;
        if (endX < maxX) endX += tickSpacing;

        for (int x = startX; x <= endX; x += tickSpacing)
        {
            var (screenX, _) = WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
            var color = (x == 0) ? axesColor : gridColor;
            renderer.DrawLine(screenX, 0f, screenX, (float)(height - 1), color, 1.0f);
        }

        // Draw horizontal grid lines
        int startY = (minY / tickSpacing) * tickSpacing;
        if (startY > minY) startY -= tickSpacing;
        int endY = (maxY / tickSpacing) * tickSpacing;
        if (endY < maxY) endY += tickSpacing;

        for (int y = startY; y <= endY; y += tickSpacing)
        {
            var (_, screenY) = WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
            var color = (y == 0) ? axesColor : gridColor;
            renderer.DrawLine(0f, screenY, (float)(width - 1), screenY, color, 1.0f);
        }

        // Draw tick marks on axes
        DrawAxisTickMarks(renderer, width, height, minX, maxX, minY, maxY, 
            tickSpacing, scaleX, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Draws tick marks on coordinate axes with numeric labels.
    /// </summary>
    private void DrawAxisTickMarks(IGraphicsRenderer renderer, int width, int height,
        int minX, int maxX, int minY, int maxY, int tickSpacing,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int tickLength = 5;
        var tickColor = Color.FromArgb(180, 120, 120, 120);

        // Calculate label spacing (show fewer labels for larger ranges)
        int range = Math.Max(maxX - minX, maxY - minY);
        int labelSpacing = tickSpacing;
        if (range > 100) labelSpacing = tickSpacing * 5;
        else if (range > 50) labelSpacing = tickSpacing * 2;

        // X-axis tick marks and labels (at y=0)
        var (_, axisY) = WorldToScreen(0, 0, scaleX, scaleY, offsetX, offsetY);
        if (axisY >= 0 && axisY < height)
        {
            int startX = (minX / tickSpacing) * tickSpacing;
            if (startX > minX) startX -= tickSpacing;
            int endX = (maxX / tickSpacing) * tickSpacing;
            if (endX < maxX) endX += tickSpacing;

            for (int x = startX; x <= endX; x += tickSpacing)
            {
                if (x == 0) continue; // Skip origin

                var (screenX, _) = WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
                if (screenX >= 0 && screenX < width)
                {
                    // Draw tick mark (cast to int for pixel operations)
                    for (int dy = -tickLength; dy <= tickLength; dy++)
                    {
                        int y = (int)axisY + dy;
                        if (y >= 0 && y < height)
                        {
                            renderer.SetPixel((int)screenX, y, tickColor);
                        }
                    }

                    // Draw label (at labelSpacing intervals)
                    if (x % labelSpacing == 0)
                    {
                        renderer.DrawText(x.ToString(), screenX - 5, axisY + 6, 
                            tickColor, 9f, "Arial", false);
                    }
                }
            }
        }

        // Y-axis tick marks and labels (at x=0)
        var (axisX, _) = WorldToScreen(0, 0, scaleX, scaleY, offsetX, offsetY);
        if (axisX >= 0 && axisX < width)
        {
            int startY = (minY / tickSpacing) * tickSpacing;
            if (startY > minY) startY -= tickSpacing;
            int endY = (maxY / tickSpacing) * tickSpacing;
            if (endY < maxY) endY += tickSpacing;

            for (int y = startY; y <= endY; y += tickSpacing)
            {
                if (y == 0) continue; // Skip origin

                var (_, screenY) = WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
                if (screenY >= 0 && screenY < height)
                {
                    // Draw tick mark (cast to int for pixel operations)
                    for (int dx = -tickLength; dx <= tickLength; dx++)
                    {
                        int x = (int)axisX + dx;
                        if (x >= 0 && x < width)
                        {
                            renderer.SetPixel(x, (int)screenY, tickColor);
                        }
                    }

                    // Draw label (at labelSpacing intervals)
                    if (y % labelSpacing == 0)
                    {
                        renderer.DrawText(y.ToString(), axisX + 6, screenY - 4, 
                            tickColor, 9f, "Arial", false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws the sequence trajectory path with colored line segments.
    /// </summary>
    private void DrawSequencePath(IGraphicsRenderer renderer, List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        if (sequence.Count < 2) return;

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var p1 = sequence[i];
            var p2 = sequence[i + 1];

            var (x1, y1) = WorldToScreen(p1.X, p1.Y, scaleX, scaleY, offsetX, offsetY);
            var (x2, y2) = WorldToScreen(p2.X, p2.Y, scaleX, scaleY, offsetX, offsetY);

            if (p1.IsInCycle)
            {
                // Magenta for cycle - thicker line with smooth anti-aliasing (2.5x matches NumVis)
                renderer.DrawLine(x1, y1, x2, y2, Colors.Magenta, 2.5f);
            }
            else
            {
                // Spectrum color from point data - smooth anti-aliased lines
                var color = Color.FromArgb(255, p2.Color.R, p2.Color.G, p2.Color.B);
                renderer.DrawLine(x1, y1, x2, y2, color, 1.2f);
            }
        }
    }

    /// <summary>
    /// Draws dots at each sequence point position.
    /// </summary>
    private void DrawPoints(IGraphicsRenderer renderer, List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int cycleStartIndex = sequence.FindIndex(p => p.IsInCycle);

        foreach (var point in sequence)
        {
            var (screenX, screenY) = WorldToScreen(point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

            if (point.Step == 0)
            {
                // Green square for start
                renderer.DrawRectangle(screenX - 2, screenY - 2, 4, 4, Colors.Green);
            }
            else if (point.IsInCycle && sequence.IndexOf(point) == cycleStartIndex)
            {
                // Yellow diamond for cycle start
                renderer.DrawCircle(screenX, screenY, 3, Colors.Yellow);
            }
            else if (point.IsInCycle)
            {
                // Magenta circles for cycle points
                renderer.DrawCircle(screenX, screenY, 3, Colors.Magenta);
            }
            else
            {
                // Small colored dots for regular points
                var color = Color.FromArgb(255, point.Color.R, point.Color.G, point.Color.B);
                renderer.DrawCircle(screenX, screenY, 2, color);
            }
        }
    }

    /// <summary>
    /// Draws text labels at each point showing (Step, X, Y).
    /// NOW RENDERED DIRECTLY ON BITMAP - no Canvas overlay needed!
    /// Uses 8pt Arial font, cyan for normal points, magenta for cycle points.
    /// NumVis uses white with semi-transparent background; we use cyan for better visibility.
    /// </summary>
    private void DrawPointLabels(IGraphicsRenderer renderer, List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        foreach (var point in sequence)
        {
            var (screenX, screenY) = WorldToScreen(point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

            string labelText = $"({point.Step},{point.X},{point.Y})";
            var color = point.IsInCycle ? Colors.Magenta : Colors.Cyan;

            // Draw label near point (offset 8px right, 2px down - matches NumVis positioning)
            renderer.DrawText(labelText, screenX + 8, screenY + 2, color, 8f, "Arial", false);
        }
    }

    /// <summary>
    /// Draws info text in top-left corner.
    /// NOW RENDERED DIRECTLY ON BITMAP - no Canvas overlay needed!
    /// </summary>
    private void DrawInfoText(IGraphicsRenderer renderer, HailstoneResult result)
    {
        // Build info text
        string infoText = "Hailstone Sequence (N,X,Y)\n";

        if (result.Sequence.Count > 0)
        {
            var startPoint = result.Sequence[0];
            infoText += $"Starting point: (0, {startPoint.X}, {startPoint.Y})\n";
        }

        infoText += $"Total points: {result.Sequence.Count}";

        // Determine color - use yellow for base info, magenta specifically for cycle details (matches NumVis)
        var textColor = Colors.Yellow;

        if (result.HasCycle && result.CycleStartIndex >= 0)
        {
            var cycleStartPoint = result.Sequence[result.CycleStartIndex];
            var lastPoint = result.Sequence[result.Sequence.Count - 1];

            int nextStep = lastPoint.Step + 1;
            int nextX = CalculateNextX(lastPoint.X, lastPoint.Y);
            int nextY = CalculateNextY(lastPoint.X, lastPoint.Y);

            infoText += $"\nCycle Detected: Point ({nextStep}, {nextX}, {nextY})";
            infoText += $"\nDuplicate of: ({cycleStartPoint.Step}, {cycleStartPoint.X}, {cycleStartPoint.Y})";
            infoText += $"\nCycle length: {result.CycleLength}";

            // Keep yellow for consistency with NumVis (magenta is for cycle lines, not text)
            // textColor remains yellow
        }

        // Draw text in top-left corner with 10px padding (14pt bold for header - matches NumVis)
        renderer.DrawText(infoText, 10, 10, textColor, 14f, "Arial", true);
    }

    // Helper methods

    /// <summary>
    /// Sets up Win2D coordinate transform to match GDI+ approach from NumericalVisualizations.
    /// Allows drawing in world/mathematical coordinates while Win2D handles screen conversion.
    /// Transform sequence matches GDI+: Translate(center) -> Scale -> Translate(-dataCenter)
    /// </summary>
    private void SetupCoordinateTransform(IGraphicsRenderer renderer, int width, int height,
        int minX, int maxX, int minY, int maxY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Calculate data center point in world coordinates
        double centerX = (minX + maxX) / 2.0;
        double centerY = (minY + maxY) / 2.0;

        // Calculate pixels per unit (for display purposes)
        double viewRangeX = maxX - minX + 2 * (maxX - minX) * PaddingPercent;
        double viewRangeY = maxY - minY + 2 * (maxY - minY) * PaddingPercent;
        float pixelsPerUnitX = (float)(width / viewRangeX);
        float pixelsPerUnitY = (float)(height / viewRangeY);

        // Build transformation matrix (applied right-to-left):
        // 1. Translate to center data at origin: T(-centerX, -centerY)
        // 2. Scale and flip Y: S(pixelsPerUnitX, -pixelsPerUnitY) 
        // 3. Translate to screen center: T(width/2, height/2)
        //
        // Matrix layout: [ m11 m12 ]  = [ scaleX      0         ]
        //                [ m21 m22 ]    [ 0           scaleY    ]
        //                [ m31 m32 ]    [ translateX  translateY]

        float m11 = (float)scaleX;           // Scale X
        float m12 = 0;                        // No skew
        float m21 = 0;                        // No skew
        float m22 = (float)scaleY;           // Scale Y (negative for Y-flip)
        float m31 = (float)offsetX;          // Translate X
        float m32 = (float)offsetY;          // Translate Y

        renderer.SetTransform(m11, m12, m21, m22, m31, m32);

        Debug.WriteLine($"Transform matrix: [{m11:F2}, {m12}, {m21}, {m22:F2}, {m31:F2}, {m32:F2}]");
    }

    /// <summary>
    /// Draws grid and axes using world coordinates (transform handles conversion).
    /// Simplified version without manual WorldToScreen conversion.
    /// </summary>
    private void DrawGridAndAxesWithTransform(IGraphicsRenderer renderer,
        int minX, int maxX, int minY, int maxY)
    {
        var gridColor = Color.FromArgb(40, 50, 50, 50);
        var axesColor = Color.FromArgb(150, 100, 100, 100);

        int range = Math.Max(maxX - minX, maxY - minY);
        int tickSpacing = CalculateTickSpacing(range);

        // Draw vertical grid lines in world coordinates
        int startX = (minX / tickSpacing) * tickSpacing;
        if (startX > minX) startX -= tickSpacing;
        int endX = (maxX / tickSpacing) * tickSpacing;
        if (endX < maxX) endX += tickSpacing;

        // Extend grid lines beyond visible range for better coverage
        float yMin = minY - range * 0.5f;
        float yMax = maxY + range * 0.5f;

        for (int x = startX; x <= endX; x += tickSpacing)
        {
            var color = (x == 0) ? axesColor : gridColor;
            // Cast to float to invoke float overload for smooth rendering
            renderer.DrawLine((float)x, yMin, (float)x, yMax, color, 1.0f);
        }

        // Draw horizontal grid lines in world coordinates
        int startY = (minY / tickSpacing) * tickSpacing;
        if (startY > minY) startY -= tickSpacing;
        int endY = (maxY / tickSpacing) * tickSpacing;
        if (endY < maxY) endY += tickSpacing;

        float xMin = minX - range * 0.5f;
        float xMax = maxX + range * 0.5f;

        for (int y = startY; y <= endY; y += tickSpacing)
        {
            var color = (y == 0) ? axesColor : gridColor;
            // Cast to float to invoke float overload for smooth rendering
            renderer.DrawLine(xMin, (float)y, xMax, (float)y, color, 1.0f);
        }
    }

    /// <summary>
    /// Draws sequence trajectory path using world coordinates (transform handles conversion).
    /// This is the key improvement - drawing directly in mathematical coordinates!
    /// Cast integers to float to ensure float overload is called for smooth rendering.
    /// </summary>
    private void DrawSequencePathWithTransform(IGraphicsRenderer renderer, List<HailstonePoint> sequence)
    {
        if (sequence.Count < 2) return;

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var p1 = sequence[i];
            var p2 = sequence[i + 1];

            if (p1.IsInCycle)
            {
                // Magenta for cycle - thicker line (2.5x matches NumVis)
                // Cast to float to invoke float overload for smooth anti-aliasing
                renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, Colors.Magenta, 2.5f);
            }
            else
            {
                // Spectrum color from point data
                var color = Color.FromArgb(255, p2.Color.R, p2.Color.G, p2.Color.B);
                // Cast to float to invoke float overload for smooth anti-aliasing
                renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, color, 1.2f);
            }
        }
    }

    /// <summary>
    /// Draws points using world coordinates (transform handles conversion).
    /// Cast integers to float for smooth rendering.
    /// </summary>
    private void DrawPointsWithTransform(IGraphicsRenderer renderer, List<HailstonePoint> sequence)
    {
        int cycleStartIndex = sequence.FindIndex(p => p.IsInCycle);

        foreach (var point in sequence)
        {
            if (point.Step == 0)
            {
                // Green square for start
                renderer.DrawRectangle((float)point.X - 2, (float)point.Y - 2, 4, 4, Colors.Green);
            }
            else if (point.IsInCycle && sequence.IndexOf(point) == cycleStartIndex)
            {
                // Yellow diamond for cycle start
                renderer.DrawCircle((float)point.X, (float)point.Y, 3, Colors.Yellow);
            }
            else if (point.IsInCycle)
            {
                // Magenta circles for cycle points
                renderer.DrawCircle((float)point.X, (float)point.Y, 3, Colors.Magenta);
            }
            else
            {
                // Small colored dots for regular points
                var color = Color.FromArgb(255, point.Color.R, point.Color.G, point.Color.B);
                renderer.DrawCircle((float)point.X, (float)point.Y, 2, color);
            }
        }
    }

    private (double scaleX, double scaleY, double offsetX, double offsetY) CalculateTransform(
        int minX, int maxX, int minY, int maxY, int width, int height, bool useFixedViewport)
    {
        double viewMinX, viewMaxX, viewMinY, viewMaxY;

        if (useFixedViewport)
        {
            viewMinX = DefaultMinX;
            viewMaxX = DefaultMaxX;
            viewMinY = DefaultMinY;
            viewMaxY = DefaultMaxY;
        }
        else
        {
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;
            if (rangeX == 0) rangeX = 2;
            if (rangeY == 0) rangeY = 2;

            double paddingX = rangeX * PaddingPercent;
            double paddingY = rangeY * PaddingPercent;

            viewMinX = minX - paddingX;
            viewMaxX = maxX + paddingX;
            viewMinY = minY - paddingY;
            viewMaxY = maxY + paddingY;
        }

        double viewRangeX = viewMaxX - viewMinX;
        double viewRangeY = viewMaxY - viewMinY;

        double scaleX = width / viewRangeX;
        double scaleY = -height / viewRangeY; // Negative to flip Y axis

        double offsetX = -viewMinX * scaleX;
        double offsetY = -viewMaxY * scaleY;

        return (scaleX, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Converts world coordinates to screen coordinates using floating-point precision.
    /// Preserves sub-pixel accuracy for smooth anti-aliased line rendering.
    /// </summary>
    private (float x, float y) WorldToScreen(int worldX, int worldY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        float screenX = (float)(worldX * scaleX + offsetX);
        float screenY = (float)(worldY * scaleY + offsetY);
        return (screenX, screenY);
    }

    private int CalculateTickSpacing(int range)
    {
        if (range <= 20) return 1;
        if (range <= 50) return 5;
        if (range <= 100) return 10;
        if (range <= 200) return 20;
        if (range <= 500) return 50;
        if (range <= 1000) return 100;
        return 200;
    }

    private static int CalculateNextX(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => x / 2,
            (true, false) => (x - y - 1) / 2,
            (false, true) => (x - y + 1) / 2,
            (false, false) => (3 * x + y) / 2
        };
    }

    private static int CalculateNextY(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => y / 2,
            (true, false) => (x + y + 1) / 2,
            (false, true) => (x + y - 1) / 2,
            (false, false) => (x + 3 * y) / 2
        };
    }
}
