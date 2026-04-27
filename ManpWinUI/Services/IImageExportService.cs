using ManpWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ManpWinUI.Services;

/// <summary>
/// Service interface for exporting fractal images with embedded metadata.
/// Supports PNG (with tEXt chunks), JPEG (with EXIF), and future SVG support.
/// </summary>
public interface IImageExportService
{
    /// <summary>
    /// Saves a fractal image to a file with embedded metadata.
    /// </summary>
    /// <param name="bitmap">The WriteableBitmap to save</param>
    /// <param name="metadata">Fractal metadata to embed</param>
    /// <param name="format">Image format (PNG or JPEG)</param>
    /// <param name="hwnd">Window handle for FileSavePicker</param>
    /// <returns>True if saved successfully, false if cancelled</returns>
    Task<bool> SaveImageAsync(
        WriteableBitmap bitmap,
        FractalMetadata metadata,
        ImageFormat format,
        IntPtr hwnd);

    /// <summary>
    /// Copies the fractal image to clipboard with metadata.
    /// </summary>
    Task CopyToClipboardAsync(WriteableBitmap bitmap, FractalMetadata metadata);
}
