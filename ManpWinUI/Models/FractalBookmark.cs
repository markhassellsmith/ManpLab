using System;
using System.Text.Json.Serialization;

namespace ManpWinUI.Models;

/// <summary>
/// Represents a saved fractal location/view bookmark.
/// Allows users to save and return to interesting fractal locations.
/// </summary>
public class FractalBookmark
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; } = "Unnamed";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fractalType")]
    public string FractalType { get; set; } = "Mandelbrot";

    [JsonPropertyName("iterationMode")]
    public string IterationMode { get; set; } = "Standard";

    [JsonPropertyName("centerX")]
    public double CenterX { get; set; }

    [JsonPropertyName("centerY")]
    public double CenterY { get; set; }

    [JsonPropertyName("zoom")]
    public double Zoom { get; set; }

    [JsonPropertyName("maxIterations")]
    public int MaxIterations { get; set; }

    [JsonPropertyName("colorPalette")]
    public string ColorPalette { get; set; } = "Classic";

    [JsonPropertyName("juliaC")]
    public JuliaParameters? JuliaC { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }

    [JsonPropertyName("isPreset")]
    public bool IsPreset { get; set; }

    /// <summary>
    /// Creates a bookmark from current fractal state.
    /// </summary>
    public static FractalBookmark FromCurrentState(
        string name,
        string description,
        string fractalType,
        string iterationMode,
        double centerX,
        double centerY,
        double zoom,
        int maxIterations,
        string colorPalette,
        double? juliaCX = null,
        double? juliaCY = null,
        bool isFavorite = false)
    {
        return new FractalBookmark
        {
            Name = name,
            Description = description,
            FractalType = fractalType,
            IterationMode = iterationMode,
            CenterX = centerX,
            CenterY = centerY,
            Zoom = zoom,
            MaxIterations = maxIterations,
            ColorPalette = colorPalette,
            JuliaC = (juliaCX.HasValue && juliaCY.HasValue)
                ? new JuliaParameters { Real = juliaCX.Value, Imaginary = juliaCY.Value }
                : null,
            IsFavorite = isFavorite,
            IsPreset = false
        };
    }

    /// <summary>
    /// Gets a display-friendly coordinate string.
    /// </summary>
    public string CoordinateDisplay => $"({CenterX:F8}, {CenterY:F8}) @ {Zoom:F2}x";
}
