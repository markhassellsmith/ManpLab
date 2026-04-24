using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Coordinate axes rendering.
    /// </summary>
    public sealed partial class MainPage
    {
        private void CoordinateAxesCanvas_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            UpdateCoordinateAxes();
        }

        private void UpdateCoordinateAxes()
        {
            CoordinateAxesCanvas.Children.Clear();

            if (ViewModel.FractalImage == null || FractalViewbox?.Child is not FrameworkElement child)
                return;

            // Get the actual Grid dimensions
            var gridWidth = CoordinateAxesCanvas.ActualWidth;
            var gridHeight = CoordinateAxesCanvas.ActualHeight;

            if (gridWidth <= 0 || gridHeight <= 0)
                return;

            // Get bitmap dimensions
            var bitmapWidth = ViewModel.FractalImage.PixelWidth;
            var bitmapHeight = ViewModel.FractalImage.PixelHeight;

            if (bitmapWidth <= 0 || bitmapHeight <= 0)
                return;

            // Calculate actual displayed size based on Viewbox Uniform stretch behavior
            var bitmapAspectRatio = (double)bitmapWidth / bitmapHeight;
            var gridAspectRatio = gridWidth / gridHeight;

            double displayWidth, displayHeight;
            if (bitmapAspectRatio > gridAspectRatio)
            {
                // Image is wider than grid - constrained by width
                displayWidth = gridWidth;
                displayHeight = gridWidth / bitmapAspectRatio;
            }
            else
            {
                // Image is taller than grid - constrained by height
                displayHeight = gridHeight;
                displayWidth = gridHeight * bitmapAspectRatio;
            }

            // Calculate grid offset (Viewbox centering)
            var imageOffsetX = Math.Max(0, (gridWidth - displayWidth) / 2.0);
            var imageOffsetY = Math.Max(0, (gridHeight - displayHeight) / 2.0);

            // Current fractal view dimensions
            var fractalWidth = 3.0 / ViewModel.Zoom;
            var fractalHeight = fractalWidth * ((double)ViewModel.ImageHeight / ViewModel.ImageWidth);

            // Calculate fractal coordinate boundaries
            var leftEdge = ViewModel.CenterX - fractalWidth / 2.0;
            var rightEdge = ViewModel.CenterX + fractalWidth / 2.0;
            var topEdge = ViewModel.CenterY + fractalHeight / 2.0;
            var bottomEdge = ViewModel.CenterY - fractalHeight / 2.0;

            // Calculate appropriate tick interval
            var tickInterval = CalculateTickInterval(fractalWidth);

            // Draw horizontal axis tick marks (X-axis)
            DrawHorizontalTicks(leftEdge, rightEdge, tickInterval, displayWidth, imageOffsetX, imageOffsetY, displayHeight);

            // Draw vertical axis tick marks (Y-axis)
            DrawVerticalTicks(topEdge, bottomEdge, tickInterval, displayHeight, imageOffsetX, imageOffsetY, displayWidth);
        }

        private static double CalculateTickInterval(double viewRange)
        {
            // Find a nice round number for tick spacing
            var roughInterval = viewRange / 8.0; // Aim for about 8 ticks
            var magnitude = Math.Pow(10, Math.Floor(Math.Log10(roughInterval)));

            // Choose 1, 2, 5, or 10 times the magnitude
            var normalized = roughInterval / magnitude;
            double niceInterval;

            if (normalized < 1.5)
                niceInterval = 1.0;
            else if (normalized < 3.0)
                niceInterval = 2.0;
            else if (normalized < 7.0)
                niceInterval = 5.0;
            else
                niceInterval = 10.0;

            return niceInterval * magnitude;
        }

        private void DrawHorizontalTicks(double leftEdge, double rightEdge, double tickInterval,
            double displayWidth, double offsetX, double offsetY, double displayHeight)
        {
            var fractalWidth = rightEdge - leftEdge;
            var startTick = Math.Ceiling(leftEdge / tickInterval) * tickInterval;

            for (var fractalX = startTick; fractalX <= rightEdge; fractalX += tickInterval)
            {
                // Convert fractal coordinate to screen pixel
                var normalizedX = (fractalX - leftEdge) / fractalWidth;
                var screenX = offsetX + normalizedX * displayWidth;

                // Draw tick mark at bottom
                var tickLine = new Microsoft.UI.Xaml.Shapes.Line
                {
                    X1 = screenX,
                    Y1 = offsetY + displayHeight,
                    X2 = screenX,
                    Y2 = offsetY + displayHeight - 8,
                    Stroke = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    StrokeThickness = 1,
                    Opacity = 0.7
                };
                CoordinateAxesCanvas.Children.Add(tickLine);

                // Add label every other tick
                if (Math.Abs(fractalX / tickInterval % 2) < 0.01)
                {
                    var label = new TextBlock
                    {
                        Text = FormatCoordinate(fractalX),
                        FontSize = 10,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                        Opacity = 0.8
                    };

                    Canvas.SetLeft(label, screenX - 20);
                    Canvas.SetTop(label, offsetY + displayHeight - 22);
                    CoordinateAxesCanvas.Children.Add(label);
                }
            }
        }

        private void DrawVerticalTicks(double topEdge, double bottomEdge, double tickInterval,
            double displayHeight, double offsetX, double offsetY, double displayWidth)
        {
            var fractalHeight = topEdge - bottomEdge;
            var startTick = Math.Ceiling(bottomEdge / tickInterval) * tickInterval;

            for (var fractalY = startTick; fractalY <= topEdge; fractalY += tickInterval)
            {
                // Convert fractal coordinate to screen pixel (Y is inverted)
                var normalizedY = (topEdge - fractalY) / fractalHeight;
                var screenY = offsetY + normalizedY * displayHeight;

                // Draw tick mark at left edge
                var tickLine = new Microsoft.UI.Xaml.Shapes.Line
                {
                    X1 = offsetX,
                    Y1 = screenY,
                    X2 = offsetX + 8,
                    Y2 = screenY,
                    Stroke = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    StrokeThickness = 1,
                    Opacity = 0.7
                };
                CoordinateAxesCanvas.Children.Add(tickLine);

                // Add label every other tick
                if (Math.Abs(fractalY / tickInterval % 2) < 0.01)
                {
                    var label = new TextBlock
                    {
                        Text = FormatCoordinate(fractalY),
                        FontSize = 10,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                        Opacity = 0.8
                    };

                    Canvas.SetLeft(label, offsetX + 12);
                    Canvas.SetTop(label, screenY - 7);
                    CoordinateAxesCanvas.Children.Add(label);
                }
            }
        }

        private static string FormatCoordinate(double value)
        {
            // Use scientific notation for very small or very large numbers
            if (Math.Abs(value) < 0.0001 || Math.Abs(value) > 10000)
            {
                return value.ToString("0.##e0");
            }
            else if (Math.Abs(value) < 0.01)
            {
                return value.ToString("F4");
            }
            else if (Math.Abs(value) < 1)
            {
                return value.ToString("F3");
            }
            else
            {
                return value.ToString("F2");
            }
        }
    }
}
