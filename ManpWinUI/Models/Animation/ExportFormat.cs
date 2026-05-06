namespace ManpWinUI.Models.Animation;

/// <summary>
/// Export format options for rendered animations.
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// MP4 video with H.264 codec (universal playback, good compression).
    /// </summary>
    MP4,

    /// <summary>
    /// Animated GIF (256 color limit, large file size, wide compatibility).
    /// </summary>
    GIF,

    /// <summary>
    /// PNG image sequence (lossless, maximum quality, post-processing friendly).
    /// </summary>
    PNGSequence,

    /// <summary>
    /// WebM video with VP9 codec (better compression, modern format).
    /// </summary>
    WebM
}
