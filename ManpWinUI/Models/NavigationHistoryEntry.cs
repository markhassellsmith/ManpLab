using System;
using System.Text.Json.Serialization;

namespace ManpWinUI.Models;

/// <summary>
/// Represents a snapshot of the fractal view state for navigation history.
/// Used for undo/redo functionality.
/// </summary>
public class NavigationHistoryEntry
{
    /// <summary>
    /// Unique identifier for this history entry.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when this state was recorded.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Auto-generated description of the navigation action.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = "Navigation state";

    /// <summary>
    /// Fractal type at this state.
    /// </summary>
    [JsonPropertyName("fractalType")]
    public string FractalType { get; set; } = "Mandelbrot";

    /// <summary>
    /// Iteration mode (Standard, Julia, etc.).
    /// </summary>
    [JsonPropertyName("iterationMode")]
    public string IterationMode { get; set; } = "Standard";

    /// <summary>
    /// Center X coordinate.
    /// </summary>
    [JsonPropertyName("centerX")]
    public double CenterX { get; set; }

    /// <summary>
    /// Center Y coordinate.
    /// </summary>
    [JsonPropertyName("centerY")]
    public double CenterY { get; set; }

    /// <summary>
    /// Zoom level (1.0 = full view).
    /// </summary>
    [JsonPropertyName("zoom")]
    public double Zoom { get; set; }

    /// <summary>
    /// Maximum iterations for rendering.
    /// </summary>
    [JsonPropertyName("maxIterations")]
    public int MaxIterations { get; set; }

    /// <summary>
    /// Color palette name.
    /// </summary>
    [JsonPropertyName("colorPalette")]
    public string ColorPalette { get; set; } = "Classic";

    /// <summary>
    /// Julia set parameters (if applicable).
    /// </summary>
    [JsonPropertyName("juliaC")]
    public JuliaParameters? JuliaC { get; set; }

    /// <summary>
    /// Gets a short coordinate display string.
    /// </summary>
    [JsonIgnore]
    public string CoordinateDisplay => $"({CenterX:F6}, {CenterY:F6}) @ {Zoom:F2}x";

    /// <summary>
    /// Gets a formatted timestamp string.
    /// </summary>
    [JsonIgnore]
    public string TimeDisplay => Timestamp.ToLocalTime().ToString("h:mm:ss tt");

    /// <summary>
    /// Creates a history entry from current fractal state with auto-generated description.
    /// </summary>
    public static NavigationHistoryEntry FromCurrentState(
        string fractalType,
        string iterationMode,
        double centerX,
        double centerY,
        double zoom,
        int maxIterations,
        string colorPalette,
        double? juliaCX = null,
        double? juliaCY = null,
        string? customDescription = null)
    {
        var entry = new NavigationHistoryEntry
        {
            FractalType = fractalType,
            IterationMode = iterationMode,
            CenterX = centerX,
            CenterY = centerY,
            Zoom = zoom,
            MaxIterations = maxIterations,
            ColorPalette = colorPalette,
            JuliaC = (juliaCX.HasValue && juliaCY.HasValue)
                ? new JuliaParameters { Real = juliaCX.Value, Imaginary = juliaCY.Value }
                : null
        };

        // Auto-generate description if not provided
        if (!string.IsNullOrWhiteSpace(customDescription))
        {
            entry.Description = customDescription;
        }
        else
        {
            entry.Description = GenerateDescription(fractalType, iterationMode, centerX, centerY, zoom);
        }

        return entry;
    }

    /// <summary>
    /// Generates a human-readable description of the navigation state.
    /// </summary>
    private static string GenerateDescription(string fractalType, string iterationMode, double centerX, double centerY, double zoom)
    {
        var isJulia = iterationMode.Contains("Julia");
        var fractalName = isJulia ? $"{fractalType} Julia" : fractalType;

        if (Math.Abs(centerX - (-0.5)) < 0.01 && Math.Abs(centerY) < 0.01 && Math.Abs(zoom - 1.0) < 0.1)
        {
            return $"{fractalName} - Full view";
        }
        else if (zoom < 2.0)
        {
            return $"{fractalName} - Overview";
        }
        else if (zoom < 100.0)
        {
            return $"{fractalName} - Zoomed {zoom:F1}x";
        }
        else if (zoom < 1000.0)
        {
            return $"{fractalName} - Deep zoom {zoom:F0}x";
        }
        else
        {
            return $"{fractalName} - Ultra deep {zoom:E2}x";
        }
    }

    /// <summary>
    /// Checks if this entry represents a significant change from another entry.
    /// Used to avoid recording micro-movements.
    /// </summary>
    public bool IsSignificantChangeFrom(NavigationHistoryEntry? other)
    {
        if (other == null)
            return true;

        // Different fractal type is always significant
        if (FractalType != other.FractalType || IterationMode != other.IterationMode)
            return true;

        // Calculate relative changes
        var viewWidth = 4.0 / Zoom; // Approximate view width in complex plane
        var deltaX = Math.Abs(CenterX - other.CenterX);
        var deltaY = Math.Abs(CenterY - other.CenterY);
        var zoomRatio = Math.Max(Zoom / other.Zoom, other.Zoom / Zoom);

        // Significant if:
        // - Moved more than 5% of current view
        // - Zoom changed by more than 10%
        // - Iterations changed significantly
        var significantPan = (deltaX > viewWidth * 0.05) || (deltaY > viewWidth * 0.05);
        var significantZoom = zoomRatio > 1.1;
        var significantIterations = Math.Abs(MaxIterations - other.MaxIterations) > 100;

        return significantPan || significantZoom || significantIterations;
    }
}
