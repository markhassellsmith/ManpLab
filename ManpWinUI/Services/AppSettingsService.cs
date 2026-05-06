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
    private const string PropertiesTabIndexKey = "PropertiesTabIndex";
    private const string SelectedFractalKey = "SelectedFractal";
    private const string FractalParametersKeyPrefix = "FractalParams_"; // Week 6 Task 6
    private const string FractalNotesKeyPrefix = "FractalNotes_"; // User notes per fractal

    // Application Settings Keys
    private const string ThemeKey = "AppTheme";
    private const string DefaultPaletteKey = "DefaultPalette";
    private const string ShowAxesByDefaultKey = "ShowAxesByDefault";
    private const string UseSmoothColoringByDefaultKey = "UseSmoothColoringByDefault";
    private const string DefaultAntialiasingLevelKey = "DefaultAntialiasingLevel";
    private const string UseDeepZoomKey = "UseDeepZoom"; // Week 9: Perturbation-theory deep zoom

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

    /// <summary>
    /// Gets saved parameter values for a specific fractal.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    public string? GetFractalParameters(string fractalName)
    {
        var key = FractalParametersKeyPrefix + fractalName;
        if (_localSettings.Values.TryGetValue(key, out var value) && value is string json)
        {
            return json;
        }
        return null;
    }

    /// <summary>
    /// Saves parameter values for a specific fractal.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    public void SetFractalParameters(string fractalName, string parametersJson)
    {
        var key = FractalParametersKeyPrefix + fractalName;
        _localSettings.Values[key] = parametersJson;
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // APPLICATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    public string GetTheme()
    {
        if (_localSettings.Values.TryGetValue(ThemeKey, out var value) && value is string theme)
        {
            return theme;
        }
        return "System"; // Default to system theme
    }

    public void SetTheme(string theme)
    {
        _localSettings.Values[ThemeKey] = theme;
    }

    public string GetDefaultPalette()
    {
        if (_localSettings.Values.TryGetValue(DefaultPaletteKey, out var value) && value is string palette)
        {
            System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Retrieved saved palette: '{palette}'");
            return palette;
        }
        System.Diagnostics.Debug.WriteLine("[AppSettingsService] No saved palette found, returning default: 'Classic'");
        return "Classic"; // Default palette
    }

    public void SetDefaultPalette(string palette)
    {
        System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Saving palette: '{palette}'");
        _localSettings.Values[DefaultPaletteKey] = palette;
    }

    public bool GetShowAxesByDefault()
    {
        if (_localSettings.Values.TryGetValue(ShowAxesByDefaultKey, out var value) && value is bool show)
        {
            return show;
        }
        return true; // Default to showing axes
    }

    public void SetShowAxesByDefault(bool show)
    {
        _localSettings.Values[ShowAxesByDefaultKey] = show;
    }

    public bool GetUseSmoothColoringByDefault()
    {
        if (_localSettings.Values.TryGetValue(UseSmoothColoringByDefaultKey, out var value) && value is bool use)
        {
            return use;
        }
        return false; // Default to standard coloring
    }

    public void SetUseSmoothColoringByDefault(bool use)
    {
        _localSettings.Values[UseSmoothColoringByDefaultKey] = use;
    }

    public string GetDefaultAntialiasingLevel()
    {
        if (_localSettings.Values.TryGetValue(DefaultAntialiasingLevelKey, out var value) && value is string level)
        {
            return level;
        }
        return "None"; // Default to no antialiasing
    }

    public void SetDefaultAntialiasingLevel(string level)
    {
        _localSettings.Values[DefaultAntialiasingLevelKey] = level;
    }

    public int? GetPropertiesTabIndex()
    {
        if (_localSettings.Values.TryGetValue(PropertiesTabIndexKey, out var value) && value is int index)
        {
            return index;
        }
        return null;
    }

    public void SetPropertiesTabIndex(int index)
    {
        _localSettings.Values[PropertiesTabIndexKey] = index;
    }

    public bool GetUseDeepZoom()
    {
        if (_localSettings.Values.TryGetValue(UseDeepZoomKey, out var value) && value is bool use)
        {
            return use;
        }
        return false; // Default to disabled for safety
    }

    public void SetUseDeepZoom(bool use)
    {
        _localSettings.Values[UseDeepZoomKey] = use;
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // USER NOTES
    // ═══════════════════════════════════════════════════════════════════════════════

    public string? GetFractalNotes(string fractalName)
    {
        var key = FractalNotesKeyPrefix + fractalName;
        if (_localSettings.Values.TryGetValue(key, out var value) && value is string notes)
        {
            return notes;
        }
        return null;
    }

    public void SetFractalNotes(string fractalName, string? notes)
    {
        var key = FractalNotesKeyPrefix + fractalName;
        if (string.IsNullOrWhiteSpace(notes))
        {
            // Remove the key if notes are empty
            _localSettings.Values.Remove(key);
        }
        else
        {
            _localSettings.Values[key] = notes;
        }
    }
}
