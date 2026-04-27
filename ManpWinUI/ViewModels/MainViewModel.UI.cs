using CommunityToolkit.Mvvm.ComponentModel;

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
}
