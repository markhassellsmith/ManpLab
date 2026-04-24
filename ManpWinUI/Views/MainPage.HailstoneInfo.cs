using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Linq;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Hailstone info overlay management.
    /// Renders info text directly on the Canvas like point labels.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Updates the Hailstone info text overlay in the upper-left corner.
        /// Matches NumericalVisualizations format: Arial 14pt Bold, Yellow text (Magenta for cycles),
        /// semi-transparent black background.
        /// </summary>
        public void UpdateHailstoneInfo()
        {
            // Info is now rendered on the same canvas as labels
            // This method will add info text to HailstoneLabelsCanvas
            // Called after UpdateHailstoneLabels() completes

            // Only show info if we have a current result and we're in Hailstone mode
            if (!ViewModel.IsHailstoneMode || ViewModel.CurrentHailstoneResult == null)
            {
                return;
            }

            var result = ViewModel.CurrentHailstoneResult;

            // Get the actual display size and offset from the Viewbox
            var imageSize = GetDisplayedImageSize();
            if (imageSize.Width == 0 || imageSize.Height == 0)
            {
                return;
            }

            // Calculate the offset caused by Viewbox centering (same as point labels)
            double viewboxOffsetX = (HailstoneLabelsCanvas.ActualWidth - imageSize.Width) / 2.0;
            double viewboxOffsetY = (HailstoneLabelsCanvas.ActualHeight - imageSize.Height) / 2.0;

            // Fixed position at upper-left corner, absolute top-left
            double textX = viewboxOffsetX;
            double textY = viewboxOffsetY;

            // Build info text (matching NumericalVisualizations format exactly)
            string infoText = "Hailstone Sequence (N,X,Y)\n";

            if (result.Sequence.Count > 0)
            {
                var startPoint = result.Sequence[0];
                infoText += $"Starting point: (0, {startPoint.X}, {startPoint.Y})\n";
            }

            infoText += $"Total points: {result.Sequence.Count}";

            // Determine text color based on cycle detection
            var textColor = Colors.Yellow; // Default yellow

            if (result.HasCycle && result.CycleStartIndex >= 0 && result.CycleStartIndex < result.Sequence.Count)
            {
                var cycleStartPoint = result.Sequence[result.CycleStartIndex];
                var lastPoint = result.Sequence[result.Sequence.Count - 1];

                // The "Cycle Detected" point is the next point that would have been calculated
                // (which is a duplicate of cycleStartPoint). Calculate it from the last point.
                int nextStep = lastPoint.Step + 1;
                int nextX = CalculateNextX(lastPoint.X, lastPoint.Y);
                int nextY = CalculateNextY(lastPoint.X, lastPoint.Y);

                infoText += $"\nCycle Detected: Point ({nextStep}, {nextX}, {nextY})";
                infoText += $"\nDuplicate of: ({cycleStartPoint.Step}, {cycleStartPoint.X}, {cycleStartPoint.Y})";
                infoText += $"\nCycle length: {result.CycleLength}";

                textColor = Colors.Magenta; // Use magenta for cycle info
            }

            // Create semi-transparent black background (matching NumericalVisualizations: ARGB 220,0,0,0)
            var background = new Rectangle
            {
                Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(220, 0, 0, 0))
            };

            // Create text block with Arial 3pt Bold for very subtle info display
            var textBlock = new TextBlock
            {
                Text = infoText,
                FontSize = 3,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                FontFamily = new FontFamily("Arial"),
                Foreground = new SolidColorBrush(textColor),
                TextWrapping = TextWrapping.NoWrap
            };

            // Measure text size
            textBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = textBlock.DesiredSize.Width;
            double textHeight = textBlock.DesiredSize.Height;

            // Position background with 5px padding (matching NumericalVisualizations)
            Canvas.SetLeft(background, textX - 5);
            Canvas.SetTop(background, textY - 5);
            background.Width = textWidth + 10;
            background.Height = textHeight + 10;

            // Position text
            Canvas.SetLeft(textBlock, textX);
            Canvas.SetTop(textBlock, textY);

            // Add to canvas (background first, then text on top)
            HailstoneLabelsCanvas.Children.Add(background);
            HailstoneLabelsCanvas.Children.Add(textBlock);
        }

        /// <summary>
        /// Calculates the next X coordinate for Hailstone sequence (same logic as HailstoneService).
        /// </summary>
        private static int CalculateNextX(int x, int y)
        {
            bool xEven = (x % 2 == 0);
            bool yEven = (y % 2 == 0);

            return (xEven, yEven) switch
            {
                (true, true) => x / 2,           // Both even
                (true, false) => x / 2 + 1,      // X even, Y odd
                (false, true) => 3 * x - 1,      // X odd, Y even
                (false, false) => 3 * x + 1      // Both odd
            };
        }

        /// <summary>
        /// Calculates the next Y coordinate for Hailstone sequence (same logic as HailstoneService).
        /// </summary>
        private static int CalculateNextY(int x, int y)
        {
            bool xEven = (x % 2 == 0);
            bool yEven = (y % 2 == 0);

            return (xEven, yEven) switch
            {
                (true, true) => y / 2,           // Both even
                (true, false) => 3 * y - 1,      // X even, Y odd
                (false, true) => y / 2 - 1,      // X odd, Y even
                (false, false) => 3 * y - 3      // Both odd
            };
        }
    }
}
