using ManpWinUI.Models.Parameters;

namespace ManpWinUI.Models;

/// <summary>
/// Managed representation of fractal metadata.
/// Cached from native FractalRegistry at app startup.
/// Provides thread-safe access to fractal definitions without P/Invoke overhead.
/// </summary>
public class FractalDescriptor
{
    /// <summary>
    /// Internal fractal name (matches native registry key).
    /// Example: "Mandelbrot", "BurningShip", "Nova"
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display name for UI.
    /// Example: "Mandelbrot Set", "Burning Ship", "Nova Fractal"
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Category for browser organization.
    /// Example: "Classic Escape-Time", "Newton Method", "Special & Exotic"
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Optional description text.
    /// Displayed in UI tooltips or info panels.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether this fractal supports Julia mode.
    /// If true, Julia parameters should be added to parameter set.
    /// </summary>
    public bool SupportsJulia { get; set; }

    /// <summary>
    /// Default viewport center X coordinate (real axis).
    /// Used when first loading the fractal.
    /// </summary>
    public double DefaultCenterX { get; set; }

    /// <summary>
    /// Default viewport center Y coordinate (imaginary axis).
    /// Used when first loading the fractal.
    /// </summary>
    public double DefaultCenterY { get; set; }

    /// <summary>
    /// Default zoom level.
    /// 1.0 = standard view, higher = more zoomed in.
    /// </summary>
    public double DefaultZoom { get; set; } = 1.0;

    /// <summary>
    /// Default maximum iterations for this fractal.
    /// Some fractals need more iterations to show detail.
    /// </summary>
    public int DefaultMaxIterations { get; set; } = 512;

    /// <summary>
    /// Default bailout radius.
    /// Escape threshold for escape-time fractals.
    /// </summary>
    public double DefaultBailout { get; set; } = 256.0;

    /// <summary>
    /// Parameter set for this fractal (from Task 1).
    /// Null if parameters haven't been loaded yet.
    /// Lazy-loaded on first access via IFractalParameterService.
    /// </summary>
    public FractalParameterSet? Parameters { get; set; }

    /// <summary>
    /// Tags for search/filtering.
    /// Example: ["escape-time", "polynomial", "classic"]
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Whether this fractal is experimental/unstable.
    /// If true, may show warning in UI.
    /// </summary>
    public bool IsExperimental { get; set; }

    /// <summary>
    /// Render complexity hint (low/medium/high).
    /// Used to warn users about slow fractals.
    /// </summary>
    public string RenderComplexity { get; set; } = "medium";

    /// <summary>
    /// URL to documentation or examples (optional).
    /// </summary>
    public string? DocumentationUrl { get; set; }

    /// <summary>
    /// Creates a deep copy of this descriptor.
    /// Useful for creating fractal variations.
    /// </summary>
    public FractalDescriptor Clone()
    {
        return new FractalDescriptor
        {
            Name = this.Name,
            DisplayName = this.DisplayName,
            Category = this.Category,
            Description = this.Description,
            SupportsJulia = this.SupportsJulia,
            DefaultCenterX = this.DefaultCenterX,
            DefaultCenterY = this.DefaultCenterY,
            DefaultZoom = this.DefaultZoom,
            DefaultMaxIterations = this.DefaultMaxIterations,
            DefaultBailout = this.DefaultBailout,
            Parameters = this.Parameters, // Shallow copy - parameters are immutable
            Tags = new List<string>(this.Tags),
            IsExperimental = this.IsExperimental,
            RenderComplexity = this.RenderComplexity,
            DocumentationUrl = this.DocumentationUrl
        };
    }

    /// <summary>
    /// Creates a generic fallback descriptor for unknown fractals.
    /// Used when native registry lookup fails.
    /// </summary>
    public static FractalDescriptor CreateGenericFallback(string name)
    {
        return new FractalDescriptor
        {
            Name = name,
            DisplayName = name,
            Category = "Unknown",
            Description = $"Fractal '{name}' metadata not found in registry. Using generic defaults.",
            SupportsJulia = false,
            DefaultCenterX = 0.0,
            DefaultCenterY = 0.0,
            DefaultZoom = 1.0,
            DefaultMaxIterations = 512,
            DefaultBailout = 256.0,
            Tags = new List<string> { "unknown" },
            IsExperimental = true,
            RenderComplexity = "unknown"
        };
    }

    public override string ToString()
    {
        return $"{DisplayName} ({Category})";
    }
}
