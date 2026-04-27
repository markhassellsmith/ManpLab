using Windows.UI;

namespace ManpWinUI.Services.Hailstone;

/// <summary>
/// Low-level pixel manipulation utilities for direct bitmap rendering.
/// Provides fast pixel-based drawing primitives (lines, circles, shapes).
/// </summary>
public static class HailstonePixelRenderer
{
    /// <summary>
    /// Sets a single pixel in the bitmap buffer with bounds checking.
    /// </summary>
    /// <param name="pixels">Pixel buffer (BGRA format)</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="color">Color to set</param>
    public static void SetPixel(byte[] pixels, int width, int height, int x, int y, Color color)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        int index = (y * width + x) * 4;
        pixels[index] = color.B;     // Blue
        pixels[index + 1] = color.G; // Green
        pixels[index + 2] = color.R; // Red
        pixels[index + 3] = color.A; // Alpha
    }

    /// <summary>
    /// Draws a line between two points using Bresenham's algorithm.
    /// </summary>
    public static void DrawLine(byte[] pixels, int width, int height,
        int x1, int y1, int x2, int y2, Color color)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            SetPixel(pixels, width, height, x1, y1, color);

            if (x1 == x2 && y1 == y2)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }

    /// <summary>
    /// Draws a filled circle using midpoint circle algorithm.
    /// </summary>
    public static void DrawCircle(byte[] pixels, int width, int height,
        int centerX, int centerY, int radius, Color color)
    {
        // Draw filled circle by drawing horizontal lines
        for (int y = -radius; y <= radius; y++)
        {
            int x = (int)Math.Sqrt(radius * radius - y * y);
            for (int i = -x; i <= x; i++)
            {
                SetPixel(pixels, width, height, centerX + i, centerY + y, color);
            }
        }
    }

    /// <summary>
    /// Draws a filled square.
    /// </summary>
    public static void DrawSquare(byte[] pixels, int width, int height,
        int centerX, int centerY, int size, Color color)
    {
        int halfSize = size / 2;
        for (int y = centerY - halfSize; y <= centerY + halfSize; y++)
        {
            for (int x = centerX - halfSize; x <= centerX + halfSize; x++)
            {
                SetPixel(pixels, width, height, x, y, color);
            }
        }
    }

    /// <summary>
    /// Draws a filled diamond (rotated square).
    /// </summary>
    public static void DrawDiamond(byte[] pixels, int width, int height,
        int centerX, int centerY, int size, Color color)
    {
        int halfSize = size / 2;
        for (int y = -halfSize; y <= halfSize; y++)
        {
            int rowWidth = halfSize - Math.Abs(y);
            for (int x = -rowWidth; x <= rowWidth; x++)
            {
                SetPixel(pixels, width, height, centerX + x, centerY + y, color);
            }
        }
    }
}
