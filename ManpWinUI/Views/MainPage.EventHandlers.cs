using ManpWinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Button clicks and UI event handlers.
    /// </summary>
    public sealed partial class MainPage
    {
        private void MatchWindowSize_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Get the actual size of the fractal display area (Grid Row 1)
            if (FractalViewbox?.ActualWidth > 0 && FractalViewbox?.ActualHeight > 0)
            {
                // Use the Viewbox size as the target resolution
                var width = (int)FractalViewbox.ActualWidth;
                var height = (int)FractalViewbox.ActualHeight;

                // Round to nearest multiple of 16 for better performance
                width = (width / 16) * 16;
                height = (height / 16) * 16;

                ViewModel.ImageWidth = width;
                ViewModel.ImageHeight = height;
                ViewModel.StatusMessage = $"Resolution matched to window: {width}×{height}";
            }
            else
            {
                ViewModel.StatusMessage = "Window size not available yet";
            }
        }

        private void JuliaPreset1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Classic Julia set - beautiful spiral structure
            ViewModel.JuliaCX = -0.7;
            ViewModel.JuliaCY = 0.27015;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Classic Spiral";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset2_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Dendrite - spectacular tree-like structure
            ViewModel.JuliaCX = -0.8;
            ViewModel.JuliaCY = 0.156;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.5;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Dendrite";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset3_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // San Marco Jewel - intricate circular pattern
            ViewModel.JuliaCX = 0.285;
            ViewModel.JuliaCY = 0.01;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.5;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: San Marco Jewel";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset4_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Paisley/Swirl - organic flowing shapes
            ViewModel.JuliaCX = -0.4;
            ViewModel.JuliaCY = 0.6;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Paisley Swirl";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void HailstonePreset1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Classic cycle - known to produce a cycle
            ViewModel.HailstoneStartX = -10;
            ViewModel.HailstoneStartY = 6;
            ViewModel.HailstoneMaxIterations = 150;
            ViewModel.StatusMessage = "Hailstone preset: Classic Cycle (-10, 6)";
        }

        private void HailstonePreset2_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Origin - fixed point
            ViewModel.HailstoneStartX = 0;
            ViewModel.HailstoneStartY = 0;
            ViewModel.HailstoneMaxIterations = 150;
            ViewModel.StatusMessage = "Hailstone preset: Origin (0, 0) - Fixed point";
        }

        private void HailstonePreset3_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Large test case
            ViewModel.HailstoneStartX = 100;
            ViewModel.HailstoneStartY = 100;
            ViewModel.HailstoneMaxIterations = 500;
            ViewModel.StatusMessage = "Hailstone preset: Large Test (100, 100)";
        }

        private void HailstonePreset4_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Negative coordinates
            ViewModel.HailstoneStartX = -20;
            ViewModel.HailstoneStartY = -30;
            ViewModel.HailstoneMaxIterations = 200;
            ViewModel.StatusMessage = "Hailstone preset: Negative (-20, -30)";
        }

        private async void SaveImage_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Default to PNG when clicking main button
            await SaveImageAsync(ImageFormat.PNG);
        }

        private async void SavePNG_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await SaveImageAsync(ImageFormat.PNG);
        }

        private async void SaveJPEG_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await SaveImageAsync(ImageFormat.JPEG);
        }

        private async void SaveSVG_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (!ViewModel.IsHailstoneMode || ViewModel.CurrentHailstoneResult == null)
            {
                ViewModel.StatusMessage = "SVG export is only available for Hailstone sequences";
                return;
            }

            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.SuggestedFileName = $"Hailstone_{DateTime.Now:yyyyMMdd_HHmmss}";
                savePicker.FileTypeChoices.Add("SVG Image", new[] { ".svg" });

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    var exportService = new HailstoneExportService();
                    var metadata = ViewModel.CreateMetadata();

                    var success = await exportService.ExportAsSvgAsync(
                        ViewModel.CurrentHailstoneResult,
                        ViewModel.HailstoneScaleX,
                        ViewModel.HailstoneScaleY,
                        ViewModel.HailstoneOffsetX,
                        ViewModel.HailstoneOffsetY,
                        ViewModel.ImageWidth,
                        ViewModel.ImageHeight,
                        file.Path,
                        metadata);

                    if (success)
                    {
                        ViewModel.StatusMessage = $"Hailstone sequence saved as SVG with metadata: {file.Name}";
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Error saving SVG file";
                    }
                }
                else
                {
                    ViewModel.StatusMessage = "Save cancelled";
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Error saving SVG: {ex.Message}";
            }
        }

        private async void CopyToClipboard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel.FractalImage == null)
            {
                ViewModel.StatusMessage = "No image to copy";
                return;
            }

            try
            {
                var exportService = App.Current.Services.GetRequiredService<ImageExportService>();
                var metadata = ViewModel.CreateMetadata();

                await exportService.CopyToClipboardAsync(ViewModel.FractalImage, metadata);

                ViewModel.StatusMessage = "Image copied to clipboard with metadata!";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Error copying to clipboard: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task SaveImageAsync(ImageFormat format)
        {
            if (ViewModel.FractalImage == null)
            {
                ViewModel.StatusMessage = "No image to save";
                return;
            }

            try
            {
                var exportService = App.Current.Services.GetRequiredService<ImageExportService>();
                var metadata = ViewModel.CreateMetadata();

                // Get window handle for file picker
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);

                var saved = await exportService.SaveImageAsync(
                    ViewModel.FractalImage,
                    metadata,
                    format,
                    hwnd);

                if (saved)
                {
                    var formatName = format == ImageFormat.PNG ? "PNG" : "JPEG";
                    ViewModel.StatusMessage = $"Image saved as {formatName} with embedded metadata!";
                }
                else
                {
                    ViewModel.StatusMessage = "Save cancelled";
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Error saving image: {ex.Message}";
            }
        }

        private void FractalViewbox_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            // Update Hailstone labels when the viewbox size changes
            UpdateHailstoneLabels();
        }

        private void HailstoneLabelsCanvas_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            // Update Hailstone labels when the canvas size changes (window resize)
            UpdateHailstoneLabels();
        }

        /// <summary>
        /// Win2D validation test button handler.
        /// Creates a test image to verify Win2D rendering is working.
        /// This is a temporary button for validation during Win2D integration.
        /// </summary>
        private void TestWin2D_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("\n=== Win2D Test Button Clicked ===");
            ViewModel.StatusMessage = "Testing Win2D rendering...";

            try
            {
                // Create test bitmap
                var testBitmap = Win2DValidationTest.CreateTestBitmap(800, 600);

                if (testBitmap != null)
                {
                    // Display the test image
                    ViewModel.FractalImage = testBitmap;
                    ViewModel.StatusMessage = "✓ Win2D test passed! GPU-accelerated rendering working.";
                    System.Diagnostics.Debug.WriteLine("✓ Win2D test image displayed successfully!");
                }
                else
                {
                    ViewModel.StatusMessage = "✗ Win2D test failed - check debug output";
                    System.Diagnostics.Debug.WriteLine("✗ Win2D test failed!");
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"✗ Win2D test error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"✗ Win2D test exception: {ex}");
            }
        }

        /// <summary>
        /// Test Win2D Hailstone rendering.
        /// Renders current Hailstone sequence using Win2D renderer instead of legacy renderer.
        /// </summary>
        private async void TestWin2DHailstone_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("\n=== Testing Win2D Hailstone Rendering ===");
            ViewModel.StatusMessage = "Testing Win2D Hailstone renderer...";

            try
            {
                var win2dService = App.Current.Services.GetRequiredService<HailstoneRenderServiceWin2D>();
                var hailstoneService = App.Current.Services.GetRequiredService<IHailstoneService>();

                // Calculate sequence
                var result = await hailstoneService.CalculateSequenceAsync(
                    ViewModel.HailstoneStartX,
                    ViewModel.HailstoneStartY,
                    ViewModel.HailstoneMaxIterations,
                    colorSpread: 7,
                    exportToCsv: false);

                ViewModel.StatusMessage = $"Rendering with Win2D ({result.Sequence.Count} points)...";

                // Render with Win2D
                var renderResult = await win2dService.RenderSequenceAsync(
                    result,
                    ViewModel.ImageWidth,
                    ViewModel.ImageHeight,
                    ViewModel.ShowHailstoneAxes,
                    ViewModel.ShowHailstonePoints,
                    ViewModel.ShowHailstoneLabels,
                    ViewModel.UseFixedHailstoneViewport);

                // Display result
                ViewModel.FractalImage = renderResult.Bitmap;
                ViewModel.CurrentHailstoneResult = result;
                ViewModel.HailstoneScaleX = renderResult.ScaleX;
                ViewModel.HailstoneScaleY = renderResult.ScaleY;
                ViewModel.HailstoneOffsetX = renderResult.OffsetX;
                ViewModel.HailstoneOffsetY = renderResult.OffsetY;

                string cycleInfo = result.HasCycle
                    ? $" | Cycle at step {result.CycleStartIndex} (length {result.CycleLength})"
                    : " | No cycle";

                ViewModel.StatusMessage = $"✓ Win2D Hailstone: {result.Sequence.Count} points{cycleInfo} | NO Canvas overlays needed!";
                System.Diagnostics.Debug.WriteLine("✓ Win2D Hailstone rendering successful!");
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"✗ Win2D Hailstone error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"✗ Win2D Hailstone exception: {ex}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
        }
    }
}
