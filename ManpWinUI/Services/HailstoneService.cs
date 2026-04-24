using ManpWinUI.Models;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ManpWinUI.Services;

/// <summary>
/// Service for calculating 2D Hailstone sequences using parity-based transformations.
/// </summary>
public class HailstoneService : IHailstoneService
{
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
    public Task<HailstoneResult> CalculateSequenceAsync(int startX, int startY, int maxIterations, int colorSpread = 7, bool exportToCsv = false)
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
                var color = ColorSpectrum.GetColor((step * colorSpread) % 360);

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
            Debug.WriteLine($"=== HailstoneService: Sequence Calculation Complete ===");
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

            // Export to CSV if requested
            if (exportToCsv)
            {
                ExportToCsv(result, startX, startY);
            }

            return result;
        });
    }

    /// <summary>
    /// Exports the sequence to a CSV file for verification.
    /// </summary>
    private void ExportToCsv(HailstoneResult result, int startX, int startY)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = $"Hailstone_{startX}_{startY}_{timestamp}.csv";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filepath = Path.Combine(documentsPath, filename);

            var sb = new StringBuilder();
            sb.AppendLine($"# Hailstone Sequence (N,X,Y)");
            sb.AppendLine($"# Starting point: (0, {startX}, {startY})");
            sb.AppendLine($"# Total points: {result.Sequence.Count}");

            if (result.HasCycle)
            {
                var cyclePoint = result.Sequence[result.CycleStartIndex];
                sb.AppendLine($"# Cycle Detected: Point ({result.Sequence.Count}, {cyclePoint.X}, {cyclePoint.Y})");
                sb.AppendLine($"# Duplicate of: ({result.CycleStartIndex}, {cyclePoint.X}, {cyclePoint.Y})");
                sb.AppendLine($"# Cycle length: {result.CycleLength}");
            }

            sb.AppendLine();
            sb.AppendLine("Step,X,Y,IsInCycle");

            foreach (var point in result.Sequence)
            {
                sb.AppendLine($"{point.Step},{point.X},{point.Y},{point.IsInCycle}");
            }

            File.WriteAllText(filepath, sb.ToString());
            Debug.WriteLine($"Sequence exported to: {filepath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to export CSV: {ex.Message}");
        }
    }
}
