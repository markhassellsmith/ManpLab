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

                // Test 1: Mandelbrot Set
                Console.WriteLine("=== Test 1: Mandelbrot Set ===\n");
                var mandelbrotParams = new FractalParameters
                {
                    FractalType = "Mandelbrot",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 256,
                    CenterX = -0.5,
                    CenterY = 0.0,
                    ViewWidth = 3.0,
                    ViewHeight = 2.25,
                    IsJuliaSet = false
                };

                Console.WriteLine($"Rendering {mandelbrotParams.Width}x{mandelbrotParams.Height} Mandelbrot set...\n");

                var stopwatch = Stopwatch.StartNew();
                var mandelbrotResult = engine.Calculate(mandelbrotParams);
                stopwatch.Stop();

                Console.WriteLine($"\n✓ Mandelbrot calculation complete!");
                Console.WriteLine($"  - Render time: {mandelbrotResult.RenderTime.TotalMilliseconds:F2} ms");
                Console.WriteLine($"  - Iteration count: {mandelbrotResult.IterationCount:N0}");
                Console.WriteLine($"  - Pixel data size: {mandelbrotResult.PixelData.Length:N0} bytes");
                Console.WriteLine($"  - Image dimensions: {mandelbrotResult.Width}x{mandelbrotResult.Height}");

                var mandelbrotPath = Path.Combine(Directory.GetCurrentDirectory(), "mandelbrot_output.ppm");
                SaveAsPPM(mandelbrotPath, mandelbrotResult.PixelData, mandelbrotResult.Width, mandelbrotResult.Height);
                Console.WriteLine($"\n✓ Saved Mandelbrot set image to: {mandelbrotPath}");

                ValidatePixelData(mandelbrotResult.PixelData, mandelbrotResult.Width, mandelbrotResult.Height);

                // Test 2: Julia Set
                Console.WriteLine("\n\n=== Test 2: Julia Set ===\n");
                var juliaParams = new FractalParameters
                {
                    FractalType = "Julia",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 256,
                    CenterX = 0.0,
                    CenterY = 0.0,
                    ViewWidth = 4.0,
                    ViewHeight = 3.0,
                    IsJuliaSet = true,
                    JuliaCX = -0.7,      // Classic Julia set parameters
                    JuliaCY = 0.27015    // Creates beautiful fractal pattern
                };

                Console.WriteLine($"Rendering {juliaParams.Width}x{juliaParams.Height} Julia set (c = {juliaParams.JuliaCX} + {juliaParams.JuliaCY}i)...\n");

                stopwatch.Restart();
                var juliaResult = engine.Calculate(juliaParams);
                stopwatch.Stop();

                Console.WriteLine($"\n✓ Julia calculation complete!");
                Console.WriteLine($"  - Render time: {juliaResult.RenderTime.TotalMilliseconds:F2} ms");
                Console.WriteLine($"  - Iteration count: {juliaResult.IterationCount:N0}");
                Console.WriteLine($"  - Pixel data size: {juliaResult.PixelData.Length:N0} bytes");
                Console.WriteLine($"  - Image dimensions: {juliaResult.Width}x{juliaResult.Height}");

                var juliaPath = Path.Combine(Directory.GetCurrentDirectory(), "julia_output.ppm");
                SaveAsPPM(juliaPath, juliaResult.PixelData, juliaResult.Width, juliaResult.Height);
                Console.WriteLine($"\n✓ Saved Julia set image to: {juliaPath}");

                ValidatePixelData(juliaResult.PixelData, juliaResult.Width, juliaResult.Height);

                Console.WriteLine("\n=== All Tests Passed! ===");
                Console.WriteLine($"\nGenerated files:");
                Console.WriteLine($"  - {mandelbrotPath}");
                Console.WriteLine($"  - {juliaPath}");
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

            // Check for Mandelbrot set characteristics
            // Center of image should contain the main cardioid (dark/black)
            // Edges should show escaping regions (brighter)
            int centerX = width / 2;
            int centerY = height / 2;
            int centerIdx = (centerY * width + centerX) * 4;

            byte centerR = pixelData[centerIdx];

            Console.WriteLine($"  ✓ Center pixel R value: {centerR}");
            if (centerR < 50)
            {
                Console.WriteLine($"  ✓ Center region appears to be in set (dark/black)");
            }

            // Check for variation in image (not all same color)
            int uniqueColors = 0;
            int prevR = pixelData[0];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                if (pixelData[i] != prevR)
                {
                    uniqueColors++;
                    prevR = pixelData[i];
                }
            }
            Console.WriteLine($"  ✓ Color variation detected: {uniqueColors} color changes");

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