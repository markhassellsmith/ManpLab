namespace ManpWinUI.Models.Animation;

/// <summary>
/// Easing/interpolation functions for smooth parameter transitions.
/// Based on Robert Penner's easing equations (https://easings.net).
/// </summary>
public enum EasingFunction
{
    /// <summary>
    /// Constant speed interpolation (t).
    /// </summary>
    Linear,

    /// <summary>
    /// Quadratic ease-in (t²) - slow start, fast end.
    /// </summary>
    EaseInQuad,

    /// <summary>
    /// Quadratic ease-out (1 - (1-t)²) - fast start, slow end.
    /// </summary>
    EaseOutQuad,

    /// <summary>
    /// Quadratic ease-in-out - slow start and end, fast middle.
    /// </summary>
    EaseInOutQuad,

    /// <summary>
    /// Cubic ease-in (t³) - slower start, faster end than quadratic.
    /// </summary>
    EaseInCubic,

    /// <summary>
    /// Cubic ease-out - faster start, slower end than quadratic.
    /// </summary>
    EaseOutCubic,

    /// <summary>
    /// Cubic ease-in-out - smoother acceleration/deceleration.
    /// </summary>
    EaseInOutCubic,

    /// <summary>
    /// Exponential ease for extreme zoom transitions (2^(10*(t-1))).
    /// </summary>
    Exponential
}
