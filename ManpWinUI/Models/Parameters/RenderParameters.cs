using System.Collections.Generic;

namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Structured parameter package for native fractal rendering.
/// Converts flexible parameter sets into a format suitable for native interop.
/// </summary>
public class RenderParameters
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // CORE IDENTIFICATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Fractal type name (e.g., "Mandelbrot", "Lambda", "Phoenix").
    /// Used by native renderer to select calculation algorithm.
    /// </summary>
    public string FractalType { get; set; } = "Mandelbrot";

    // ═══════════════════════════════════════════════════════════════════════════════
    // VIEW PARAMETERS (Always Required)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Center X coordinate in complex plane (real component).
    /// </summary>
    public double CenterX { get; set; } = -0.5;

    /// <summary>
    /// Center Y coordinate in complex plane (imaginary component).
    /// </summary>
    public double CenterY { get; set; } = 0.0;

    /// <summary>
    /// Zoom level (1.0 = default view, higher = more zoomed in).
    /// </summary>
    public double Zoom { get; set; } = 1.0;

    /// <summary>
    /// Output image width in pixels.
    /// </summary>
    public int Width { get; set; } = 800;

    /// <summary>
    /// Output image height in pixels.
    /// </summary>
    public int Height { get; set; } = 600;

    // ═══════════════════════════════════════════════════════════════════════════════
    // ALGORITHM PARAMETERS (Common)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Maximum iteration count before declaring a point non-escaping.
    /// </summary>
    public int MaxIterations { get; set; } = 512;

    /// <summary>
    /// Escape radius threshold (typically 2.0 for standard fractals).
    /// </summary>
    public double EscapeRadius { get; set; } = 2.0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // JULIA MODE PARAMETERS (Optional)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// True to render Julia set, false for standard Mandelbrot-style iteration.
    /// </summary>
    public bool IsJuliaMode { get; set; } = false;

    /// <summary>
    /// Julia constant (real component).
    /// Only used when IsJuliaMode = true.
    /// </summary>
    public double JuliaCReal { get; set; } = 0.0;

    /// <summary>
    /// Julia constant (imaginary component).
    /// Only used when IsJuliaMode = true.
    /// </summary>
    public double JuliaCImaginary { get; set; } = 0.0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // COLOR PARAMETERS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Color palette name (e.g., "Classic", "Fire", "Ocean").
    /// </summary>
    public string Palette { get; set; } = "Classic";

    /// <summary>
    /// Color animation speed (0-100).
    /// </summary>
    public int ColorCycleSpeed { get; set; } = 50;

    /// <summary>
    /// Color rotation offset in degrees (0-360).
    /// </summary>
    public int ColorOffset { get; set; } = 0;

    /// <summary>
    /// Enable smooth/continuous coloring (gradients instead of bands).
    /// </summary>
    public bool UseSmoothColoring { get; set; } = false;

    /// <summary>
    /// Enable arbitrary-precision (deep zoom) mode for extreme magnification.
    /// Week 9 Task 2: Deep zoom toggle support.
    /// </summary>
    public bool UseDeepZoom { get; set; } = false;

    // ═══════════════════════════════════════════════════════════════════════════════
    // EXTENDED PARAMETERS (Fractal-Specific)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Additional fractal-specific parameters as key-value pairs.
    /// Examples:
    /// - "exponent": 3 (for Multibrot)
    /// - "lambda": 0.5 + 0.7i (for Lambda fractal)
    /// - "relaxation": 1.0 (for Newton method)
    /// </summary>
    public Dictionary<string, object> ExtendedParameters { get; set; } = new();

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Get an extended parameter with type conversion and fallback.
    /// </summary>
    public T? GetExtended<T>(string key, T? fallback = default)
    {
        if (!ExtendedParameters.TryGetValue(key, out var value))
            return fallback;

        try
        {
            return (T?)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return fallback;
        }
    }

    /// <summary>
    /// Set an extended parameter.
    /// </summary>
    public void SetExtended(string key, object value)
    {
        ExtendedParameters[key] = value;
    }

    /// <summary>
    /// Check if an extended parameter exists.
    /// </summary>
    public bool HasExtended(string key) => ExtendedParameters.ContainsKey(key);

    /// <summary>
    /// Create a copy with modified values (builder pattern).
    /// </summary>
    public RenderParameters With(
        string? fractalType = null,
        double? centerX = null,
        double? centerY = null,
        double? zoom = null,
        int? width = null,
        int? height = null,
        int? maxIterations = null)
    {
        return new RenderParameters
        {
            FractalType = fractalType ?? FractalType,
            CenterX = centerX ?? CenterX,
            CenterY = centerY ?? CenterY,
            Zoom = zoom ?? Zoom,
            Width = width ?? Width,
            Height = height ?? Height,
            MaxIterations = maxIterations ?? MaxIterations,
            EscapeRadius = EscapeRadius,
            IsJuliaMode = IsJuliaMode,
            JuliaCReal = JuliaCReal,
            JuliaCImaginary = JuliaCImaginary,
            Palette = Palette,
            ColorCycleSpeed = ColorCycleSpeed,
            ColorOffset = ColorOffset,
            UseSmoothColoring = UseSmoothColoring,
            UseDeepZoom = UseDeepZoom,
            ExtendedParameters = new Dictionary<string, object>(ExtendedParameters)
        };
    }
}
