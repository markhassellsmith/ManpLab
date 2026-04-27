using Microsoft.UI.Xaml.Media.Imaging;

namespace ManpWinUI.Models;

/// <summary>
/// Represents the result of a Hailstone sequence rendering operation,
/// including the bitmap and coordinate transform parameters for label overlay.
/// </summary>
public class HailstoneRenderResult
{
    /// <summary>
    /// The rendered bitmap image.
    /// NOTE: May be null if pixel data is provided instead (for thread-safe bitmap creation).
    /// </summary>
    public WriteableBitmap? Bitmap { get; init; }

    /// <summary>
    /// Raw pixel data in BGRA format (used when bitmap creation must happen on UI thread).
    /// </summary>
    public byte[]? PixelData { get; init; }

    /// <summary>
    /// Image width in pixels (used with PixelData).
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// Image height in pixels (used with PixelData).
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// The Hailstone sequence that was rendered.
    /// </summary>
    public required HailstoneResult SequenceResult { get; init; }

    /// <summary>
    /// X-axis scale factor for world-to-screen coordinate transform.
    /// </summary>
    public required double ScaleX { get; init; }

    /// <summary>
    /// Y-axis scale factor for world-to-screen coordinate transform (negative for Y-flip).
    /// </summary>
    public required double ScaleY { get; init; }

    /// <summary>
    /// X-axis offset for world-to-screen coordinate transform.
    /// </summary>
    public required double OffsetX { get; init; }

    /// <summary>
    /// Y-axis offset for world-to-screen coordinate transform.
    /// </summary>
    public required double OffsetY { get; init; }
}
