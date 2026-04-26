using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Hailstone label overlay management.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Updates the Hailstone labels overlay with point labels.
        /// Called when the Hailstone sequence is rendered or when ShowHailstoneLabels changes.
        /// Uses smart placement to minimize overlap with line segments.
        /// </summary>
        public void UpdateHailstoneLabels()
        {
            HailstoneLabelsCanvas.Children.Clear();

            // Only render point labels if we have a current result and labels are enabled
            if (ViewModel.ShowHailstoneLabels && ViewModel.CurrentHailstoneResult != null)
            {
                var result = ViewModel.CurrentHailstoneResult;

                // Get transform parameters - use current viewport if set, otherwise use stored values
                double scaleX, scaleY, offsetX, offsetY;

                if (ViewModel.HasCustomHailstoneViewport)
                {
                    // Recalculate transform for current custom viewport
                    (scaleX, scaleY, offsetX, offsetY) = CalculateHailstoneTransform(
                        ViewModel.HailstoneViewportMinX!.Value,
                        ViewModel.HailstoneViewportMaxX!.Value,
                        ViewModel.HailstoneViewportMinY!.Value,
                        ViewModel.HailstoneViewportMaxY!.Value);
                }
                else
                {
                    // Use stored transform from last render
                    scaleX = ViewModel.HailstoneScaleX;
                    scaleY = ViewModel.HailstoneScaleY;
                    offsetX = ViewModel.HailstoneOffsetX;
                    offsetY = ViewModel.HailstoneOffsetY;
                }

                // Get the actual display size and offset from the Viewbox
                var imageSize = GetDisplayedImageSize();
                if (imageSize.Width > 0 && imageSize.Height > 0)
                {
                    // Calculate the offset caused by Viewbox centering
                    double viewboxOffsetX = (HailstoneLabelsCanvas.ActualWidth - imageSize.Width) / 2.0;
                    double viewboxOffsetY = (HailstoneLabelsCanvas.ActualHeight - imageSize.Height) / 2.0;

                    // Calculate scale factors for label positioning
                    double displayScaleX = imageSize.Width / ViewModel.ImageWidth;
                    double displayScaleY = imageSize.Height / ViewModel.ImageHeight;

                    // Convert all points to display coordinates for spatial analysis
                    var displayPoints = new List<(double x, double y, int index)>();
                    for (int i = 0; i < result.Sequence.Count; i++)
                    {
                        var point = result.Sequence[i];

                        // Transform world coordinates to bitmap coordinates
                        int screenX = (int)(point.X * scaleX + offsetX);
                        int screenY = (int)(point.Y * scaleY + offsetY);

                        if (screenX >= 0 && screenX < ViewModel.ImageWidth &&
                            screenY >= 0 && screenY < ViewModel.ImageHeight)
                        {
                            // Scale to display size and add viewbox offset
                            double displayX = screenX * displayScaleX + viewboxOffsetX;
                            double displayY = screenY * displayScaleY + viewboxOffsetY;
                            displayPoints.Add((displayX, displayY, i));
                        }
                    }

                    // Label each point with smart placement
                    // Track placed label bounds to avoid overlaps
                    var placedLabels = new List<(double left, double top, double right, double bottom)>();

                    foreach (var (pointX, pointY, index) in displayPoints)
                    {
                        var point = result.Sequence[index];

                        // Create label with format (N, x, y) where N is the step number (0-based)
                        var label = new TextBlock
                        {
                            Text = $"({point.Step}, {point.X}, {point.Y})",
                            FontSize = 0.5,  // Extremely small labels (half the previous size)
                            Foreground = point.IsInCycle
                                ? new SolidColorBrush(Colors.Magenta)
                                : new SolidColorBrush(Colors.Cyan),
                            FontWeight = Microsoft.UI.Text.FontWeights.ExtraLight,  // Thinnest weight
                            Opacity = 0.7  // More transparent for even more subtlety
                        };

                        // Measure label size for accurate overlap detection
                        label.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                        double labelWidth = label.DesiredSize.Width;
                        double labelHeight = label.DesiredSize.Height;

                        // Find best label position to avoid line segments and other labels
                        var (labelX, labelY) = FindBestLabelPosition(
                            pointX, pointY, index, displayPoints, result.Sequence, 
                            placedLabels, labelWidth, labelHeight);

                        Canvas.SetLeft(label, labelX);
                        Canvas.SetTop(label, labelY);

                        // Track this label's bounds
                        placedLabels.Add((labelX, labelY, labelX + labelWidth, labelY + labelHeight));

                        HailstoneLabelsCanvas.Children.Add(label);
                    }
                } // End of if (imageSize.Width > 0)
            } // End of if (ShowHailstoneLabels)

            // Always render info text and axis labels (independently of point labels)
            UpdateHailstoneInfo();
            UpdateAxisLabels();
        }

        /// <summary>
        /// Adds numeric labels to axis tick marks for coordinate reference.
        /// Shows very small gray labels at selected tick mark positions.
        /// </summary>
        private void UpdateAxisLabels()
        {
            // Only render axis labels if we have a current result and axes are enabled
            if (!ViewModel.ShowHailstoneAxes || ViewModel.CurrentHailstoneResult == null)
            {
                return;
            }

            var result = ViewModel.CurrentHailstoneResult;
            var scaleX = ViewModel.HailstoneScaleX;
            var scaleY = ViewModel.HailstoneScaleY;
            var offsetX = ViewModel.HailstoneOffsetX;
            var offsetY = ViewModel.HailstoneOffsetY;

            // Get the actual display size and offset from the Viewbox
            var imageSize = GetDisplayedImageSize();
            if (imageSize.Width == 0 || imageSize.Height == 0)
            {
                return;
            }

            // Calculate the offset caused by Viewbox centering
            double viewboxOffsetX = (HailstoneLabelsCanvas.ActualWidth - imageSize.Width) / 2.0;
            double viewboxOffsetY = (HailstoneLabelsCanvas.ActualHeight - imageSize.Height) / 2.0;

            // Calculate scale factors for label positioning
            double displayScaleX = imageSize.Width / ViewModel.ImageWidth;
            double displayScaleY = imageSize.Height / ViewModel.ImageHeight;

            // Determine coordinate bounds from sequence
            int minX = result.Sequence.Min(p => p.X);
            int maxX = result.Sequence.Max(p => p.X);
            int minY = result.Sequence.Min(p => p.Y);
            int maxY = result.Sequence.Max(p => p.Y);

            // Calculate appropriate tick spacing
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;

            // Handle single-point case
            if (rangeX == 0) rangeX = 2;
            if (rangeY == 0) rangeY = 2;

            // Apply 15% padding to match the viewport used for tick mark rendering
            const double PaddingPercent = 0.15;
            double paddingX = rangeX * PaddingPercent;
            double paddingY = rangeY * PaddingPercent;

            double viewMinX = minX - paddingX;
            double viewMaxX = maxX + paddingX;
            double viewMinY = minY - paddingY;
            double viewMaxY = maxY + paddingY;

            int range = Math.Max(rangeX, rangeY);

            int tickSpacing = CalculateTickSpacing(range);

            // Place labels at EVERY tick mark for clarity
            int labelSpacing = tickSpacing;

            // World-to-screen conversion helper
            // Must match RenderService's WorldToScreen which truncates to int
            (double, double) WorldToScreen(int worldX, int worldY)
            {
                // Calculate bitmap coordinates and truncate to int (matching RenderService)
                int bitmapX = (int)(worldX * scaleX + offsetX);
                int bitmapY = (int)(worldY * scaleY + offsetY);

                // Then convert to display coordinates
                return (viewboxOffsetX + bitmapX * displayScaleX, viewboxOffsetY + bitmapY * displayScaleY);
            }

            // Draw X-axis labels (at y=0)
            var (_, axisY) = WorldToScreen(0, 0);

            // Calculate label positions to match tick marks (which use raw bounds, not padded viewport)
            // This matches the logic in HailstoneRenderService.DrawAxisTickMarks
            int startX = (minX / labelSpacing) * labelSpacing;
            if (startX > minX) startX -= labelSpacing;
            int endX = (maxX / labelSpacing) * labelSpacing;
            if (endX < maxX) endX += labelSpacing;

            for (int x = startX; x <= endX; x += labelSpacing)
            {
                if (x == 0) continue; // Skip origin

                var (screenX, _) = WorldToScreen(x, 0);

                // Check if on screen
                if (screenX >= viewboxOffsetX && screenX < viewboxOffsetX + imageSize.Width)
                {
                    var label = new TextBlock
                    {
                        Text = x.ToString(),
                        FontSize = 3,
                        Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(180, 120, 120, 120)),
                        FontWeight = Microsoft.UI.Text.FontWeights.Light
                    };

                    // Measure text to center it horizontally on the tick mark
                    label.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                    double labelWidth = label.DesiredSize.Width;

                    // Position centered horizontally on tick mark, directly below with 1px gap
                    double labelLeft = screenX - labelWidth / 2;
                    Canvas.SetLeft(label, labelLeft);
                    Canvas.SetTop(label, axisY + 1);

                    HailstoneLabelsCanvas.Children.Add(label);
                }
            }

            // Draw Y-axis labels (at x=0)
            var (axisX, _) = WorldToScreen(0, 0);

            // Calculate label positions to match tick marks (which use raw bounds, not padded viewport)
            // This matches the logic in HailstoneRenderService.DrawAxisTickMarks
            int startY = (minY / labelSpacing) * labelSpacing;
            if (startY > minY) startY -= labelSpacing;
            int endY = (maxY / labelSpacing) * labelSpacing;
            if (endY < maxY) endY += labelSpacing;

            for (int y = startY; y <= endY; y += labelSpacing)
            {
                if (y == 0) continue; // Skip origin

                var (_, screenY) = WorldToScreen(0, y);

                // Check if on screen
                if (screenY >= viewboxOffsetY && screenY < viewboxOffsetY + imageSize.Height)
                {
                    var label = new TextBlock
                    {
                        Text = y.ToString(),
                        FontSize = 3,
                        Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(180, 120, 120, 120)),
                        FontWeight = Microsoft.UI.Text.FontWeights.Light
                    };

                    // Measure text to position it to the left of the tick mark
                    label.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                    double labelWidth = label.DesiredSize.Width;
                    double labelHeight = label.DesiredSize.Height;

                    // Position directly to the LEFT of the tick mark, centered vertically
                    Canvas.SetLeft(label, axisX - labelWidth - 2);
                    Canvas.SetTop(label, screenY - labelHeight / 2);

                    HailstoneLabelsCanvas.Children.Add(label);
                }
            }
        }

        /// <summary>
        /// Calculates appropriate tick spacing for axis labels based on coordinate range.
        /// </summary>
        private int CalculateTickSpacing(int range)
        {
            if (range <= 20) return 1;
            if (range <= 50) return 5;
            if (range <= 100) return 10;
            if (range <= 200) return 20;
            if (range <= 500) return 50;
            if (range <= 1000) return 100;
            return 200;
        }

        /// <summary>
        /// Finds the best position for a point label to avoid overlaps with other labels.
        /// Tries multiple candidate positions around the point and selects the one with minimal overlap.
        /// </summary>
        private (double x, double y) FindBestLabelPosition(
            double pointX, double pointY, int pointIndex,
            List<(double x, double y, int index)> displayPoints,
            List<ManpWinUI.Models.HailstonePoint> sequence,
            List<(double left, double top, double right, double bottom)> placedLabels,
            double labelWidth, double labelHeight)
        {
            // Define candidate positions around the point (8 directions + center-right)
            var candidates = new List<(double x, double y, int priority)>
            {
                // Priority 0: Preferred position (right and slightly above)
                (pointX + 2, pointY - labelHeight / 2, 0),

                // Priority 1: Alternative horizontal positions
                (pointX - labelWidth - 2, pointY - labelHeight / 2, 1), // Left

                // Priority 2: Vertical positions
                (pointX - labelWidth / 2, pointY - labelHeight - 2, 2), // Above
                (pointX - labelWidth / 2, pointY + 2, 2), // Below

                // Priority 3: Diagonal positions
                (pointX + 2, pointY - labelHeight - 2, 3), // Top-right
                (pointX + 2, pointY + 2, 3), // Bottom-right
                (pointX - labelWidth - 2, pointY - labelHeight - 2, 3), // Top-left
                (pointX - labelWidth - 2, pointY + 2, 3), // Bottom-left
            };

            // Find the candidate with the least overlap
            double bestScore = double.MaxValue;
            (double x, double y) bestPosition = (candidates[0].x, candidates[0].y);

            foreach (var (candX, candY, priority) in candidates)
            {
                double score = CalculateOverlapScore(candX, candY, labelWidth, labelHeight, 
                    placedLabels, priority);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestPosition = (candX, candY);
                }

                // If we found a perfect spot (no overlap), use it
                if (score == 0)
                    break;
            }

            return bestPosition;
        }

        /// <summary>
        /// Calculates an overlap score for a candidate label position.
        /// Lower score is better (less overlap).
        /// </summary>
        private double CalculateOverlapScore(
            double labelX, double labelY, double labelWidth, double labelHeight,
            List<(double left, double top, double right, double bottom)> placedLabels,
            int priority)
        {
            double score = priority * 10; // Base score based on position priority

            double labelRight = labelX + labelWidth;
            double labelBottom = labelY + labelHeight;

            // Add penalty for each overlapping label
            foreach (var placed in placedLabels)
            {
                // Check for overlap
                bool overlaps = !(labelRight < placed.left || 
                                 labelX > placed.right ||
                                 labelBottom < placed.top ||
                                 labelY > placed.bottom);

                if (overlaps)
                {
                    // Calculate overlap area
                    double overlapWidth = Math.Min(labelRight, placed.right) - Math.Max(labelX, placed.left);
                    double overlapHeight = Math.Min(labelBottom, placed.bottom) - Math.Max(labelY, placed.top);
                    double overlapArea = overlapWidth * overlapHeight;

                    // Heavy penalty for overlaps (overlap area normalized by label area)
                    score += (overlapArea / (labelWidth * labelHeight)) * 1000;
                }
                else
                {
                    // Small bonus for being close but not overlapping (encourages compact layout)
                    double centerX = labelX + labelWidth / 2;
                    double centerY = labelY + labelHeight / 2;
                    double placedCenterX = (placed.left + placed.right) / 2;
                    double placedCenterY = (placed.top + placed.bottom) / 2;
                    double distance = Math.Sqrt(
                        Math.Pow(centerX - placedCenterX, 2) + 
                        Math.Pow(centerY - placedCenterY, 2));

                    // Very small penalty for being far from other labels
                    if (distance > 50)
                        score += (distance - 50) * 0.01;
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates coordinate transformation parameters for the given viewport bounds.
        /// This mirrors the logic in HailstoneRenderService.CalculateTransform.
        /// </summary>
        private (double scaleX, double scaleY, double offsetX, double offsetY) CalculateHailstoneTransform(
            double viewMinX, double viewMaxX, double viewMinY, double viewMaxY)
        {
            var width = ViewModel.ImageWidth;
            var height = ViewModel.ImageHeight;

            // Calculate the viewport dimensions
            double viewRangeX = viewMaxX - viewMinX;
            double viewRangeY = viewMaxY - viewMinY;

            // Calculate scale (negative Y to flip for screen coordinates)
            double scaleX = width / viewRangeX;
            double scaleY = -height / viewRangeY; // Negative to flip Y axis

            // Calculate offset to map world coordinates to screen
            // For X: viewMinX should map to screen x=0
            // For Y: viewMaxY should map to screen y=0 (top), viewMinY to y=height (bottom)
            double offsetX = -viewMinX * scaleX;
            double offsetY = -viewMaxY * scaleY;  // Use viewMaxY, not viewMinY

            return (scaleX, scaleY, offsetX, offsetY);
        }

        /// <summary>
        /// Gets the actual displayed size of the fractal image considering the Viewbox scaling.
        /// </summary>
        private Windows.Foundation.Size GetDisplayedImageSize()
        {
            if (FractalImage.Source == null || FractalViewbox.ActualWidth == 0)
            {
                return new Windows.Foundation.Size(0, 0);
            }

            // The Viewbox uses Fill stretch (from XAML), which means the image fills the entire viewbox
            // without maintaining aspect ratio. Therefore, the displayed size equals the viewbox size
            // and there is NO centering offset (viewboxOffset should be 0,0)
            return new Windows.Foundation.Size(FractalViewbox.ActualWidth, FractalViewbox.ActualHeight);
        }
    }
}
