using CommunityToolkit.Mvvm.ComponentModel;
using ManpCore.Services.Models;
using ManpWinUI.Models;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Hailstone sequence parameters.
/// Discrete dynamical system on integer lattice (ℤ × ℤ).
/// </summary>
public partial class MainViewModel
{
    // Hailstone sequence parameters
    [ObservableProperty]
    public partial int HailstoneStartX { get; set; } = -10;

    [ObservableProperty]
    public partial int HailstoneStartY { get; set; } = 6;

    [ObservableProperty]
    public partial int HailstoneMaxIterations { get; set; } = 150;

    [ObservableProperty]
    public partial bool ShowHailstoneAxes { get; set; } = true;

    partial void OnShowHailstoneAxesChanged(bool value)
    {
        // Auto-rerender when display option changes
        if (IsHailstoneMode && CurrentHailstoneResult != null && !IsRendering)
        {
            _ = RenderHailstoneCommand.ExecuteAsync(null);
        }
    }

    [ObservableProperty]
    public partial bool ShowHailstonePoints { get; set; } = true;

    partial void OnShowHailstonePointsChanged(bool value)
    {
        // Auto-rerender when display option changes
        if (IsHailstoneMode && CurrentHailstoneResult != null && !IsRendering)
        {
            _ = RenderHailstoneCommand.ExecuteAsync(null);
        }
    }

    [ObservableProperty]
    public partial bool ShowHailstoneLabels { get; set; } = true;

    partial void OnShowHailstoneLabelsChanged(bool value)
    {
        // Labels are overlay only, just update the overlay (handled by property change notification)
    }

    [ObservableProperty]
    public partial bool UseFixedHailstoneViewport { get; set; } = false;  // Default to auto-scale

    partial void OnUseFixedHailstoneViewportChanged(bool value)
    {
        // Auto-rerender when viewport mode changes
        if (IsHailstoneMode && CurrentHailstoneResult != null && !IsRendering)
        {
            _ = RenderHailstoneCommand.ExecuteAsync(null);
        }
    }

    // Current Hailstone sequence result (for label overlay)
    [ObservableProperty]
    public partial HailstoneResult? CurrentHailstoneResult { get; set; }

    // Current transform parameters (for label positioning)
    [ObservableProperty]
    public partial double HailstoneScaleX { get; set; }

    [ObservableProperty]
    public partial double HailstoneScaleY { get; set; }

    [ObservableProperty]
    public partial double HailstoneOffsetX { get; set; }

    [ObservableProperty]
    public partial double HailstoneOffsetY { get; set; }

    // Viewport manipulation for interactive zoom/pan
    [ObservableProperty]
    public partial double? HailstoneViewportMinX { get; set; }

    [ObservableProperty]
    public partial double? HailstoneViewportMaxX { get; set; }

    [ObservableProperty]
    public partial double? HailstoneViewportMinY { get; set; }

    [ObservableProperty]
    public partial double? HailstoneViewportMaxY { get; set; }

    // Computed property: whether a custom viewport is active
    public bool HasCustomHailstoneViewport => 
        HailstoneViewportMinX.HasValue && 
        HailstoneViewportMaxX.HasValue && 
        HailstoneViewportMinY.HasValue && 
        HailstoneViewportMaxY.HasValue;

    /// <summary>
    /// Resets the Hailstone viewport to auto-scale (clears custom bounds).
    /// </summary>
    public void ResetHailstoneViewport()
    {
        HailstoneViewportMinX = null;
        HailstoneViewportMaxX = null;
        HailstoneViewportMinY = null;
        HailstoneViewportMaxY = null;
    }

    /// <summary>
    /// Sets a custom Hailstone viewport (for interactive zoom/pan).
    /// </summary>
    public void SetHailstoneViewport(double minX, double maxX, double minY, double maxY)
    {
        HailstoneViewportMinX = minX;
        HailstoneViewportMaxX = maxX;
        HailstoneViewportMinY = minY;
        HailstoneViewportMaxY = maxY;
    }

    /// <summary>
    /// Flag to indicate if we should use 2-D trajectory rendering (HailstoneRenderService)
    /// instead of standard fractal rendering.
    /// Set by MainPage when "Hailstone2D" is selected from browser.
    /// </summary>
    [ObservableProperty]
    private bool _useHailstoneTrajectoryMode;

    // Computed property: Hailstone mode is active when trajectory mode is explicitly enabled
    // (for "Hailstone2D" selections, not the original "Hailstone" fractal)
    public bool IsHailstoneMode => UseHailstoneTrajectoryMode;

    // Computed property: Show coordinate axes only for Mandelbrot/Julia, not for Hailstone trajectory
    public bool ShowMandelbrotAxes => ShowCoordinateAxes && !IsHailstoneMode;
}