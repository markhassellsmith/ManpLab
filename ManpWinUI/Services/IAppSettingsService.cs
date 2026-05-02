namespace ManpWinUI.Services;

/// <summary>
/// Service for persisting application settings across sessions.
/// </summary>
public interface IAppSettingsService
{
    /// <summary>
    /// Gets the saved browser panel width, or null if not previously saved.
    /// </summary>
    double? GetBrowserPanelWidth();

    /// <summary>
    /// Saves the browser panel width.
    /// </summary>
    void SetBrowserPanelWidth(double width);

    /// <summary>
    /// Gets the saved properties panel width, or null if not previously saved.
    /// </summary>
    double? GetPropertiesPanelWidth();

    /// <summary>
    /// Saves the properties panel width.
    /// </summary>
    void SetPropertiesPanelWidth(double width);

    /// <summary>
    /// Gets whether the browser panel is visible.
    /// </summary>
    bool GetBrowserPanelVisible();

    /// <summary>
    /// Saves whether the browser panel is visible.
    /// </summary>
    void SetBrowserPanelVisible(bool visible);

    /// <summary>
    /// Gets whether the properties panel is visible.
    /// </summary>
    bool GetPropertiesPanelVisible();

    /// <summary>
    /// Saves whether the properties panel is visible.
    /// </summary>
    void SetPropertiesPanelVisible(bool visible);

    /// <summary>
    /// Gets the name of the last selected fractal, or null if none saved.
    /// Week 5 Task 8: Persist browser selection across app restarts.
    /// </summary>
    string? GetSelectedFractal();

    /// <summary>
    /// Saves the name of the currently selected fractal.
    /// Week 5 Task 8: Persist browser selection across app restarts.
    /// </summary>
    void SetSelectedFractal(string fractalName);

    /// <summary>
    /// Gets saved parameter values for a specific fractal, or null if none saved.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    /// <param name="fractalName">Name of the fractal</param>
    /// <returns>JSON string of saved parameters, or null</returns>
    string? GetFractalParameters(string fractalName);

    /// <summary>
    /// Saves parameter values for a specific fractal.
    /// Week 6 Task 6: Persist parameter values per fractal type.
    /// </summary>
    /// <param name="fractalName">Name of the fractal</param>
    /// <param name="parametersJson">JSON string of parameter values</param>
    void SetFractalParameters(string fractalName, string parametersJson);

    // ═══════════════════════════════════════════════════════════════════════════════
    // APPLICATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Gets the application theme preference (Light, Dark, or System).
    /// </summary>
    string GetTheme();

    /// <summary>
    /// Saves the application theme preference.
    /// </summary>
    void SetTheme(string theme);

    /// <summary>
    /// Gets the default color palette for new fractals.
    /// </summary>
    string GetDefaultPalette();

    /// <summary>
    /// Saves the default color palette.
    /// </summary>
    void SetDefaultPalette(string palette);

    /// <summary>
    /// Gets whether coordinate axes should be shown by default.
    /// </summary>
    bool GetShowAxesByDefault();

    /// <summary>
    /// Saves whether coordinate axes should be shown by default.
    /// </summary>
    void SetShowAxesByDefault(bool show);

    /// <summary>
    /// Gets whether smooth coloring should be enabled by default.
    /// </summary>
    bool GetUseSmoothColoringByDefault();

    /// <summary>
    /// Saves whether smooth coloring should be enabled by default.
    /// </summary>
    void SetUseSmoothColoringByDefault(bool use);

    /// <summary>
    /// Gets the default antialiasing level.
    /// </summary>
    string GetDefaultAntialiasingLevel();

    /// <summary>
    /// Saves the default antialiasing level.
    /// </summary>
    void SetDefaultAntialiasingLevel(string level);

    /// <summary>
    /// Gets the saved properties panel tab index, or null if not previously saved.
    /// Used to restore which tab was selected (Parameters, Colors, Render, Settings, etc.).
    /// </summary>
    int? GetPropertiesTabIndex();

    /// <summary>
    /// Saves the properties panel tab index.
    /// </summary>
    void SetPropertiesTabIndex(int index);
}
