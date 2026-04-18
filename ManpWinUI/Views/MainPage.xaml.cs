using ManpWinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views
{
    /// <summary>
    /// Main fractal explorer page with MVVM architecture.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        private bool _isDragging;
        private bool _isPanning; // Track if we're panning (right-click) vs zooming (left-click)
        private Windows.Foundation.Point _dragStartPoint;
        private System.Threading.Timer? _zoomTimer;
        private Grid? _fractalGrid; // Reference to the main fractal display grid

        public MainPage()
        {
            this.InitializeComponent();

            // Get ViewModel from DI container
            ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
            DataContext = ViewModel;

            // Subscribe to property changes to update coordinate axes
            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.CenterX) ||
                    e.PropertyName == nameof(ViewModel.CenterY) ||
                    e.PropertyName == nameof(ViewModel.Zoom) ||
                    e.PropertyName == nameof(ViewModel.ImageWidth) ||
                    e.PropertyName == nameof(ViewModel.ImageHeight) ||
                    e.PropertyName == nameof(ViewModel.FractalImage))
                {
                    UpdateCoordinateAxes();
                }
            };
        }

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

        private void MatchWindowSize_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Get the actual size of the fractal display area (Grid Row 1)
            if (FractalViewbox?.ActualWidth > 0 && FractalViewbox?.ActualHeight > 0)
            {
                // Use the Viewbox size as the target resolution
                var width = (int)FractalViewbox.ActualWidth;
                var height = (int)FractalViewbox.ActualHeight;

                // Round to nearest multiple of 16 for better performance
                width = (width / 16) * 16;
                height = (height / 16) * 16;

                ViewModel.ImageWidth = width;
                ViewModel.ImageHeight = height;
                ViewModel.StatusMessage = $"Resolution matched to window: {width}×{height}";
            }
            else
            {
                ViewModel.StatusMessage = "Window size not available yet";
            }
        }

        private void JuliaModeToggle_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // IsChecked is already bound TwoWay, so IsJuliaMode is updated automatically

            if (ViewModel.IsJuliaMode)
            {
                // Switching TO Julia mode - set appropriate defaults
                ViewModel.CenterX = 0.0;
                ViewModel.CenterY = 0.0;
                ViewModel.Zoom = 0.7;  // Wider view for Julia sets
                ViewModel.StatusMessage = $"Switched to Julia Mode: c = ({ViewModel.JuliaCX:F4}, {ViewModel.JuliaCY:F4})";
            }
            else
            {
                // Switching back to Mandelbrot - restore typical view
                ViewModel.CenterX = -0.5;
                ViewModel.CenterY = 0.0;
                ViewModel.Zoom = 1.0;
                ViewModel.StatusMessage = "Switched to Mandelbrot Mode";
            }

            // Trigger render with new settings
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Classic Julia set - dragon-like shape
            ViewModel.JuliaCX = -0.7;
            ViewModel.JuliaCY = 0.27015;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.7;  // Wider view to see full shape
            ViewModel.StatusMessage = "Julia preset: Classic (Douady's Rabbit)";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset2_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Douady Rabbit - connected fractal
            ViewModel.JuliaCX = -0.12;
            ViewModel.JuliaCY = 0.75;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;  // Wider view for full rabbit shape
            ViewModel.StatusMessage = "Julia preset: Douady Rabbit";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset3_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // San Marco - circular dendrites
            ViewModel.JuliaCX = -0.75;
            ViewModel.JuliaCY = 0.0;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.5;  // Wide view to see dendrite structure
            ViewModel.StatusMessage = "Julia preset: San Marco";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset4_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Siegel Disk - quasi-circular with spirals
            ViewModel.JuliaCX = -0.4;
            ViewModel.JuliaCY = 0.6;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;  // View to see spiral structure
            ViewModel.StatusMessage = "Julia preset: Siegel Disk";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void FractalImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.FractalImage == null)
                return;

            var grid = sender as Grid;
            if (grid == null)
                return;

            _fractalGrid = grid; // Store reference to the grid

            var point = e.GetCurrentPoint(grid);
            _dragStartPoint = point.Position;

            // Check if it's right-click (for panning) or left-click (for zoom)
            if (point.Properties.IsRightButtonPressed)
            {
                _isPanning = true;
                _isDragging = true;
                ViewModel.StatusMessage = "Panning - drag to move view...";
            }
            else if (point.Properties.IsLeftButtonPressed)
            {
                _isPanning = false;
                _isDragging = true;

                // Show selection rectangle for zoom
                SelectionRectangle.Visibility = Visibility.Visible;
                Canvas.SetLeft(SelectionRectangle, _dragStartPoint.X);
                Canvas.SetTop(SelectionRectangle, _dragStartPoint.Y);
                SelectionRectangle.Width = 0;
                SelectionRectangle.Height = 0;

                ViewModel.StatusMessage = "Draw rectangle to zoom...";
            }

            grid.CapturePointer(e.Pointer);
            e.Handled = true;
        }

        private void FractalImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging || ViewModel.FractalImage == null)
                return;

            var grid = sender as Grid;
            if (grid == null)
                return;

            var currentPosition = e.GetCurrentPoint(grid).Position;

            if (_isPanning)
            {
                // Panning mode - move the view
                var deltaX = currentPosition.X - _dragStartPoint.X;
                var deltaY = currentPosition.Y - _dragStartPoint.Y;

                // Get the actual displayed size of the image in the Viewbox
                if (FractalViewbox?.Child is FrameworkElement child)
                {
                    var displayWidth = child.ActualWidth;
                    var displayHeight = child.ActualHeight;

                    if (displayWidth > 0 && displayHeight > 0)
                    {
                        // Current fractal view dimensions (must match FractalRenderService!)
                        var fractalWidth = 3.0 / ViewModel.Zoom;
                        var fractalHeight = fractalWidth * ((double)ViewModel.ImageHeight / ViewModel.ImageWidth);

                        // Calculate the scale factor (fractal units per screen pixel)
                        var scaleX = fractalWidth / displayWidth;
                        var scaleY = fractalHeight / displayHeight;

                        // Update center coordinates (grab-and-drag: drag right to see what's on the left)
                        // X: drag right (positive deltaX) means move view left (negative centerX change)
                        ViewModel.CenterX -= deltaX * scaleX;

                        // Y: drag down (positive deltaY) means move view down in screen coords
                        // But fractal Y increases upward, so we SUBTRACT to move fractal view down
                        ViewModel.CenterY -= deltaY * scaleY;
                    }
                }

                _dragStartPoint = currentPosition;
            }
            else
            {
                // Zoom rectangle mode - draw selection
                var deltaX = currentPosition.X - _dragStartPoint.X;
                var deltaY = currentPosition.Y - _dragStartPoint.Y;

                // Get the aspect ratio from the image (4:3 or width:height)
                var targetAspectRatio = (double)ViewModel.ImageWidth / ViewModel.ImageHeight;

                // Adjust rectangle to maintain aspect ratio
                double rectWidth, rectHeight;
                if (Math.Abs(deltaX / deltaY) > targetAspectRatio)
                {
                    // Width is limiting factor
                    rectWidth = Math.Abs(deltaX);
                    rectHeight = rectWidth / targetAspectRatio;
                    if (deltaY < 0) rectHeight = -rectHeight;
                }
                else
                {
                    // Height is limiting factor
                    rectHeight = Math.Abs(deltaY);
                    rectWidth = rectHeight * targetAspectRatio;
                    if (deltaX < 0) rectWidth = -rectWidth;
                }

                // Update selection rectangle position and size
                var finalLeft = Math.Min(_dragStartPoint.X, _dragStartPoint.X + rectWidth);
                var finalTop = Math.Min(_dragStartPoint.Y, _dragStartPoint.Y + rectHeight);
                Canvas.SetLeft(SelectionRectangle, finalLeft);
                Canvas.SetTop(SelectionRectangle, finalTop);
                SelectionRectangle.Width = Math.Abs(rectWidth);
                SelectionRectangle.Height = Math.Abs(rectHeight);
            }

            e.Handled = true;
        }

        private void FractalImage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                var grid = sender as Grid;
                grid?.ReleasePointerCapture(e.Pointer);

                if (_isPanning)
                {
                    // Pan complete - auto-render the new view
                    if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
                    {
                        ViewModel.RenderMandelbrotCommand.Execute(null);
                    }
                }
                else
                {
                    // Hide selection rectangle
                    SelectionRectangle.Visibility = Visibility.Collapsed;

                    // Only zoom if rectangle is significant size
                    if (SelectionRectangle.Width > 10 && SelectionRectangle.Height > 10)
                    {
                        ZoomToRectangle();
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Rectangle too small - no zoom applied";
                    }
                }

                e.Handled = true;
            }
        }

        private void FractalImage_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.FractalImage == null)
                return;

            var delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;

            // Zoom in if scrolling up, zoom out if scrolling down
            if (delta > 0)
            {
                ViewModel.Zoom *= 2.0;
                ViewModel.StatusMessage = $"Zooming in to {ViewModel.Zoom:F2}x...";
            }
            else if (delta < 0)
            {
                ViewModel.Zoom /= 2.0;
                ViewModel.StatusMessage = $"Zooming out to {ViewModel.Zoom:F2}x...";
            }

            // Debounce: wait 300ms after last scroll before auto-rendering
            _zoomTimer?.Dispose();
            _zoomTimer = new System.Threading.Timer(_ =>
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
                    {
                        ViewModel.RenderMandelbrotCommand.Execute(null);
                    }
                });
            }, null, 300, System.Threading.Timeout.Infinite);

            e.Handled = true;
        }

        private void ZoomToRectangle()
        {
            // Get the selection rectangle bounds in screen coordinates
            var rectLeft = Canvas.GetLeft(SelectionRectangle);
            var rectTop = Canvas.GetTop(SelectionRectangle);
            var rectWidth = SelectionRectangle.Width;
            var rectHeight = SelectionRectangle.Height;

            // Get the Viewbox child (Border or Image container)
            if (FractalViewbox?.Child is FrameworkElement child && 
                ViewModel.FractalImage is Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap bitmap)
            {
                // Get actual bitmap dimensions (not scaled)
                var bitmapWidth = bitmap.PixelWidth;
                var bitmapHeight = bitmap.PixelHeight;

                // Calculate the offset of the image within the grid (due to Viewbox centering)
                // Use the actual Grid dimensions
                var gridWidth = _fractalGrid?.ActualWidth ?? SelectionCanvas.ActualWidth;
                var gridHeight = _fractalGrid?.ActualHeight ?? SelectionCanvas.ActualHeight;

                // Calculate actual displayed size based on Viewbox Uniform stretch behavior
                // The Viewbox scales the image to fit within the grid while maintaining aspect ratio
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

                // The Viewbox centers and scales the image within the canvas
                var imageOffsetX = Math.Max(0, (gridWidth - displayWidth) / 2.0);
                var imageOffsetY = Math.Max(0, (gridHeight - displayHeight) / 2.0);

                // Adjust rectangle coordinates to be relative to the displayed image
                var imageRelativeLeft = rectLeft - imageOffsetX;
                var imageRelativeTop = rectTop - imageOffsetY;

                // Calculate selection center on display
                var rectCenterDisplayX = imageRelativeLeft + rectWidth / 2.0;
                var rectCenterDisplayY = imageRelativeTop + rectHeight / 2.0;

                // Convert display coordinates to bitmap pixel coordinates
                var displayScale = displayWidth / bitmapWidth;
                var rectCenterPixelX = rectCenterDisplayX / displayScale;
                var rectCenterPixelY = rectCenterDisplayY / displayScale;
                var rectWidthPixels = rectWidth / displayScale;
                var rectHeightPixels = rectHeight / displayScale;

                // Current fractal view dimensions (must match FractalRenderService calculation!)
                var fractalWidth = 3.0 / ViewModel.Zoom;
                var fractalHeight = fractalWidth * ((double)bitmapHeight / bitmapWidth);

                // Scale factors (fractal units per BITMAP pixel) for CURRENT view
                var scaleX = fractalWidth / bitmapWidth;
                var scaleY = fractalHeight / bitmapHeight;

                // Convert bitmap pixel position to fractal coordinates
                // Pixel (0,0) maps to top-left of fractal view
                // Pixel center of image (bitmapWidth/2, bitmapHeight/2) maps to fractal center
                var offsetX = (rectCenterPixelX - bitmapWidth / 2.0) * scaleX;
                var offsetY = -(rectCenterPixelY - bitmapHeight / 2.0) * scaleY;

                // New center - ALWAYS use the selection rectangle's center
                var newCenterX = ViewModel.CenterX + offsetX;
                var newCenterY = ViewModel.CenterY + offsetY;

                // Calculate zoom level based on which dimension requires LESS expansion
                // This ensures minimal adjustment to match the bitmap's aspect ratio
                var selectionAspectRatio = rectWidthPixels / rectHeightPixels;
                var targetAspectRatio = (double)bitmapWidth / bitmapHeight;

                double zoomFactor;
                if (selectionAspectRatio > targetAspectRatio)
                {
                    // Selection is wider than target - constrain by width, expand vertically
                    zoomFactor = bitmapWidth / rectWidthPixels;
                }
                else
                {
                    // Selection is taller than target - constrain by height, expand horizontally
                    zoomFactor = bitmapHeight / rectHeightPixels;
                }

                var newZoom = ViewModel.Zoom * zoomFactor;

                // Calculate new view dimensions (always matches bitmap aspect ratio)
                var newFractalWidth = 3.0 / newZoom;
                var newFractalHeight = newFractalWidth * ((double)bitmapHeight / bitmapWidth);

                // Update ViewModel
                ViewModel.CenterX = newCenterX;
                ViewModel.CenterY = newCenterY;
                ViewModel.Zoom = newZoom;

                ViewModel.StatusMessage = $"Zoom: {newZoom:F2}x @ ({newCenterX:F8}, {newCenterY:F8}) | View: {newFractalWidth:F8}×{newFractalHeight:F8}";

                // Auto-render the new view
                if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
                {
                    ViewModel.RenderMandelbrotCommand.Execute(null);
                }
            }
        }
    }
}