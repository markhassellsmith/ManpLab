using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Service for rendering fractals using the ManpCore.Native wrapper.
/// </summary>
public interface IFractalRenderService
{
    /// <summary>
    /// Renders the Mandelbrot or Julia set with the specified parameters.
    /// </summary>
    /// <param name="centerX">Real component of center point</param>
    /// <param name="centerY">Imaginary component of center point</param>
    /// <param name="zoom">Zoom level (1.0 = default view)</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="maxIterations">Maximum iteration count</param>
    /// <param name="palette">Color palette name</param>
    /// <param name="isJuliaMode">True to render Julia set, false for Mandelbrot</param>
    /// <param name="juliaCX">Julia constant (real part)</param>
    /// <param name="juliaCY">Julia constant (imaginary part)</param>
    /// <param name="progress">Progress callback (0.0 to 1.0)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered fractal with diagnostic information</returns>
    Task<FractalRenderResult> RenderMandelbrotAsync(
        double centerX,
        double centerY,
        double zoom,
        int width,
        int height,
        int maxIterations,
        string palette,
        bool isJuliaMode = false,
        double juliaCX = 0.0,
        double juliaCY = 0.0,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders the Julia set with the specified parameters.
    /// </summary>
    Task<FractalRenderResult> RenderJuliaAsync(
        double cReal,
        double cImaginary,
        double centerX,
        double centerY,
        double zoom,
        int width,
        int height,
        int maxIterations,
        string palette,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available color palette names.
    /// </summary>
    string[] GetAvailablePalettes();
}
