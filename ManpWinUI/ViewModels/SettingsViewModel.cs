using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Services;
using System.Collections.Generic;

namespace ManpWinUI.ViewModels;

/// <summary>
/// ViewModel for application settings.
/// Manages user preferences and persists them across sessions.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly IAppSettingsService _settingsService;
    private bool _isInitializing = false;

    // ═══════════════════════════════════════════════════════════════════════════════
    // THEME SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Available theme options.
    /// </summary>
    public List<string> AvailableThemes { get; } = new() { "Light", "Dark", "Ocean Blue", "System" };

    /// <summary>
    /// Currently selected application theme.
    /// </summary>
    [ObservableProperty]
    private string _selectedTheme = "System";

    partial void OnSelectedThemeChanged(string value)
    {
        _settingsService.SetTheme(value);

        // Only apply theme if not initializing (user actively changed it)
        if (!_isInitializing && App.Current?.MainWindow != null)
        {
            App.Current.ApplyTheme();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // VISUAL SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Available color palettes.
    /// </summary>
    public List<string> AvailablePalettes { get; } = new()
    {
        "Classic",
        "Fire",
        "Ice",
        "Rainbow",
        "Grayscale",
        "Neon",
        "Ocean",
        "Forest"
    };

    /// <summary>
    /// Default color palette for new fractals.
    /// </summary>
    [ObservableProperty]
    private string _defaultPalette = "Classic";

    partial void OnDefaultPaletteChanged(string value)
    {
        _settingsService.SetDefaultPalette(value);
    }

    /// <summary>
    /// Whether to show coordinate axes by default.
    /// </summary>
    [ObservableProperty]
    private bool _showAxesByDefault = true;

    partial void OnShowAxesByDefaultChanged(bool value)
    {
        _settingsService.SetShowAxesByDefault(value);
    }

    /// <summary>
    /// Whether to use smooth coloring by default.
    /// </summary>
    [ObservableProperty]
    private bool _useSmoothColoringByDefault = false;

    partial void OnUseSmoothColoringByDefaultChanged(bool value)
    {
        _settingsService.SetUseSmoothColoringByDefault(value);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDERING SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Available antialiasing levels.
    /// </summary>
    public List<string> AvailableAntialiasingLevels { get; } = new()
    {
        "None",
        "2x",
        "4x",
        "8x"
    };

    /// <summary>
    /// Default antialiasing level.
    /// </summary>
    [ObservableProperty]
    private string _defaultAntialiasingLevel = "None";

    partial void OnDefaultAntialiasingLevelChanged(string value)
    {
        _settingsService.SetDefaultAntialiasingLevel(value);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONSTRUCTOR
    // ═══════════════════════════════════════════════════════════════════════════════

    public SettingsViewModel(IAppSettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadSettings();
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // LOAD/RESET
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loads all settings from the settings service.
    /// </summary>
    private void LoadSettings()
    {
        _isInitializing = true;
        SelectedTheme = _settingsService.GetTheme();
        DefaultPalette = _settingsService.GetDefaultPalette();
        ShowAxesByDefault = _settingsService.GetShowAxesByDefault();
        UseSmoothColoringByDefault = _settingsService.GetUseSmoothColoringByDefault();
        DefaultAntialiasingLevel = _settingsService.GetDefaultAntialiasingLevel();
        _isInitializing = false;
    }

    /// <summary>
    /// Resets all settings to their default values.
    /// </summary>
    [RelayCommand]
    private void ResetToDefaults()
    {
        SelectedTheme = "System";
        DefaultPalette = "Classic";
        ShowAxesByDefault = true;
        UseSmoothColoringByDefault = false;
        DefaultAntialiasingLevel = "None";
    }
}
