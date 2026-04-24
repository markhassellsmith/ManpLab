using System;
using System.Text.Json.Serialization;

namespace ManpWinUI.Models;

/// <summary>
/// Metadata model for fractal images.
/// Contains all parameters needed to reproduce a fractal render.
/// Serialized to JSON and embedded in PNG tEXt chunks or JPEG EXIF.
/// </summary>
public class FractalMetadata
{
    [JsonPropertyName("software")]
    public string Software { get; set; } = "ManpWinUI";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

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

    [JsonPropertyName("imageWidth")]
    public int ImageWidth { get; set; }

    [JsonPropertyName("imageHeight")]
    public int ImageHeight { get; set; }

    [JsonPropertyName("juliaC")]
    public JuliaParameters? JuliaC { get; set; }

    [JsonPropertyName("renderDate")]
    public DateTime RenderDate { get; set; }

    [JsonPropertyName("computeTimeMs")]
    public double ComputeTimeMs { get; set; }

    [JsonPropertyName("autoScaleIterations")]
    public bool AutoScaleIterations { get; set; }

    /// <summary>
    /// Creates metadata from current view model state.
    /// </summary>
    public static FractalMetadata FromViewModel(
        string fractalType,
        string iterationMode,
        double centerX,
        double centerY,
        double zoom,
        int maxIterations,
        string colorPalette,
        int imageWidth,
        int imageHeight,
        bool autoScaleIterations,
        double? juliaCX = null,
        double? juliaCY = null,
        TimeSpan? renderTime = null)
    {
        return new FractalMetadata
        {
            FractalType = fractalType,
            IterationMode = iterationMode,
            CenterX = centerX,
            CenterY = centerY,
            Zoom = zoom,
            MaxIterations = maxIterations,
            ColorPalette = colorPalette,
            ImageWidth = imageWidth,
            ImageHeight = imageHeight,
            AutoScaleIterations = autoScaleIterations,
            JuliaC = (juliaCX.HasValue && juliaCY.HasValue) 
                ? new JuliaParameters { Real = juliaCX.Value, Imaginary = juliaCY.Value }
                : null,
            RenderDate = DateTime.UtcNow,
            ComputeTimeMs = renderTime?.TotalMilliseconds ?? 0.0
        };
    }
}

/// <summary>
/// Julia set constant parameter (complex number).
/// </summary>
public class JuliaParameters
{
    [JsonPropertyName("real")]
    public double Real { get; set; }

    [JsonPropertyName("imaginary")]
    public double Imaginary { get; set; }
}
