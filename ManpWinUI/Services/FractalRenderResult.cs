namespace ManpWinUI.Services;

/// <summary>
/// Result of fractal rendering including pixel data and diagnostic information.
/// </summary>
public class FractalRenderResult
{
    /// <summary>
    /// BGRA pixel data (4 bytes per pixel).
    /// </summary>
    public required byte[] PixelData { get; init; }

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public required int Width { get; init; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public required int Height { get; init; }

    /// <summary>
    /// Total rendering time.
    /// </summary>
    public required TimeSpan RenderTime { get; init; }

    /// <summary>
    /// Total iterations across all pixels.
    /// </summary>
    public required long TotalIterations { get; init; }

    /// <summary>
    /// Number of pixels that escaped (diverged) vs stayed in the set.
    /// </summary>
    public required int EscapedPixels { get; init; }

    /// <summary>
    /// Percentage of pixels that escaped (0-100).
    /// Low values indicate you're looking at the interior of the set.
    /// </summary>
    public double EscapePercentage => (double)EscapedPixels / (Width * Height) * 100.0;
}
