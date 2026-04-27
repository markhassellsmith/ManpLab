using ManpCore.Services.Models;
using Microsoft.UI;
using Windows.UI;

namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Renders Hailstone sequence trajectory paths and point markers using pixel operations.
/// Optimized for direct pixel buffer manipulation.
/// </summary>
public class HailstonePixelTrajectoryRenderer
{
    private readonly HailstoneCoordinateTransform _transform;

    public HailstonePixelTrajectoryRenderer(HailstoneCoordinateTransform transform)
    {
        _transform = transform;
    }

    /// <summary>
    /// Draws the sequence trajectory path connecting points.
    /// </summary>
    public void DrawSequence(byte[] pixels, int width, int height,
        List<HailstonePoint> sequence, double scaleX, double scaleY, double offsetX, double offsetY)
    {
        if (sequence.Count < 2) return;

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var p1 = sequence[i];
            var p2 = sequence[i + 1];

            var (x1, y1) = _transform.WorldToScreen(p1.X, p1.Y, scaleX, scaleY, offsetX, offsetY);
            var (x2, y2) = _transform.WorldToScreen(p2.X, p2.Y, scaleX, scaleY, offsetX, offsetY);

            Color lineColor;
            if (p1.IsInCycle)
            {
                // Magenta for cycle segments
                lineColor = Colors.Magenta;
            }
            else
            {
                // Spectrum color from point data
                lineColor = Color.FromArgb(255, p2.Color.R, p2.Color.G, p2.Color.B);
            }

            HailstonePixelRenderer.DrawLine(pixels, width, height,
                (int)x1, (int)y1, (int)x2, (int)y2, lineColor);
        }
    }

    /// <summary>
    /// Draws point markers at each sequence position.
    /// </summary>
    public void DrawPoints(byte[] pixels, int width, int height,
        List<HailstonePoint> sequence, double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int cycleStartIndex = sequence.FindIndex(p => p.IsInCycle);

        foreach (var point in sequence)
        {
            var (screenX, screenY) = _transform.WorldToScreen(
                point.X, point.Y, scaleX, scaleY, offsetX, offsetY);

            DrawMarkerForPoint(pixels, width, height, point, (int)screenX, (int)screenY,
                sequence, cycleStartIndex);
        }
    }

    /// <summary>
    /// Draws the appropriate marker for a specific point based on its type.
    /// </summary>
    private void DrawMarkerForPoint(byte[] pixels, int width, int height,
        HailstonePoint point, int screenX, int screenY,
        List<HailstonePoint> sequence, int cycleStartIndex)
    {
        if (point.Step == 0)
        {
            // Green square for sequence start
            HailstonePixelRenderer.DrawSquare(pixels, width, height, screenX, screenY, 6, Colors.Green);
        }
        else if (point.IsInCycle && sequence.IndexOf(point) == cycleStartIndex)
        {
            // Yellow diamond for cycle start
            HailstonePixelRenderer.DrawDiamond(pixels, width, height, screenX, screenY, 6, Colors.Yellow);
        }
        else if (point.IsInCycle)
        {
            // Magenta circles for cycle points
            HailstonePixelRenderer.DrawCircle(pixels, width, height, screenX, screenY, 3, Colors.Magenta);
        }
        else
        {
            // Small colored dots for regular points
            var color = Color.FromArgb(255, point.Color.R, point.Color.G, point.Color.B);
            HailstonePixelRenderer.DrawCircle(pixels, width, height, screenX, screenY, 2, color);
        }
    }
}
