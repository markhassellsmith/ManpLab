using ManpWinUI.ViewModels.Browser;
using ManpWinUI.Services;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views.Browser;

/// <summary>
/// Fractal Browser panel - displays categories and fractals in a TreeView.
/// Week 5: Populate from FractalRegistry, implement search and selection.
/// Task 2: Accept ViewModel via dependency injection instead of creating it.
/// </summary>
public sealed partial class FractalBrowserView : UserControl
{
    /// <summary>
    /// ViewModel for the browser.
    /// Set by MainPage after DI injection.
    /// </summary>
    public FractalBrowserViewModel? ViewModel { get; set; }

    public FractalBrowserView()
    {
        InitializeComponent();
        // Task 2: ViewModel is now injected by MainPage, not created here
        // DataContext will be set by MainPage after ViewModel is assigned
    }
}
