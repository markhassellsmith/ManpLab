namespace ManpWinUI.Services;

/// <summary>
/// Fractal rendering category determines appropriate visualization behavior and UI feedback.
/// </summary>
/// <remarks>
/// Matches the native FractalCategory enum in ManpCore.Native.
/// </remarks>
public enum FractalCategory
{
    /// <summary>Standard escape-time fractals (Mandelbrot, Julia, Burning Ship, etc.) - escape percentage is meaningful</summary>
    EscapeTime2D = 0,

    /// <summary>Sequence-based fractals (Hailstone, bifurcation) - use step/trajectory counts</summary>
    Sequence2D = 1,

    /// <summary>Legacy 3D attractor rendering (deprecated - use HistogramBased instead)</summary>
    AttractorBased3D = 2,

    /// <summary>Histogram/orbit accumulation rendering (attractors, flame fractals) - escape percentage not meaningful</summary>
    HistogramBased = 3,

    /// <summary>Special renderers (Buddhabrot, perturbation) - custom metrics</summary>
    Special = 4
}

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
    /// Fractal rendering category for context-aware status messages.
    /// </summary>
    /// <remarks>
    /// Determines appropriate UI feedback:
    /// - EscapeTime2D: Show escape percentage and boundary-finding guidance
    /// - HistogramBased: Show render time only, no escape metrics
    /// - Sequence2D: Show trajectory counts
    /// </remarks>
    public required FractalCategory Category { get; init; }

    /// <summary>
    /// Percentage of pixels that escaped (0-100).
    /// Low values indicate you're looking at the interior of the set.
    /// NOTE: Only meaningful for EscapeTime2D fractals.
    /// </summary>
    public double EscapePercentage => (double)EscapedPixels / (Width * Height) * 100.0;
}
