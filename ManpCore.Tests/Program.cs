using System;
using System.Diagnostics;
using System.IO;
using ManpCore.Native;

namespace ManpCore.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ManpCore.Native C++/CLI Wrapper Test ===\n");

            try
            {
                // Create fractal engine wrapper
                using var engine = new FractalEngineWrapper();

                // Subscribe to progress events
                engine.ProgressChanged += (sender, e) =>
                {
                    Console.WriteLine($"Progress: {e.Percentage:F1}% - {e.StatusMessage}");
                };

                // Get available fractal types
                Console.WriteLine("Available fractal types:");
                var types = engine.GetAvailableFractalTypes();
                foreach (var type in types)
                {
                    Console.WriteLine($"  - {type}");
                }
                Console.WriteLine();

                // Create test parameters
                var parameters = new FractalParameters
                {
                    FractalType = "Mandelbrot",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 256,
                    CenterX = -0.5,
                    CenterY = 0.0,
                    ViewWidth = 3.0,
                    ViewHeight = 2.25
                };

                Console.WriteLine($"Rendering {parameters.Width}x{parameters.Height} test pattern...\n");

                // Calculate fractal (test pattern for now)
                var stopwatch = Stopwatch.StartNew();
                var result = engine.Calculate(parameters);
                stopwatch.Stop();

                Console.WriteLine($"\n✓ Calculation complete!");
                Console.WriteLine($"  - Render time: {result.RenderTime.TotalMilliseconds:F2} ms");
                Console.WriteLine($"  - Iteration count: {result.IterationCount:N0}");
                Console.WriteLine($"  - Pixel data size: {result.PixelData.Length:N0} bytes");
                Console.WriteLine($"  - Image dimensions: {result.Width}x{result.Height}");

                // Save as simple PPM file (easier than PNG for test)
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "test_output.ppm");
                SaveAsPPM(outputPath, result.PixelData, result.Width, result.Height);
                Console.WriteLine($"\n✓ Saved test image to: {outputPath}");
                Console.WriteLine($"  (Open with image viewer that supports PPM format)");

                // Validate pixel data
                ValidatePixelData(result.PixelData, result.Width, result.Height);

                Console.WriteLine("\n=== All Tests Passed! ===");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error: {ex.Message}");
                Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        static void SaveAsPPM(string path, byte[] pixelData, int width, int height)
        {
            // PPM is a simple uncompressed format (easy to write, view in IrfanView, GIMP, etc.)
            using var writer = new StreamWriter(path);

            // PPM header
            writer.WriteLine("P3");
            writer.WriteLine($"{width} {height}");
            writer.WriteLine("255");

            // Write RGB pixel data
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte r = pixelData[i];
                byte g = pixelData[i + 1];
                byte b = pixelData[i + 2];
                // Alpha (pixelData[i + 3]) is ignored in PPM

                writer.Write($"{r} {g} {b}  ");

                // Newline every 5 pixels for readability
                if ((i / 4) % 5 == 4)
                    writer.WriteLine();
            }
        }

        static void ValidatePixelData(byte[] pixelData, int width, int height)
        {
            Console.WriteLine("\nValidating pixel data...");

            // Check size
            int expectedSize = width * height * 4; // RGBA
            if (pixelData.Length != expectedSize)
            {
                throw new Exception($"Invalid pixel data size: expected {expectedSize}, got {pixelData.Length}");
            }
            Console.WriteLine($"  ✓ Pixel data size correct: {pixelData.Length:N0} bytes");

            // Check for gradient pattern (our test implementation)
            // Top-left should be dark, bottom-right should be brighter
            int topLeft = (0 * width + 0) * 4;
            int bottomRight = ((height - 1) * width + (width - 1)) * 4;

            byte topLeftR = pixelData[topLeft];
            byte bottomRightR = pixelData[bottomRight];

            if (bottomRightR <= topLeftR)
            {
                Console.WriteLine($"  ⚠ Warning: Expected gradient pattern not detected");
                Console.WriteLine($"    Top-left R: {topLeftR}, Bottom-right R: {bottomRightR}");
            }
            else
            {
                Console.WriteLine($"  ✓ Gradient pattern detected (R: {topLeftR} → {bottomRightR})");
            }

            // Check alpha channel
            bool hasAlpha = false;
            for (int i = 3; i < pixelData.Length; i += 4)
            {
                if (pixelData[i] > 0)
                {
                    hasAlpha = true;
                    break;
                }
            }
            if (hasAlpha)
            {
                Console.WriteLine($"  ✓ Alpha channel populated");
            }
            else
            {
                Console.WriteLine($"  ⚠ Warning: Alpha channel is zero");
            }
        }
    }
}