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
            System.Diagnostics.Debug.WriteLine($"[PointerPressed] EVENT FIRED - _isDragging={_isDragging}, _lastClickTime={_lastClickTime:HH:mm:ss.fff}");

            if (ViewModel.FractalImage == null)
            {
                System.Diagnostics.Debug.WriteLine("[PointerPressed] Exiting - no fractal image");
                return;
            }

            var grid = sender as Grid;
            if (grid == null)
            {
                System.Diagnostics.Debug.WriteLine("[PointerPressed] Exiting - no grid");
                return;
            }

            _fractalGrid = grid; // Store reference to the grid

            var point = e.GetCurrentPoint(grid);
            _dragStartPoint = point.Position;

            // Check if it's right-click (for panning) or left-click (for zoom)
            if (point.Properties.IsRightButtonPressed)
            {
                System.Diagnostics.Debug.WriteLine("[PointerPressed] Right button - starting pan");
                _isPanning = true;
                _isDragging = true;
                ViewModel.StatusMessage = ViewModel.IsHailstoneMode 
                    ? "Panning Hailstone viewport - drag to move view..." 
                    : "Panning - drag to move view...";
                grid.CapturePointer(e.Pointer);
            }
            else if (point.Properties.IsLeftButtonPressed)
            {
                // Check if this is a potential double-click (within 500ms of last click)
                var now = DateTime.Now;
                var timeSinceLastClick = (now - _lastClickTime).TotalMilliseconds;
                System.Diagnostics.Debug.WriteLine($"[PointerPressed] Left button - time since last click: {timeSinceLastClick:F0}ms");

                // If this looks like a double-click, mark it for processing in PointerReleased
                if (timeSinceLastClick < 500)
                {
                    System.Diagnostics.Debug.WriteLine("[PointerPressed] Detected double-click - marking for recenter");
                    _doubleClickHandled = true; // Flag to process recenter on release
                    _lastClickTime = now;
                    e.Handled = true;
                    return; // Don't start drag
                }

                // Regular single click - start box zoom
                _lastClickTime = now;
                System.Diagnostics.Debug.WriteLine("[PointerPressed] Starting box zoom drag");
                _isPanning = false;
                _isDragging = true;

                // Show selection rectangle for zoom
                SelectionRectangle.Visibility = Visibility.Visible;
                Canvas.SetLeft(SelectionRectangle, _dragStartPoint.X);
                Canvas.SetTop(SelectionRectangle, _dragStartPoint.Y);
                SelectionRectangle.Width = 0;
                SelectionRectangle.Height = 0;

                ViewModel.StatusMessage = "Draw rectangle to zoom...";
                grid.CapturePointer(e.Pointer);
            }

            e.Handled = true;
            System.Diagnostics.Debug.WriteLine($"[PointerPressed] Exiting - _isDragging={_isDragging}, _doubleClickHandled={_doubleClickHandled}");
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
                        if (ViewModel.IsHailstoneMode)
                        {
                            // Hailstone panning: manipulate viewport bounds
                            if (ViewModel.CurrentHailstoneResult != null)
                            {
                                // Get current viewport (or use sequence bounds if no custom viewport)
                                double viewMinX = ViewModel.HailstoneViewportMinX ?? ViewModel.CurrentHailstoneResult.MinX;
                                double viewMaxX = ViewModel.HailstoneViewportMaxX ?? ViewModel.CurrentHailstoneResult.MaxX;
                                double viewMinY = ViewModel.HailstoneViewportMinY ?? ViewModel.CurrentHailstoneResult.MinY;
                                double viewMaxY = ViewModel.HailstoneViewportMaxY ?? ViewModel.CurrentHailstoneResult.MaxY;

                                // Add 15% padding if using auto-bounds
                                if (!ViewModel.HasCustomHailstoneViewport)
                                {
                                    double rangeX = viewMaxX - viewMinX;
                                    double rangeY = viewMaxY - viewMinY;
                                    if (rangeX == 0) rangeX = 2;
                                    if (rangeY == 0) rangeY = 2;
                                    double paddingX = rangeX * 0.15;
                                    double paddingY = rangeY * 0.15;
                                    viewMinX -= paddingX;
                                    viewMaxX += paddingX;
                                    viewMinY -= paddingY;
                                    viewMaxY += paddingY;
                                }

                                var viewRangeX = viewMaxX - viewMinX;
                                var viewRangeY = viewMaxY - viewMinY;

                                // Calculate scale (units per pixel)
                                var scaleX = viewRangeX / displayWidth;
                                var scaleY = viewRangeY / displayHeight;

                                // Update viewport (paper-on-desk: drag right to move image right)
                                // X: drag right shifts image right, revealing left side
                                viewMinX -= deltaX * scaleX;
                                viewMaxX -= deltaX * scaleX;

                                // Y: drag down shifts image down, revealing top side
                                // Screen Y increases downward, world Y increases upward
                                viewMinY += deltaY * scaleY;
                                viewMaxY += deltaY * scaleY;

                                ViewModel.SetHailstoneViewport(viewMinX, viewMaxX, viewMinY, viewMaxY);
                            }
                        }
                        else
                        {
                            // Standard fractal panning
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

                            // Reset tracked center point since user explicitly moved the view
                            ViewModel.ResetTrackedCenterPoint();
                        }
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
            System.Diagnostics.Debug.WriteLine($"[PointerReleased] EVENT FIRED - _isDragging={_isDragging}, _doubleClickHandled={_doubleClickHandled}");

            // Handle double-click recenter
            if (_doubleClickHandled)
            {
                System.Diagnostics.Debug.WriteLine("[PointerReleased] Processing double-click recenter");
                _doubleClickHandled = false;
                _isDragging = false;
                _isPanning = false;

                var grid = sender as Grid;
                if (grid != null && ViewModel.FractalImage != null)
                {
                    // Get click position
                    var clickPosition = e.GetCurrentPoint(grid).Position;

                    // Get the Viewbox child for coordinate conversion
                    if (FractalViewbox?.Child is FrameworkElement child &&
                        ViewModel.FractalImage is Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap bitmap)
                    {
                        // Get actual bitmap dimensions
                        var bitmapWidth = bitmap.PixelWidth;
                        var bitmapHeight = bitmap.PixelHeight;

                        // Calculate the offset of the image within the grid
                        var gridWidth = _fractalGrid?.ActualWidth ?? grid.ActualWidth;
                        var gridHeight = _fractalGrid?.ActualHeight ?? grid.ActualHeight;

                        // Calculate actual displayed size based on Viewbox behavior
                        var bitmapAspectRatio = (double)bitmapWidth / bitmapHeight;
                        var gridAspectRatio = gridWidth / gridHeight;

                        double displayWidth, displayHeight;
                        if (bitmapAspectRatio > gridAspectRatio)
                        {
                            displayWidth = gridWidth;
                            displayHeight = gridWidth / bitmapAspectRatio;
                        }
                        else
                        {
                            displayHeight = gridHeight;
                            displayWidth = gridHeight * bitmapAspectRatio;
                        }

                        var imageOffsetX = Math.Max(0, (gridWidth - displayWidth) / 2.0);
                        var imageOffsetY = Math.Max(0, (gridHeight - displayHeight) / 2.0);

                        // Adjust click position to be relative to the displayed image
                        var imageRelativeX = clickPosition.X - imageOffsetX;
                        var imageRelativeY = clickPosition.Y - imageOffsetY;

                        // Check if click is within the image bounds
                        if (imageRelativeX >= 0 && imageRelativeX <= displayWidth &&
                            imageRelativeY >= 0 && imageRelativeY <= displayHeight)
                        {
                            // Convert display coordinates to bitmap pixel coordinates
                            var displayScale = displayWidth / bitmapWidth;
                            var pixelX = imageRelativeX / displayScale;
                            var pixelY = imageRelativeY / displayScale;

                            if (ViewModel.IsHailstoneMode)
                            {
                                // Hailstone double-click: recenter viewport on clicked point
                                if (ViewModel.CurrentHailstoneResult != null)
                                {
                                    // Get current viewport
                                    double viewMinX = ViewModel.HailstoneViewportMinX ?? ViewModel.CurrentHailstoneResult.MinX;
                                    double viewMaxX = ViewModel.HailstoneViewportMaxX ?? ViewModel.CurrentHailstoneResult.MaxX;
                                    double viewMinY = ViewModel.HailstoneViewportMinY ?? ViewModel.CurrentHailstoneResult.MinY;
                                    double viewMaxY = ViewModel.HailstoneViewportMaxY ?? ViewModel.CurrentHailstoneResult.MaxY;

                                    if (!ViewModel.HasCustomHailstoneViewport)
                                    {
                                        double rangeX = viewMaxX - viewMinX;
                                        double rangeY = viewMaxY - viewMinY;
                                        if (rangeX == 0) rangeX = 2;
                                        if (rangeY == 0) rangeY = 2;
                                        double paddingX = rangeX * 0.15;
                                        double paddingY = rangeY * 0.15;
                                        viewMinX -= paddingX;
                                        viewMaxX += paddingX;
                                        viewMinY -= paddingY;
                                        viewMaxY += paddingY;
                                    }

                                    var viewRangeX = viewMaxX - viewMinX;
                                    var viewRangeY = viewMaxY - viewMinY;

                                    // Convert pixel to world coordinates
                                    var scaleX = viewRangeX / bitmapWidth;
                                    var scaleY = viewRangeY / bitmapHeight;

                                    var worldX = viewMinX + pixelX * scaleX;
                                    var worldY = viewMaxY - pixelY * scaleY;

                                    // Calculate new viewport centered on clicked point
                                    var newMinX = worldX - viewRangeX / 2.0;
                                    var newMaxX = worldX + viewRangeX / 2.0;
                                    var newMinY = worldY - viewRangeY / 2.0;
                                    var newMaxY = worldY + viewRangeY / 2.0;

                                    ViewModel.SetHailstoneViewport(newMinX, newMaxX, newMinY, newMaxY);
                                    ViewModel.StatusMessage = $"Recentered on ({worldX:F1}, {worldY:F1})";
                                    System.Diagnostics.Debug.WriteLine($"[PointerReleased] Hailstone recentered on ({worldX:F1}, {worldY:F1})");

                                    if (ViewModel.RenderCommand.CanExecute(null))
                                    {
                                        ViewModel.RenderCommand.Execute(null);
                                    }
                                }
                            }
                            else
                            {
                                // Standard fractal double-click: recenter on clicked point
                                var fractalWidth = 3.0 / ViewModel.Zoom;
                                var fractalHeight = fractalWidth * ((double)bitmapHeight / bitmapWidth);

                                // Calculate offset from center pixel
                                var scaleX = fractalWidth / bitmapWidth;
                                var scaleY = fractalHeight / bitmapHeight;

                                var offsetX = (pixelX - bitmapWidth / 2.0) * scaleX;
                                var offsetY = -(pixelY - bitmapHeight / 2.0) * scaleY;

                                System.Diagnostics.Debug.WriteLine($"[PointerReleased] Recenter calculation:");
                                System.Diagnostics.Debug.WriteLine($"  Click pixel: ({pixelX:F2}, {pixelY:F2})");
                                System.Diagnostics.Debug.WriteLine($"  Current center: ({ViewModel.CenterX:F8}, {ViewModel.CenterY:F8})");
                                System.Diagnostics.Debug.WriteLine($"  Offset: ({offsetX:F8}, {offsetY:F8})");

                                // Update center
                                ViewModel.CenterX += offsetX;
                                ViewModel.CenterY += offsetY;

                                System.Diagnostics.Debug.WriteLine($"  New center: ({ViewModel.CenterX:F8}, {ViewModel.CenterY:F8})");

                                // Reset locked center so new center becomes the lock point
                                ViewModel.ResetTrackedCenterPoint();

                                ViewModel.StatusMessage = $"Recentered on ({ViewModel.CenterX:F8}, {ViewModel.CenterY:F8})";

                                if (ViewModel.RenderCommand.CanExecute(null))
                                {
                                    ViewModel.RenderCommand.Execute(null);
                                }
                            }
                        }
                        else
                        {
                            ViewModel.StatusMessage = "Click inside the fractal to recenter";
                            System.Diagnostics.Debug.WriteLine("[PointerReleased] Click outside image bounds");
                        }
                    }
                }

                e.Handled = true;
                return;
            }

            if (_isDragging)
            {
                System.Diagnostics.Debug.WriteLine($"[PointerReleased] Processing drag release - _isPanning={_isPanning}, SelectionRect: {SelectionRectangle.Width}x{SelectionRectangle.Height}");
                _isDragging = false;
                var grid = sender as Grid;
                grid?.ReleasePointerCapture(e.Pointer);

                if (_isPanning)
                {
                    System.Diagnostics.Debug.WriteLine("[PointerReleased] Pan complete - triggering render");
                    // Pan complete - auto-render the new view
                    if (ViewModel.RenderCommand.CanExecute(null))
                    {
                        ViewModel.RenderCommand.Execute(null);
                    }
                }
                else
                {
                    // Hide selection rectangle
                    SelectionRectangle.Visibility = Visibility.Collapsed;

                    // Only zoom if rectangle is significant size (and not already handled by double-click)
                    if (SelectionRectangle.Width > 10 && SelectionRectangle.Height > 10)
                    {
                        System.Diagnostics.Debug.WriteLine("[PointerReleased] Rectangle large enough - calling ZoomToRectangle()");
                        ZoomToRectangle();
                    }
                    else if (SelectionRectangle.Width > 0 || SelectionRectangle.Height > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[PointerReleased] Rectangle too small - showing error message");
                        // Only show "too small" message if there was actually a drag attempt
                        // (Double-click already resets to 0x0, so this won't trigger)
                        ViewModel.StatusMessage = "Rectangle too small - no zoom applied";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[PointerReleased] Rectangle is 0x0 - no action");
                    }
                }

                e.Handled = true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[PointerReleased] Not dragging - no action");
            }
        }

        private void FractalImage_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.FractalImage == null)
                return;

            var delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;

            if (ViewModel.IsHailstoneMode)
            {
                // Hailstone mouse wheel zoom: manipulate viewport bounds
                if (ViewModel.CurrentHailstoneResult != null)
                {
                    // Get current viewport (or use sequence bounds if no custom viewport)
                    double viewMinX = ViewModel.HailstoneViewportMinX ?? ViewModel.CurrentHailstoneResult.MinX;
                    double viewMaxX = ViewModel.HailstoneViewportMaxX ?? ViewModel.CurrentHailstoneResult.MaxX;
                    double viewMinY = ViewModel.HailstoneViewportMinY ?? ViewModel.CurrentHailstoneResult.MinY;
                    double viewMaxY = ViewModel.HailstoneViewportMaxY ?? ViewModel.CurrentHailstoneResult.MaxY;

                    // Add 15% padding if using auto-bounds
                    if (!ViewModel.HasCustomHailstoneViewport)
                    {
                        double rangeX = viewMaxX - viewMinX;
                        double rangeY = viewMaxY - viewMinY;
                        if (rangeX == 0) rangeX = 2;
                        if (rangeY == 0) rangeY = 2;
                        double paddingX = rangeX * 0.15;
                        double paddingY = rangeY * 0.15;
                        viewMinX -= paddingX;
                        viewMaxX += paddingX;
                        viewMinY -= paddingY;
                        viewMaxY += paddingY;
                    }

                    // Calculate center and size
                    double centerX = (viewMinX + viewMaxX) / 2.0;
                    double centerY = (viewMinY + viewMaxY) / 2.0;
                    double halfWidth = (viewMaxX - viewMinX) / 2.0;
                    double halfHeight = (viewMaxY - viewMinY) / 2.0;

                    // Zoom factor: 2x for zoom in, 0.5x for zoom out
                    double zoomFactor = delta > 0 ? 0.5 : 2.0;

                    // Apply zoom around center
                    double newHalfWidth = halfWidth * zoomFactor;
                    double newHalfHeight = halfHeight * zoomFactor;

                    double newMinX = centerX - newHalfWidth;
                    double newMaxX = centerX + newHalfWidth;
                    double newMinY = centerY - newHalfHeight;
                    double newMaxY = centerY + newHalfHeight;

                    ViewModel.SetHailstoneViewport(newMinX, newMaxX, newMinY, newMaxY);
                    ViewModel.StatusMessage = delta > 0 
                        ? $"Zooming in Hailstone viewport to [{newMinX:F1}, {newMaxX:F1}] × [{newMinY:F1}, {newMaxY:F1}]..."
                        : $"Zooming out Hailstone viewport to [{newMinX:F1}, {newMaxX:F1}] × [{newMinY:F1}, {newMaxY:F1}]...";

                    // Hide labels temporarily during zoom to prevent mismatch with bitmap
                    HailstoneLabelsCanvas.Opacity = 0.3;

                    // Debounce: wait 500ms after last scroll before auto-rendering
                    _zoomTimer?.Dispose();
                    _zoomTimer = new System.Threading.Timer(_ =>
                    {
                        this.DispatcherQueue.TryEnqueue(async () =>
                        {
                            if (ViewModel.RenderCommand.CanExecute(null))
                            {
                                await ViewModel.RenderCommand.ExecuteAsync(null);
                                // Restore labels after render completes
                                HailstoneLabelsCanvas.Opacity = 1.0;
                            }
                        });
                    }, null, 500, System.Threading.Timeout.Infinite);
                }
                else
                {
                    ViewModel.StatusMessage = "No Hailstone sequence loaded - render first before zooming";
                }
            }
            else
            {
                // Standard fractal mouse wheel zoom with center correction
                if (delta > 0)
                {
                    ViewModel.ApplyZoomCorrection(2.0);
                    ViewModel.StatusMessage = $"Zooming in to {ViewModel.Zoom:F2}x...";
                }
                else if (delta < 0)
                {
                    ViewModel.ApplyZoomCorrection(0.5);
                    ViewModel.StatusMessage = $"Zooming out to {ViewModel.Zoom:F2}x...";
                }

                // Debounce: wait 300ms after last scroll before auto-rendering
                _zoomTimer?.Dispose();
                _zoomTimer = new System.Threading.Timer(_ =>
                {
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        if (ViewModel.RenderCommand.CanExecute(null))
                        {
                            ViewModel.RenderCommand.Execute(null);
                        }
                    });
                }, null, 300, System.Threading.Timeout.Infinite);
            }

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

                if (ViewModel.IsHailstoneMode)
                {
                    // Hailstone box zoom: manipulate viewport bounds
                    if (ViewModel.CurrentHailstoneResult != null)
                    {
                        // Get current viewport (or use sequence bounds if no custom viewport)
                        double viewMinX = ViewModel.HailstoneViewportMinX ?? ViewModel.CurrentHailstoneResult.MinX;
                        double viewMaxX = ViewModel.HailstoneViewportMaxX ?? ViewModel.CurrentHailstoneResult.MaxX;
                        double viewMinY = ViewModel.HailstoneViewportMinY ?? ViewModel.CurrentHailstoneResult.MinY;
                        double viewMaxY = ViewModel.HailstoneViewportMaxY ?? ViewModel.CurrentHailstoneResult.MaxY;

                        // Add 15% padding if using auto-bounds
                        if (!ViewModel.HasCustomHailstoneViewport)
                        {
                            double rangeX = viewMaxX - viewMinX;
                            double rangeY = viewMaxY - viewMinY;
                            if (rangeX == 0) rangeX = 2;
                            if (rangeY == 0) rangeY = 2;
                            double paddingX = rangeX * 0.15;
                            double paddingY = rangeY * 0.15;
                            viewMinX -= paddingX;
                            viewMaxX += paddingX;
                            viewMinY -= paddingY;
                            viewMaxY += paddingY;
                        }

                        var viewRangeX = viewMaxX - viewMinX;
                        var viewRangeY = viewMaxY - viewMinY;

                        // Scale factors (world units per bitmap pixel) for CURRENT view
                        var scaleX = viewRangeX / bitmapWidth;
                        var scaleY = viewRangeY / bitmapHeight;

                        // Convert bitmap pixel position to world coordinates
                        // Pixel (0,0) maps to top-left corner: (viewMinX, viewMaxY)
                        var worldLeft = viewMinX + (imageRelativeLeft / displayScale) * scaleX;
                        var worldRight = viewMinX + ((imageRelativeLeft + rectWidth) / displayScale) * scaleX;
                        var worldTop = viewMaxY - (imageRelativeTop / displayScale) * scaleY;
                        var worldBottom = viewMaxY - ((imageRelativeTop + rectHeight) / displayScale) * scaleY;

                        // Set new viewport to the selected rectangle
                        ViewModel.SetHailstoneViewport(worldLeft, worldRight, worldBottom, worldTop);
                        ViewModel.StatusMessage = $"Zooming to Hailstone region [{worldLeft:F1}, {worldRight:F1}] × [{worldBottom:F1}, {worldTop:F1}]...";

                        // Auto-render the new view
                        if (ViewModel.RenderCommand.CanExecute(null))
                        {
                            ViewModel.RenderCommand.Execute(null);
                        }
                    }
                    else
                    {
                        ViewModel.StatusMessage = "No Hailstone sequence loaded - render first before zooming";
                    }
                }
                else
                {
                    // Standard fractal box zoom
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

                    // Reset tracked center point since user explicitly selected a new region
                    ViewModel.ResetTrackedCenterPoint();

                    ViewModel.StatusMessage = $"Zoom: {newZoom:F2}x @ ({newCenterX:F8}, {newCenterY:F8}) | View: {newFractalWidth:F8}×{newFractalHeight:F8}";

                    // Auto-render the new view
                    if (ViewModel.RenderCommand.CanExecute(null))
                    {
                        ViewModel.RenderCommand.Execute(null);
                    }
                }
            }
        }
    }
}
