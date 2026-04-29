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
    [RelayCommand]
    private async Task RenderMandelbrotAsync()
    {
        // Guard: Don't start a new render if already rendering
        if (IsRendering)
        {
            System.Diagnostics.Debug.WriteLine("[RenderMandelbrotAsync] Already rendering - ignoring request");
            return;
        }

        // Create a new cancellation token for this render
        _renderCancellationSource?.Dispose();
        _renderCancellationSource = new CancellationTokenSource();
        var cancellationToken = _renderCancellationSource.Token;

        var fractalName = IsJuliaMode ? $"{SelectedFractalType} Julia" : SelectedFractalType;

        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = true;
            RenderProgress = 0;
            IsBookmarksPanelOpen = false;
            StatusMessage = IsJuliaMode 
                ? $"Rendering {fractalName} set (c = {JuliaCX:F4}, {JuliaCY:F4})..." 
                : $"Rendering {fractalName} set...";
        });

        // Auto-scale iterations based on zoom if enabled
        if (AutoScaleIterations)
        {
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                var oldIterations = MaxIterations;
                _dispatcherQueue.TryEnqueue(() =>
                {
                    MaxIterations = recommendedIterations;
                    IterationSuggestion = $"Auto-increased iterations: {oldIterations} → {recommendedIterations} for zoom {Zoom:F2}x";
                });
            }
            else
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    IterationSuggestion = $"Using {MaxIterations} iterations at zoom {Zoom:F2}x";
                });
            }
        }
        else
        {
            // Check if user might need more iterations
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    IterationSuggestion = $"⚠️ Consider increasing iterations to ~{recommendedIterations} for better detail at zoom {Zoom:F2}x (currently {MaxIterations})";
                });
            }
            else
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    IterationSuggestion = string.Empty;
                });
            }
        }

        try
        {
            var startTime = DateTime.Now;

            // Progress reporting callback - ALWAYS use dispatcher to avoid COM exceptions
            // Progress updates are always posted, even if paused (pause now cancels the render)
            Action<double> progressCallback = percentage =>
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RenderProgress = percentage * 100.0;
                });
            };
            var progress = new Progress<double>(progressCallback);

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

            // Check if render was cancelled
            if (cancellationToken.IsCancellationRequested)
            {
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

            var renderTime = DateTime.Now - startTime;

            // Show diagnostic info if escape percentage is very low
            var escapePercent = result.EscapePercentage;

            // DETAILED DIAGNOSTIC LOGGING
            var logMessage = $@"
───────────────────────────────────────────────────────────────
RENDER COMPLETE - DIAGNOSTIC INFO
───────────────────────────────────────────────────────────────
Resolution: {ImageWidth} × {ImageHeight} ({result.TotalIterations:N0} total iterations)
Render time: {renderTime.TotalMilliseconds:F0} ms
Escape stats: {result.EscapedPixels:N0} / {ImageWidth * ImageHeight:N0} pixels ({escapePercent:F2}%)
Max iterations: {MaxIterations}
Zoom: {Zoom:F4}x
Center: ({CenterX:F10}, {CenterY:F10})
View dimensions: {3.0 / Zoom:F10} × {(3.0 / Zoom) * ((double)ImageHeight / ImageWidth):F10}
───────────────────────────────────────────────────────────────
";
            System.Diagnostics.Debug.WriteLine(logMessage);

            // Update UI-bound properties on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                LastRenderTime = renderTime;

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
                    StatusMessage = $"Rendered in {renderTime.TotalMilliseconds:F0} ms ({escapePercent:F1}% escaped)";
                }
            });
        }
        catch (Exception ex)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Error: {ex.Message}";
            });
        }
        finally
        {
            // Reset state on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsRendering = false;
            });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // HAILSTONE RENDERING
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Renders the Hailstone 2D sequence with current parameters.
    /// </summary>
    [RelayCommand]
    private async Task RenderHailstoneAsync()
    {
        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Called - IsHailstoneMode={IsHailstoneMode}");

        if (!IsHailstoneMode)
        {
            System.Diagnostics.Debug.WriteLine("[RenderHailstoneAsync] EARLY EXIT - Not in Hailstone mode!");
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = "Please select Hailstone fractal type first.";
            });
            return;
        }

        // Guard: Don't start a new render if already rendering
        if (IsRendering)
        {
            System.Diagnostics.Debug.WriteLine("[RenderHailstoneAsync] Already rendering - ignoring request");
            return;
        }

        // Create a new cancellation token for this render
        _renderCancellationSource?.Dispose();
        _renderCancellationSource = new CancellationTokenSource();
        var cancellationToken = _renderCancellationSource.Token;

        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = true;
            RenderProgress = 0;
            IsBookmarksPanelOpen = false;
            StatusMessage = $"Calculating Hailstone sequence from ({HailstoneStartX}, {HailstoneStartY})...";
        });

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

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Rendering Hailstone sequence ({result.Sequence.Count} points)...";
                RenderProgress = 50;
            });

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

            // Check if render was cancelled
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

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

            var renderTime = DateTime.Now - startTime;

            // Build status message
            string cycleInfo = result.HasCycle
                ? $" | Cycle detected at step {result.CycleStartIndex} (length {result.CycleLength})"
                : " | No cycle detected";

            _dispatcherQueue.TryEnqueue(() =>
            {
                LastRenderTime = renderTime;
                RenderProgress = 100;
                StatusMessage = $"Rendered {result.Sequence.Count} points in {renderTime.TotalMilliseconds:F0} ms{cycleInfo}";
            });
        }
        catch (Exception ex)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Error: {ex.Message}";
            });
            System.Diagnostics.Debug.WriteLine($"Hailstone render error: {ex}");
        }
        finally
        {
            // Reset state on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsRendering = false;
            });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // UNIFIED RENDER COMMAND
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Unified render command that routes to the appropriate render method.
    /// </summary>
    [RelayCommand]
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

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDER CONTROL
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Stops the current rendering operation.
    /// Clears intermediate computations and returns to pre-render state.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStopRender))]
    private void StopRender()
    {
        System.Diagnostics.Debug.WriteLine("[StopRender] Called - Cancelling render");

        // Cancel the render operation
        _renderCancellationSource?.Cancel();

        // Reset all rendering state on UI thread
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = false;
            RenderProgress = 0;
            StatusMessage = "Rendering stopped by user";
            System.Diagnostics.Debug.WriteLine("[StopRender] State reset complete");
        });
    }

    /// <summary>
    /// Determines whether stop can be executed (rendering is active).
    /// </summary>
    private bool CanStopRender() => IsRendering;
}
