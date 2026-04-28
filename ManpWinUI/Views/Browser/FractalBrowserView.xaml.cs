using ManpWinUI.ViewModels.Browser;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views.Browser;

/// <summary>
/// Fractal Browser panel - displays categories and fractals in a TreeView.
/// Week 4: Stub implementation with hardcoded data.
/// Week 5: Populate from FractalRegistry, implement search and selection.
/// </summary>
public sealed partial class FractalBrowserView : UserControl
{
    public FractalBrowserViewModel ViewModel { get; }

    public FractalBrowserView()
    {
        ViewModel = new FractalBrowserViewModel();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
