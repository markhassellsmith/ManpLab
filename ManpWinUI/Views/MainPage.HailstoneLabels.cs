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

            // Only render labels if we have a current result and labels are enabled
            if (!ViewModel.ShowHailstoneLabels || ViewModel.CurrentHailstoneResult == null)
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
            foreach (var (pointX, pointY, index) in displayPoints)
            {
                var point = result.Sequence[index];

                // Create label with format (N, x, y) where N is the step number (0-based)
                var label = new TextBlock
                {
                    Text = $"({point.Step}, {point.X}, {point.Y})",
                    FontSize = 2.5,  // Very small, subtle labels
                    Foreground = point.IsInCycle
                        ? new SolidColorBrush(Colors.Magenta)
                        : new SolidColorBrush(Colors.Cyan),
                    FontWeight = Microsoft.UI.Text.FontWeights.Light,  // Thinner weight
                    Opacity = 0.85  // Slightly transparent for subtlety
                };

                // Find best label position to avoid line segments
                var (labelX, labelY) = FindBestLabelPosition(
                    pointX, pointY, index, displayPoints, result.Sequence);

                Canvas.SetLeft(label, labelX);
                Canvas.SetTop(label, labelY);

                HailstoneLabelsCanvas.Children.Add(label);
            }

            // After rendering point labels, add info text and axis labels to the same canvas
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
            int range = Math.Max(rangeX, rangeY);

            int tickSpacing = CalculateTickSpacing(range);

            // Calculate label spacing - show labels less frequently
            int labelSpacing = tickSpacing;
            if (range > 100) labelSpacing = tickSpacing * 5; // Every 5th tick
            else if (range > 50) labelSpacing = tickSpacing * 2; // Every 2nd tick

            // World-to-screen conversion helper
            (double, double) WorldToScreen(int worldX, int worldY)
            {
                double screenX = worldX * scaleX + offsetX;
                double screenY = worldY * scaleY + offsetY;
                return (viewboxOffsetX + screenX * displayScaleX, viewboxOffsetY + screenY * displayScaleY);
            }

            // Draw X-axis labels (at y=0)
            var (_, axisY) = WorldToScreen(0, 0);
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

                    // Position directly below the tick mark, centered
                    Canvas.SetLeft(label, screenX - 5);
                    Canvas.SetTop(label, axisY + 6);

                    HailstoneLabelsCanvas.Children.Add(label);
                }
            }

            // Draw Y-axis labels (at x=0)
            var (axisX, _) = WorldToScreen(0, 0);
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

                    // Position directly to the right of the tick mark
                    Canvas.SetLeft(label, axisX + 6);
                    Canvas.SetTop(label, screenY - 4);

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
        /// Finds the best position for a label to minimize overlap with line segments.
        /// Tests 8 positions around the point (cardinal and diagonal directions).
        /// </summary>
        private (double x, double y) FindBestLabelPosition(
            double pointX, double pointY, int pointIndex,
            List<(double x, double y, int index)> displayPoints,
            List<ManpWinUI.Models.HailstonePoint> sequence)
        {
            // Define candidate positions: right, bottom-right, bottom, bottom-left, left, top-left, top, top-right
            var offsets = new[]
            {
                (dx: 2.0, dy: 0.0),      // Right (preferred - most readable)
                (dx: 2.0, dy: 2.0),      // Bottom-right
                (dx: 0.0, dy: 2.0),      // Bottom
                (dx: -12.0, dy: 2.0),    // Bottom-left (offset left by approx label width)
                (dx: -12.0, dy: 0.0),    // Left
                (dx: -12.0, dy: -4.0),   // Top-left
                (dx: 0.0, dy: -4.0),     // Top
                (dx: 2.0, dy: -4.0)      // Top-right
            };

            double bestScore = double.MaxValue;
            var bestPosition = (x: pointX + offsets[0].dx, y: pointY + offsets[0].dy);

            foreach (var (dx, dy) in offsets)
            {
                double labelX = pointX + dx;
                double labelY = pointY + dy;

                // Calculate score based on potential overlaps
                double score = CalculateLabelScore(labelX, labelY, pointX, pointY, 
                    pointIndex, displayPoints);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestPosition = (labelX, labelY);
                }
            }

            return bestPosition;
        }

        /// <summary>
        /// Calculates a score for a label position based on proximity to line segments.
        /// Lower score is better (less overlap risk).
        /// </summary>
        private double CalculateLabelScore(
            double labelX, double labelY, double pointX, double pointY,
            int pointIndex, List<(double x, double y, int index)> displayPoints)
        {
            double score = 0.0;

            // Approximate label dimensions (2.5px font, roughly 8-12 characters)
            double labelWidth = 18.0;  // Smaller estimate for 2.5px font
            double labelHeight = 4.0;

            // Label bounding box
            double labelLeft = labelX;
            double labelRight = labelX + labelWidth;
            double labelTop = labelY;
            double labelBottom = labelY + labelHeight;

            // Check proximity to adjacent line segments (previous and next connections)
            for (int offset = -1; offset <= 1; offset += 2)  // Check prev and next
            {
                int adjacentIndex = pointIndex + offset;
                if (adjacentIndex >= 0 && adjacentIndex < displayPoints.Count)
                {
                    var adjacentPoint = displayPoints.FirstOrDefault(p => p.index == adjacentIndex);
                    if (adjacentPoint != default)
                    {
                        // Calculate distance from label center to line segment
                        double labelCenterX = labelX + labelWidth / 2;
                        double labelCenterY = labelY + labelHeight / 2;

                        double distance = DistanceToLineSegment(
                            labelCenterX, labelCenterY,
                            pointX, pointY,
                            adjacentPoint.x, adjacentPoint.y);

                        // Penalize positions closer to line segments
                        // Use inverse square to heavily penalize very close positions
                        if (distance < 20.0)  // Only penalize if within 20 pixels
                        {
                            score += 100.0 / (distance + 1.0);
                        }
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates the perpendicular distance from a point to a line segment.
        /// </summary>
        private double DistanceToLineSegment(
            double px, double py,
            double x1, double y1,
            double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double lengthSquared = dx * dx + dy * dy;

            if (lengthSquared == 0.0)
            {
                // Line segment is a point
                return Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
            }

            // Calculate projection parameter t
            double t = Math.Max(0.0, Math.Min(1.0, 
                ((px - x1) * dx + (py - y1) * dy) / lengthSquared));

            // Find closest point on segment
            double closestX = x1 + t * dx;
            double closestY = y1 + t * dy;

            // Return distance to closest point
            return Math.Sqrt((px - closestX) * (px - closestX) + 
                           (py - closestY) * (py - closestY));
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

            // The Viewbox uses Uniform stretch, so we need to calculate the actual displayed size
            double imageAspectRatio = ViewModel.ImageWidth / (double)ViewModel.ImageHeight;
            double viewboxAspectRatio = FractalViewbox.ActualWidth / FractalViewbox.ActualHeight;

            double displayWidth, displayHeight;

            if (imageAspectRatio > viewboxAspectRatio)
            {
                // Image is wider relative to viewbox - width is constrained
                displayWidth = FractalViewbox.ActualWidth;
                displayHeight = displayWidth / imageAspectRatio;
            }
            else
            {
                // Image is taller relative to viewbox - height is constrained
                displayHeight = FractalViewbox.ActualHeight;
                displayWidth = displayHeight * imageAspectRatio;
            }

            return new Windows.Foundation.Size(displayWidth, displayHeight);
        }
    }
}
