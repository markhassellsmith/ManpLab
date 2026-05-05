using ManpWinUI.Models.Parameters;

namespace ManpWinUI.Models.Animation;

/// <summary>
/// Configuration for zoom animation type.
/// </summary>
public class ZoomAnimationSettings
{
    /// <summary>
    /// Starting zoom level.
    /// </summary>
    public double StartZoom { get; set; } = 1.0;

    /// <summary>
    /// Ending zoom level.
    /// </summary>
    public double EndZoom { get; set; } = 100.0;

    /// <summary>
    /// Center point to zoom into (if null, uses current center).
    /// </summary>
    public (double X, double Y)? TargetCenter { get; set; }
}

/// <summary>
/// Configuration for pan animation type.
/// </summary>
public class PanAnimationSettings
{
    /// <summary>
    /// Starting center point.
    /// </summary>
    public (double X, double Y) StartCenter { get; set; }

    /// <summary>
    /// Ending center point.
    /// </summary>
    public (double X, double Y) EndCenter { get; set; }

    /// <summary>
    /// Keep zoom constant during pan.
    /// </summary>
    public bool LockZoom { get; set; } = true;
}

/// <summary>
/// Configuration for parameter sweep animation.
/// </summary>
public class ParameterAnimationSettings
{
    /// <summary>
    /// Name of parameter to animate (e.g., "exponent", "JuliaCReal").
    /// </summary>
    public string ParameterName { get; set; } = "";

    /// <summary>
    /// Starting parameter value.
    /// </summary>
    public double StartValue { get; set; }

    /// <summary>
    /// Ending parameter value.
    /// </summary>
    public double EndValue { get; set; }
}

/// <summary>
/// Configuration for color cycle animation.
/// </summary>
public class ColorCycleSettings
{
    /// <summary>
    /// Starting color offset (0-360 degrees).
    /// </summary>
    public int StartOffset { get; set; } = 0;

    /// <summary>
    /// Ending color offset (can be > 360 for multiple rotations).
    /// </summary>
    public int EndOffset { get; set; } = 360;

    /// <summary>
    /// Optional: Fade between two different palettes.
    /// </summary>
    public string? StartPalette { get; set; }

    /// <summary>
    /// Optional: End palette for cross-fade.
    /// </summary>
    public string? EndPalette { get; set; }
}

/// <summary>
/// Complete animation configuration including all settings and export options.
/// </summary>
public class AnimationSettings
{
    /// <summary>
    /// Type of animation to create.
    /// </summary>
    public AnimationType Type { get; set; } = AnimationType.Zoom;

    /// <summary>
    /// Total number of frames to render.
    /// </summary>
    public int FrameCount { get; set; } = 150;

    /// <summary>
    /// Target frame rate (frames per second).
    /// </summary>
    public int FrameRate { get; set; } = 30;

    /// <summary>
    /// Easing function for smooth transitions.
    /// </summary>
    public EasingFunction Easing { get; set; } = EasingFunction.Linear;

    /// <summary>
    /// Export format for the final animation.
    /// </summary>
    public ExportFormat ExportFormat { get; set; } = ExportFormat.MP4;

    /// <summary>
    /// Output file path (including extension).
    /// </summary>
    public string OutputPath { get; set; } = "";

    /// <summary>
    /// Base render parameters (used as template for animation).
    /// </summary>
    public RenderParameters BaseParameters { get; set; } = new();

    /// <summary>
    /// Starting render parameters (for two-point animations).
    /// </summary>
    public RenderParameters? StartParameters { get; set; }

    /// <summary>
    /// Ending render parameters (for two-point animations).
    /// </summary>
    public RenderParameters? EndParameters { get; set; }

    // Type-specific settings (only one will be used based on Type)

    /// <summary>
    /// Zoom animation configuration.
    /// </summary>
    public ZoomAnimationSettings? ZoomSettings { get; set; }

    /// <summary>
    /// Pan animation configuration.
    /// </summary>
    public PanAnimationSettings? PanSettings { get; set; }

    /// <summary>
    /// Parameter sweep configuration.
    /// </summary>
    public ParameterAnimationSettings? ParameterSettings { get; set; }

    /// <summary>
    /// Color cycle configuration.
    /// </summary>
    public ColorCycleSettings? ColorSettings { get; set; }

    /// <summary>
    /// Get duration in seconds.
    /// </summary>
    public double DurationSeconds => (double)FrameCount / FrameRate;

    /// <summary>
    /// Validate animation settings.
    /// </summary>
    public (bool IsValid, string? ErrorMessage) Validate()
    {
        if (FrameCount <= 0)
            return (false, "Frame count must be greater than 0.");

        if (FrameRate <= 0)
            return (false, "Frame rate must be greater than 0.");

        if (string.IsNullOrWhiteSpace(OutputPath))
            return (false, "Output path is required.");

        if (StartParameters == null || EndParameters == null)
            return (false, "Start and end parameters are required.");

        return (true, null);
    }
}
