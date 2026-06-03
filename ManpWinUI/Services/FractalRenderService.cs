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
        int renderMode = 0,
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

                // Debug diagnostics removed - initial conditions system is working

                                // Parse palette string to enum
                                var paletteEnum = ParsePalette(palette);

                try
                {

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
                        JuliaCY = juliaCY,
                        RenderMode = renderMode,
                        UseSmoothColoring = useSmoothColoring
                    };

                    // Week 9 Task 1: Enable deep zoom (arbitrary precision) if requested
                    // Phase 1 Complete: Now using perturbation theory for true deep zoom support
                    if (useDeepZoom)
                    {
                        // Calculate required precision based on viewport width (actual coordinate scale)
                        // Formula: -log10(viewWidth) gives scale order of magnitude, add 20 for safety
                        // Example: viewWidth 1E-14 needs 14 digits for scale + 20 margin = 34 digits
                        // This ensures sufficient precision for the actual coordinate values being computed
                        int scaleDigits = (int)Math.Ceiling(-Math.Log10(viewWidth));
                        int requiredPrecision = scaleDigits + 20;  // 20-digit safety margin for computation
                        int precision = Math.Max(30, requiredPrecision); // Minimum 30 digits for safety

                        parameters.Precision = precision;  // Pass precision to native layer
                        parameters.BigCenterX = new BigDouble(centerX, precision);
                        parameters.BigCenterY = new BigDouble(centerY, precision);
                        parameters.BigViewWidth = new BigDouble(viewWidth, precision);
                        parameters.BigViewHeight = new BigDouble(viewHeight, precision);
                    }

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
                                    // Suppress progress report errors
                                }
                            });

                            // Enqueue failures are expected during fast renders
                        };
                        _engine.ProgressChanged += progressHandler;
                    }

                    FractalResult result;

                    // Choose rendering path based on deep zoom requirement
                    if (useDeepZoom && parameters.BigCenterX != null)
                    {

                        // Check if reference orbit is valid for current parameters
                        bool needsRebuild = !_engine.IsReferenceOrbitValid(
                            parameters.BigCenterX.ToString(),
                            parameters.BigCenterY.ToString(),
                            parameters.BigViewWidth.ToString(),
                            parameters.MaxIterations,
                            256.0,  // bailout (TODO: make configurable)
                            2       // power (Mandelbrot is z^2 + c)
                        );

                        if (needsRebuild)
                        {
                            var orbitResult = _engine.BuildReferenceOrbit(
                                parameters.BigCenterX.ToString(),
                                parameters.BigCenterY.ToString(),
                                parameters.BigViewWidth.ToString(),
                                parameters.MaxIterations,
                                256.0,  // bailout
                                2,      // power
                                0,      // subtype (0 = auto-detect)
                                parameters.Precision,
                                true,   // enable BLA
                                parameters.Width,   // image width for BLA size calculation
                                parameters.Height   // image height for BLA size calculation
                            );

                            if (orbitResult < 0)
                            {
                                throw new InvalidOperationException("Failed to build reference orbit");
                            }
                        }

                        // Render using perturbation theory
                        result = _engine.CalculateWithPerturbation(parameters);
                    }
                    else
                    {
                        // Standard rendering path for shallow zooms
                        result = _engine.Calculate(parameters);
                    }

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
                        EscapedPixels = result.EscapedPixelCount,
                        Category = (FractalCategory)(int)result.Category
                    };
                }
                catch (Exception ex)
                {
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
                    EscapedPixels = result.EscapedPixelCount,
                    Category = (FractalCategory)(int)result.Category
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
            0,  // renderMode - default to EscapeTime for now (TODO: add to RenderParameters)
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
            "Neon" => ColorPalette.Neon,
            _ => ColorPalette.Classic // Default fallback
        };
    }
}
