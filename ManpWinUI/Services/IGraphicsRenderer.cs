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
        /// Integer coordinates version for grid/axis rendering.
        /// </summary>
        void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness = 1.0f);

        /// <summary>
        /// Draws a line from (x1, y1) to (x2, y2) with the specified color and thickness.
        /// Floating-point version for smooth anti-aliased rendering.
        /// </summary>
        void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1.0f);

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
        /// Sets a 2D coordinate transformation matrix for all subsequent drawing operations.
        /// This allows drawing in world/mathematical coordinates while the renderer handles
        /// the conversion to screen coordinates (matching GDI+ Graphics.Transform behavior).
        /// </summary>
        /// <param name="m11">Scale X component</param>
        /// <param name="m12">Skew Y component</param>
        /// <param name="m21">Skew X component</param>
        /// <param name="m22">Scale Y component</param>
        /// <param name="m31">Translate X component</param>
        /// <param name="m32">Translate Y component</param>
        void SetTransform(float m11, float m12, float m21, float m22, float m31, float m32);

        /// <summary>
        /// Resets the transformation matrix to identity (no transformation).
        /// </summary>
        void ResetTransform();

        /// <summary>
        /// Gets the raw pixel data as a byte array (BGRA format).
        /// This method can be called from any thread.
        /// </summary>
        byte[] GetPixelData();

        /// <summary>
        /// Gets the rendered image as a WriteableBitmap for display in WinUI.
        /// NOTE: This must be called on the UI thread!
        /// </summary>
        Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap ToWriteableBitmap();

        /// <summary>
        /// Saves the rendered image to a file (PNG format).
        /// </summary>
        Task SaveToFileAsync(string filePath);
    }
}
