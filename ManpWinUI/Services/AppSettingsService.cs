using Windows.Storage;

namespace ManpWinUI.Services;

/// <summary>
/// Implementation of app settings service using ApplicationData.LocalSettings.
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private readonly ApplicationDataContainer _localSettings;

    private const string BrowserWidthKey = "BrowserPanelWidth";
    private const string PropertiesWidthKey = "PropertiesPanelWidth";
    private const string BrowserVisibleKey = "BrowserPanelVisible";
    private const string PropertiesVisibleKey = "PropertiesPanelVisible";
    private const string SelectedFractalKey = "SelectedFractal";

    // Default values
    private const double DefaultBrowserWidth = 250.0;
    private const double DefaultPropertiesWidth = 300.0;

    public AppSettingsService()
    {
        _localSettings = ApplicationData.Current.LocalSettings;
    }

    public double? GetBrowserPanelWidth()
    {
        if (_localSettings.Values.TryGetValue(BrowserWidthKey, out var value) && value is double width)
        {
            return width;
        }
        return null;
    }

    public void SetBrowserPanelWidth(double width)
    {
        _localSettings.Values[BrowserWidthKey] = width;
    }

    public double? GetPropertiesPanelWidth()
    {
        if (_localSettings.Values.TryGetValue(PropertiesWidthKey, out var value) && value is double width)
        {
            return width;
        }
        return null;
    }

    public void SetPropertiesPanelWidth(double width)
    {
        _localSettings.Values[PropertiesWidthKey] = width;
    }

    public bool GetBrowserPanelVisible()
    {
        if (_localSettings.Values.TryGetValue(BrowserVisibleKey, out var value) && value is bool visible)
        {
            return visible;
        }
        return true; // Default to visible
    }

    public void SetBrowserPanelVisible(bool visible)
    {
        _localSettings.Values[BrowserVisibleKey] = visible;
    }

    public bool GetPropertiesPanelVisible()
    {
        if (_localSettings.Values.TryGetValue(PropertiesVisibleKey, out var value) && value is bool visible)
        {
            return visible;
        }
        return true; // Default to visible
    }

    public void SetPropertiesPanelVisible(bool visible)
    {
        _localSettings.Values[PropertiesVisibleKey] = visible;
    }

    public string? GetSelectedFractal()
    {
        if (_localSettings.Values.TryGetValue(SelectedFractalKey, out var value) && value is string fractalName)
        {
            return fractalName;
        }
        return null;
    }

    public void SetSelectedFractal(string fractalName)
    {
        _localSettings.Values[SelectedFractalKey] = fractalName;
    }
}
