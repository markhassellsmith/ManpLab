using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using System.Runtime.Versioning;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Panel management (collapse/expand, resize).
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class MainPage
    {
        private bool _isBrowserSplitterDragging = false;
        private bool _isPropertiesSplitterDragging = false;
        private double _browserSplitterStartX = 0;
        private double _propertiesSplitterStartX = 0;
        private double _browserPanelStartWidth = 0;
        private double _propertiesPanelStartWidth = 0;

        // ═══════════════════════════════════════════════════════════════════════════════
        // LEFT PANEL SPLITTER (Browser/Bookmarks)
        // ═══════════════════════════════════════════════════════════════════════════════

        private void BrowserSplitter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Change cursor to resize cursor when hovering over splitter
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
        }

        private void BrowserSplitter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Restore default cursor when leaving splitter
            if (!_isBrowserSplitterDragging)
            {
                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            }
        }

        private void BrowserSplitter_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var border = sender as Microsoft.UI.Xaml.Controls.Border;
            if (border == null) return;

            _isBrowserSplitterDragging = true;
            _browserSplitterStartX = e.GetCurrentPoint(this).Position.X;
            _browserPanelStartWidth = ViewModel.BrowserPanelWidth;
            border.CapturePointer(e.Pointer);
            e.Handled = true;
        }

        private void BrowserSplitter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isBrowserSplitterDragging) return;

            var currentX = e.GetCurrentPoint(this).Position.X;
            var deltaX = currentX - _browserSplitterStartX;
            var newWidth = _browserPanelStartWidth + deltaX;

            // Clamp width to reasonable bounds (150px - 600px)
            newWidth = System.Math.Max(150, System.Math.Min(600, newWidth));

            ViewModel.BrowserPanelWidth = newWidth;
            e.Handled = true;
        }

        private void BrowserSplitter_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isBrowserSplitterDragging) return;

            var border = sender as Microsoft.UI.Xaml.Controls.Border;
            border?.ReleasePointerCapture(e.Pointer);
            _isBrowserSplitterDragging = false;

            // Restore cursor
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            e.Handled = true;
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // PROPERTIES PANEL SPLITTER
        // ═══════════════════════════════════════════════════════════════════════════════

        private void PropertiesSplitter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Change cursor to resize cursor when hovering over splitter
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
        }

        private void PropertiesSplitter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Restore default cursor when leaving splitter
            if (!_isPropertiesSplitterDragging)
            {
                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            }
        }

        private void PropertiesSplitter_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var border = sender as Microsoft.UI.Xaml.Controls.Border;
            if (border == null) return;

            _isPropertiesSplitterDragging = true;
            _propertiesSplitterStartX = e.GetCurrentPoint(this).Position.X;
            _propertiesPanelStartWidth = ViewModel.PropertiesPanelWidth;
            border.CapturePointer(e.Pointer);
            e.Handled = true;
        }

        private void PropertiesSplitter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isPropertiesSplitterDragging) return;

            var currentX = e.GetCurrentPoint(this).Position.X;
            var deltaX = _propertiesSplitterStartX - currentX; // Inverted for right-side panel
            var newWidth = _propertiesPanelStartWidth + deltaX;

            // Clamp width to reasonable bounds (200px - 800px)
            newWidth = System.Math.Max(200, System.Math.Min(800, newWidth));

            ViewModel.PropertiesPanelWidth = newWidth;
            e.Handled = true;
        }

        private void PropertiesSplitter_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isPropertiesSplitterDragging) return;

            var border = sender as Microsoft.UI.Xaml.Controls.Border;
            border?.ReleasePointerCapture(e.Pointer);
            _isPropertiesSplitterDragging = false;

            // Restore cursor
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            e.Handled = true;
        }
    }
}
