using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ManpWinUI.ViewModels.Properties;

namespace ManpWinUI.Views.Properties
{
    /// <summary>
    /// Render settings control for quality and mode configuration.
    /// Week 7 Task 1: Foundation setup for render customization.
    /// </summary>
    public sealed partial class RenderSettingsView : UserControl
    {
        public RenderSettingsViewModel ViewModel { get; }

        public RenderSettingsView()
        {
            this.InitializeComponent();
            ViewModel = new RenderSettingsViewModel();
        }

        /// <summary>
        /// Constructor accepting a ViewModel (for MainPage injection).
        /// Week 7 Task 2: Enable event subscription from MainPage.
        /// </summary>
        public RenderSettingsView(RenderSettingsViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
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
        /// Handle resolution change.
        /// Week 7 Task 4: Update output dimensions.
        /// </summary>
        private void Resolution_Changed(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"[RenderSettingsView] Resolution: {ViewModel.RenderWidth}x{ViewModel.RenderHeight}");
        }

        /// <summary>
        /// Apply SD resolution preset (800x600).
        /// </summary>
        private void ResolutionSD_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyResolutionPreset("sd");
        }

        /// <summary>
        /// Apply HD resolution preset (1280x720).
        /// </summary>
        private void ResolutionHD_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyResolutionPreset("hd");
        }

        /// <summary>
        /// Apply Full HD resolution preset (1920x1080).
        /// </summary>
        private void ResolutionFullHD_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyResolutionPreset("fullhd");
        }

        /// <summary>
        /// Apply 4K resolution preset (3840x2160).
        /// </summary>
        private void Resolution4K_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyResolutionPreset("4k");
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
