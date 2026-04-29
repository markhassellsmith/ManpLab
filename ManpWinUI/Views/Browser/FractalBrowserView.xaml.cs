using ManpWinUI.ViewModels.Browser;
using ManpWinUI.Services;
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
        // Get settings service from DI container for persistence (Week 5 Task 8)
        var settingsService = App.Current.Services.GetService(typeof(IAppSettingsService)) as IAppSettingsService;

        ViewModel = new FractalBrowserViewModel(settingsService);
        DataContext = ViewModel;
        InitializeComponent();
    }
}
