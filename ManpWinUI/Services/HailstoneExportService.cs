using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ManpWinUI.Services
{
    /// <summary>
    /// Service for exporting Hailstone visualizations with overlays to various formats.
    /// </summary>
    public class HailstoneExportService
    {
        /// <summary>
        /// Creates a composite bitmap that includes the Hailstone visualization with info overlay.
        /// </summary>
        public async Task<WriteableBitmap> CreateExportBitmapAsync(
            WriteableBitmap baseBitmap,
            Models.HailstoneResult result)
        {
            if (baseBitmap == null || result == null)
                return baseBitmap;

            // Create a new bitmap with the same dimensions
            var exportBitmap = new WriteableBitmap(baseBitmap.PixelWidth, baseBitmap.PixelHeight);

            // Copy the base bitmap pixels using DataReader
            byte[] pixels = new byte[baseBitmap.PixelBuffer.Length];
            using (var dataReader = DataReader.FromBuffer(baseBitmap.PixelBuffer))
            {
                dataReader.ReadBytes(pixels);
            }

            // Draw info text directly onto the pixel buffer
            DrawInfoText(pixels, baseBitmap.PixelWidth, baseBitmap.PixelHeight, result);

            // Copy to export bitmap
            System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.CopyTo(
                pixels, 0, exportBitmap.PixelBuffer, 0, pixels.Length);

            exportBitmap.Invalidate();

            return exportBitmap;
        }

        /// <summary>
        /// Exports the Hailstone visualization as SVG with embedded metadata.
        /// </summary>
        public async Task<bool> ExportAsSvgAsync(
            Models.HailstoneResult result,
            double scaleX, double scaleY,
            double offsetX, double offsetY,
            int width, int height,
            string filePath,
            Models.FractalMetadata? metadata = null)
        {
            try
            {
                var svg = GenerateSvg(result, scaleX, scaleY, offsetX, offsetY, width, height, metadata);
                await File.WriteAllTextAsync(filePath, svg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates SVG markup for the Hailstone sequence with optional metadata.
        /// </summary>
        private string GenerateSvg(
            Models.HailstoneResult result,
            double scaleX, double scaleY,
            double offsetX, double offsetY,
            int width, int height,
            Models.FractalMetadata? metadata = null)
        {
            var svg = new StringBuilder();
            svg.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\">");

            // Embed metadata as SVG comment for reproducibility
            if (metadata != null)
            {
                svg.AppendLine("  <!--");
                svg.AppendLine("  ManpLab Hailstone Visualization Metadata");
                svg.AppendLine("  This metadata allows exact reproduction of the visualization.");
                svg.AppendLine();
                var jsonMetadata = JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                svg.AppendLine(jsonMetadata);
                svg.AppendLine("  -->");
            }

            svg.AppendLine("  <rect width=\"100%\" height=\"100%\" fill=\"black\"/>");

            // Draw line segments
            svg.AppendLine("  <g id=\"trajectory\">");
            for (int i = 0; i < result.Sequence.Count - 1; i++)
            {
                var p1 = result.Sequence[i];
                var p2 = result.Sequence[i + 1];

                int x1 = (int)(p1.X * scaleX + offsetX);
                int y1 = (int)(p1.Y * scaleY + offsetY);
                int x2 = (int)(p2.X * scaleX + offsetX);
                int y2 = (int)(p2.Y * scaleY + offsetY);

                string color = p1.IsInCycle ? "#FF40B1" : "#42A5F5"; // Magenta or light blue
                svg.AppendLine($"    <line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"{color}\" stroke-width=\"1\"/>");
            }
            svg.AppendLine("  </g>");

            // Draw points
            svg.AppendLine("  <g id=\"points\">");
            int cycleStartIndex = result.HasCycle ? result.CycleStartIndex : -1;

            for (int i = 0; i < result.Sequence.Count; i++)
            {
                var point = result.Sequence[i];
                int x = (int)(point.X * scaleX + offsetX);
                int y = (int)(point.Y * scaleY + offsetY);

                if (point.Step == 0)
                {
                    // Green square for start
                    svg.AppendLine($"    <rect x=\"{x - 3}\" y=\"{y - 3}\" width=\"6\" height=\"6\" fill=\"#00FF00\"/>");
                }
                else if (i == cycleStartIndex)
                {
                    // Yellow diamond for cycle start
                    svg.AppendLine($"    <polygon points=\"{x},{y - 3} {x + 3},{y} {x},{y + 3} {x - 3},{y}\" fill=\"#FFFF00\"/>");
                }
                else
                {
                    string color = point.IsInCycle ? "#FF40B1" : "#42A5F5";
                    svg.AppendLine($"    <circle cx=\"{x}\" cy=\"{y}\" r=\"2\" fill=\"{color}\"/>");
                }
            }
            svg.AppendLine("  </g>");

            // Draw info text with smaller fonts
            svg.AppendLine("  <g id=\"info\" font-family=\"monospace\" font-size=\"5\">");
            svg.AppendLine($"    <rect x=\"10\" y=\"10\" width=\"140\" height=\"{(result.HasCycle ? 50 : 35)}\" fill=\"rgba(0,0,0,0.86)\" stroke=\"#FF40B1\" stroke-width=\"2\" rx=\"4\"/>");

            int yPos = 17;
            svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"white\" font-weight=\"600\" font-size=\"6\">Hailstone Sequence (N,X,Y)</text>");
            yPos += 8;

            if (result.Sequence.Count > 0)
            {
                var startPoint = result.Sequence[0];
                svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">Starting point: ({startPoint.Step},{startPoint.X},{startPoint.Y})</text>");
                yPos += 7;
            }

            svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">Total points: {result.Sequence.Count}</text>");
            yPos += 7;

            if (result.HasCycle && cycleStartIndex >= 0)
            {
                var cycleStart = result.Sequence[cycleStartIndex];
                var lastPoint = result.Sequence[result.Sequence.Count - 1];

                svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">Cycle Detected: Point({lastPoint.Step},{lastPoint.X},{lastPoint.Y})</text>");
                yPos += 7;
                svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">Duplicate of: ({cycleStart.Step},{cycleStart.X},{cycleStart.Y})</text>");
                yPos += 7;
                svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">Cycle length: {result.CycleLength}</text>");
            }
            else
            {
                svg.AppendLine($"    <text x=\"16\" y=\"{yPos}\" fill=\"#FF40B1\" font-weight=\"bold\">No cycle detected</text>");
            }

            svg.AppendLine("  </g>");
            svg.AppendLine("</svg>");

            return svg.ToString();
        }

        /// <summary>
        /// Draws info text onto the pixel buffer (simplified - draws basic rectangles and would need font rendering).
        /// For now, this is a placeholder - actual implementation would need proper text rendering.
        /// </summary>
        private void DrawInfoText(byte[] pixels, int width, int height, Models.HailstoneResult result)
        {
            // Draw a semi-transparent black rectangle with magenta border in upper left
            // This is a simplified version - actual text rendering would require more work
            // For now, the info overlay in the UI will be visible and users can screenshot

            // Draw background rectangle (220x90 pixels, upper left)
            int rectWidth = 220;
            int rectHeight = result.HasCycle ? 110 : 75;
            int margin = 10;

            // Draw filled rectangle with border
            for (int y = margin; y < margin + rectHeight && y < height; y++)
            {
                for (int x = margin; x < margin + rectWidth && x < width; x++)
                {
                    bool isBorder = (x == margin || x == margin + rectWidth - 1 ||
                                   y == margin || y == margin + rectHeight - 1);

                    int index = (y * width + x) * 4;
                    if (index >= 0 && index < pixels.Length - 3)
                    {
                        if (isBorder)
                        {
                            // Magenta border
                            pixels[index] = 177;     // B
                            pixels[index + 1] = 64;  // G
                            pixels[index + 2] = 255; // R
                            pixels[index + 3] = 255; // A
                        }
                        else
                        {
                            // Semi-transparent black background
                            pixels[index] = 0;
                            pixels[index + 1] = 0;
                            pixels[index + 2] = 0;
                            pixels[index + 3] = 220; // 86% opacity
                        }
                    }
                }
            }

            // Note: Actual text rendering would require a proper font rasterization library
            // For PNG export with text, we'll use a different approach (render the UI element to bitmap)
        }
    }
}
