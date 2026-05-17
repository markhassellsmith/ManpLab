using System.Text.Json;
using Windows.Storage;

namespace ManpWinUI.Services;

/// <summary>
/// Implementation of app settings service that works for both packaged (MSIX) and unpackaged (portable ZIP) scenarios.
/// For packaged apps: uses ApplicationData.LocalSettings
/// For unpackaged apps: uses JSON file in AppData\Local
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private readonly ApplicationDataContainer? _localSettings;
    private readonly string? _settingsFilePath;
    private readonly Dictionary<string, object> _fileSettings;
    private readonly bool _isPackaged;

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
    private const string UseSmoothColoringKey = "UseSmoothColoring"; // Anti-banding persistent setting
    private const string AnimationLastDirectoryKey = "AnimationLastDirectory"; // Last used animation export folder

    // Window Layout & Position Keys
    private const string WindowWidthKey = "WindowWidth";
    private const string WindowHeightKey = "WindowHeight";
    private const string WindowXKey = "WindowX";
    private const string WindowYKey = "WindowY";

    // Default values
    private const double DefaultBrowserWidth = 250.0;
    private const double DefaultPropertiesWidth = 300.0;

    public AppSettingsService()
    {
        // Try to detect if we're running as a packaged app (MSIX) or unpackaged (portable ZIP)
        try
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _isPackaged = true;
            System.Diagnostics.Debug.WriteLine("[AppSettingsService] Running as packaged app (MSIX) - using ApplicationData.LocalSettings");
        }
        catch (Exception ex)
        {
            // If ApplicationData.Current fails, we're running unpackaged
            _isPackaged = false;
            System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Running as unpackaged app (portable) - using JSON file storage: {ex.Message}");

            // Use JSON file in LocalAppData for unpackaged scenarios
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localAppData, "ManpLab");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");

            // Load existing settings from file
            _fileSettings = LoadSettingsFromFile();
            System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Settings file: {_settingsFilePath}");
        }
    }

    private Dictionary<string, object> LoadSettingsFromFile()
    {
        if (!string.IsNullOrEmpty(_settingsFilePath) && File.Exists(_settingsFilePath))
        {
            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (settings != null)
                {
                    // Convert JsonElement to appropriate types
                    var result = new Dictionary<string, object>();
                    foreach (var kvp in settings)
                    {
                        result[kvp.Key] = ConvertJsonElement(kvp.Value);
                    }
                    System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Loaded {result.Count} settings from file");
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Error loading settings file: {ex.Message}");
            }
        }
        return new Dictionary<string, object>();
    }

    private object ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt32(out var i) ? (object)i : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => element.ToString()
        };
    }

    private void SaveSettingsToFile()
    {
        if (string.IsNullOrEmpty(_settingsFilePath)) return;

        try
        {
            var json = JsonSerializer.Serialize(_fileSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
            System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Saved {_fileSettings.Count} settings to file");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Error saving settings file: {ex.Message}");
        }
    }

    private object? GetValue(string key)
    {
        if (_isPackaged && _localSettings != null)
        {
            return _localSettings.Values.TryGetValue(key, out var value) ? value : null;
        }
        else
        {
            return _fileSettings.TryGetValue(key, out var value) ? value : null;
        }
    }

    private void SetValue(string key, object value)
    {
        if (_isPackaged && _localSettings != null)
        {
            _localSettings.Values[key] = value;
        }
        else
        {
            _fileSettings[key] = value;
            SaveSettingsToFile();
        }
    }

    public double? GetBrowserPanelWidth()
    {
        var value = GetValue(BrowserWidthKey);
        return value is double width ? width : null;
    }

    public void SetBrowserPanelWidth(double width)
    {
        SetValue(BrowserWidthKey, width);
    }

    public double? GetPropertiesPanelWidth()
    {
        var value = GetValue(PropertiesWidthKey);
        return value is double width ? width : null;
    }

    public void SetPropertiesPanelWidth(double width)
    {
        SetValue(PropertiesWidthKey, width);
    }

    public bool GetBrowserPanelVisible()
    {
        var value = GetValue(BrowserVisibleKey);
        return value is bool visible ? visible : true; // Default to visible
    }

    public void SetBrowserPanelVisible(bool visible)
    {
        SetValue(BrowserVisibleKey, visible);
    }

    public bool GetPropertiesPanelVisible()
    {
        var value = GetValue(PropertiesVisibleKey);
        return value is bool visible ? visible : true; // Default to visible
    }

    public void SetPropertiesPanelVisible(bool visible)
    {
        SetValue(PropertiesVisibleKey, visible);
    }

    public string? GetSelectedFractal()
    {
        var value = GetValue(SelectedFractalKey);
        return value as string;
    }

    public void SetSelectedFractal(string fractalName)
    {
        SetValue(SelectedFractalKey, fractalName);
    }

    /// <summary>
    /// Gets saved parameter values for a specific fractal.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    public string? GetFractalParameters(string fractalName)
    {
        var key = FractalParametersKeyPrefix + fractalName;
        var value = GetValue(key);
        return value as string;
    }

    /// <summary>
    /// Saves parameter values for a specific fractal.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    public void SetFractalParameters(string fractalName, string parametersJson)
    {
        var key = FractalParametersKeyPrefix + fractalName;
        SetValue(key, parametersJson);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // APPLICATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    public string GetTheme()
    {
        var value = GetValue(ThemeKey);
        return value as string ?? "System"; // Default to system theme
    }

    public void SetTheme(string theme)
    {
        SetValue(ThemeKey, theme);
    }

    public string GetDefaultPalette()
    {
        var value = GetValue(DefaultPaletteKey);
        var palette = value as string ?? "Classic"; // Default palette
        System.Diagnostics.Debug.WriteLine(value != null
            ? $"[AppSettingsService] Retrieved saved palette: '{palette}'"
            : "[AppSettingsService] No saved palette found, returning default: 'Classic'");
        return palette;
    }

    public void SetDefaultPalette(string palette)
    {
        System.Diagnostics.Debug.WriteLine($"[AppSettingsService] Saving palette: '{palette}'");
        SetValue(DefaultPaletteKey, palette);
    }

    public bool GetShowAxesByDefault()
    {
        var value = GetValue(ShowAxesByDefaultKey);
        return value is bool show ? show : true; // Default to showing axes
    }

    public void SetShowAxesByDefault(bool show)
    {
        SetValue(ShowAxesByDefaultKey, show);
    }

    public bool GetUseSmoothColoringByDefault()
    {
        var value = GetValue(UseSmoothColoringByDefaultKey);
        return value is bool use ? use : false; // Default to standard coloring
    }

    public void SetUseSmoothColoringByDefault(bool use)
    {
        SetValue(UseSmoothColoringByDefaultKey, use);
    }

    public string GetDefaultAntialiasingLevel()
    {
        var value = GetValue(DefaultAntialiasingLevelKey);
        return value as string ?? "None"; // Default to no antialiasing
    }

    public void SetDefaultAntialiasingLevel(string level)
    {
        SetValue(DefaultAntialiasingLevelKey, level);
    }

    public int? GetPropertiesTabIndex()
    {
        var value = GetValue(PropertiesTabIndexKey);
        return value is int index ? index : null;
    }

    public void SetPropertiesTabIndex(int index)
    {
        SetValue(PropertiesTabIndexKey, index);
    }

    public bool GetUseDeepZoom()
    {
        var value = GetValue(UseDeepZoomKey);
        return value is bool use ? use : false; // Default to disabled for safety
    }

    public void SetUseDeepZoom(bool use)
    {
        SetValue(UseDeepZoomKey, use);
    }

    public bool GetUseSmoothColoring()
    {
        var value = GetValue(UseSmoothColoringKey);
        return value is bool use ? use : true; // Default to enabled (anti-banding ON)
    }

    public void SetUseSmoothColoring(bool use)
    {
        SetValue(UseSmoothColoringKey, use);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    public string? GetAnimationLastDirectory()
    {
        var value = GetValue(AnimationLastDirectoryKey);
        return value as string;
    }

    public void SetAnimationLastDirectory(string directory)
    {
        SetValue(AnimationLastDirectoryKey, directory);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // USER NOTES
    // ═══════════════════════════════════════════════════════════════════════════════

    public string? GetFractalNotes(string fractalName)
    {
        var key = FractalNotesKeyPrefix + fractalName;
        var value = GetValue(key);
        return value as string;
    }

    public void SetFractalNotes(string fractalName, string notes)
    {
        var key = FractalNotesKeyPrefix + fractalName;
        SetValue(key, notes);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // WINDOW LAYOUT & POSITION
    // ═══════════════════════════════════════════════════════════════════════════════

    public int? GetWindowWidth()
    {
        var value = GetValue(WindowWidthKey);
        return value is int width ? width : null;
    }

    public void SetWindowWidth(int width)
    {
        SetValue(WindowWidthKey, width);
    }

    public int? GetWindowHeight()
    {
        var value = GetValue(WindowHeightKey);
        return value is int height ? height : null;
    }

    public void SetWindowHeight(int height)
    {
        SetValue(WindowHeightKey, height);
    }

    public int? GetWindowX()
    {
        var value = GetValue(WindowXKey);
        return value is int x ? x : null;
    }

    public void SetWindowX(int x)
    {
        SetValue(WindowXKey, x);
    }

    public int? GetWindowY()
    {
        var value = GetValue(WindowYKey);
        return value is int y ? y : null;
    }

    public void SetWindowY(int y)
    {
        SetValue(WindowYKey, y);
    }
}
