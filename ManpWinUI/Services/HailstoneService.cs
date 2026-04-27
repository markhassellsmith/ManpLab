using ManpCore.Services.Algorithms;
using ManpCore.Services.Models;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ManpWinUI.Services;

/// <summary>
/// WinUI-specific service for calculating 2D Hailstone sequences.
/// Wraps the shared HailstoneCalculator and adds UI-specific features like CSV export.
/// </summary>
public class HailstoneService : IHailstoneService
{
    private readonly IHailstoneCalculator _calculator;

    /// <summary>
    /// Initializes a new instance of the HailstoneService class.
    /// </summary>
    /// <param name="calculator">The shared platform-agnostic calculator.</param>
    public HailstoneService(IHailstoneCalculator calculator)
    {
        _calculator = calculator;
    }

    /// <summary>
    /// Calculates a 2D Hailstone sequence with cycle detection.
    /// </summary>
    public async Task<HailstoneResult> CalculateSequenceAsync(int startX, int startY, int maxIterations, int colorSpread = 7, bool exportToCsv = false)
    {
        // Use the shared calculator
        var result = await _calculator.CalculateSequenceAsync(startX, startY, maxIterations, colorSpread);

        // Export to CSV if requested (UI-specific feature)
        if (exportToCsv)
        {
            ExportToCsv(result, startX, startY);
        }

        return result;
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
