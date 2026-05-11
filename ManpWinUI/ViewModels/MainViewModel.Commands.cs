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
            return;
        }

        // Create a new cancellation token for this render
        _renderCancellationSource?.Dispose();
        _renderCancellationSource = new CancellationTokenSource();
        var cancellationToken = _renderCancellationSource.Token;

        var fractalName = IsJuliaMode ? $"{SelectedFractalType} Julia" : SelectedFractalType;

        // Calculate viewport width to determine if arbitrary precision is needed
        double viewWidth = 3.0 / Zoom;

        // Pre-calculate deep zoom status for early display
        bool userRequestedDeepZoom = _renderSettingsViewModel.UseDeepZoom;

        // Deep zoom threshold: When viewport width drops below 1e-12, double precision
        // loses significant digits and arbitrary precision (BigDouble/perturbation) is required
        const double VIEWPORT_PRECISION_LIMIT = 1e-12;
        bool needsArbitraryPrecision = (viewWidth < VIEWPORT_PRECISION_LIMIT);
        bool willUseDeepZoom = userRequestedDeepZoom && needsArbitraryPrecision;

        string deepZoomPrefix = willUseDeepZoom ? "[Deep Zoom] " : "";

        // Log precision status for diagnostics
        if (needsArbitraryPrecision && !userRequestedDeepZoom)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] Viewport width {viewWidth:E2} requires arbitrary precision, but deep zoom is disabled!");
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = true;
            RenderProgress = 0;
            IsBookmarksPanelOpen = false;
            StatusMessage = IsJuliaMode 
                ? $"{deepZoomPrefix}Rendering {fractalName} set (c = {JuliaCX:F4}, {JuliaCY:F4})..." 
                : $"{deepZoomPrefix}Rendering {fractalName} set...";
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

            // Progress reporting callback - always enqueue to UI thread to avoid COM exceptions
            Action<double> progressCallback = percentage =>
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RenderProgress = percentage * 100.0;
                });
            };
            var progress = new Progress<double>(progressCallback);

            // ═════════════════════════════════════════════════════════════════════════
            // TASK 6: Use parameter system if available (new architecture)
            // ═════════════════════════════════════════════════════════════════════════
            FractalRenderResult result;
            bool shouldUseDeepZoom = false; // Track whether deep zoom was actually used

            if (CurrentParameters != null && UseParameterSystem)
            {
                System.Diagnostics.Debug.WriteLine("[RenderCommand] Using PARAMETER SYSTEM for render");

                // Create structured parameters from parameter set
                var renderParams = CurrentParameters.ToStructuredRenderParameters(ImageWidth, ImageHeight);

                // Override view/camera settings from ViewModel (these are UI state, not fractal-specific parameters)
                renderParams.CenterX = CenterX;
                renderParams.CenterY = CenterY;
                renderParams.Zoom = Zoom;

                // Override color settings from ViewModel (not part of parameter set)
                renderParams.Palette = SelectedPalette;
                renderParams.ColorCycleSpeed = ColorCycleSpeed;
                renderParams.ColorOffset = ColorOffset;
                renderParams.UseSmoothColoring = UseSmoothColoring;

                // Week 9 Task 1: Deep zoom toggle with automatic optimization
                // Deep zoom activates when viewport width requires arbitrary precision
                shouldUseDeepZoom = false; // Default to disabled

                // Auto-enable deep zoom when viewport width requires arbitrary precision
                if (userRequestedDeepZoom && needsArbitraryPrecision)
                {
                    shouldUseDeepZoom = true;
                    System.Diagnostics.Debug.WriteLine($"[DeepZoom] ENABLED: viewport width {viewWidth:E2} < {VIEWPORT_PRECISION_LIMIT:E2} (arbitrary precision required)");
                }
                else if (userRequestedDeepZoom && !needsArbitraryPrecision)
                {
                    shouldUseDeepZoom = false;
                    System.Diagnostics.Debug.WriteLine($"[Optimization] Deep zoom not needed: viewport width {viewWidth:E2} >= {VIEWPORT_PRECISION_LIMIT:E2}");
                    System.Diagnostics.Debug.WriteLine($"[Optimization] Using double precision (50-100x faster)");
                }
                else if (!userRequestedDeepZoom && needsArbitraryPrecision)
                {
                    shouldUseDeepZoom = false;
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Viewport width {viewWidth:E2} needs arbitrary precision, but deep zoom is DISABLED!");
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Image may show precision artifacts or solid colors");
                }

                renderParams.UseDeepZoom = shouldUseDeepZoom;
                System.Diagnostics.Debug.WriteLine($"[RenderCommand] Deep Zoom Setting: {shouldUseDeepZoom} (User requested: {userRequestedDeepZoom}, Zoom: {Zoom:E2})");

                // Call new parameter-based render method
                result = await _renderService.RenderFractalAsync(
                    renderParams,
                    progress);

                System.Diagnostics.Debug.WriteLine("[RenderCommand] Parameter-based render completed");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[RenderCommand] Using LEGACY property-based render (fallback)");

                // Week 9 Task 1: Deep zoom toggle with automatic optimization (legacy path)
                // Deep zoom activates when viewport width requires arbitrary precision
                shouldUseDeepZoom = false; // Default to disabled

                // Auto-enable deep zoom when viewport width requires arbitrary precision
                if (userRequestedDeepZoom && needsArbitraryPrecision)
                {
                    shouldUseDeepZoom = true;
                    System.Diagnostics.Debug.WriteLine($"[DeepZoom] ENABLED (Legacy): viewport width {viewWidth:E2} < {VIEWPORT_PRECISION_LIMIT:E2}");
                }
                else if (userRequestedDeepZoom && !needsArbitraryPrecision)
                {
                    shouldUseDeepZoom = false;
                    System.Diagnostics.Debug.WriteLine($"[Optimization] Deep zoom not needed (Legacy): viewport width {viewWidth:E2} >= {VIEWPORT_PRECISION_LIMIT:E2}");
                }
                else if (!userRequestedDeepZoom && needsArbitraryPrecision)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Viewport width {viewWidth:E2} needs arbitrary precision, but deep zoom is DISABLED!");
                }

                System.Diagnostics.Debug.WriteLine($"[RenderCommand] Deep Zoom Setting (Legacy): {shouldUseDeepZoom} (User requested: {userRequestedDeepZoom}, ViewWidth: {viewWidth:E2})");

                // Fallback: use old individual-property method
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
                    ColorCycleSpeed,
                    ColorOffset,
                    UseSmoothColoring,
                    shouldUseDeepZoom,
                    progress);
            }

            // Check if render was cancelled
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Convert byte[] to WriteableBitmap on UI thread (MUST be on UI thread for WinRT)
            var enqueued = _dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[RenderCommand] Converting pixel data to bitmap on UI thread");
                    ConvertPixelDataToBitmap(result.PixelData, ImageWidth, ImageHeight);
                }
                catch (Exception bitmapEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[RenderCommand] Bitmap conversion failed: {bitmapEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"[RenderCommand] Stack trace: {bitmapEx.StackTrace}");
                    StatusMessage = $"Error creating image: {bitmapEx.Message}";
                }
            });

            if (!enqueued)
            {
                System.Diagnostics.Debug.WriteLine("[RenderCommand] WARNING: Failed to enqueue bitmap conversion to UI thread!");
                _dispatcherQueue.TryEnqueue(() =>
                {
                    StatusMessage = "Error: Failed to update image (dispatcher error)";
                });
            }

            var renderTime = DateTime.Now - startTime;
            var escapePercent = result.EscapePercentage;

            // Update UI-bound properties on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                LastRenderTime = renderTime;

                // Build status message with optional Deep Zoom indicator
                string deepZoomIndicator = shouldUseDeepZoom ? " | Deep Zoom mode" : "";

                if (escapePercent < 1.0)
                {
                    StatusMessage = $"⚠️ Only {escapePercent:F2}% of pixels escaped - You're inside the Mandelbrot set! Zoom to the boundary for detail.{deepZoomIndicator}";
                }
                else if (escapePercent < 10.0)
                {
                    StatusMessage = $"Low detail: {escapePercent:F1}% escaped - Try zooming to colorful boundaries{deepZoomIndicator}";
                }
                else
                {
                    StatusMessage = $"Rendered in {renderTime.TotalSeconds:F4} s ({escapePercent:F1}% escaped){deepZoomIndicator}";
                }

                // Record navigation state after successful render
                RecordNavigationState();
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
        if (!IsHailstoneMode)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = "Please select Hailstone fractal type first.";
            });
            return;
        }

        // Guard: Don't start a new render if already rendering
        if (IsRendering)
        {
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

        try
        {
            var startTime = DateTime.Now;

            // Calculate sequence with color spread and optional CSV export
            var result = await _hailstoneService.CalculateSequenceAsync(
                HailstoneStartX,
                HailstoneStartY,
                HailstoneMaxIterations,
                colorSpread: 7,
                exportToCsv: false);

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
                        bitmap = new WriteableBitmap(renderResult.Width, renderResult.Height);
                        using (var stream = bitmap.PixelBuffer.AsStream())
                        {
                            stream.Write(renderResult.PixelData, 0, renderResult.PixelData.Length);
                        }
                    }
                    else if (renderResult.Bitmap != null)
                    {
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
                StatusMessage = $"Rendered {result.Sequence.Count} points in {renderTime.TotalSeconds:F4} s{cycleInfo}";

                // Record navigation state after successful render
                RecordNavigationState();
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
    // UNIFIED RENDER COMMAND
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Unified render command that routes to the appropriate render method.
    /// Disabled while rendering is in progress.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderAsync()
    {
        if (IsHailstoneMode)
        {
            await RenderHailstoneAsync();
        }
        else
        {
            await RenderMandelbrotAsync();
        }
    }

    /// <summary>
    /// Determines whether render can be executed (not currently rendering).
    /// </summary>
    private bool CanRender() => !IsRendering;

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
        _renderCancellationSource?.Cancel();

        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = false;
            RenderProgress = 0;
            StatusMessage = "Rendering stopped by user";
        });
    }

    /// <summary>
    /// Determines whether stop can be executed (rendering is active).
    /// </summary>
    private bool CanStopRender() => IsRendering;
}
