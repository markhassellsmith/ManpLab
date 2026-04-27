namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Handles coordinate transformation calculations for Hailstone sequence rendering.
/// Converts between world coordinates (mathematical space) and screen coordinates (pixels).
/// </summary>
public class HailstoneCoordinateTransform
{
    private const double PaddingPercent = 0.15; // 15% padding around content

    // Default fixed viewport bounds (standard view for Hailstone sequences)
    public const int DefaultMinX = -40;
    public const int DefaultMaxX = 40;
    public const int DefaultMinY = -30;
    public const int DefaultMaxY = 30;

    /// <summary>
    /// Calculates the transformation matrix parameters for world-to-screen coordinate conversion.
    /// </summary>
    /// <param name="minX">Minimum X in world coordinates</param>
    /// <param name="maxX">Maximum X in world coordinates</param>
    /// <param name="minY">Minimum Y in world coordinates</param>
    /// <param name="maxY">Maximum Y in world coordinates</param>
    /// <param name="width">Screen width in pixels</param>
    /// <param name="height">Screen height in pixels</param>
    /// <param name="useFixedViewport">Whether to use fixed viewport bounds</param>
    /// <returns>Transformation parameters (scaleX, scaleY, offsetX, offsetY)</returns>
    public (double scaleX, double scaleY, double offsetX, double offsetY) CalculateTransform(
        int minX, int maxX, int minY, int maxY, int width, int height, bool useFixedViewport)
    {
        double viewMinX, viewMaxX, viewMinY, viewMaxY;

        if (useFixedViewport)
        {
            viewMinX = DefaultMinX;
            viewMaxX = DefaultMaxX;
            viewMinY = DefaultMinY;
            viewMaxY = DefaultMaxY;
        }
        else
        {
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;
            if (rangeX == 0) rangeX = 2;
            if (rangeY == 0) rangeY = 2;

            double paddingX = rangeX * PaddingPercent;
            double paddingY = rangeY * PaddingPercent;

            viewMinX = minX - paddingX;
            viewMaxX = maxX + paddingX;
            viewMinY = minY - paddingY;
            viewMaxY = maxY + paddingY;
        }

        double viewRangeX = viewMaxX - viewMinX;
        double viewRangeY = viewMaxY - viewMinY;

        double scaleX = width / viewRangeX;
        double scaleY = -height / viewRangeY; // Negative to flip Y axis

        double offsetX = -viewMinX * scaleX;
        double offsetY = -viewMaxY * scaleY;

        return (scaleX, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Converts world coordinates to screen coordinates using floating-point precision.
    /// Preserves sub-pixel accuracy for smooth anti-aliased line rendering.
    /// </summary>
    /// <param name="worldX">X coordinate in world space</param>
    /// <param name="worldY">Y coordinate in world space</param>
    /// <param name="scaleX">X axis scale factor</param>
    /// <param name="scaleY">Y axis scale factor</param>
    /// <param name="offsetX">X axis offset</param>
    /// <param name="offsetY">Y axis offset</param>
    /// <returns>Screen coordinates (x, y) in pixels</returns>
    public (float x, float y) WorldToScreen(int worldX, int worldY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        float screenX = (float)(worldX * scaleX + offsetX);
        float screenY = (float)(worldY * scaleY + offsetY);
        return (screenX, screenY);
    }

    /// <summary>
    /// Sets up Win2D coordinate transform matrix to match GDI+ approach from NumericalVisualizations.
    /// Allows drawing in world/mathematical coordinates while Win2D handles screen conversion.
    /// </summary>
    /// <param name="renderer">Graphics renderer to configure</param>
    /// <param name="width">Screen width</param>
    /// <param name="height">Screen height</param>
    /// <param name="minX">Minimum X in world coordinates</param>
    /// <param name="maxX">Maximum X in world coordinates</param>
    /// <param name="minY">Minimum Y in world coordinates</param>
    /// <param name="maxY">Maximum Y in world coordinates</param>
    /// <param name="scaleX">X axis scale factor</param>
    /// <param name="scaleY">Y axis scale factor</param>
    /// <param name="offsetX">X axis offset</param>
    /// <param name="offsetY">Y axis offset</param>
    public void SetupCoordinateTransform(IGraphicsRenderer renderer, int width, int height,
        int minX, int maxX, int minY, int maxY,
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        // Matrix layout: [ m11 m12 ]  = [ scaleX      0         ]
        //                [ m21 m22 ]    [ 0           scaleY    ]
        //                [ m31 m32 ]    [ translateX  translateY]

        float m11 = (float)scaleX;     // Scale X
        float m12 = 0;                  // No skew
        float m21 = 0;                  // No skew
        float m22 = (float)scaleY;     // Scale Y (negative for Y-flip)
        float m31 = (float)offsetX;    // Translate X
        float m32 = (float)offsetY;    // Translate Y

        renderer.SetTransform(m11, m12, m21, m22, m31, m32);
    }
}
