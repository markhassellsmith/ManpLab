using ManpWinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Button clicks and UI event handlers.
    /// </summary>
    public sealed partial class MainPage
    {
        private void MatchWindowSize_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Get the actual size of the fractal display grid (the container for the fractal)
            System.Diagnostics.Debug.WriteLine($"Match Window clicked - FractalDisplayGrid null: {FractalDisplayGrid == null}");

            if (FractalDisplayGrid != null)
            {
                System.Diagnostics.Debug.WriteLine($"FractalDisplayGrid ActualWidth: {FractalDisplayGrid.ActualWidth}, ActualHeight: {FractalDisplayGrid.ActualHeight}");
            }

            if (FractalDisplayGrid?.ActualWidth > 0 && FractalDisplayGrid?.ActualHeight > 0)
            {
                // Use the Grid size as the target resolution
                var width = (int)FractalDisplayGrid.ActualWidth;
                var height = (int)FractalDisplayGrid.ActualHeight;

                // Round to nearest multiple of 16 for better performance
                width = (width / 16) * 16;
                height = (height / 16) * 16;

                System.Diagnostics.Debug.WriteLine($"Setting resolution to: {width}×{height}");
                ViewModel.ImageWidth = width;
                ViewModel.ImageHeight = height;
                ViewModel.StatusMessage = $"Resolution matched to window: {width}×{height}";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Window size not available - dimensions are zero or negative");
                ViewModel.StatusMessage = "Window size not available yet - try resizing the window first";
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

        /// <summary>
        /// Opens a save file dialog with appropriate format choices based on fractal type.
        /// For Hailstone: PNG, JPEG, SVG. For regular fractals: PNG, JPEG.
        /// </summary>
        private async void SaveImageWithDialog_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel.FractalImage == null)
            {
                ViewModel.StatusMessage = "No image to save";
                return;
            }

            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

                // Generate filename based on fractal type
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fractalName = ViewModel.SelectedFractalType.Replace(" ", "");
                var mode = ViewModel.SelectedIterationMode == "Julia" ? "_Julia" : "";
                savePicker.SuggestedFileName = $"{fractalName}{mode}_{timestamp}";

                // Add file type choices based on fractal type
                savePicker.FileTypeChoices.Add("PNG Image", new[] { ".png" });
                savePicker.FileTypeChoices.Add("JPEG Image", new[] { ".jpg", ".jpeg" });

                // Add SVG option for Hailstone fractals
                if (ViewModel.IsHailstoneMode && ViewModel.CurrentHailstoneResult != null)
                {
                    savePicker.FileTypeChoices.Add("SVG Vector Image", new[] { ".svg" });
                    savePicker.SuggestedFileName = $"Hailstone_{timestamp}";
                }

                var file = await savePicker.PickSaveFileAsync();
                if (file == null)
                {
                    ViewModel.StatusMessage = "Save cancelled";
                    return;
                }

                // Determine format from file extension
                var extension = System.IO.Path.GetExtension(file.Name).ToLowerInvariant();

                if (extension == ".svg")
                {
                    // Handle SVG export for Hailstone
                    if (!ViewModel.IsHailstoneMode || ViewModel.CurrentHailstoneResult == null)
                    {
                        ViewModel.StatusMessage = "SVG export is only available for Hailstone sequences";
                        return;
                    }

                    var hailstoneExportService = new HailstoneExportService();
                    var metadata = ViewModel.CreateMetadata();

                    var success = await hailstoneExportService.ExportAsSvgAsync(
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
                        ViewModel.StatusMessage = $"Hailstone sequence saved as SVG: {file.Name}";
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Error saving SVG file";
                    }
                }
                else
                {
                    // Handle PNG/JPEG export
                    var format = extension == ".png" ? ImageFormat.PNG : ImageFormat.JPEG;
                    var metadata = ViewModel.CreateMetadata();

                    // Save directly to the chosen file
                    using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        // Create encoder based on format
                        var encoder = format == ImageFormat.PNG
                            ? await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(
                                Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, stream)
                            : await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(
                                Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId, stream);

                        // Get pixel data from WriteableBitmap
                        var pixelBuffer = ViewModel.FractalImage.PixelBuffer;
                        byte[] pixels = new byte[pixelBuffer.Length];
                        pixelBuffer.CopyTo(pixels);

                        // Set pixel data
                        encoder.SetPixelData(
                            Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                            Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied,
                            (uint)ViewModel.FractalImage.PixelWidth,
                            (uint)ViewModel.FractalImage.PixelHeight,
                            96.0, // DPI X
                            96.0, // DPI Y
                            pixels);

                        // Add metadata
                        var properties = encoder.BitmapProperties;
                        var jsonMetadata = System.Text.Json.JsonSerializer.Serialize(metadata, 
                            new System.Text.Json.JsonSerializerOptions { WriteIndented = false });

                        if (format == ImageFormat.PNG)
                        {
                            // Add PNG tEXt chunks
                            var metadataProps = new[]
                            {
                                new Windows.Graphics.Imaging.BitmapPropertySet
                                {
                                    { "/tEXt/Software", new Windows.Graphics.Imaging.BitmapTypedValue($"{metadata.Software} {metadata.Version}", Windows.Foundation.PropertyType.String) }
                                },
                                new Windows.Graphics.Imaging.BitmapPropertySet
                                {
                                    { "/tEXt/ManpLabMetadata", new Windows.Graphics.Imaging.BitmapTypedValue(jsonMetadata, Windows.Foundation.PropertyType.String) }
                                }
                            };

                            foreach (var propSet in metadataProps)
                            {
                                try { await properties.SetPropertiesAsync(propSet); } catch { }
                            }
                        }

                        await encoder.FlushAsync();
                    }

                    var formatName = format == ImageFormat.PNG ? "PNG" : "JPEG";
                    ViewModel.StatusMessage = $"Image saved as {formatName} with embedded metadata: {file.Name}";
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Error saving image: {ex.Message}";
            }
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
                var exportService = App.Current.Services.GetRequiredService<IImageExportService>();
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
                var exportService = App.Current.Services.GetRequiredService<IImageExportService>();
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
        /// Handles clicking on a navigation history entry to jump to that state.
        /// </summary>
        private async void HistoryEntry_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.Controls.Button button && 
                button.DataContext is ManpWinUI.Models.NavigationHistoryEntry entry)
            {
                // Find the index of this entry in the history
                var index = ViewModel.NavigationHistory.IndexOf(entry);
                if (index >= 0)
                {
                    await ViewModel.JumpToHistoryCommand.ExecuteAsync(index);
                }
            }
        }

        /// <summary>
        /// Handles pointer pressed on zoom fine-tune slider.
        /// Sets dragging flag to prevent rendering during slider manipulation.
        /// </summary>
        private void ZoomSlider_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ViewModel.IsZoomSliderDragging = true;
        }

        /// <summary>
        /// Handles manipulation starting on zoom fine-tune slider.
        /// Ensures dragging flag is set when manipulation begins.
        /// </summary>
        private void ZoomSlider_ManipulationStarting(object sender, Microsoft.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
            ViewModel.IsZoomSliderDragging = true;
        }

        /// <summary>
        /// Handles manipulation started on zoom fine-tune slider.
        /// Ensures dragging flag remains set during manipulation.
        /// </summary>
        private void ZoomSlider_ManipulationStarted(object sender, Microsoft.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            ViewModel.IsZoomSliderDragging = true;
        }

        /// <summary>
        /// Handles pointer released on zoom fine-tune slider.
        /// Triggers single render with final zoom value.
        /// </summary>
        private async void ZoomSlider_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ViewModel.IsZoomSliderDragging = false;
            await ViewModel.ApplyZoomFineTuneAsync();
        }

        /// <summary>
        /// Handles manipulation completed on zoom fine-tune slider.
        /// Triggers single render with final zoom value.
        /// </summary>
        private async void ZoomSlider_ManipulationCompleted(object sender, Microsoft.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            ViewModel.IsZoomSliderDragging = false;
            await ViewModel.ApplyZoomFineTuneAsync();
        }
    }
}

