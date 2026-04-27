using ManpWinUI.Models;
using Microsoft.UI;
using Windows.UI;

namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Renders Hailstone sequence trajectory paths and point markers.
/// Handles both line segments and point visualization with proper color coding.
/// </summary>
public class HailstoneTrajectoryRenderer
{
    private readonly HailstoneCoordinateTransform _transform;

    public HailstoneTrajectoryRenderer(HailstoneCoordinateTransform transform)
    {
        _transform = transform;
    }

    /// <summary>
    /// Draws the sequence trajectory path using world coordinates.
    /// Line thickness is automatically adjusted for scale to maintain 1.2 pixel width.
    /// All lines use uniform thickness - cycle segments distinguished by color only.
    /// </summary>
    /// <param name="renderer">Graphics renderer with transform already set up</param>
    /// <param name="sequence">List of points in the Hailstone sequence</param>
    /// <param name="scaleX">X axis scale factor (for thickness compensation)</param>
    /// <param name="scaleY">Y axis scale factor (for thickness compensation)</param>
    public void DrawTrajectoryPath(IGraphicsRenderer renderer, List<HailstonePoint> sequence,
        double scaleX, double scaleY)
    {
        if (sequence.Count < 2) return;

        // Calculate line thickness in world units to achieve 1.2 pixel width
        // This compensates for the coordinate transform scaling
        float avgScale = (float)((Math.Abs(scaleX) + Math.Abs(scaleY)) / 2.0);
        float lineThickness = 1.2f / avgScale;

        // Draw line segments between consecutive points
        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var p1 = sequence[i];
            var p2 = sequence[i + 1];

            Color lineColor;
            if (p1.IsInCycle)
            {
                // Magenta for cycle segments - same thickness as normal lines
                lineColor = Colors.Magenta;
            }
            else
            {
                // Spectrum color from point data for normal segments
                lineColor = Color.FromArgb(255, p2.Color.R, p2.Color.G, p2.Color.B);
            }

            renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, lineColor, lineThickness);
        }
    }

    /// <summary>
    /// Draws point markers at each position in the sequence.
    /// Uses fixed pixel-size markers by temporarily resetting the transform.
    /// </summary>
    /// <param name="renderer">Graphics renderer (transform will be reset)</param>
    /// <param name="sequence">List of points in the Hailstone sequence</param>
    /// <param name="scaleX">X axis scale factor (for position conversion)</param>
    /// <param name="scaleY">Y axis scale factor (for position conversion)</param>
    /// <param name="offsetX">X axis offset (for position conversion)</param>
    /// <param name="offsetY">Y axis offset (for position conversion)</param>
    public void DrawPointMarkers(IGraphicsRenderer renderer, List<HailstonePoint> sequence,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int cycleStartIndex = sequence.FindIndex(p => p.IsInCycle);

        // Reset transform to prevent scaling the marker sizes
        // We'll manually convert each point's world position to screen position
        renderer.ResetTransform();

        foreach (var point in sequence)
        {
            // Convert world position to screen position
            var (screenX, screenY) = _transform.WorldToScreen(
                point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

            DrawMarkerForPoint(renderer, point, screenX, screenY, sequence, cycleStartIndex);
        }
    }

    /// <summary>
    /// Draws the appropriate marker for a specific point based on its type.
    /// </summary>
    private void DrawMarkerForPoint(IGraphicsRenderer renderer, HailstonePoint point,
        float screenX, float screenY, List<HailstonePoint> sequence, int cycleStartIndex)
    {
        if (point.Step == 0)
        {
            // Green square for sequence start point (4x4 pixels)
            renderer.DrawRectangle(screenX - 2, screenY - 2, 4, 4, Colors.Green);
        }
        else if (point.IsInCycle && sequence.IndexOf(point) == cycleStartIndex)
        {
            // Yellow circle for cycle start point (3 pixel radius)
            renderer.DrawCircle(screenX, screenY, 3, Colors.Yellow);
        }
        else if (point.IsInCycle)
        {
            // Magenta circles for cycle points (3 pixel radius)
            renderer.DrawCircle(screenX, screenY, 3, Colors.Magenta);
        }
        else
        {
            // Small colored dots for regular points (2 pixel radius)
            var color = Color.FromArgb(255, point.Color.R, point.Color.G, point.Color.B);
            renderer.DrawCircle(screenX, screenY, 2, color);
        }
    }
}
