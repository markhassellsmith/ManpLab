using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ManpWinUI.ViewModels.Properties;
using ManpWinUI.Services;

namespace ManpWinUI.Views.Properties
{
    /// <summary>
    /// Render settings control for quality and mode configuration.
    /// Week 7 Task 1: Foundation setup for render customization.
    /// </summary>
    public sealed partial class RenderSettingsView : UserControl
    {
        // Use DataContext set by MainPage instead of creating our own instance
        public RenderSettingsViewModel ViewModel => (RenderSettingsViewModel)DataContext;

        public RenderSettingsView()
        {
            this.InitializeComponent();
            // DataContext will be set by MainPage to use the DI singleton
        }

        /// <summary>
        /// Handle render mode selection change.
        /// Week 7 Task 3: Update render algorithm.
        /// </summary>
        private void RenderMode_Changed(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[RenderSettingsView] Render mode changed: {ViewModel.SelectedRenderMode}");
        }

        /// <summary>
        /// Handle antialiasing selection change.
        /// Week 7 Task 4: Update quality settings.
        /// </summary>
        private void Antialiasing_Changed(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[RenderSettingsView] Antialiasing changed: {ViewModel.AntialiasingLevel}");
        }

        /// <summary>
        /// Handle smooth coloring toggle.
        /// Week 7 Task 3: Enable/disable continuous coloring.
        /// </summary>
        private void SmoothColoring_Changed(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[RenderSettingsView] Smooth coloring: {ViewModel.UseSmoothColoring}");
        }

        /// <summary>
        /// Handle deep zoom toggle.
        /// Week 7 Task 4: Enable/disable arbitrary precision.
        /// </summary>
        private void DeepZoom_Changed(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[RenderSettingsView] Deep zoom: {ViewModel.UseDeepZoom}");
        }

        /// <summary>
        /// Handle reset button click.
        /// Week 7 Task 1: Reset all settings to defaults.
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetToDefaults();
            System.Diagnostics.Debug.WriteLine("[RenderSettingsView] Reset to defaults");
        }
    }
}
