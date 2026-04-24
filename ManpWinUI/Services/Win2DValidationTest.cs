using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using System.Diagnostics;

namespace ManpWinUI.Services
{
    /// <summary>
    /// Simple test to validate Win2D rendering is working correctly.
    /// Creates a test image with various drawing operations.
    /// </summary>
    public static class Win2DValidationTest
    {
        /// <summary>
        /// Creates a test bitmap using Win2D renderer to verify functionality.
        /// Draws lines, text, circles, and rectangles to validate all features.
        /// </summary>
        /// <param name="width">Width of test image in pixels.</param>
        /// <param name="height">Height of test image in pixels.</param>
        /// <returns>WriteableBitmap containing test rendering, or null if Win2D failed.</returns>
        public static WriteableBitmap? CreateTestBitmap(int width = 800, int height = 600)
        {
            try
            {
                Debug.WriteLine("=== Win2D Validation Test Starting ===");
                var stopwatch = Stopwatch.StartNew();

                // Check if Win2D backend is available
                if (!GraphicsRendererFactory.IsBackendAvailable(GraphicsBackend.Win2D))
                {
                    Debug.WriteLine("ERROR: Win2D backend not available!");
                    return null;
                }

                Debug.WriteLine($"Creating Win2D renderer ({width}x{height})...");
                using var renderer = GraphicsRendererFactory.Create(width, height, GraphicsBackend.Win2D);

                // Test 1: Clear background
                Debug.WriteLine("Test 1: Clear background (black)");
                renderer.Clear(Colors.Black);

                // Test 2: Draw colored lines (crossing pattern)
                Debug.WriteLine("Test 2: Draw diagonal lines");
                renderer.DrawLine(0, 0, width, height, Colors.Cyan, 2.0f);      // Diagonal cyan
                renderer.DrawLine(width, 0, 0, height, Colors.Magenta, 2.0f);  // Diagonal magenta

                // Test 3: Draw grid lines
                Debug.WriteLine("Test 3: Draw grid pattern");
                var gridColor = Color.FromArgb(100, 100, 100, 100); // Semi-transparent gray
                for (int x = 0; x < width; x += 100)
                {
                    renderer.DrawLine(x, 0, x, height, gridColor, 1.0f);
                }
                for (int y = 0; y < height; y += 100)
                {
                    renderer.DrawLine(0, y, width, y, gridColor, 1.0f);
                }

                // Test 4: Draw circles
                Debug.WriteLine("Test 4: Draw colored circles");
                renderer.DrawCircle(width / 4, height / 4, 50, Colors.Red);
                renderer.DrawCircle(3 * width / 4, height / 4, 50, Colors.Green);
                renderer.DrawCircle(width / 4, 3 * height / 4, 50, Colors.Blue);
                renderer.DrawCircle(3 * width / 4, 3 * height / 4, 50, Colors.Yellow);

                // Test 5: Draw rectangles
                Debug.WriteLine("Test 5: Draw semi-transparent rectangles");
                var rectColor = Color.FromArgb(128, 255, 128, 0); // Orange, 50% transparent
                renderer.DrawRectangle(width / 2 - 100, height / 2 - 100, 200, 200, rectColor);

                // Test 6: Draw text (THE MAIN TEST!)
                Debug.WriteLine("Test 6: Draw text at various sizes");
                renderer.DrawText("Win2D Test!", 50, 50, Colors.Yellow, 48f, "Arial", true);
                renderer.DrawText("GPU-Accelerated Rendering", 50, 120, Colors.Cyan, 24f, "Arial", false);
                renderer.DrawText("Text sizes: 12pt", 50, 160, Colors.White, 12f, "Arial", false);
                renderer.DrawText("Text sizes: 18pt", 50, 180, Colors.White, 18f, "Arial", false);
                renderer.DrawText("Text sizes: 24pt", 50, 205, Colors.White, 24f, "Arial", false);

                // Test 7: Draw coordinate labels (like axis labels)
                Debug.WriteLine("Test 7: Draw small coordinate-style labels");
                for (int x = 100; x < width; x += 100)
                {
                    renderer.DrawText(x.ToString(), x - 10, height - 30, Colors.LightGray, 10f);
                }
                for (int y = 100; y < height; y += 100)
                {
                    renderer.DrawText(y.ToString(), 10, y - 5, Colors.LightGray, 10f);
                }

                // Test 8: Alpha blending test
                Debug.WriteLine("Test 8: Alpha blending test");
                renderer.SetAlpha(128); // 50% transparent
                renderer.DrawCircle(width / 2, height / 2, 80, Colors.White);
                renderer.SetAlpha(255); // Back to opaque

                // Test 9: Draw status text
                Debug.WriteLine("Test 9: Draw status info");
                renderer.DrawText($"Resolution: {width}×{height}", width - 250, height - 80, 
                    Colors.LightGreen, 14f, "Arial", false);
                renderer.DrawText("Backend: Win2D (DirectX)", width - 250, height - 60, 
                    Colors.LightGreen, 14f, "Arial", false);
                renderer.DrawText("Graphics Abstraction Layer", width - 250, height - 40, 
                    Colors.LightGreen, 14f, "Arial", false);
                renderer.DrawText("✓ All tests passed", width - 250, height - 20, 
                    Colors.Lime, 14f, "Arial", true);

                // Convert to bitmap
                Debug.WriteLine("Converting to WriteableBitmap...");
                var bitmap = renderer.ToWriteableBitmap();

                stopwatch.Stop();
                Debug.WriteLine($"=== Win2D Validation Test Complete ({stopwatch.ElapsedMilliseconds}ms) ===");
                Debug.WriteLine("SUCCESS: Win2D rendering functional!");

                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Win2D validation test failed: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Quick test that can be called from UI to display validation result.
        /// </summary>
        public static bool RunQuickTest()
        {
            Debug.WriteLine("\n========================================");
            Debug.WriteLine("Win2D Quick Validation Test");
            Debug.WriteLine("========================================");

            var bitmap = CreateTestBitmap(400, 300);

            if (bitmap != null)
            {
                Debug.WriteLine("✓ Test PASSED - Win2D is working!");
                Debug.WriteLine($"✓ Created {bitmap.PixelWidth}×{bitmap.PixelHeight} test bitmap");
                Debug.WriteLine("========================================\n");
                return true;
            }
            else
            {
                Debug.WriteLine("✗ Test FAILED - Win2D not working!");
                Debug.WriteLine("========================================\n");
                return false;
            }
        }
    }
}
