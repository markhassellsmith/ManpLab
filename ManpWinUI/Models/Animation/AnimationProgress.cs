namespace ManpWinUI.Models.Animation;

/// <summary>
/// Progress information for animation rendering and export operations.
/// </summary>
public class AnimationProgress
{
    /// <summary>
    /// Current frame being processed (1-based).
    /// </summary>
    public int CurrentFrame { get; set; }

    /// <summary>
    /// Total number of frames in animation.
    /// </summary>
    public int TotalFrames { get; set; }

    /// <summary>
    /// Current phase of operation (e.g., "Rendering", "Exporting", "Encoding").
    /// </summary>
    public string Phase { get; set; } = "Processing";

    /// <summary>
    /// Optional detailed status message.
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Progress percentage (0.0 to 100.0).
    /// </summary>
    public double ProgressPercentage => TotalFrames > 0
        ? (double)CurrentFrame / TotalFrames * 100.0
        : 0.0;

    /// <summary>
    /// Estimated time remaining (if available).
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// Average time per frame.
    /// </summary>
    public TimeSpan? AverageFrameTime { get; set; }
}
