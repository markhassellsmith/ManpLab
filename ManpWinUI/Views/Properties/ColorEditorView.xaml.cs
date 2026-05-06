using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ManpWinUI.ViewModels.Properties;

namespace ManpWinUI.Views.Properties
{
    /// <summary>
    /// Color editor control for palette selection and color adjustments.
    /// Week 7 Task 1: Foundation setup for color customization.
    /// </summary>
    public sealed partial class ColorEditorView : UserControl
    {
        public ColorEditorViewModel ViewModel { get; set; } = null!;

        public ColorEditorView()
        {
            // ViewModel will be set via DataContext from MainPage
            this.InitializeComponent();
        }

        /// <summary>
        /// Handle palette selection button click.
        /// Week 7 Task 1: Update selected palette on user interaction.
        /// </summary>
        private void PaletteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PaletteItem palette)
            {
                ViewModel.SelectedPalette = palette;
                System.Diagnostics.Debug.WriteLine($"[ColorEditorView] Palette selected: {palette.Name}");
            }
        }

        /// <summary>
        /// Handle reset button click.
        /// Week 7 Task 1: Reset all color settings to defaults.
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetToDefaults();
            System.Diagnostics.Debug.WriteLine("[ColorEditorView] Reset to defaults");
        }
    }
}
