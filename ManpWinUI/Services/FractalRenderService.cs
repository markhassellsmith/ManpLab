using ManpCore.Native;
using Microsoft.Extensions.Logging;
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
                    IsJuliaSet = isJuliaMode,
                    JuliaCX = juliaCX,
                    JuliaCY = juliaCY
                };

                // Set up progress reporting
                EventHandler<ManpCore.Native.ProgressEventArgs>? progressHandler = null;
                if (progress != null)
                {
                    progressHandler = (sender, args) =>
                    {
                        progress.Report(args.Percentage / 100.0);
                    };
                    _engine.ProgressChanged += progressHandler;
                }

                // Render the fractal using Calculate method
                var result = _engine.Calculate(parameters);

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
                    Palette = paletteEnum
                };

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
            "Rainbow",
            "Psychedelic"
        };
    }

    private ColorPalette ParsePalette(string paletteName)
    {
        return paletteName switch
        {
            "Grayscale" => ColorPalette.Grayscale,
            "Classic" => ColorPalette.Classic,
            "Fire" => ColorPalette.Fire,
            "Ocean" => ColorPalette.Ocean,
            "Rainbow" => ColorPalette.Rainbow,
            "Psychedelic" => ColorPalette.Psychedelic,
            _ => ColorPalette.Classic // Default fallback
        };
    }
}