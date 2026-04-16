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
    /// Renders the Mandelbrot set with the specified parameters.
    /// </summary>
    /// <param name="centerX">Real component of center point</param>
    /// <param name="centerY">Imaginary component of center point</param>
    /// <param name="zoom">Zoom level (1.0 = default view)</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="maxIterations">Maximum iteration count</param>
    /// <param name="palette">Color palette name</param>
    /// <param name="progress">Progress callback (0.0 to 1.0)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered fractal image data</returns>
    Task<byte[]> RenderMandelbrotAsync(
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
    /// Renders the Julia set with the specified parameters.
    /// </summary>
    Task<byte[]> RenderJuliaAsync(
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
