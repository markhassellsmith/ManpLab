namespace ManpWinUI.Models;

/// <summary>
/// Configuration options for Hailstone sequence rendering.
/// </summary>
public class HailstoneConfig
{
    /// <summary>
    /// Starting X coordinate (integer).
    /// </summary>
    public int StartX { get; set; } = -10;

    /// <summary>
    /// Starting Y coordinate (integer).
    /// </summary>
    public int StartY { get; set; } = 6;

    /// <summary>
    /// Maximum number of iterations.
    /// </summary>
    public int MaxIterations { get; set; } = 150;

    /// <summary>
    /// Color progression speed (degrees per step in spectrum).
    /// </summary>
    public int ColorSpread { get; set; } = 7;

    /// <summary>
    /// X-axis scale factor (0 = auto-calculate).
    /// </summary>
    public double ScaleFactorX { get; set; } = 0.0;

    /// <summary>
    /// Y-axis scale factor (0 = auto-calculate).
    /// </summary>
    public double ScaleFactorY { get; set; } = 0.0;

    /// <summary>
    /// Show coordinate axes and grid.
    /// </summary>
    public bool ShowAxes { get; set; } = true;

    /// <summary>
    /// Show dots at trajectory points.
    /// </summary>
    public bool ShowDots { get; set; } = true;

    /// <summary>
    /// Show (N, X, Y) labels at each point.
    /// </summary>
    public bool ShowPointLabels { get; set; } = true;

    /// <summary>
    /// Detect and highlight cycles.
    /// </summary>
    public bool DetectCycles { get; set; } = true;

    /// <summary>
    /// Use fixed viewport bounds instead of auto-scaling to data.
    /// </summary>
    public bool UseFixedViewport { get; set; } = false;

    /// <summary>
    /// Export sequence data to CSV file for verification.
    /// </summary>
    public bool ExportToCsv { get; set; } = false;
}
