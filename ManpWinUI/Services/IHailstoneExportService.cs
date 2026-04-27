using ManpCore.Services.Models;
using ManpWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ManpWinUI.Services;

/// <summary>
/// Service interface for exporting Hailstone visualizations with overlays to various formats.
/// </summary>
public interface IHailstoneExportService
{
    /// <summary>
    /// Creates a composite bitmap that includes the Hailstone visualization with info overlay.
    /// </summary>
    /// <param name="baseBitmap">The base rendered bitmap.</param>
    /// <param name="result">The Hailstone sequence result.</param>
    /// <returns>A new bitmap with info overlay added.</returns>
    Task<WriteableBitmap> CreateExportBitmapAsync(
        WriteableBitmap baseBitmap,
        HailstoneResult result);

    /// <summary>
    /// Exports the Hailstone visualization as SVG with embedded metadata.
    /// </summary>
    /// <param name="result">The Hailstone sequence result.</param>
    /// <param name="scaleX">X-axis scaling factor.</param>
    /// <param name="scaleY">Y-axis scaling factor.</param>
    /// <param name="offsetX">X-axis offset.</param>
    /// <param name="offsetY">Y-axis offset.</param>
    /// <param name="width">SVG viewport width.</param>
    /// <param name="height">SVG viewport height.</param>
    /// <param name="filePath">Output file path.</param>
    /// <param name="metadata">Optional fractal metadata.</param>
    /// <returns>True if export succeeded, false otherwise.</returns>
    Task<bool> ExportAsSvgAsync(
        HailstoneResult result,
        double scaleX, double scaleY,
        double offsetX, double offsetY,
        int width, int height,
        string filePath,
        FractalMetadata? metadata = null);
}
