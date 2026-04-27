namespace ManpCore.Services.Models;

/// <summary>
/// Represents a single point in a 2D Hailstone sequence.
/// </summary>
public class HailstonePoint
{
    /// <summary>
    /// Step number (iteration index, starting from 0).
    /// </summary>
    public int Step { get; set; }

    /// <summary>
    /// X coordinate (integer).
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y coordinate (integer).
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// True if this point is part of a detected cycle.
    /// </summary>
    public bool IsInCycle { get; set; }

    /// <summary>
    /// Color for this point/segment (R, G, B).
    /// </summary>
    public (byte R, byte G, byte B) Color { get; set; }
}
