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
}
