namespace ManpWinUI.Models;

/// <summary>
/// Represents the result of a 2D Hailstone sequence calculation.
/// </summary>
public class HailstoneResult
{
    /// <summary>
    /// The complete sequence of points from start to cycle or max iterations.
    /// </summary>
    public List<HailstonePoint> Sequence { get; set; } = new();

    /// <summary>
    /// True if a cycle was detected in the sequence.
    /// </summary>
    public bool HasCycle { get; set; }

    /// <summary>
    /// The index in the sequence where the cycle begins (if HasCycle is true).
    /// </summary>
    public int CycleStartIndex { get; set; }

    /// <summary>
    /// The length of the detected cycle (if HasCycle is true).
    /// </summary>
    public int CycleLength { get; set; }

    /// <summary>
    /// Time taken to calculate the sequence.
    /// </summary>
    public TimeSpan CalculationTime { get; set; }

    /// <summary>
    /// Minimum X coordinate in the sequence (for auto-scaling).
    /// </summary>
    public int MinX { get; set; }

    /// <summary>
    /// Maximum X coordinate in the sequence (for auto-scaling).
    /// </summary>
    public int MaxX { get; set; }

    /// <summary>
    /// Minimum Y coordinate in the sequence (for auto-scaling).
    /// </summary>
    public int MinY { get; set; }

    /// <summary>
    /// Maximum Y coordinate in the sequence (for auto-scaling).
    /// </summary>
    public int MaxY { get; set; }
}
