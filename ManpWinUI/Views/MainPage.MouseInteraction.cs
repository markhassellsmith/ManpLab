using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Mouse/pointer events for zoom and pan.
    /// </summary>
    public sealed partial class MainPage
    {
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

                        // Update center coordinates (paper-on-desk: drag right to move image right)
                        // X: drag right (positive deltaX) shifts image right, revealing left side
                        ViewModel.CenterX -= deltaX * scaleX;

                        // Y: drag down (positive deltaY) shifts image down, revealing top side
                        // Fractal Y increases upward (opposite of screen Y), so ADD to move image down with mouse
                        ViewModel.CenterY += deltaY * scaleY;
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