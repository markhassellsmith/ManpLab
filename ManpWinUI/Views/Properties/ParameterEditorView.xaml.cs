using Microsoft.UI.Xaml.Controls;
using ManpWinUI.ViewModels.Properties;

namespace ManpWinUI.Views.Properties
{
    /// <summary>
    /// Parameter editor control with type-specific inputs and reset functionality.
    /// Week 6: Dynamic parameter editing based on selected fractal.
    /// </summary>
    public sealed partial class ParameterEditorView : UserControl
    {
        public ParameterEditorView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Reset all parameters to their default values.
        /// Week 6 Task 5: Restore fractal defaults.
        /// </summary>
        private void ResetButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext is ParameterEditorViewModel viewModel)
            {
                viewModel.ResetToDefaults();
            }
        }

        /// <summary>
        /// Reload last saved parameter values from persistent storage.
        /// Week 6 Bonus: Restore previously saved state.
        /// </summary>
        private void ReloadButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext is ParameterEditorViewModel viewModel)
            {
                viewModel.ReloadLastSaved();
            }
        }
    }
}
