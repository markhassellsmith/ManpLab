using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Rendering commands.
/// Handles all fractal rendering operations (Mandelbrot, Hailstone, unified render).
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // MANDELBROT/JULIA RENDERING
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Renders the Mandelbrot set with current parameters.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderMandelbrotAsync()
    {
        IsRendering = true;
        RenderProgress = 0;
        var fractalName = IsJuliaMode ? $"{SelectedFractalType} Julia" : SelectedFractalType;
        StatusMessage = IsJuliaMode 
            ? $"Rendering {fractalName} set (c = {JuliaCX:F4}, {JuliaCY:F4})..." 
            : $"Rendering {fractalName} set...";

        // Auto-scale iterations based on zoom if enabled
        if (AutoScaleIterations)
        {
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                var oldIterations = MaxIterations;
                MaxIterations = recommendedIterations;
                IterationSuggestion = $"Auto-increased iterations: {oldIterations} → {MaxIterations} for zoom {Zoom:F2}x";
            }
            else
            {
                IterationSuggestion = $"Using {MaxIterations} iterations at zoom {Zoom:F2}x";
            }
        }
        else
        {
            // Check if user might need more iterations
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                IterationSuggestion = $"⚠️ Consider increasing iterations to ~{recommendedIterations} for better detail at zoom {Zoom:F2}x (currently {MaxIterations})";
            }
            else
            {
                IterationSuggestion = string.Empty;
            }
        }

        try
        {
            var startTime = DateTime.Now;

            // Progress reporting callback
            var progress = new Progress<double>(percentage =>
            {
                try
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        RenderProgress = percentage * 100.0;
                    });
                }
                catch (InvalidCastException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"InvalidCastException in Progress callback: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            });

            // Call FractalRenderService to render the fractal
            FractalRenderResult result;
            try
            {
                result = await _renderService.RenderMandelbrotAsync(
                    CenterX,
                    CenterY,
                    Zoom,
                    ImageWidth,
                    ImageHeight,
                    MaxIterations,
                    SelectedPalette,
                    SelectedFractalType,
                    IsJuliaMode,
                    JuliaCX,
                    JuliaCY,
                    progress);
            }
            catch (InvalidCastException ex)
            {
                StatusMessage = $"InvalidCastException during render: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"InvalidCastException in RenderMandelbrotAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return;
            }

            // Convert byte[] to WriteableBitmap on UI thread
            try
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        ConvertPixelDataToBitmap(result.PixelData, ImageWidth, ImageHeight);
                    }
                    catch (InvalidCastException ex)
                    {
                        StatusMessage = $"InvalidCastException in ConvertPixelDataToBitmap: {ex.Message}";
                        System.Diagnostics.Debug.WriteLine($"InvalidCastException in ConvertPixelDataToBitmap: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                });
            }
            catch (InvalidCastException ex)
            {
                StatusMessage = $"InvalidCastException in TryEnqueue: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"InvalidCastException in TryEnqueue: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            LastRenderTime = DateTime.Now - startTime;

            // Show diagnostic info if escape percentage is very low
            var escapePercent = result.EscapePercentage;

            // DETAILED DIAGNOSTIC LOGGING
            var logMessage = $@"
───────────────────────────────────────────────────────────────
RENDER COMPLETE - DIAGNOSTIC INFO
───────────────────────────────────────────────────────────────
Resolution: {ImageWidth} × {ImageHeight} ({result.TotalIterations:N0} total iterations)
Render time: {LastRenderTime.TotalMilliseconds:F0} ms
Escape stats: {result.EscapedPixels:N0} / {ImageWidth * ImageHeight:N0} pixels ({escapePercent:F2}%)
Max iterations: {MaxIterations}
Zoom: {Zoom:F4}x
Center: ({CenterX:F10}, {CenterY:F10})
View dimensions: {3.0 / Zoom:F10} × {(3.0 / Zoom) * ((double)ImageHeight / ImageWidth):F10}
───────────────────────────────────────────────────────────────
";
            System.Diagnostics.Debug.WriteLine(logMessage);

            if (escapePercent < 1.0)
            {
                StatusMessage = $"⚠️ Only {escapePercent:F2}% of pixels escaped - You're inside the Mandelbrot set! Zoom to the boundary for detail.";
            }
            else if (escapePercent < 10.0)
            {
                StatusMessage = $"Low detail: {escapePercent:F1}% escaped - Try zooming to colorful boundaries";
            }
            else
            {
                StatusMessage = $"Rendered in {LastRenderTime.TotalMilliseconds:F0} ms ({escapePercent:F1}% escaped)";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsRendering = false;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // HAILSTONE RENDERING
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Renders the Hailstone 2D sequence with current parameters.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderHailstoneAsync()
    {
        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Called - IsHailstoneMode={IsHailstoneMode}");

        if (!IsHailstoneMode)
        {
            System.Diagnostics.Debug.WriteLine("[RenderHailstoneAsync] EARLY EXIT - Not in Hailstone mode!");
            StatusMessage = "Please select Hailstone fractal type first.";
            return;
        }

        IsRendering = true;
        RenderProgress = 0;
        StatusMessage = $"Calculating Hailstone sequence from ({HailstoneStartX}, {HailstoneStartY})...";

        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Starting render - ({HailstoneStartX}, {HailstoneStartY}), MaxIter={HailstoneMaxIterations}");

        try
        {
            var startTime = DateTime.Now;

            // Calculate sequence with color spread and optional CSV export
            var result = await _hailstoneService.CalculateSequenceAsync(
                HailstoneStartX,
                HailstoneStartY,
                HailstoneMaxIterations,
                colorSpread: 7,  // Default color spread
                exportToCsv: false);  // Set to true if you want CSV export

            System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Sequence calculated - {result.Sequence.Count} points");

            StatusMessage = $"Rendering Hailstone sequence ({result.Sequence.Count} points)...";
            RenderProgress = 50;

            // Render to bitmap with optional custom viewport
            var renderResult = await _hailstoneRenderService.RenderSequenceAsync(
                result,
                ImageWidth,
                ImageHeight,
                ShowHailstoneAxes,
                ShowHailstonePoints,
                ShowHailstoneLabels,
                UseFixedHailstoneViewport,
                HailstoneViewportMinX,
                HailstoneViewportMaxX,
                HailstoneViewportMinY,
                HailstoneViewportMaxY);

            System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Got render result with {(renderResult.PixelData != null ? renderResult.PixelData.Length : 0)} bytes of pixel data");

            // Create bitmap on UI thread (WriteableBitmap must be created on UI thread!)
            WriteableBitmap? bitmap = null;
            _dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (renderResult.PixelData != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Creating WriteableBitmap on UI thread ({renderResult.Width}x{renderResult.Height})");
                        bitmap = new WriteableBitmap(renderResult.Width, renderResult.Height);
                        using (var stream = bitmap.PixelBuffer.AsStream())
                        {
                            stream.Write(renderResult.PixelData, 0, renderResult.PixelData.Length);
                        }
                        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Bitmap created successfully");
                    }
                    else if (renderResult.Bitmap != null)
                    {
                        // Legacy path: bitmap already created
                        bitmap = renderResult.Bitmap;
                    }

                    FractalImage = bitmap;
                    CurrentHailstoneResult = result;
                    HailstoneScaleX = renderResult.ScaleX;
                    HailstoneScaleY = renderResult.ScaleY;
                    HailstoneOffsetX = renderResult.OffsetX;
                    HailstoneOffsetY = renderResult.OffsetY;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] ERROR creating bitmap on UI thread: {ex.Message}");
                    StatusMessage = $"Error creating bitmap: {ex.Message}";
                }
            });

            LastRenderTime = DateTime.Now - startTime;
            RenderProgress = 100;

            // Build status message
            string cycleInfo = result.HasCycle
                ? $" | Cycle detected at step {result.CycleStartIndex} (length {result.CycleLength})"
                : " | No cycle detected";

            StatusMessage = $"Rendered {result.Sequence.Count} points in {LastRenderTime.TotalMilliseconds:F0} ms{cycleInfo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Hailstone render error: {ex}");
        }
        finally
        {
            IsRendering = false;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // UNIFIED RENDER COMMAND
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Unified render command that routes to the appropriate render method.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderAsync()
    {
        System.Diagnostics.Debug.WriteLine($"[RenderAsync] Called - IsHailstoneMode={IsHailstoneMode}, SelectedFractalType={SelectedFractalType}");

        if (IsHailstoneMode)
        {
            System.Diagnostics.Debug.WriteLine("[RenderAsync] Routing to RenderHailstoneAsync");
            await RenderHailstoneAsync();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[RenderAsync] Routing to RenderMandelbrotAsync");
            await RenderMandelbrotAsync();
        }
    }
}
