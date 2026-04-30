using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Services;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - UI state and visual settings.
/// Handles color palette selection, coordinate axes display, and status messages.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // COLOR PALETTE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Selected color palette for fractal rendering.
    /// </summary>
    [ObservableProperty]
    public partial string SelectedPalette { get; set; } = "Classic";

    /// <summary>
    /// Color cycle animation speed (0-100).
    /// Week 7 Task 3: Dynamic color palette rotation effect.
    /// </summary>
    [ObservableProperty]
    private int _colorCycleSpeed = 50;

    /// <summary>
    /// Color palette rotation offset in degrees (0-360).
    /// Week 7 Task 3: Shifts the color mapping for visual variety.
    /// </summary>
    [ObservableProperty]
    private int _colorOffset = 0;

    /// <summary>
    /// Enable smooth/continuous coloring instead of discrete color bands.
    /// Week 7 Task 3: Produces gradients without banding artifacts.
    /// </summary>
    [ObservableProperty]
    private bool _useSmoothColoring = false;

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL TYPE SELECTION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Currently selected fractal type (Mandelbrot, Hailstone, etc.).
    /// </summary>
    [ObservableProperty]
    public partial string SelectedFractalType { get; set; } = "Mandelbrot";

    // ═══════════════════════════════════════════════════════════════════════════════
    // COORDINATE AXES & VISUAL TOGGLES
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Whether to display coordinate axes overlay on fractal image.
    /// </summary>
    [ObservableProperty]
    public partial bool ShowCoordinateAxes { get; set; } = true;

    partial void OnShowCoordinateAxesChanged(bool value)
    {
        // Notify computed property that depends on this
        OnPropertyChanged(nameof(ShowMandelbrotAxes));
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // STATUS MESSAGES
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Status message displayed in the UI (render progress, warnings, errors).
    /// </summary>
    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Ready";

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL TYPE CHANGE HANDLER
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Handles fractal type changes - updates computed properties, clears stale data,
    /// and sets appropriate default parameters for each fractal type.
    /// </summary>
    partial void OnSelectedFractalTypeChanged(string value)
    {
        System.Diagnostics.Debug.WriteLine($"[OnSelectedFractalTypeChanged] Fractal type changed to: {value}");

        // Notify that computed properties have changed
        OnPropertyChanged(nameof(IsHailstoneMode));
        OnPropertyChanged(nameof(ShowMandelbrotAxes));

        System.Diagnostics.Debug.WriteLine($"[OnSelectedFractalTypeChanged] IsHailstoneMode is now: {IsHailstoneMode}");

        // Clear Hailstone-specific data when switching away from Hailstone mode
        if (value != "Hailstone")
        {
            CurrentHailstoneResult = null;
            HailstoneScaleX = 0;
            HailstoneScaleY = 0;
            HailstoneOffsetX = 0;
            HailstoneOffsetY = 0;
            ResetHailstoneViewport(); // Clear custom viewport
        }

        // Clear the current fractal image to avoid showing stale data from previous fractal type
        FractalImage = null;
        StatusMessage = $"Switched to {value} - Click Render to generate";

        // Set appropriate default view parameters for each fractal type
        switch (value)
        {
            case "Mandelbrot":
                if (!IsJuliaMode)
                {
                    CenterX = -0.5;
                    CenterY = 0.0;
                    Zoom = 1.0;
                }
                break;
            case "BurningShip":
                if (!IsJuliaMode)
                {
                    CenterX = -0.5;
                    CenterY = -0.5;
                    Zoom = 0.8;
                }
                break;
            case "Tricorn":
                if (!IsJuliaMode)
                {
                    CenterX = 0.0;
                    CenterY = 0.0;
                    Zoom = 0.8;
                }
                break;
            case "Phoenix":
                if (!IsJuliaMode)
                {
                    CenterX = 0.0;
                    CenterY = 0.0;
                    Zoom = 0.6;
                }
                break;
            case "Hailstone":
                // Set default Hailstone parameters (classic starting point)
                HailstoneStartX = -10;
                HailstoneStartY = 6;
                HailstoneMaxIterations = 150;
                StatusMessage = "Hailstone Mode - Click Render to generate sequence";
                break;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PANEL VISIBILITY & SIZING
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Indicates whether the browser panel is visible.
    /// </summary>
    [ObservableProperty]
    private bool isBrowserPanelVisible = true;

    partial void OnIsBrowserPanelVisibleChanged(bool value)
    {
        _settingsService.SetBrowserPanelVisible(value);
    }

    /// <summary>
    /// Indicates whether the properties panel is visible.
    /// </summary>
    [ObservableProperty]
    private bool isPropertiesPanelVisible = true;

    partial void OnIsPropertiesPanelVisibleChanged(bool value)
    {
        _settingsService.SetPropertiesPanelVisible(value);
    }

    /// <summary>
    /// Browser panel width in pixels.
    /// </summary>
    [ObservableProperty]
    private double browserPanelWidth = 250.0;

    partial void OnBrowserPanelWidthChanged(double value)
    {
        // Only save if panel is visible and width is reasonable
        if (IsBrowserPanelVisible && value >= 150 && value <= 600)
        {
            _settingsService.SetBrowserPanelWidth(value);
        }
    }

    /// <summary>
    /// Properties panel width in pixels.
    /// </summary>
    [ObservableProperty]
    private double propertiesPanelWidth = 300.0;

    partial void OnPropertiesPanelWidthChanged(double value)
    {
        // Only save if panel is visible and width is reasonable
        if (IsPropertiesPanelVisible && value >= 200 && value <= 800)
        {
            _settingsService.SetPropertiesPanelWidth(value);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PANEL COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Toggles the browser panel visibility (Ctrl+B).
    /// </summary>
    [RelayCommand]
    private void ToggleBrowserPanel()
    {
        IsBrowserPanelVisible = !IsBrowserPanelVisible;
    }

    /// <summary>
    /// Toggles the properties panel visibility (Ctrl+P).
    /// </summary>
    [RelayCommand]
    private void TogglePropertiesPanel()
    {
        IsPropertiesPanelVisible = !IsPropertiesPanelVisible;
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PANEL STATE INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Restores panel state from settings.
    /// Call this after constructor initialization.
    /// </summary>
    private void RestorePanelState()
    {
        // Restore visibility
        IsBrowserPanelVisible = _settingsService.GetBrowserPanelVisible();
        IsPropertiesPanelVisible = _settingsService.GetPropertiesPanelVisible();

        // Restore sizes (with fallback to defaults)
        BrowserPanelWidth = _settingsService.GetBrowserPanelWidth() ?? 250.0;
        PropertiesPanelWidth = _settingsService.GetPropertiesPanelWidth() ?? 300.0;
    }
}
