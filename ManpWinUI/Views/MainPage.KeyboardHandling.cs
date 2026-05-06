using Microsoft.UI.Xaml.Input;
using Windows.System;
using System;
using System.Runtime.Versioning;

namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Keyboard shortcuts and accelerators.
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class MainPage
    {
        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Check for modifier keys
            var ctrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var shiftPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var altPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            // Don't process if a text input has focus
            if (FocusManager.GetFocusedElement(this.XamlRoot) is Microsoft.UI.Xaml.Controls.TextBox ||
                FocusManager.GetFocusedElement(this.XamlRoot) is Microsoft.UI.Xaml.Controls.NumberBox)
            {
                return;
            }

            switch (e.Key)
            {
                // F1: Show keyboard shortcuts help
                case VirtualKey.F1:
                    _ = ShowKeyboardShortcutsHelp();
                    e.Handled = true;
                    break;

                // Ctrl+B: Toggle Browser Panel
                case VirtualKey.B when ctrlPressed:
                    if (ViewModel.ToggleBrowserPanelCommand.CanExecute(null))
                    {
                        ViewModel.ToggleBrowserPanelCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;

                // Ctrl+P: Toggle Properties Panel
                case VirtualKey.P when ctrlPressed:
                    if (ViewModel.TogglePropertiesPanelCommand.CanExecute(null))
                    {
                        ViewModel.TogglePropertiesPanelCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;

                // Ctrl+Z: Undo Navigation
                case VirtualKey.Z when ctrlPressed:
                    if (ViewModel.UndoNavigationCommand.CanExecute(null))
                    {
                        ViewModel.UndoNavigationCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;

                // Ctrl+Y: Redo Navigation
                case VirtualKey.Y when ctrlPressed:
                    if (ViewModel.RedoNavigationCommand.CanExecute(null))
                    {
                        ViewModel.RedoNavigationCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;

                // Ctrl+R or F5: Render
                case VirtualKey.R when ctrlPressed:
                case VirtualKey.F5:
                    if (ViewModel.RenderCommand.CanExecute(null))
                    {
                        ViewModel.RenderCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                // Space or Home: Reset View
                case VirtualKey.Space:
                case VirtualKey.Home:
                    if (ViewModel.ResetViewCommand.CanExecute(null))
                    {
                        ViewModel.ResetViewCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                // + or = or Ctrl+Plus: Zoom In (Mandelbrot/Julia only)
                case VirtualKey.Add:
                    if (!ViewModel.IsHailstoneMode && ViewModel.ZoomInCommand.CanExecute(null))
                    {
                        ViewModel.ZoomInCommand.Execute(null);
                        e.Handled = true;
                    }
                    else if (ViewModel.IsHailstoneMode)
                    {
                        ViewModel.StatusMessage = "Zoom commands not applicable to Hailstone sequences";
                        e.Handled = true;
                    }
                    break;

                // - or _ or Ctrl+Minus: Zoom Out (Mandelbrot/Julia only)
                case VirtualKey.Subtract:
                    if (!ViewModel.IsHailstoneMode && ViewModel.ZoomOutCommand.CanExecute(null))
                    {
                        ViewModel.ZoomOutCommand.Execute(null);
                        e.Handled = true;
                    }
                    else if (ViewModel.IsHailstoneMode)
                    {
                        ViewModel.StatusMessage = "Zoom commands not applicable to Hailstone sequences";
                        e.Handled = true;
                    }
                    break;

                // Arrow Keys: Pan (Shift for faster panning) - Mandelbrot/Julia only
                case VirtualKey.Up:
                    if (!ViewModel.IsHailstoneMode)
                    {
                        PanView(0, shiftPressed ? -0.25 : -0.1); // Pan up
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Arrow key panning not applicable to Hailstone sequences";
                    }
                    e.Handled = true;
                    break;

                case VirtualKey.Down:
                    if (!ViewModel.IsHailstoneMode)
                    {
                        PanView(0, shiftPressed ? 0.25 : 0.1); // Pan down
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Arrow key panning not applicable to Hailstone sequences";
                    }
                    e.Handled = true;
                    break;

                case VirtualKey.Left:
                    if (!ViewModel.IsHailstoneMode)
                    {
                        PanView(shiftPressed ? -0.25 : -0.1, 0); // Pan left
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Arrow key panning not applicable to Hailstone sequences";
                    }
                    e.Handled = true;
                    break;

                case VirtualKey.Right:
                    if (!ViewModel.IsHailstoneMode)
                    {
                        PanView(shiftPressed ? 0.25 : 0.1, 0); // Pan right
                    }
                    else
                    {
                        ViewModel.StatusMessage = "Arrow key panning not applicable to Hailstone sequences";
                    }
                    e.Handled = true;
                    break;

                // Page Up/Down: Increase/Decrease iterations
                case VirtualKey.PageUp:
                    AdjustIterations(shiftPressed ? 500 : 100);
                    e.Handled = true;
                    break;

                case VirtualKey.PageDown:
                    AdjustIterations(shiftPressed ? -500 : -100);
                    e.Handled = true;
                    break;

                // Number keys 1-4: Resolution presets
                case VirtualKey.Number1 when !ctrlPressed && !altPressed:
                    ViewModel.SetResolutionCommand.Execute("HD");
                    e.Handled = true;
                    break;

                case VirtualKey.Number2 when !ctrlPressed && !altPressed:
                    ViewModel.SetResolutionCommand.Execute("FullHD");
                    e.Handled = true;
                    break;

                case VirtualKey.Number3 when !ctrlPressed && !altPressed:
                    ViewModel.SetResolutionCommand.Execute("2K");
                    e.Handled = true;
                    break;

                case VirtualKey.Number4 when !ctrlPressed && !altPressed:
                    ViewModel.SetResolutionCommand.Execute("4K");
                    e.Handled = true;
                    break;

                // A: Toggle coordinate axes (context-sensitive)
                case VirtualKey.A when !ctrlPressed:
                    if (ViewModel.IsHailstoneMode)
                    {
                        ViewModel.ShowHailstoneAxes = !ViewModel.ShowHailstoneAxes;
                        ViewModel.StatusMessage = $"Hailstone axes {(ViewModel.ShowHailstoneAxes ? "shown" : "hidden")}";
                    }
                    else
                    {
                        ViewModel.ShowCoordinateAxes = !ViewModel.ShowCoordinateAxes;
                        ViewModel.StatusMessage = $"Coordinate axes {(ViewModel.ShowCoordinateAxes ? "shown" : "hidden")}";
                    }
                    e.Handled = true;
                    break;

                // T: Toggle between Standard and Julia mode (Mandelbrot/Julia only)
                case VirtualKey.T when !ctrlPressed:
                    if (!ViewModel.IsHailstoneMode)
                    {
                        ToggleIterationMode();
                    }
                    else
                    {
                        ViewModel.StatusMessage = "'T' key toggles Julia mode (not applicable to Hailstone)";
                    }
                    e.Handled = true;
                    break;

                // I: Toggle auto-scale iterations
                case VirtualKey.I when !ctrlPressed:
                    ViewModel.AutoScaleIterations = !ViewModel.AutoScaleIterations;
                    ViewModel.StatusMessage = $"Auto-scale iterations {(ViewModel.AutoScaleIterations ? "enabled" : "disabled")}";
                    e.Handled = true;
                    break;

                // Escape: Cancel render (future - need to implement cancellation)
                case VirtualKey.Escape:
                    // TODO: Implement render cancellation
                    ViewModel.StatusMessage = "Render cancellation not yet implemented";
                    e.Handled = true;
                    break;

                // Ctrl+S: Save Image
                case VirtualKey.S when ctrlPressed:
                    _ = SaveImageAsync(Services.ImageFormat.PNG);
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Pan the view by a percentage of the current view width/height.
        /// </summary>
        /// <param name="xPercent">Horizontal pan (-1.0 to 1.0, where 0.1 = 10% of view width)</param>
        /// <param name="yPercent">Vertical pan (-1.0 to 1.0, where 0.1 = 10% of view height)</param>
        private void PanView(double xPercent, double yPercent)
        {
            // Calculate current view dimensions
            var fractalWidth = 3.0 / ViewModel.Zoom;
            var fractalHeight = fractalWidth * ((double)ViewModel.ImageHeight / ViewModel.ImageWidth);

            // Apply pan offset
            ViewModel.CenterX += xPercent * fractalWidth;
            ViewModel.CenterY += yPercent * fractalHeight;

            // Auto-render after pan
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }

            ViewModel.StatusMessage = $"Panned to ({ViewModel.CenterX:F6}, {ViewModel.CenterY:F6})";
        }

        /// <summary>
        /// Adjust the max iterations by a specified amount.
        /// </summary>
        /// <param name="delta">Amount to add/subtract from iterations</param>
        private void AdjustIterations(int delta)
        {
            var oldIterations = ViewModel.MaxIterations;
            ViewModel.MaxIterations = Math.Max(50, Math.Min(50000, ViewModel.MaxIterations + delta));

            if (ViewModel.MaxIterations != oldIterations)
            {
                ViewModel.StatusMessage = $"Max iterations: {oldIterations} → {ViewModel.MaxIterations}";
            }
        }

        /// <summary>
        /// Toggle between Standard and Julia iteration modes.
        /// </summary>
        private void ToggleIterationMode()
        {
            ViewModel.SelectedIterationMode = ViewModel.SelectedIterationMode == "Standard" ? "Julia" : "Standard";
            var mode = ViewModel.SelectedIterationMode;
            ViewModel.StatusMessage = mode == "Julia" 
                ? $"Switched to Julia mode (c = {ViewModel.JuliaCX:F4}, {ViewModel.JuliaCY:F4})"
                : "Switched to Standard mode";
        }

        /// <summary>
        /// Show a dialog with all available keyboard shortcuts.
        /// </summary>
        private async System.Threading.Tasks.Task ShowKeyboardShortcutsHelp()
        {
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Keyboard Shortcuts",
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot,
                Content = new Microsoft.UI.Xaml.Controls.ScrollViewer
                {
                    Content = new Microsoft.UI.Xaml.Controls.TextBlock
                    {
                        Text = @"═══════════════════════════════════════
RENDERING & VIEW
═══════════════════════════════════════
F5, Ctrl+R         Render fractal
Space, Home        Reset to default view
Escape             Cancel render (not implemented)

═══════════════════════════════════════
ZOOM & NAVIGATION
═══════════════════════════════════════
+, =               Zoom in 2×
-, _               Zoom out 2×
Arrow Keys         Pan view (10% of view)
Shift+Arrows       Pan view faster (25% of view)

═══════════════════════════════════════
PARAMETERS
═══════════════════════════════════════
Page Up            Increase iterations (+100)
Page Down          Decrease iterations (-100)
Shift+Page Up      Increase iterations (+500)
Shift+Page Down    Decrease iterations (-500)
I                  Toggle auto-scale iterations

═══════════════════════════════════════
RESOLUTION PRESETS
═══════════════════════════════════════
1                  HD (1280×720)
2                  Full HD (1920×1080)
3                  2K (2560×1440)
4                  4K (3840×2160)

═══════════════════════════════════════
MODES & DISPLAY
═══════════════════════════════════════
T                  Toggle Standard/Julia mode
A                  Toggle coordinate axes
Ctrl+S             Save image

═══════════════════════════════════════
NAVIGATION HISTORY (Week 8)
═══════════════════════════════════════
Ctrl+Z             Undo - Go back in history
Ctrl+Y             Redo - Go forward in history

═══════════════════════════════════════
PANELS
═══════════════════════════════════════
Ctrl+B             Toggle Browser panel
Ctrl+P             Toggle Properties panel

═══════════════════════════════════════
HELP
═══════════════════════════════════════
F1                 Show this help dialog",
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                        TextWrapping = Microsoft.UI.Xaml.TextWrapping.NoWrap
                    }
                }
            };

            await dialog.ShowAsync();
        }
    }
}
