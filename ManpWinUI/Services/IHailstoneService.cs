using ManpWinUI.Models;

namespace ManpWinUI.Services;

/// <summary>
/// Service for calculating 2D Hailstone sequences on the integer lattice.
/// </summary>
public interface IHailstoneService
{
    /// <summary>
    /// Calculates a 2D Hailstone sequence starting from (startX, startY).
    /// </summary>
    /// <param name="startX">Starting X coordinate (integer).</param>
    /// <param name="startY">Starting Y coordinate (integer).</param>
    /// <param name="maxIterations">Maximum number of steps to calculate.</param>
    /// <param name="colorSpread">Color progression speed (degrees per step).</param>
    /// <param name="exportToCsv">Whether to export the sequence to a CSV file.</param>
    /// <returns>Result containing the sequence and cycle detection information.</returns>
    Task<HailstoneResult> CalculateSequenceAsync(int startX, int startY, int maxIterations, int colorSpread = 7, bool exportToCsv = false);
}
