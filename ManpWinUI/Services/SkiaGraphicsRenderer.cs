using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace ManpWinUI.Services
{
    /// <summary>
    /// SkiaSharp implementation of graphics rendering abstraction.
    /// Uses cross-platform CPU-based rendering.
    /// 
    /// TODO: Implement in future branch when adding SkiaSharp NuGet package.
    /// NuGet packages needed:
    ///   - SkiaSharp
    ///   - SkiaSharp.Views.WinUI
    /// </summary>
    public class SkiaGraphicsRenderer : IGraphicsRenderer
    {
        // Fields for SkiaSharp objects (add when implementing):
        // - SKSurface _surface
        // - SKCanvas _canvas
        // - SKBitmap _bitmap

        public int Width { get; }
        public int Height { get; }

        public SkiaGraphicsRenderer(int width, int height)
        {
            Width = width;
            Height = height;

            throw new NotImplementedException(
                "SkiaSharp renderer not yet implemented. " +
                "Install SkiaSharp NuGet packages and implement this class in a future branch.");

            // Future implementation:
            // _bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            // _surface = SKSurface.Create(_bitmap.Info, _bitmap.GetPixels());
            // _canvas = _surface.Canvas;
        }

        public void Clear(Color color)
        {
            throw new NotImplementedException();
            // _canvas.Clear(new SKColor(color.R, color.G, color.B, color.A));
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness = 1.0f)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { Color = ..., StrokeWidth = thickness, IsAntialias = true };
            // _canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1.0f)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { Color = ..., StrokeWidth = thickness, IsAntialias = true };
            // _canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        public void DrawText(string text, float x, float y, Color color, float fontSize,
            string fontFamily = "Arial", bool bold = false)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { Color = ..., TextSize = fontSize, Typeface = ... };
            // _canvas.DrawText(text, x, y, paint);
        }

        public (float width, float height) MeasureText(string text, float fontSize,
            string fontFamily = "Arial", bool bold = false)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { TextSize = fontSize, Typeface = ... };
            // var bounds = new SKRect();
            // paint.MeasureText(text, ref bounds);
            // return (bounds.Width, bounds.Height);
        }

        public void DrawCircle(float centerX, float centerY, float radius, Color color)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { Color = ..., IsAntialias = true };
            // _canvas.DrawCircle(centerX, centerY, radius, paint);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            throw new NotImplementedException();
            // var paint = new SKPaint { Color = ... };
            // _canvas.DrawRect(x, y, width, height, paint);
        }

        public void SetPixel(int x, int y, Color color)
        {
            throw new NotImplementedException();
            // _bitmap.SetPixel(x, y, new SKColor(color.R, color.G, color.B, color.A));
        }

        public void SetAlpha(byte alpha)
        {
            throw new NotImplementedException();
            // Store alpha value for use in subsequent draw calls
        }

        public WriteableBitmap ToWriteableBitmap()
        {
            throw new NotImplementedException();
            // Get pixel data from SKBitmap and copy to WriteableBitmap
        }

        public async Task SaveToFileAsync(string filePath)
        {
            throw new NotImplementedException();
            // Use SKImage.Encode() to save as PNG
        }

        public void Dispose()
        {
            throw new NotImplementedException();
            // _canvas?.Dispose();
            // _surface?.Dispose();
            // _bitmap?.Dispose();
        }
    }
}
