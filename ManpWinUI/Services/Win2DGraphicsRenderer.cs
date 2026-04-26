#if HAS_WIN2D
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
#endif
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ManpWinUI.Services
{
    /// <summary>
    /// Win2D implementation of graphics rendering abstraction.
    /// Uses GPU-accelerated DirectX rendering.
    /// 
    /// NOTE: Requires Win2D NuGet package to be installed.
    /// Add to ManpWinUI.csproj:
    ///   &lt;PackageReference Include="Microsoft.Graphics.Win2D" Version="1.2.0" /&gt;
    /// 
    /// To enable this implementation, add to csproj PropertyGroup:
    ///   &lt;DefineConstants&gt;$(DefineConstants);HAS_WIN2D&lt;/DefineConstants&gt;
    /// </summary>
    public class Win2DGraphicsRenderer : IGraphicsRenderer
    {
#if HAS_WIN2D
        private readonly CanvasRenderTarget _renderTarget;
        private CanvasDrawingSession? _drawingSession;
        private byte _currentAlpha = 255;

        public int Width { get; }
        public int Height { get; }

        public Win2DGraphicsRenderer(int width, int height)
        {
            Width = width;
            Height = height;

            // Create Win2D render target with high-quality anti-aliasing
            var device = CanvasDevice.GetSharedDevice();
            _renderTarget = new CanvasRenderTarget(device, width, height, 96);
            _drawingSession = _renderTarget.CreateDrawingSession();

            // Enable high-quality anti-aliasing for smooth lines
            _drawingSession.Antialiasing = CanvasAntialiasing.Antialiased;
        }

        public void Clear(Color color)
        {
            _drawingSession?.Clear(color);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness = 1.0f)
        {
            if (_drawingSession == null) return;

            var adjustedColor = Color.FromArgb(_currentAlpha, color.R, color.G, color.B);
            _drawingSession.DrawLine(x1, y1, x2, y2, adjustedColor, thickness);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1.0f)
        {
            if (_drawingSession == null) return;

            var adjustedColor = Color.FromArgb(_currentAlpha, color.R, color.G, color.B);
            // Win2D supports sub-pixel rendering for smooth anti-aliased lines
            _drawingSession.DrawLine(x1, y1, x2, y2, adjustedColor, thickness);
        }

        public void DrawText(string text, float x, float y, Color color, float fontSize,
            string fontFamily = "Arial", bool bold = false)
        {
            if (_drawingSession == null) return;

            var format = new CanvasTextFormat
            {
                FontFamily = fontFamily,
                FontSize = fontSize,
                FontWeight = bold ? Microsoft.UI.Text.FontWeights.Bold : Microsoft.UI.Text.FontWeights.Normal
            };

            var adjustedColor = Color.FromArgb(_currentAlpha, color.R, color.G, color.B);

            // Use high-quality text anti-aliasing for smooth rendering
            _drawingSession.TextAntialiasing = CanvasTextAntialiasing.Grayscale;
            _drawingSession.DrawText(text, x, y, adjustedColor, format);
        }

        public (float width, float height) MeasureText(string text, float fontSize,
            string fontFamily = "Arial", bool bold = false)
        {
            var format = new CanvasTextFormat
            {
                FontFamily = fontFamily,
                FontSize = fontSize,
                FontWeight = bold ? Microsoft.UI.Text.FontWeights.Bold : Microsoft.UI.Text.FontWeights.Normal
            };

            var device = CanvasDevice.GetSharedDevice();
            using var textLayout = new CanvasTextLayout(device, text, format, 0, 0);

            return ((float)textLayout.LayoutBounds.Width, (float)textLayout.LayoutBounds.Height);
        }

        public void DrawCircle(float centerX, float centerY, float radius, Color color)
        {
            if (_drawingSession == null) return;

            var adjustedColor = Color.FromArgb(_currentAlpha, color.R, color.G, color.B);
            _drawingSession.FillCircle(centerX, centerY, radius, adjustedColor);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            if (_drawingSession == null) return;

            var adjustedColor = Color.FromArgb(_currentAlpha, color.R, color.G, color.B);
            _drawingSession.FillRectangle(x, y, width, height, adjustedColor);
        }

        public void SetPixel(int x, int y, Color color)
        {
            // For individual pixels, draw a tiny filled rectangle
            // Not as efficient as direct pixel access, but maintains abstraction
            DrawRectangle(x, y, 1, 1, color);
        }

        public void SetAlpha(byte alpha)
        {
            _currentAlpha = alpha;
        }

        public void SetTransform(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            if (_drawingSession == null) return;

            // Win2D uses System.Numerics.Matrix3x2 for 2D transformations
            // Matrix layout: [ m11 m12 0 ]
            //                [ m21 m22 0 ]
            //                [ m31 m32 1 ]
            // This matches GDI+ Graphics.Transform behavior
            _drawingSession.Transform = new Matrix3x2(m11, m12, m21, m22, m31, m32);
        }

        public void ResetTransform()
        {
            if (_drawingSession == null) return;

            // Reset to identity matrix (no transformation)
            _drawingSession.Transform = Matrix3x2.Identity;
        }

        public WriteableBitmap ToWriteableBitmap()
        {
            // Flush the drawing session
            _drawingSession?.Dispose();
            _drawingSession = null;

            // Get pixel data from Win2D render target
            var pixels = _renderTarget.GetPixelBytes();

            // Create WriteableBitmap and copy pixels
            var bitmap = new WriteableBitmap(Width, Height);
            using (var stream = bitmap.PixelBuffer.AsStream())
            {
                stream.Write(pixels, 0, pixels.Length);
            }

            // Recreate drawing session for potential future operations
            _drawingSession = _renderTarget.CreateDrawingSession();
            _drawingSession.Antialiasing = CanvasAntialiasing.Antialiased;

            return bitmap;
        }

        public async Task SaveToFileAsync(string filePath)
        {
            // Flush the drawing session
            _drawingSession?.Dispose();
            _drawingSession = null;

            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            using var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            await _renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);

            // Recreate drawing session
            _drawingSession = _renderTarget.CreateDrawingSession();
            _drawingSession.Antialiasing = CanvasAntialiasing.Antialiased;
        }

        public void Dispose()
        {
            _drawingSession?.Dispose();
            _renderTarget?.Dispose();
        }
#else
        // Stub implementation when Win2D is not available
        public int Width { get; }
        public int Height { get; }

        public Win2DGraphicsRenderer(int width, int height)
        {
            Width = width;
            Height = height;
            throw new NotSupportedException(
                "Win2D renderer requires Microsoft.Graphics.Win2D NuGet package. " +
                "Install the package and add <DefineConstants>$(DefineConstants);HAS_WIN2D</DefineConstants> " +
                "to your project PropertyGroup to enable Win2D rendering.");
        }

        public void Clear(Color color) => throw new NotSupportedException();
        public void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness = 1.0f) => throw new NotSupportedException();
        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1.0f) => throw new NotSupportedException();
        public void DrawText(string text, float x, float y, Color color, float fontSize, string fontFamily = "Arial", bool bold = false) => throw new NotSupportedException();
        public (float width, float height) MeasureText(string text, float fontSize, string fontFamily = "Arial", bool bold = false) => throw new NotSupportedException();
        public void DrawCircle(float centerX, float centerY, float radius, Color color) => throw new NotSupportedException();
        public void DrawRectangle(float x, float y, float width, float height, Color color) => throw new NotSupportedException();
        public void SetPixel(int x, int y, Color color) => throw new NotSupportedException();
        public void SetAlpha(byte alpha) => throw new NotSupportedException();
        public void SetTransform(float m11, float m12, float m21, float m22, float m31, float m32) => throw new NotSupportedException();
        public void ResetTransform() => throw new NotSupportedException();
        public WriteableBitmap ToWriteableBitmap() => throw new NotSupportedException();
        public Task SaveToFileAsync(string filePath) => throw new NotSupportedException();
        public void Dispose() { }
#endif
    }
}
