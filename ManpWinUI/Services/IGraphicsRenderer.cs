using Windows.UI;

namespace ManpWinUI.Services
{
    /// <summary>
    /// Abstraction for 2D graphics rendering.
    /// Allows swapping between Win2D, SkiaSharp, or other rendering backends.
    /// </summary>
    public interface IGraphicsRenderer : IDisposable
    {
        /// <summary>
        /// Gets the width of the rendering surface in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the rendering surface in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Clears the entire surface with the specified color.
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Draws a line from (x1, y1) to (x2, y2) with the specified color and thickness.
        /// </summary>
        void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness = 1.0f);

        /// <summary>
        /// Draws text at the specified position with the given font parameters.
        /// </summary>
        void DrawText(string text, float x, float y, Color color, float fontSize, 
            string fontFamily = "Arial", bool bold = false);

        /// <summary>
        /// Measures the dimensions of text with the given font parameters.
        /// </summary>
        (float width, float height) MeasureText(string text, float fontSize, 
            string fontFamily = "Arial", bool bold = false);

        /// <summary>
        /// Draws a filled circle at the specified center point.
        /// </summary>
        void DrawCircle(float centerX, float centerY, float radius, Color color);

        /// <summary>
        /// Draws a filled rectangle with the specified dimensions.
        /// </summary>
        void DrawRectangle(float x, float y, float width, float height, Color color);

        /// <summary>
        /// Sets a pixel at the specified coordinate to the given color.
        /// For compatibility with existing pixel-based rendering code.
        /// </summary>
        void SetPixel(int x, int y, Color color);

        /// <summary>
        /// Applies alpha blending when drawing (useful for transparency effects).
        /// </summary>
        void SetAlpha(byte alpha);

        /// <summary>
        /// Gets the rendered image as a WriteableBitmap for display in WinUI.
        /// </summary>
        Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap ToWriteableBitmap();

        /// <summary>
        /// Saves the rendered image to a file (PNG format).
        /// </summary>
        Task SaveToFileAsync(string filePath);
    }
}
