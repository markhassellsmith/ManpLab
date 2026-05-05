namespace ManpWinUI.Models.Animation;

/// <summary>
/// Types of fractal animations supported.
/// </summary>
public enum AnimationType
{
    /// <summary>
    /// Zoom in or out smoothly between two zoom levels.
    /// </summary>
    Zoom,

    /// <summary>
    /// Pan/navigate across the complex plane between two points.
    /// </summary>
    Pan,

    /// <summary>
    /// Sweep a fractal parameter (e.g., power, julia constant) between two values.
    /// </summary>
    Parameter,

    /// <summary>
    /// Cycle or morph color palettes without recalculating the fractal.
    /// </summary>
    ColorCycle,

    /// <summary>
    /// Rotate the view around the center point.
    /// </summary>
    Rotation
}
