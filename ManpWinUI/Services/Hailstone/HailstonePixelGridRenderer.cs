using Windows.UI;

namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Renders coordinate grid and axes for Hailstone visualization using pixel operations.
/// Optimized for direct pixel buffer manipulation.
/// </summary>
public class HailstonePixelGridRenderer
{
    // Subtle gray grid, slightly brighter axes
    private static readonly Color GridColor = Color.FromArgb(80, 50, 50, 50);
    private static readonly Color AxesColor = Color.FromArgb(180, 100, 100, 100);
    private static readonly Color TickColor = Color.FromArgb(180, 120, 120, 120);

    private readonly HailstoneCoordinateTransform _transform;

    public HailstonePixelGridRenderer(HailstoneCoordinateTransform transform)
    {
        _transform = transform;
    }

    /// <summary>
    /// Draws grid lines and coordinate axes with tick marks.
    /// </summary>
    public void DrawGrid(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int range = Math.Max(maxX - minX, maxY - minY);
        int tickSpacing = HailstoneRenderHelpers.CalculateTickSpacing(range);

        // Draw vertical grid lines
        DrawVerticalGridLines(pixels, width, height, minX, maxX, minY, maxY,
            tickSpacing, scaleX, scaleY, offsetX, offsetY);

        // Draw horizontal grid lines
        DrawHorizontalGridLines(pixels, width, height, minX, maxX, minY, maxY,
            tickSpacing, scaleX, scaleY, offsetX, offsetY);

        // Draw tick marks on axes
        DrawAxisTickMarks(pixels, width, height, minX, maxX, minY, maxY,
            tickSpacing, scaleX, scaleY, offsetX, offsetY);
    }

    private void DrawVerticalGridLines(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY, int tickSpacing,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int startX = (minX / tickSpacing) * tickSpacing;
        if (startX > minX) startX -= tickSpacing;
        int endX = (maxX / tickSpacing) * tickSpacing;
        if (endX < maxX) endX += tickSpacing;

        for (int x = startX; x <= endX; x += tickSpacing)
        {
            var (screenX, _) = _transform.WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
            var color = (x == 0) ? AxesColor : GridColor;

            // Draw vertical line
            for (int y = 0; y < height; y++)
            {
                HailstonePixelRenderer.SetPixel(pixels, width, height, (int)screenX, y, color);
            }
        }
    }

    private void DrawHorizontalGridLines(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY, int tickSpacing,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int startY = (minY / tickSpacing) * tickSpacing;
        if (startY > minY) startY -= tickSpacing;
        int endY = (maxY / tickSpacing) * tickSpacing;
        if (endY < maxY) endY += tickSpacing;

        for (int y = startY; y <= endY; y += tickSpacing)
        {
            var (_, screenY) = _transform.WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
            var color = (y == 0) ? AxesColor : GridColor;

            // Draw horizontal line
            for (int x = 0; x < width; x++)
            {
                HailstonePixelRenderer.SetPixel(pixels, width, height, x, (int)screenY, color);
            }
        }
    }

    private void DrawAxisTickMarks(byte[] pixels, int width, int height,
        int minX, int maxX, int minY, int maxY, int tickSpacing,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        int tickLength = 5;

        // X-axis tick marks
        var (_, axisY) = _transform.WorldToScreen(0, 0, scaleX, scaleY, offsetX, offsetY);
        if (axisY >= 0 && axisY < height)
        {
            int startX = (minX / tickSpacing) * tickSpacing;
            if (startX > minX) startX -= tickSpacing;
            int endX = (maxX / tickSpacing) * tickSpacing;
            if (endX < maxX) endX += tickSpacing;

            for (int x = startX; x <= endX; x += tickSpacing)
            {
                if (x == 0) continue; // Skip origin

                var (screenX, _) = _transform.WorldToScreen(x, 0, scaleX, scaleY, offsetX, offsetY);
                if (screenX >= 0 && screenX < width)
                {
                    // Draw tick mark
                    for (int dy = -tickLength; dy <= tickLength; dy++)
                    {
                        int y = (int)axisY + dy;
                        if (y >= 0 && y < height)
                        {
                            HailstonePixelRenderer.SetPixel(pixels, width, height, (int)screenX, y, TickColor);
                        }
                    }
                }
            }
        }

        // Y-axis tick marks
        var (axisX, _) = _transform.WorldToScreen(0, 0, scaleX, scaleY, offsetX, offsetY);
        if (axisX >= 0 && axisX < width)
        {
            int startY = (minY / tickSpacing) * tickSpacing;
            if (startY > minY) startY -= tickSpacing;
            int endY = (maxY / tickSpacing) * tickSpacing;
            if (endY < maxY) endY += tickSpacing;

            for (int y = startY; y <= endY; y += tickSpacing)
            {
                if (y == 0) continue; // Skip origin

                var (_, screenY) = _transform.WorldToScreen(0, y, scaleX, scaleY, offsetX, offsetY);
                if (screenY >= 0 && screenY < height)
                {
                    // Draw tick mark
                    for (int dx = -tickLength; dx <= tickLength; dx++)
                    {
                        int x = (int)axisX + dx;
                        if (x >= 0 && x < width)
                        {
                            HailstonePixelRenderer.SetPixel(pixels, width, height, x, (int)screenY, TickColor);
                        }
                    }
                }
            }
        }
    }
}
