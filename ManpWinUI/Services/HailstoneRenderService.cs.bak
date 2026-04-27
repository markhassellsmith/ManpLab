using ManpWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.Graphics.Imaging;

namespace ManpWinUI.Services;

/// <summary>
/// Service for rendering 2D Hailstone sequences to bitmap images.
/// </summary>
public class HailstoneRenderService
{
    private const double PaddingPercent = 0.15; // 15% padding around content

    // Default fixed viewport bounds (standard view for Hailstone sequences)
    private const int DefaultMinX = -40;
    private const int DefaultMaxX = 40;
    private const int DefaultMinY = -30;
    private const int DefaultMaxY = 30;

    /// <summary>
    /// Renders a Hailstone sequence to a WriteableBitmap.
    /// </summary>
    /// <param name="result">The calculated Hailstone sequence.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="showAxes">Whether to draw coordinate axes and grid lines.</param>
    /// <param name="showPoints">Whether to draw dots at trajectory points.</param>
    /// <param name="showLabels">Whether to draw point labels (handled by overlay, not bitmap).</param>
    /// <param name="useFixedViewport">If true, uses fixed viewport bounds; if false, auto-scales to data.</param>
    /// <param name="customViewportMinX">Custom viewport minimum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxX">Custom viewport maximum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMinY">Custom viewport minimum Y (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxY">Custom viewport maximum Y (overrides auto-scale/fixed).</param>
    /// <returns>A HailstoneRenderResult containing the bitmap and transform parameters.</returns>
    public async Task<HailstoneRenderResult> RenderSequenceAsync(
        HailstoneResult result,
        int width,
        int height,
        bool showAxes,
        bool showPoints,
        bool showLabels,
        bool useFixedViewport = false,
        double? customViewportMinX = null,
        double? customViewportMaxX = null,
        double? customViewportMinY = null,
        double? customViewportMaxY = null)
    {
        var stopwatch = Stopwatch.StartNew();

        // Debug output: Show sequence statistics
        Debug.WriteLine($"=== Hailstone Render Debug ===");
        Debug.WriteLine($"Sequence points: {result.Sequence.Count}");
        Debug.WriteLine($"Data bounds: X=[{result.MinX}, {result.MaxX}], Y=[{result.MinY}, {result.MaxY}]");
        Debug.WriteLine($"Has cycle: {result.HasCycle}");
        if (result.HasCycle)
        {
            Debug.WriteLine($"Cycle start: {result.CycleStartIndex}, length: {result.CycleLength}");
        }

        // Show first few and last few points
        Debug.WriteLine("First 5 points:");
        for (int i = 0; i < Math.Min(5, result.Sequence.Count); i++)
        {
            var pt = result.Sequence[i];
            Debug.WriteLine($"  [{pt.Step}]: ({pt.X}, {pt.Y})");
        }
        if (result.Sequence.Count > 5)
        {
            Debug.WriteLine("Last 3 points:");
            for (int i = Math.Max(0, result.Sequence.Count - 3); i < result.Sequence.Count; i++)
            {
                var pt = result.Sequence[i];
                Debug.WriteLine($"  [{pt.Step}]: ({pt.X}, {pt.Y})");
            }
        }

        // Declare transform parameters outside the Task.Run so we can return them
        double scaleX = 0, scaleY = 0, offsetX = 0, offsetY = 0;

        // Do pixel rendering on background thread
        byte[] pixels = await Task.Run(() =>
        {
            // Determine bounds to use for rendering
            int minX, maxX, minY, maxY;

            // Check if custom viewport is provided (takes highest priority)
            bool hasCustomViewport = customViewportMinX.HasValue && 
                                      customViewportMaxX.HasValue && 
                                      customViewportMinY.HasValue && 
                                      customViewportMaxY.HasValue;

            if (hasCustomViewport)
            {
                // Use custom viewport (from interactive zoom/pan)
                minX = (int)Math.Floor(customViewportMinX!.Value);
                maxX = (int)Math.Ceiling(customViewportMaxX!.Value);
                minY = (int)Math.Floor(customViewportMinY!.Value);
                maxY = (int)Math.Ceiling(customViewportMaxY!.Value);
                Debug.WriteLine($"Using CUSTOM viewport: X=[{customViewportMinX:F2}, {customViewportMaxX:F2}], Y=[{customViewportMinY:F2}, {customViewportMaxY:F2}]");
                Debug.WriteLine($"  Rounded to integers: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
            }
            else if (useFixedViewport)
            {
                // Use fixed viewport bounds
                minX = DefaultMinX;
                maxX = DefaultMaxX;
                minY = DefaultMinY;
                maxY = DefaultMaxY;
                Debug.WriteLine($"Using FIXED viewport: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
            }
            else
            {
                // Use auto-scaling based on actual data
                minX = result.MinX;
                maxX = result.MaxX;
                minY = result.MinY;
                maxY = result.MaxY;
                Debug.WriteLine($"Using AUTO-SCALE viewport fitted to data: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");

                // Calculate viewport with padding
                int rangeX = maxX - minX;
                int rangeY = maxY - minY;
                if (rangeX == 0) rangeX = 2;
                if (rangeY == 0) rangeY = 2;
                double paddingX = rangeX * PaddingPercent;
                double paddingY = rangeY * PaddingPercent;
                Debug.WriteLine($"  Data range: X={rangeX}, Y={rangeY}");
                Debug.WriteLine($"  With {PaddingPercent*100}% padding: viewport will be X=[{minX-paddingX:F1}, {maxX+paddingX:F1}], Y=[{minY-paddingY:F1}, {maxY+paddingY:F1}]");
            }

            // Calculate scaling (with padding only for auto-scale mode, not for custom viewport)
            (scaleX, scaleY, offsetX, offsetY) = CalculateTransform(
                minX, maxX, minY, maxY,
                width, height, useFixedViewport || hasCustomViewport);

            Debug.WriteLine($"Transform: scaleX={scaleX:F4}, scaleY={scaleY:F4}, offsetX={offsetX:F2}, offsetY={offsetY:F2}");

            // Allocate pixel buffer (BGRA format)
            byte[] pixelData = new byte[width * height * 4];

            // Fill background (black)
            FillBackground(pixelData, width, height);

            // Draw grid if requested
            if (showAxes)
            {
                DrawGrid(pixelData, width, height, 
                    minX, maxX, minY, maxY,
                    scaleX, scaleY, offsetX, offsetY);
            }

            // Draw sequence path
            DrawSequence(pixelData, width, height, result.Sequence,
                scaleX, scaleY, offsetX, offsetY);

            // Draw points if requested
            if (showPoints)
            {
                DrawPoints(pixelData, width, height, result.Sequence,
                    scaleX, scaleY, offsetX, offsetY);
            }

            return pixelData;
        });

        // Create bitmap and copy pixels on UI thread
        var bitmap = new WriteableBitmap(width, height);

        System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.CopyTo(
            pixels, 0, bitmap.PixelBuffer, 0, pixels.Length);

        bitmap.Invalidate();

        stopwatch.Stop();
        Debug.WriteLine($"Hailstone render time: {stopwatch.ElapsedMilliseconds}ms");

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
    /// Calculates coordinate transformation for rendering.
    /// Fits the viewport to contain all computed points with appropriate padding.
    /// </summary>
    private (double scaleX, double scaleY, double offsetX, double offsetY) CalculateTransform(
        int minX, int maxX, int minY, int maxY,
        int width, int height, bool useFixedViewport)
    {
        double viewMinX, viewMaxX, viewMinY, viewMaxY;

        if (useFixedViewport)
        {
            // Use fixed viewport bounds (centered on origin)
            viewMinX = -DefaultMaxX;
            viewMaxX = DefaultMaxX;
            viewMinY = -DefaultMaxY;
            viewMaxY = DefaultMaxY;
        }
        else
        {
            // Fit viewport to actual data with padding
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;

            // Handle single-point case
            if (rangeX == 0) rangeX = 2;
            if (rangeY == 0) rangeY = 2;

            // Add padding around the data
            double paddingX = rangeX * PaddingPercent;
            double paddingY = rangeY * PaddingPercent;

            viewMinX = minX - paddingX;
            viewMaxX = maxX + paddingX;
            viewMinY = minY - paddingY;
            viewMaxY = maxY + paddingY;
        }

        // Calculate the viewport dimensions
        double viewRangeX = viewMaxX - viewMinX;
        double viewRangeY = viewMaxY - viewMinY;

        // Calculate scale (negative Y to flip for screen coordinates)
        double scaleX = width / viewRangeX;
        double scaleY = -height / viewRangeY; // Negative to flip Y axis

        // Calculate offset to map world coordinates to screen
        // For X: viewMinX should map to screen x=0
        // For Y: viewMaxY should map to screen y=0 (top), viewMinY to y=height (bottom)
        double offsetX = -viewMinX * scaleX;
        double offsetY = -viewMaxY * scaleY;  // Use viewMaxY, not viewMinY

        return (scaleX, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Converts world coordinates to screen coordinates.
    /// </summary>
    private (int screenX, int screenY) WorldToScreen(
        int worldX, int worldY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int screenX = (int)(worldX * scaleX + offsetX);
        int screenY = (int)(worldY * scaleY + offsetY);
        return (screenX, screenY);
    }

    /// <summary>
    /// Fills the background with black.
    /// </summary>
    private void FillBackground(byte[] pixels, int width, int height)
    {
        // Black background (BGRA = 0, 0, 0, 255)
        for (int i = 0; i < pixels.Length; i += 4)
        {
            pixels[i] = 0;     // B
            pixels[i + 1] = 0; // G
            pixels[i + 2] = 0; // R
            pixels[i + 3] = 255; // A
        }
    }

    /// <summary>
    /// Draws grid lines at integer coordinates.
    /// </summary>
    private void DrawGrid(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Much more subtle grid - darker and more transparent
        byte gridB = 50, gridG = 50, gridR = 50;
        byte gridAlpha = 80; // More transparent

        // Axes at origin - gray but slightly brighter than grid
        byte axisB = 100, axisG = 100, axisR = 100;
        byte axisAlpha = 150;

        // Calculate range for tick spacing
        int rangeX = maxX - minX;
        int rangeY = maxY - minY;
        int range = Math.Max(rangeX, rangeY);

        // Calculate appropriate tick spacing
        int tickSpacing = CalculateTickSpacing(range);

        // Draw vertical grid lines
        int startX = (minX / tickSpacing) * tickSpacing;
        if (startX > minX) startX -= tickSpacing;
        int endX = (maxX / tickSpacing) * tickSpacing;
        if (endX < maxX) endX += tickSpacing;

        for (int x = startX; x <= endX; x += tickSpacing)
        {
            var (screenX, _) = WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
            if (screenX >= 0 && screenX < width)
            {
                // Use brighter color for Y-axis (x=0)
                byte b = (x == 0) ? axisB : gridB;
                byte g = (x == 0) ? axisG : gridG;
                byte r = (x == 0) ? axisR : gridR;
                byte alpha = (x == 0) ? axisAlpha : gridAlpha;

                for (int y = 0; y < height; y++)
                {
                    int index = (y * width + screenX) * 4;
                    if (index >= 0 && index < pixels.Length - 3)
                    {
                        // Blend with existing background for transparency effect
                        float alphaFactor = alpha / 255.0f;
                        pixels[index] = (byte)(pixels[index] * (1 - alphaFactor) + b * alphaFactor);
                        pixels[index + 1] = (byte)(pixels[index + 1] * (1 - alphaFactor) + g * alphaFactor);
                        pixels[index + 2] = (byte)(pixels[index + 2] * (1 - alphaFactor) + r * alphaFactor);
                        pixels[index + 3] = 255;
                    }
                }
            }
        }

        // Draw horizontal grid lines
        int startY = (minY / tickSpacing) * tickSpacing;
        if (startY > minY) startY -= tickSpacing;
        int endY = (maxY / tickSpacing) * tickSpacing;
        if (endY < maxY) endY += tickSpacing;

        for (int y = startY; y <= endY; y += tickSpacing)
        {
            var (_, screenY) = WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
            if (screenY >= 0 && screenY < height)
            {
                // Use brighter color for X-axis (y=0)
                byte b = (y == 0) ? axisB : gridB;
                byte g = (y == 0) ? axisG : gridG;
                byte r = (y == 0) ? axisR : gridR;
                byte alpha = (y == 0) ? axisAlpha : gridAlpha;

                for (int x = 0; x < width; x++)
                {
                    int index = (screenY * width + x) * 4;
                    if (index >= 0 && index < pixels.Length - 3)
                    {
                        // Blend with existing background for transparency effect
                        float alphaFactor = alpha / 255.0f;
                        pixels[index] = (byte)(pixels[index] * (1 - alphaFactor) + b * alphaFactor);
                        pixels[index + 1] = (byte)(pixels[index + 1] * (1 - alphaFactor) + g * alphaFactor);
                        pixels[index + 2] = (byte)(pixels[index + 2] * (1 - alphaFactor) + r * alphaFactor);
                        pixels[index + 3] = 255;
                    }
                }
            }
        }

        // Draw tick marks on axes
        DrawAxisTickMarks(pixels, width, height, minX, maxX, minY, maxY, 
            tickSpacing, scaleX, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Draws tick marks on the coordinate axes at grid line intersections.
    /// </summary>
    private void DrawAxisTickMarks(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY, int tickSpacing,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Tick mark color - slightly brighter gray for visibility
        byte tickB = 120, tickG = 120, tickR = 120;
        int tickLength = 5; // pixels

        // Calculate label spacing - show labels less frequently for larger ranges
        int range = Math.Max(maxX - minX, maxY - minY);
        int labelSpacing = tickSpacing;
        if (range > 100) labelSpacing = tickSpacing * 5; // Every 5th tick
        else if (range > 50) labelSpacing = tickSpacing * 2; // Every 2nd tick

        // Draw tick marks on X-axis (at y=0)
        var (_, axisY) = WorldToScreen(0, 0, scaleX, scaleY, offsetX, offsetY);
        if (axisY >= 0 && axisY < height)
        {
            int startX = (minX / tickSpacing) * tickSpacing;
            if (startX > minX) startX -= tickSpacing;
            int endX = (maxX / tickSpacing) * tickSpacing;
            if (endX < maxX) endX += tickSpacing;

            for (int x = startX; x <= endX; x += tickSpacing)
            {
                if (x == 0) continue; // Skip origin (already marked by axes intersection)

                var (screenX, _) = WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);

                if (screenX >= 0 && screenX < width)
                {
                    // Draw vertical tick mark
                    for (int dy = -tickLength; dy <= tickLength; dy++)
                    {
                        int y = axisY + dy;
                        if (y >= 0 && y < height)
                        {
                            int index = (y * width + screenX) * 4;
                            if (index >= 0 && index < pixels.Length - 3)
                            {
                                pixels[index] = tickB;
                                pixels[index + 1] = tickG;
                                pixels[index + 2] = tickR;
                                pixels[index + 3] = 255;
                            }
                        }
                    }
                }
            }
        }

        // Draw tick marks on Y-axis (at x=0)
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
                    // Draw horizontal tick mark
                    for (int dx = -tickLength; dx <= tickLength; dx++)
                    {
                        int x = axisX + dx;
                        if (x >= 0 && x < width)
                        {
                            int index = (screenY * width + x) * 4;
                            if (index >= 0 && index < pixels.Length - 3)
                            {
                                pixels[index] = tickB;
                                pixels[index + 1] = tickG;
                                pixels[index + 2] = tickR;
                                pixels[index + 3] = 255;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates appropriate tick spacing for grid lines.
    /// </summary>
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

    /// <summary>
    /// Draws lines connecting sequence points.
    /// </summary>
    private void DrawSequence(byte[] pixels, int width, int height,
        List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        Debug.WriteLine($"DrawSequence called with {sequence.Count} points");

        if (sequence.Count < 2)
        {
            Debug.WriteLine("Not enough points to draw (need at least 2)");
            return;
        }

        int drawnLines = 0;
        int skippedLines = 0;

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var p1 = sequence[i];
            var p2 = sequence[i + 1];

            var (x1, y1) = WorldToScreen(p1.X, p1.Y, scaleX, scaleY, offsetX, offsetY);
            var (x2, y2) = WorldToScreen(p2.X, p2.Y, scaleX, scaleY, offsetX, offsetY);

            // Debug first few lines
            if (i < 3)
            {
                Debug.WriteLine($"  Line {i}: World({p1.X},{p1.Y}) -> ({p2.X},{p2.Y}) = Screen({x1},{y1}) -> ({x2},{y2})");
            }

            // Check if line is on screen
            bool onScreen = (x1 >= -100 && x1 < width + 100 && y1 >= -100 && y1 < height + 100) ||
                           (x2 >= -100 && x2 < width + 100 && y2 >= -100 && y2 < height + 100);

            if (onScreen)
                drawnLines++;
            else
                skippedLines++;

            // Choose color based on whether point is in cycle
            byte b, g, r;
            if (p1.IsInCycle)
            {
                // Bright magenta for cycle (matching NumericalVisualizations)
                b = 255; g = 0; r = 255;
                // Draw slightly thicker line for cycle (2 parallel lines for subtle emphasis)
                DrawLine(pixels, width, height, x1, y1, x2, y2, b, g, r);
                DrawLine(pixels, width, height, x1+1, y1, x2+1, y2, b, g, r);
            }
            else
            {
                // Use spectrum color from point data
                var color = p2.Color;
                b = color.B;
                g = color.G;
                r = color.R;
                // Single thin line for regular segments
                DrawLine(pixels, width, height, x1, y1, x2, y2, b, g, r);
            }
        }

        Debug.WriteLine($"Drew {drawnLines} lines, skipped {skippedLines} off-screen lines");
    }

    /// <summary>
    /// Draws points at each sequence position.
    /// </summary>
    private void DrawPoints(byte[] pixels, int width, int height,
        List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Get cycle start index if exists
        int cycleStartIndex = -1;
        if (sequence.Count > 0 && sequence.Any(p => p.IsInCycle))
        {
            cycleStartIndex = sequence.FindIndex(p => p.IsInCycle);
        }

        for (int i = 0; i < sequence.Count; i++)
        {
            var point = sequence[i];
            var (screenX, screenY) = WorldToScreen(point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

            // Special marker for starting point (Step == 0)
            if (point.Step == 0)
            {
                // Draw a bright green square for the start point
                DrawSquare(pixels, width, height, screenX, screenY, 3, 0, 255, 0);
            }
            // Special marker for cycle start point (first repeated point)
            else if (i == cycleStartIndex && cycleStartIndex >= 0)
            {
                // Draw a bright yellow diamond for the cycle start
                DrawDiamond(pixels, width, height, screenX, screenY, 3, 0, 255, 255);
            }
            else
            {
                // Regular points - smaller dots matching NumericalVisualizations style
                byte b, g, r;
                int radius;

                if (point.IsInCycle)
                {
                    // Bright magenta for cycle points
                    b = 255; g = 0; r = 255;
                    radius = 2; // Smaller dots like NumericalVisualizations
                }
                else
                {
                    // Use spectrum color from point data
                    var color = point.Color;
                    b = color.B;
                    g = color.G;
                    r = color.R;
                    radius = 2; // Smaller dots like NumericalVisualizations
                }

                DrawCircle(pixels, width, height, screenX, screenY, radius, b, g, r);
            }
        }
    }

    /// <summary>
    /// Draws a line using Bresenham's algorithm.
    /// </summary>
    private void DrawLine(byte[] pixels, int width, int height,
        int x0, int y0, int x1, int y1,
        byte b, byte g, byte r)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Plot point
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
            {
                int index = (y0 * width + x0) * 4;
                if (index >= 0 && index < pixels.Length - 3)
                {
                    pixels[index] = b;
                    pixels[index + 1] = g;
                    pixels[index + 2] = r;
                    pixels[index + 3] = 255;
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    /// <summary>
    /// Draws a filled circle.
    /// </summary>
    private void DrawCircle(byte[] pixels, int width, int height,
        int centerX, int centerY, int radius,
        byte b, byte g, byte r)
    {
        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (dx * dx + dy * dy <= radius * radius)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        int index = (y * width + x) * 4;
                        if (index >= 0 && index < pixels.Length - 3)
                        {
                            pixels[index] = b;
                            pixels[index + 1] = g;
                            pixels[index + 2] = r;
                            pixels[index + 3] = 255;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws a filled square (for starting point marker).
    /// </summary>
    private void DrawSquare(byte[] pixels, int width, int height,
        int centerX, int centerY, int halfSize,
        byte b, byte g, byte r)
    {
        for (int dy = -halfSize; dy <= halfSize; dy++)
        {
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                int x = centerX + dx;
                int y = centerY + dy;

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    int index = (y * width + x) * 4;
                    if (index >= 0 && index < pixels.Length - 3)
                    {
                        pixels[index] = b;
                        pixels[index + 1] = g;
                        pixels[index + 2] = r;
                        pixels[index + 3] = 255;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws a filled diamond (for cycle start marker).
    /// </summary>
    private void DrawDiamond(byte[] pixels, int width, int height,
        int centerX, int centerY, int halfSize,
        byte b, byte g, byte r)
    {
        for (int dy = -halfSize; dy <= halfSize; dy++)
        {
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                // Diamond shape: |dx| + |dy| <= halfSize
                if (Math.Abs(dx) + Math.Abs(dy) <= halfSize)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        int index = (y * width + x) * 4;
                        if (index >= 0 && index < pixels.Length - 3)
                        {
                            pixels[index] = b;
                            pixels[index + 1] = g;
                            pixels[index + 2] = r;
                            pixels[index + 3] = 255;
                        }
                    }
                }
            }
        }
    }
}
