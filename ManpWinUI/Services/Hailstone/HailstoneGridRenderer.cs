using Windows.UI;

namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Renders coordinate grid and axes for Hailstone sequence visualization.
/// Handles both world-space and screen-space rendering modes.
/// </summary>
public class HailstoneGridRenderer
{
    // Subtle gray grid, slightly brighter axes (matches NumVis aesthetic)
    private static readonly Color GridColor = Color.FromArgb(40, 50, 50, 50);
    private static readonly Color AxesColor = Color.FromArgb(150, 100, 100, 100);

    /// <summary>
    /// Draws grid and axes using world coordinates with transform applied.
    /// Line thickness is automatically adjusted for scale to maintain 1-pixel width.
    /// </summary>
    /// <param name="renderer">Graphics renderer with transform already set up</param>
    /// <param name="minX">Minimum X in world coordinates</param>
    /// <param name="maxX">Maximum X in world coordinates</param>
    /// <param name="minY">Minimum Y in world coordinates</param>
    /// <param name="maxY">Maximum Y in world coordinates</param>
    /// <param name="scaleX">X axis scale factor (for thickness compensation)</param>
    /// <param name="scaleY">Y axis scale factor (for thickness compensation)</param>
    public void DrawGridWithTransform(IGraphicsRenderer renderer,
        int minX, int maxX, int minY, int maxY, double scaleX, double scaleY)
    {
        int range = Math.Max(maxX - minX, maxY - minY);
        int tickSpacing = HailstoneRenderHelpers.CalculateTickSpacing(range);

        // Calculate line thickness in world units to achieve 1-pixel width
        // This compensates for the coordinate transform scaling
        float avgScale = (float)((Math.Abs(scaleX) + Math.Abs(scaleY)) / 2.0);
        float lineThickness = 1.0f / avgScale;

        // Draw vertical grid lines
        DrawVerticalGridLines(renderer, minX, maxX, minY, maxY, range, tickSpacing, lineThickness);

        // Draw horizontal grid lines
        DrawHorizontalGridLines(renderer, minX, maxX, minY, maxY, range, tickSpacing, lineThickness);
    }

    /// <summary>
    /// Draws vertical grid lines in world coordinates.
    /// </summary>
    private void DrawVerticalGridLines(IGraphicsRenderer renderer,
        int minX, int maxX, int minY, int maxY, int range, int tickSpacing, float lineThickness)
    {
        // Calculate grid line start/end positions
        int startX = (minX / tickSpacing) * tickSpacing;
        if (startX > minX) startX -= tickSpacing;
        int endX = (maxX / tickSpacing) * tickSpacing;
        if (endX < maxX) endX += tickSpacing;

        // Extend grid lines beyond visible range for better coverage
        float yMin = minY - range * 0.5f;
        float yMax = maxY + range * 0.5f;

        for (int x = startX; x <= endX; x += tickSpacing)
        {
            // Use brighter color for Y-axis (x=0), subtle gray for other lines
            var color = (x == 0) ? AxesColor : GridColor;
            renderer.DrawLine((float)x, yMin, (float)x, yMax, color, lineThickness);
        }
    }

    /// <summary>
    /// Draws horizontal grid lines in world coordinates.
    /// </summary>
    private void DrawHorizontalGridLines(IGraphicsRenderer renderer,
        int minX, int maxX, int minY, int maxY, int range, int tickSpacing, float lineThickness)
    {
        // Calculate grid line start/end positions
        int startY = (minY / tickSpacing) * tickSpacing;
        if (startY > minY) startY -= tickSpacing;
        int endY = (maxY / tickSpacing) * tickSpacing;
        if (endY < maxY) endY += tickSpacing;

        // Extend grid lines beyond visible range for better coverage
        float xMin = minX - range * 0.5f;
        float xMax = maxX + range * 0.5f;

        for (int y = startY; y <= endY; y += tickSpacing)
        {
            // Use brighter color for X-axis (y=0), subtle gray for other lines
            var color = (y == 0) ? AxesColor : GridColor;
            renderer.DrawLine(xMin, (float)y, xMax, (float)y, color, lineThickness);
        }
    }
}
