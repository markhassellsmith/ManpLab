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
    /// </summary>
    public required WriteableBitmap Bitmap { get; init; }

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
