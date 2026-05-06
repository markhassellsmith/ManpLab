using ManpWinUI.Models.Animation;
using ManpWinUI.Models.Parameters;

namespace ManpWinUI.Services.Animation;

/// <summary>
/// Interpolates render parameters between keyframes for smooth animations.
/// Supports various easing functions and parameter-specific interpolation strategies.
/// </summary>
public class FrameInterpolator
{
    /// <summary>
    /// Interpolate between two render parameter sets at time t.
    /// </summary>
    /// <param name="start">Starting parameters (t=0)</param>
    /// <param name="end">Ending parameters (t=1)</param>
    /// <param name="t">Normalized time (0.0 to 1.0)</param>
    /// <param name="easing">Easing function to apply</param>
    /// <returns>Interpolated render parameters</returns>
    public RenderParameters InterpolateFrame(
        RenderParameters start,
        RenderParameters end,
        double t,
        EasingFunction easing)
    {
        // Clamp t to valid range
        t = Math.Clamp(t, 0.0, 1.0);

        // Apply easing function
        double easedT = ApplyEasing(t, easing);

        // Create interpolated parameters
        var result = new RenderParameters
        {
            FractalType = start.FractalType, // Type doesn't change mid-animation

            // Spatial parameters
            CenterX = Lerp(start.CenterX, end.CenterX, easedT),
            CenterY = Lerp(start.CenterY, end.CenterY, easedT),
            Zoom = LerpExponential(start.Zoom, end.Zoom, easedT), // Exponential for zoom

            // Image dimensions (typically constant)
            Width = start.Width,
            Height = start.Height,

            // Algorithm parameters
            MaxIterations = LerpInt(start.MaxIterations, end.MaxIterations, easedT),
            EscapeRadius = Lerp(start.EscapeRadius, end.EscapeRadius, easedT),

            // Julia mode
            IsJuliaMode = start.IsJuliaMode, // Boolean doesn't interpolate
            JuliaCReal = Lerp(start.JuliaCReal, end.JuliaCReal, easedT),
            JuliaCImaginary = Lerp(start.JuliaCImaginary, end.JuliaCImaginary, easedT),

            // Color parameters
            Palette = start.Palette, // Palette name doesn't interpolate (use color cycling for that)
            ColorCycleSpeed = LerpInt(start.ColorCycleSpeed, end.ColorCycleSpeed, easedT),
            ColorOffset = LerpInt(start.ColorOffset, end.ColorOffset, easedT),
            UseSmoothColoring = start.UseSmoothColoring,
            UseDeepZoom = start.UseDeepZoom,

            // Extended parameters (interpolate matching keys)
            ExtendedParameters = InterpolateExtendedParameters(
                start.ExtendedParameters,
                end.ExtendedParameters,
                easedT)
        };

        return result;
    }

    /// <summary>
    /// Linear interpolation between two values.
    /// </summary>
    private double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

    /// <summary>
    /// Integer interpolation with rounding.
    /// </summary>
    private int LerpInt(int a, int b, double t)
    {
        return (int)Math.Round(Lerp(a, b, t));
    }

    /// <summary>
    /// Exponential interpolation for zoom (logarithmic space).
    /// This ensures smooth perceived zoom speed across orders of magnitude.
    /// </summary>
    private double LerpExponential(double a, double b, double t)
    {
        if (a <= 0 || b <= 0)
        {
            // Fallback to linear if either value is non-positive
            return Lerp(a, b, t);
        }

        // Interpolate in log space: log(result) = lerp(log(a), log(b), t)
        double logA = Math.Log(a);
        double logB = Math.Log(b);
        double logResult = Lerp(logA, logB, t);
        return Math.Exp(logResult);
    }

    /// <summary>
    /// Interpolate extended parameters dictionary.
    /// Only interpolates numeric values that exist in both dictionaries.
    /// </summary>
    private Dictionary<string, object> InterpolateExtendedParameters(
        Dictionary<string, object> start,
        Dictionary<string, object> end,
        double t)
    {
        var result = new Dictionary<string, object>();

        // Copy all start parameters
        foreach (var kvp in start)
        {
            result[kvp.Key] = kvp.Key;
        }

        // Interpolate matching numeric parameters
        foreach (var key in start.Keys.Intersect(end.Keys))
        {
            var startValue = start[key];
            var endValue = end[key];

            // Try to interpolate if both are numeric
            if (TryGetNumericValue(startValue, out double startNum) &&
                TryGetNumericValue(endValue, out double endNum))
            {
                result[key] = Lerp(startNum, endNum, t);
            }
            else
            {
                // Non-numeric: use start value (no interpolation)
                result[key] = startValue;
            }
        }

        return result;
    }

    /// <summary>
    /// Try to extract numeric value from object.
    /// </summary>
    private bool TryGetNumericValue(object value, out double result)
    {
        result = 0;

        if (value is double d)
        {
            result = d;
            return true;
        }
        if (value is float f)
        {
            result = f;
            return true;
        }
        if (value is int i)
        {
            result = i;
            return true;
        }
        if (value is long l)
        {
            result = l;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply easing function to normalized time value.
    /// Based on Robert Penner's easing equations.
    /// </summary>
    public double ApplyEasing(double t, EasingFunction easing)
    {
        return easing switch
        {
            EasingFunction.Linear => t,

            EasingFunction.EaseInQuad => t * t,
            EasingFunction.EaseOutQuad => 1 - (1 - t) * (1 - t),
            EasingFunction.EaseInOutQuad => t < 0.5
                ? 2 * t * t
                : 1 - Math.Pow(-2 * t + 2, 2) / 2,

            EasingFunction.EaseInCubic => t * t * t,
            EasingFunction.EaseOutCubic => 1 - Math.Pow(1 - t, 3),
            EasingFunction.EaseInOutCubic => t < 0.5
                ? 4 * t * t * t
                : 1 - Math.Pow(-2 * t + 2, 3) / 2,

            EasingFunction.Exponential => t == 0 ? 0 : Math.Pow(2, 10 * (t - 1)),

            _ => t // Fallback to linear
        };
    }

    /// <summary>
    /// Calculate total number of frames for given duration and frame rate.
    /// </summary>
    public int CalculateFrameCount(double durationSeconds, int frameRate)
    {
        return (int)Math.Ceiling(durationSeconds * frameRate);
    }

    /// <summary>
    /// Get normalized time (0.0 to 1.0) for a given frame index.
    /// </summary>
    public double GetNormalizedTime(int frameIndex, int totalFrames)
    {
        if (totalFrames <= 1)
            return 0.0;

        return (double)frameIndex / (totalFrames - 1);
    }
}
