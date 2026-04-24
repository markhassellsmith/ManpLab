using ManpWinUI.Models;
using Microsoft.UI;
using System.Diagnostics;

namespace ManpWinUI.Services
{
    /// <summary>
    /// Example of how to refactor HailstoneRenderService to use the graphics abstraction.
    /// This allows swapping Win2D ↔ SkiaSharp without changing business logic.
    /// </summary>
    public class HailstoneRenderServiceRefactored
    {
        /// <summary>
        /// Renders a Hailstone sequence using the abstracted graphics renderer.
        /// </summary>
        public async Task<HailstoneRenderResult> RenderSequenceAsync(
            HailstoneResult result,
            int width,
            int height,
            bool showAxes,
            bool showPoints,
            bool showLabels)
        {
            var stopwatch = Stopwatch.StartNew();

            // Create renderer using factory (automatically uses Win2D or SkiaSharp based on config)
            using var renderer = GraphicsRendererFactory.Create(width, height);

            // Clear background
            renderer.Clear(Colors.Black);

            // Calculate coordinate bounds and transformation
            int minX = result.Sequence.Min(p => p.X);
            int maxX = result.Sequence.Max(p => p.X);
            int minY = result.Sequence.Min(p => p.Y);
            int maxY = result.Sequence.Max(p => p.Y);

            var (scaleX, scaleY, offsetX, offsetY) = CalculateTransform(
                minX, maxX, minY, maxY, width, height);

            // Draw grid and axes (if enabled)
            if (showAxes)
            {
                DrawGridWithRenderer(renderer, minX, maxX, minY, maxY, 
                    scaleX, scaleY, offsetX, offsetY, width, height);
            }

            // Draw sequence path
            DrawSequenceWithRenderer(renderer, result.Sequence, 
                scaleX, scaleY, offsetX, offsetY);

            // Draw points (if enabled)
            if (showPoints)
            {
                DrawPointsWithRenderer(renderer, result.Sequence, 
                    scaleX, scaleY, offsetX, offsetY);
            }

            // Draw labels directly on the bitmap (if enabled)
            if (showLabels)
            {
                DrawLabelsWithRenderer(renderer, result.Sequence, 
                    scaleX, scaleY, offsetX, offsetY);
            }

            // Draw info text directly on the bitmap
            DrawInfoTextWithRenderer(renderer, result);

            // Convert to WriteableBitmap for WinUI display
            var bitmap = renderer.ToWriteableBitmap();

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

        private void DrawGridWithRenderer(IGraphicsRenderer renderer,
            int minX, int maxX, int minY, int maxY,
            double scaleX, double scaleY, double offsetX, double offsetY,
            int width, int height)
        {
            var gridColor = Windows.UI.Color.FromArgb(80, 50, 50, 50); // Subtle gray
            var axesColor = Windows.UI.Color.FromArgb(150, 100, 100, 100); // Brighter gray

            int range = Math.Max(maxX - minX, maxY - minY);
            int tickSpacing = CalculateTickSpacing(range);

            // Draw vertical grid lines
            for (int x = minX; x <= maxX; x += tickSpacing)
            {
                var (screenX, _) = WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
                var color = (x == 0) ? axesColor : gridColor;
                renderer.DrawLine(screenX, 0, screenX, height - 1, color, 1.0f);
            }

            // Draw horizontal grid lines
            for (int y = minY; y <= maxY; y += tickSpacing)
            {
                var (_, screenY) = WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
                var color = (y == 0) ? axesColor : gridColor;
                renderer.DrawLine(0, screenY, width - 1, screenY, color, 1.0f);
            }
        }

        private void DrawSequenceWithRenderer(IGraphicsRenderer renderer,
            List<HailstonePoint> sequence,
            double scaleX, double scaleY, double offsetX, double offsetY)
        {
            for (int i = 0; i < sequence.Count - 1; i++)
            {
                var p1 = sequence[i];
                var p2 = sequence[i + 1];

                var (x1, y1) = WorldToScreen(p1.X, p1.Y, scaleX, scaleY, offsetX, offsetY);
                var (x2, y2) = WorldToScreen(p2.X, p2.Y, scaleX, scaleY, offsetX, offsetY);

                // Choose color and thickness
                if (p1.IsInCycle)
                {
                    // Magenta for cycle, slightly thicker
                    renderer.DrawLine(x1, y1, x2, y2, Colors.Magenta, 2.0f);
                }
                else
                {
                    // Spectrum color from point data
                    var color = Windows.UI.Color.FromArgb(255, p2.Color.R, p2.Color.G, p2.Color.B);
                    renderer.DrawLine(x1, y1, x2, y2, color, 1.0f);
                }
            }
        }

        private void DrawPointsWithRenderer(IGraphicsRenderer renderer,
            List<HailstonePoint> sequence,
            double scaleX, double scaleY, double offsetX, double offsetY)
        {
            foreach (var point in sequence)
            {
                var (screenX, screenY) = WorldToScreen(point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

                if (point.Step == 0)
                {
                    // Green square for start
                    renderer.DrawRectangle(screenX - 2, screenY - 2, 4, 4, Colors.Green);
                }
                else if (point.IsInCycle)
                {
                    // Magenta circle for cycle points
                    renderer.DrawCircle(screenX, screenY, 3, Colors.Magenta);
                }
                else
                {
                    // Small colored dots
                    var color = Windows.UI.Color.FromArgb(255, point.Color.R, point.Color.G, point.Color.B);
                    renderer.DrawCircle(screenX, screenY, 2, color);
                }
            }
        }

        private void DrawLabelsWithRenderer(IGraphicsRenderer renderer,
            List<HailstonePoint> sequence,
            double scaleX, double scaleY, double offsetX, double offsetY)
        {
            // Now we can draw labels directly on the bitmap!
            foreach (var point in sequence)
            {
                var (screenX, screenY) = WorldToScreen(point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

                string labelText = $"({point.Step},{point.X},{point.Y})";
                var color = point.IsInCycle ? Colors.Magenta : Colors.Cyan;

                // Draw label near point (offset to avoid overlap)
                renderer.DrawText(labelText, screenX + 3, screenY - 2, color, 2.5f, "Arial", false);
            }
        }

        private void DrawInfoTextWithRenderer(IGraphicsRenderer renderer, HailstoneResult result)
        {
            // Draw info text in corner
            string infoText = $"Hailstone Sequence\nStart: ({result.Sequence[0].X},{result.Sequence[0].Y})\nPoints: {result.Sequence.Count}";

            if (result.HasCycle)
            {
                infoText += $"\nCycle length: {result.CycleLength}";
            }

            var color = result.HasCycle ? Colors.Magenta : Colors.Yellow;
            renderer.DrawText(infoText, 5, 5, color, 3f, "Arial", true);
        }

        private (int x, int y) WorldToScreen(int worldX, int worldY,
            double scaleX, double scaleY, double offsetX, double offsetY)
        {
            int screenX = (int)(worldX * scaleX + offsetX);
            int screenY = (int)(worldY * scaleY + offsetY);
            return (screenX, screenY);
        }

        private (double scaleX, double scaleY, double offsetX, double offsetY) CalculateTransform(
            int minX, int maxX, int minY, int maxY, int width, int height)
        {
            // Same transform logic as existing code
            double paddingPercent = 0.1;
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;

            if (rangeX == 0) rangeX = 2;
            if (rangeY == 0) rangeY = 2;

            double paddingX = rangeX * paddingPercent;
            double paddingY = rangeY * paddingPercent;

            double viewMinX = minX - paddingX;
            double viewMaxX = maxX + paddingX;
            double viewMinY = minY - paddingY;
            double viewMaxY = maxY + paddingY;

            double viewRangeX = viewMaxX - viewMinX;
            double viewRangeY = viewMaxY - viewMinY;

            double scaleX = width / viewRangeX;
            double scaleY = -height / viewRangeY;

            double offsetX = -viewMinX * scaleX;
            double offsetY = -viewMaxY * scaleY;

            return (scaleX, scaleY, offsetX, offsetY);
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
    }
}
