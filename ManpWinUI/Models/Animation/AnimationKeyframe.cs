using ManpWinUI.Models.Parameters;

namespace ManpWinUI.Models.Animation;

/// <summary>
/// Represents a single keyframe in an animation timeline.
/// Stores complete rendering state at a specific time point.
/// </summary>
public class AnimationKeyframe
{
    /// <summary>
    /// Time position in seconds from animation start.
    /// </summary>
    public double TimeInSeconds { get; set; }

    /// <summary>
    /// Complete render parameters at this keyframe.
    /// </summary>
    public RenderParameters Parameters { get; set; } = new();

    /// <summary>
    /// Easing function to use when transitioning TO this keyframe from the previous one.
    /// </summary>
    public EasingFunction Easing { get; set; } = EasingFunction.Linear;

    /// <summary>
    /// Optional description or label for this keyframe.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Create a deep copy of this keyframe.
    /// </summary>
    public AnimationKeyframe Clone()
    {
        return new AnimationKeyframe
        {
            TimeInSeconds = TimeInSeconds,
            Parameters = Parameters.With(), // Use the With() method to create a copy
            Easing = Easing,
            Label = Label
        };
    }
}
