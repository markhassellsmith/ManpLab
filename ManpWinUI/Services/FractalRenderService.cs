using ManpCore.Native;
using ManpWinUI.Models.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Service implementation that wraps ManpCore.Native for fractal rendering.
/// </summary>
public class FractalRenderService : IFractalRenderService
{
    private readonly ILogger<FractalRenderService> _logger;
    private readonly FractalEngineWrapper _engine;

    public FractalRenderService(ILogger<FractalRenderService> logger)
    {
        _logger = logger;
        _engine = new FractalEngineWrapper();
        _logger.LogInformation("FractalRenderService initialized");
    }

    public async Task<FractalRenderResult> RenderMandelbrotAsync(
        double centerX,
        double centerY,
        double zoom,
        int width,
        int height,
        int maxIterations,
        string palette,
        string fractalType = "Mandelbrot",
        bool isJuliaMode = false,
        double juliaCX = 0.0,
        double juliaCY = 0.0,
        int colorCycleSpeed = 50,
        int colorOffset = 0,
        bool useSmoothColoring = false,
        bool useDeepZoom = false,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var displayType = isJuliaMode ? $"{fractalType} Julia" : fractalType;
        _logger.LogInformation(
            "Rendering {FractalType}: center=({CenterX}, {CenterY}), zoom={Zoom}, size={Width}x{Height}, iterations={MaxIterations}, palette={Palette}",
            displayType, centerX, centerY, zoom, width, height, maxIterations, palette);

        if (isJuliaMode)
        {
            _logger.LogInformation("Julia parameters: c = ({JuliaCX}, {JuliaCY})", juliaCX, juliaCY);
        }

        // Week 7 Task 3: Log advanced color settings
        if (colorCycleSpeed != 50 || colorOffset != 0 || useSmoothColoring)
        {
            _logger.LogInformation(
                "Advanced color settings: CycleSpeed={CycleSpeed}, Offset={Offset}°, SmoothColoring={SmoothColoring}",
                colorCycleSpeed, colorOffset, useSmoothColoring);
        }

        // IMPORTANT: Capture the dispatcher queue BEFORE entering Task.Run()
        // Inside Task.Run, we're on a background thread and GetForCurrentThread() returns null
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        return await Task.Run(() =>
        {
            try
            {
                // Convert zoom to viewWidth (zoom of 1.0 = full Mandelbrot set width of 3.0)
                double viewWidth = 3.0 / zoom;
                double viewHeight = viewWidth * ((double)height / width);

                // Calculate boundaries for logging
                double left = centerX - viewWidth / 2.0;
                double right = centerX + viewWidth / 2.0;
                double top = centerY + viewHeight / 2.0;
                double bottom = centerY - viewHeight / 2.0;

                System.Diagnostics.Debug.WriteLine($@"
═══════════════════════════════════════════════════════════════
FRACTAL RENDER SERVICE - STARTING RENDER
═══════════════════════════════════════════════════════════════
Input Parameters:
  Center: ({centerX:F10}, {centerY:F10})
  Zoom: {zoom:F4}x
  Size: {width}×{height}
  Max Iterations: {maxIterations}
  Palette: {palette}

Calculated View:
  ViewWidth: {viewWidth:F10}
  ViewHeight: {viewHeight:F10}
  Boundaries:
    Left   = {left:F10}
    Right  = {right:F10}
    Top    = {top:F10}
    Bottom = {bottom:F10}
═══════════════════════════════════════════════════════════════
");

                // Parse palette string to enum
                var paletteEnum = ParsePalette(palette);

                System.Diagnostics.Debug.WriteLine($"Palette string '{palette}' mapped to enum value: {paletteEnum} (int: {(int)paletteEnum})");
                System.Diagnostics.Debug.WriteLine($"Color offset: {colorOffset}°");

                try
                {
                    System.Diagnostics.Debug.WriteLine($"Creating FractalParameters object...");

                    // Validate parameters before native call
                    if (width <= 0 || width > 8192)
                        throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be between 1 and 8192");
                    if (height <= 0 || height > 8192)
                        throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be between 1 and 8192");
                    if (maxIterations <= 0 || maxIterations > 100000)
                        throw new ArgumentOutOfRangeException(nameof(maxIterations), maxIterations, "MaxIterations must be between 1 and 100000");

                    // Create parameters for ManpCore.Native
                    var parameters = new FractalParameters
                    {
                        FractalType = fractalType,
                        CenterX = centerX,
                        CenterY = centerY,
                        ViewWidth = viewWidth,
                        ViewHeight = viewHeight,
                        Width = width,
                        Height = height,
                        MaxIterations = maxIterations,
                        Palette = paletteEnum,
                        ColorOffset = colorOffset,  // Apply color offset for palette rotation
                        IsJuliaSet = isJuliaMode,
                        JuliaCX = juliaCX,
                        JuliaCY = juliaCY
                    };

                    // Week 9 Task 1: Enable deep zoom (arbitrary precision) if requested
                    if (useDeepZoom)
                    {
                        // Convert double coordinates to BigDouble for arbitrary precision
                        // Precision: 25 decimal digits is sufficient for zoom levels up to 10^20
                        const int precision = 25;

                        parameters.BigCenterX = new BigDouble(centerX, precision);
                        parameters.BigCenterY = new BigDouble(centerY, precision);
                        parameters.BigViewWidth = new BigDouble(viewWidth, precision);
                        parameters.BigViewHeight = new BigDouble(viewHeight, precision);

                        System.Diagnostics.Debug.WriteLine($"[DeepZoom] Enabled with {precision} digit precision");
                        System.Diagnostics.Debug.WriteLine($"[DeepZoom] BigCenterX: {parameters.BigCenterX}");
                        System.Diagnostics.Debug.WriteLine($"[DeepZoom] BigCenterY: {parameters.BigCenterY}");
                        System.Diagnostics.Debug.WriteLine($"[DeepZoom] BigViewWidth: {parameters.BigViewWidth}");
                    }

                    System.Diagnostics.Debug.WriteLine($"FractalParameters created successfully");
                    System.Diagnostics.Debug.WriteLine($"  FractalType: {parameters.FractalType}");
                    System.Diagnostics.Debug.WriteLine($"  Width: {parameters.Width}, Height: {parameters.Height}");
                    System.Diagnostics.Debug.WriteLine($"  Calling _engine.Calculate()...");

                    // Set up progress reporting with dispatcher marshaling
                    EventHandler<ManpCore.Native.ProgressEventArgs>? progressHandler = null;
                    if (progress != null && dispatcherQueue != null)
                    {
                        progressHandler = (sender, args) =>
                        {
                            // Marshal progress updates to UI thread
                            var enqueued = dispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    progress.Report(args.Percentage / 100.0);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Progress report error: {ex.Message}");
                                }
                            });

                            if (!enqueued)
                            {
                                System.Diagnostics.Debug.WriteLine("Warning: Failed to enqueue progress update to UI thread");
                            }
                        };
                        _engine.ProgressChanged += progressHandler;
                        System.Diagnostics.Debug.WriteLine("Progress reporting enabled with captured DispatcherQueue");
                    }
                    else if (progress != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Warning: No DispatcherQueue available for progress reporting");
                    }

                    // Render the fractal using Calculate method
                    var result = _engine.Calculate(parameters);

                    System.Diagnostics.Debug.WriteLine($"Calculate() completed successfully");

                    // Clean up progress handler
                    if (progressHandler != null)
                    {
                        _engine.ProgressChanged -= progressHandler;
                    }

                    _logger.LogInformation(
                        "Mandelbrot render complete: {Width}x{Height} in {RenderTime}ms, {Iterations} total iterations, {Escaped}/{Total} pixels escaped ({Percent:F1}%)",
                        result.Width, result.Height, result.RenderTime.TotalMilliseconds, result.IterationCount, 
                        result.EscapedPixelCount, result.Width * result.Height, 
                        (double)result.EscapedPixelCount / (result.Width * result.Height) * 100.0);

                    return new FractalRenderResult
                    {
                        PixelData = result.PixelData,
                        Width = result.Width,
                        Height = result.Height,
                        RenderTime = result.RenderTime,
                        TotalIterations = result.IterationCount,
                        EscapedPixels = result.EscapedPixelCount
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR in native rendering: {ex.GetType().Name}: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    }
                    _logger.LogError(ex, "Error in native rendering");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering Mandelbrot set");
                throw;
            }
        }, cancellationToken);
    }

    public async Task<FractalRenderResult> RenderJuliaAsync(
        double cReal,
        double cImaginary,
        double centerX,
        double centerY,
        double zoom,
        int width,
        int height,
        int maxIterations,
        string palette,
        int colorCycleSpeed = 50,
        int colorOffset = 0,
        bool useSmoothColoring = false,
        bool useDeepZoom = false,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Rendering Julia: c=({CReal}, {CImaginary}), center=({CenterX}, {CenterY}), zoom={Zoom}, size={Width}x{Height}",
            cReal, cImaginary, centerX, centerY, zoom, width, height);

        // Week 7 Task 3: Log advanced color settings
        if (colorCycleSpeed != 50 || colorOffset != 0 || useSmoothColoring)
        {
            _logger.LogInformation(
                "Advanced color settings: CycleSpeed={CycleSpeed}, Offset={Offset}°, SmoothColoring={SmoothColoring}",
                colorCycleSpeed, colorOffset, useSmoothColoring);
        }

        return await Task.Run(() =>
        {
            try
            {
                double viewWidth = 3.0 / zoom;
                double viewHeight = viewWidth * ((double)height / width);
                var paletteEnum = ParsePalette(palette);

                var parameters = new FractalParameters
                {
                    FractalType = "Julia",
                    IsJuliaSet = true,
                    JuliaCX = cReal,
                    JuliaCY = cImaginary,
                    CenterX = centerX,
                    CenterY = centerY,
                    ViewWidth = viewWidth,
                    ViewHeight = viewHeight,
                    Width = width,
                    Height = height,
                    MaxIterations = maxIterations,
                    Palette = paletteEnum,
                    ColorOffset = colorOffset  // Apply color offset for palette rotation
                };

                // Week 9 Task 1: Enable deep zoom (arbitrary precision) if requested
                if (useDeepZoom)
                {
                    // Convert double coordinates to BigDouble for arbitrary precision
                    const int precision = 25;

                    parameters.BigCenterX = new BigDouble(centerX, precision);
                    parameters.BigCenterY = new BigDouble(centerY, precision);
                    parameters.BigViewWidth = new BigDouble(viewWidth, precision);
                    parameters.BigViewHeight = new BigDouble(viewHeight, precision);

                    System.Diagnostics.Debug.WriteLine($"[DeepZoom] Julia set enabled with {precision} digit precision");
                }

                EventHandler<ManpCore.Native.ProgressEventArgs>? progressHandler = null;
                if (progress != null)
                {
                    progressHandler = (sender, args) => progress.Report(args.Percentage / 100.0);
                    _engine.ProgressChanged += progressHandler;
                }

                var result = _engine.Calculate(parameters);

                if (progressHandler != null)
                {
                    _engine.ProgressChanged -= progressHandler;
                }

                _logger.LogInformation(
                    "Julia render complete: {Width}x{Height} in {RenderTime}ms, {Iterations} total iterations, {Escaped}/{Total} pixels escaped ({Percent:F1}%)",
                    result.Width, result.Height, result.RenderTime.TotalMilliseconds, result.IterationCount,
                    result.EscapedPixelCount, result.Width * result.Height,
                    (double)result.EscapedPixelCount / (result.Width * result.Height) * 100.0);

                return new FractalRenderResult
                {
                    PixelData = result.PixelData,
                    Width = result.Width,
                    Height = result.Height,
                    RenderTime = result.RenderTime,
                    TotalIterations = result.IterationCount,
                    EscapedPixels = result.EscapedPixelCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering Julia set");
                throw;
            }
        }, cancellationToken);
    }

    public string[] GetAvailablePalettes()
    {
        // These match the ColorPalette enum in ManpCore.Native
        return new[]
        {
            "Grayscale",
            "Classic",
            "Fire",
            "Ocean",
            "Afterimage",
            "Psychedelic",
            "Spectrum"
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // TASK 6: PARAMETER-BASED RENDERING (NEW ARCHITECTURE)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Renders a fractal using structured parameters.
    /// This is the new parameter-system approach - delegates to the existing method
    /// for now, but provides the foundation for future native parameter integration.
    /// </summary>
    public async Task<FractalRenderResult> RenderFractalAsync(
        RenderParameters parameters,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "RenderFractalAsync (Parameter System): {FractalType}, center=({CenterX}, {CenterY}), zoom={Zoom}",
            parameters.FractalType, parameters.CenterX, parameters.CenterY, parameters.Zoom);

        // For now, delegate to existing RenderMandelbrotAsync
        // Future: Once native layer accepts structured parameters, call native directly
        return await RenderMandelbrotAsync(
            parameters.CenterX,
            parameters.CenterY,
            parameters.Zoom,
            parameters.Width,
            parameters.Height,
            parameters.MaxIterations,
            parameters.Palette,
            parameters.FractalType,
            parameters.IsJuliaMode,
            parameters.JuliaCReal,
            parameters.JuliaCImaginary,
            parameters.ColorCycleSpeed,
            parameters.ColorOffset,
            parameters.UseSmoothColoring,
            parameters.UseDeepZoom,
            progress,
            cancellationToken);
    }

    private ColorPalette ParsePalette(string paletteName)
    {
        return paletteName switch
        {
            "Grayscale" => ColorPalette.Grayscale,
            "Classic" => ColorPalette.Classic,
            "Fire" => ColorPalette.Fire,
            "Ocean" => ColorPalette.Ocean,
            "Afterimage" => ColorPalette.Afterimage,
            "Psychedelic" => ColorPalette.Psychedelic,
            "Spectrum" => ColorPalette.Spectrum,
            _ => ColorPalette.Classic // Default fallback
        };
    }
}