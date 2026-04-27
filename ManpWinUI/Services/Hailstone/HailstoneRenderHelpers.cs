namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Helper utilities for Hailstone sequence rendering.
/// Provides tick spacing calculations and Hailstone sequence formulas.
/// </summary>
public static class HailstoneRenderHelpers
{
    /// <summary>
    /// Calculates appropriate tick spacing for grid lines based on coordinate range.
    /// Returns spacing that keeps grid readable (not too dense, not too sparse).
    /// </summary>
    /// <param name="range">The coordinate range (max - min)</param>
    /// <returns>Tick spacing in world coordinate units</returns>
    public static int CalculateTickSpacing(int range)
    {
        if (range <= 20) return 1;
        if (range <= 50) return 5;
        if (range <= 100) return 10;
        if (range <= 200) return 20;
        if (range <= 500) return 50;
        if (range <= 1000) return 100;
        return 200;
    }

    /// <summary>
    /// Calculates the next X coordinate in the Hailstone sequence.
    /// Formula depends on parity of both X and Y coordinates.
    /// </summary>
    /// <param name="x">Current X coordinate</param>
    /// <param name="y">Current Y coordinate</param>
    /// <returns>Next X coordinate in sequence</returns>
    public static int CalculateNextX(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => x / 2,
            (true, false) => (x - y - 1) / 2,
            (false, true) => (x - y + 1) / 2,
            (false, false) => (3 * x + y) / 2
        };
    }

    /// <summary>
    /// Calculates the next Y coordinate in the Hailstone sequence.
    /// Formula depends on parity of both X and Y coordinates.
    /// </summary>
    /// <param name="x">Current X coordinate</param>
    /// <param name="y">Current Y coordinate</param>
    /// <returns>Next Y coordinate in sequence</returns>
    public static int CalculateNextY(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => y / 2,
            (true, false) => (x + y + 1) / 2,
            (false, true) => (x + y - 1) / 2,
            (false, false) => (x + 3 * y) / 2
        };
    }
}
