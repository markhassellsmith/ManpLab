using ManpWinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views.Settings;

/// <summary>
/// Settings view for application preferences.
/// </summary>
public sealed partial class SettingsView : UserControl
{
    public SettingsViewModel ViewModel { get; }

    public SettingsView()
    {
        // Get the settings service from the app
        var settingsService = ((App)App.Current).Services.GetService(typeof(Services.IAppSettingsService)) 
            as Services.IAppSettingsService;

        if (settingsService == null)
        {
            throw new System.InvalidOperationException("IAppSettingsService not registered in DI container");
        }

        ViewModel = new SettingsViewModel(settingsService);
        this.InitializeComponent();
    }
}
