using ManpCore.Services.Color;
using ManpCore.Services.Models;
using System.Diagnostics;

namespace ManpCore.Services.Algorithms;

/// <summary>
/// Platform-agnostic implementation of 2D Hailstone sequence calculation.
/// Uses parity-based transformations with cycle detection.
/// </summary>
public class HailstoneCalculator : IHailstoneCalculator
{
    private readonly IColorPalette _colorPalette;

    /// <summary>
    /// Initializes a new instance of the HailstoneCalculator class.
    /// </summary>
    /// <param name="colorPalette">Color palette for point coloring.</param>
    public HailstoneCalculator(IColorPalette colorPalette)
    {
        _colorPalette = colorPalette;
    }

    /// <summary>
    /// Calculates the next X coordinate based on current (x, y) parities.
    /// </summary>
    private static int NextX(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => x / 2,           // Both even
            (true, false) => x / 2 + 1,      // X even, Y odd
            (false, true) => 3 * x - 1,      // X odd, Y even
            (false, false) => 3 * x + 1      // Both odd
        };
    }

    /// <summary>
    /// Calculates the next Y coordinate based on current (x, y) parities.
    /// </summary>
    private static int NextY(int x, int y)
    {
        bool xEven = (x % 2 == 0);
        bool yEven = (y % 2 == 0);

        return (xEven, yEven) switch
        {
            (true, true) => y / 2,           // Both even
            (true, false) => 3 * y - 1,      // X even, Y odd
            (false, true) => y / 2 - 1,      // X odd, Y even
            (false, false) => 3 * y - 3      // Both odd
        };
    }

    /// <summary>
    /// Calculates a 2D Hailstone sequence with cycle detection.
    /// </summary>
    public Task<HailstoneResult> CalculateSequenceAsync(int startX, int startY, int maxIterations, int colorSpread = 7)
    {
        return Task.Run(() =>
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new HailstoneResult();
            var seen = new HashSet<(int, int)>();
            var sequence = new List<HailstonePoint>();

            int x = startX;
            int y = startY;
            int minX = x, maxX = x, minY = y, maxY = y;

            for (int step = 0; step < maxIterations; step++)
            {
                // Check for cycle
                if (seen.Contains((x, y)))
                {
                    result.HasCycle = true;

                    // Find where the cycle starts
                    for (int i = 0; i < sequence.Count; i++)
                    {
                        if (sequence[i].X == x && sequence[i].Y == y)
                        {
                            result.CycleStartIndex = i;
                            result.CycleLength = step - i;

                            // Mark all points in the cycle
                            for (int j = i; j < sequence.Count; j++)
                            {
                                sequence[j].IsInCycle = true;
                            }
                            break;
                        }
                    }
                    break;
                }

                // Calculate color based on step and color spread
                var color = _colorPalette.GetColor((step * colorSpread) % 360);

                // Add current point
                seen.Add((x, y));
                sequence.Add(new HailstonePoint
                {
                    Step = step,
                    X = x,
                    Y = y,
                    IsInCycle = false,
                    Color = color
                });

                // Update bounding box
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

                // Calculate next point
                int nextX = NextX(x, y);
                int nextY = NextY(x, y);
                x = nextX;
                y = nextY;
            }

            stopwatch.Stop();

            result.Sequence = sequence;
            result.MinX = minX;
            result.MaxX = maxX;
            result.MinY = minY;
            result.MaxY = maxY;
            result.CalculationTime = stopwatch.Elapsed;

            // Debug output
            Debug.WriteLine($"=== HailstoneCalculator: Sequence Calculation Complete ===");
            Debug.WriteLine($"Starting point: ({startX}, {startY})");
            Debug.WriteLine($"Max iterations: {maxIterations}");
            Debug.WriteLine($"Points calculated: {sequence.Count}");
            Debug.WriteLine($"Bounds: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
            Debug.WriteLine($"Has cycle: {result.HasCycle}");
            if (result.HasCycle)
            {
                Debug.WriteLine($"Cycle: starts at step {result.CycleStartIndex}, length {result.CycleLength}");
            }
            Debug.WriteLine($"Calculation time: {stopwatch.ElapsedMilliseconds}ms");

            return result;
        });
    }
}
