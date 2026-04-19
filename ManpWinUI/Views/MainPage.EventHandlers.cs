namespace ManpWinUI.Views
{
    /// <summary>
    /// MainPage partial class - Button clicks and UI event handlers.
    /// </summary>
    public sealed partial class MainPage
    {
        private void MatchWindowSize_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Get the actual size of the fractal display area (Grid Row 1)
            if (FractalViewbox?.ActualWidth > 0 && FractalViewbox?.ActualHeight > 0)
            {
                // Use the Viewbox size as the target resolution
                var width = (int)FractalViewbox.ActualWidth;
                var height = (int)FractalViewbox.ActualHeight;

                // Round to nearest multiple of 16 for better performance
                width = (width / 16) * 16;
                height = (height / 16) * 16;

                ViewModel.ImageWidth = width;
                ViewModel.ImageHeight = height;
                ViewModel.StatusMessage = $"Resolution matched to window: {width}×{height}";
            }
            else
            {
                ViewModel.StatusMessage = "Window size not available yet";
            }
        }

        private void JuliaPreset1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Classic Julia set - beautiful spiral structure
            ViewModel.JuliaCX = -0.7;
            ViewModel.JuliaCY = 0.27015;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Classic Spiral";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset2_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Dendrite - spectacular tree-like structure
            ViewModel.JuliaCX = -0.8;
            ViewModel.JuliaCY = 0.156;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.5;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Dendrite";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset3_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // San Marco Jewel - intricate circular pattern
            ViewModel.JuliaCX = 0.285;
            ViewModel.JuliaCY = 0.01;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.5;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: San Marco Jewel";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }

        private void JuliaPreset4_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Paisley/Swirl - organic flowing shapes
            ViewModel.JuliaCX = -0.4;
            ViewModel.JuliaCY = 0.6;
            ViewModel.CenterX = 0.0;
            ViewModel.CenterY = 0.0;
            ViewModel.Zoom = 0.6;
            ViewModel.MaxIterations = 256;
            ViewModel.StatusMessage = "Julia preset: Paisley Swirl";
            if (ViewModel.RenderMandelbrotCommand.CanExecute(null))
            {
                ViewModel.RenderMandelbrotCommand.Execute(null);
            }
        }
    }
}
