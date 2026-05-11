using CommunityToolkit.Mvvm.ComponentModel;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Standard fractal parameters (Mandelbrot, Burning Ship, Tricorn, Phoenix).
/// Includes coordinate system, zoom, iterations, and Julia mode parameters.
/// </summary>
public partial class MainViewModel
{
    // Coordinate system parameters (used by all standard fractals)
    [ObservableProperty]
    public partial double CenterX { get; set; } = -0.5;

    [ObservableProperty]
    public partial double CenterY { get; set; } = 0.0;

    [ObservableProperty]
    public partial double Zoom { get; set; } = 1.0;

    // Iteration control
    [ObservableProperty]
    public partial int MaxIterations { get; set; } = 512;

    [ObservableProperty]
    public partial bool AutoScaleIterations { get; set; } = true;

    [ObservableProperty]
    public partial string IterationSuggestion { get; set; } = string.Empty;

    // Julia mode (applies to all standard fractal types)
    [ObservableProperty]
    public partial string SelectedIterationMode { get; set; } = "Standard";

    // Computed property: Julia mode is active when iteration mode is "Julia"
    public bool IsJuliaMode => SelectedIterationMode == "Julia";

    [ObservableProperty]
    public partial double JuliaCX { get; set; } = -0.7;

    [ObservableProperty]
    public partial double JuliaCY { get; set; } = 0.27015;

    // Computed properties for current view dimensions in fractal coordinates
    public string CurrentViewWidth
    {
        get
        {
            var width = 3.0 / Zoom;
            // Use scientific notation when width < 0.01
            return width >= 0.01 ? $"{width:F10}" : $"{width:E10}";
        }
    }

    public string CurrentViewHeight
    {
        get
        {
            var width = 3.0 / Zoom;
            var height = width * ((double)ImageHeight / ImageWidth);
            // Use scientific notation when height < 0.01
            return height >= 0.01 ? $"{height:F10}" : $"{height:E10}";
        }
    }

    // Deep zoom mode indicator
    // NOTE: This threshold must match the DEEP_ZOOM_THRESHOLD in MainViewModel.Commands.cs
    // Native code activates BigNumFlag when precision > DBL_DIG - 3 (typically > 12 decimal places)
    // which corresponds to zoom >= 1e10 (view width < 3e-10)
    public bool IsDeepZoomActive
    {
        get
        {
            // Deep zoom activates when zoom >= 1e10
            return Zoom >= 1e10;
        }
    }

    public string DeepZoomIndicator => IsDeepZoomActive ? " - Deep Zoom mode" : string.Empty;

    // Property change handlers
    partial void OnMaxIterationsChanged(int value)
    {
        // Clamp max iterations to reasonable range
        // Allow higher values for very deep zooms into nodules and mini-brots
        if (value < 50) MaxIterations = 50;
        if (value > 50000) MaxIterations = 50000;

        // Update iteration suggestion message when iterations change
        UpdateIterationSuggestion();

        // TASK 5: Sync to parameter system
        if (UseParameterSystem && CurrentParameters != null)
        {
            SetParameter("max_iterations", value);
        }
    }

    partial void OnAutoScaleIterationsChanged(bool value)
    {
        // Update iteration suggestion message when auto-scale toggle changes
        UpdateIterationSuggestion();
    }

    partial void OnSelectedIterationModeChanged(string value)
    {
        // Notify that IsJuliaMode computed property has changed
        OnPropertyChanged(nameof(IsJuliaMode));

        // Update status message to reflect the mode change
        if (value == "Julia")
        {
            StatusMessage = $"Julia Mode: c = ({JuliaCX:F4}, {JuliaCY:F4}) - Click Render to generate";
        }
        else
        {
            StatusMessage = "Standard Mode - Click Render to generate";
        }

        // Ensure render command updates its CanExecute state
        RenderMandelbrotCommand.NotifyCanExecuteChanged();

        // TASK 5: Sync to parameter system
        if (UseParameterSystem && CurrentParameters != null)
        {
            SetParameter("julia_mode", value == "Julia");
        }
    }

    partial void OnZoomChanged(double value)
    {
        // Prevent zoom from going negative or too small
        if (value < 0.001) Zoom = 0.001;

        // Update computed view dimensions and deep zoom indicator
        OnPropertyChanged(nameof(CurrentViewWidth));
        OnPropertyChanged(nameof(CurrentViewHeight));
        OnPropertyChanged(nameof(IsDeepZoomActive));
        OnPropertyChanged(nameof(DeepZoomIndicator));

        // Update iteration suggestion when zoom changes (recommended iterations depend on zoom)
        UpdateIterationSuggestion();

        System.Diagnostics.Debug.WriteLine($"[ViewModel] Zoom changed to: {value:F10}");

        // TASK 5: Sync to parameter system
        if (UseParameterSystem && CurrentParameters != null)
        {
            SetParameter("zoom", value);
        }
    }

    partial void OnCenterXChanged(double value)
    {
        System.Diagnostics.Debug.WriteLine($"[ViewModel] CenterX changed to: {value:F10}");

        // TASK 5: Sync to parameter system
        if (UseParameterSystem && CurrentParameters != null)
        {
            SetParameter("center_x", value);
        }
    }

    partial void OnCenterYChanged(double value)
    {
        System.Diagnostics.Debug.WriteLine($"[ViewModel] CenterY changed to: {value:F10}");

        // TASK 5: Sync to parameter system
        if (UseParameterSystem && CurrentParameters != null)
        {
            SetParameter("center_y", value);
        }
    }

    partial void OnImageWidthChanged(int value)
    {
        OnPropertyChanged(nameof(TotalPixels));
        OnPropertyChanged(nameof(CurrentViewHeight));
    }

    partial void OnImageHeightChanged(int value)
    {
        OnPropertyChanged(nameof(TotalPixels));
        OnPropertyChanged(nameof(CurrentViewHeight));
    }

    /// <summary>
    /// Calculates recommended iteration count based on zoom level.
    /// Uses logarithmic scaling: more zoom requires exponentially more iterations.
    /// Based on fractal depth complexity - deeper zooms need far more iterations.
    /// </summary>
    private static int CalculateRecommendedIterations(double zoom)
    {
        // Base iterations for zoom level 1.0
        const int baseIterations = 512;

        // More aggressive scaling for deep zooms
        // Every 10x zoom needs roughly 2x more iterations (empirically determined)
        // This ensures detail visibility even at nodules and mini-brots
        var logZoom = Math.Log10(Math.Max(zoom, 1.0));
        var scaleFactor = Math.Pow(2.0, logZoom);

        var recommended = (int)(baseIterations * scaleFactor);

        // Round to nearest 128 for cleaner numbers
        recommended = ((recommended + 63) / 128) * 128;

        // Clamp to reasonable range (allow up to 20000 for very deep zooms)
        return Math.Clamp(recommended, 512, 20000);
    }

    /// <summary>
    /// Updates the iteration suggestion message based on current zoom and max iterations.
    /// Called when zoom or max iterations change to keep the message current.
    /// </summary>
    private void UpdateIterationSuggestion()
    {
        var recommendedIterations = CalculateRecommendedIterations(Zoom);

        if (!AutoScaleIterations)
        {
            // Manual mode: Show warning if current iterations are below recommended
            if (MaxIterations < recommendedIterations)
            {
                IterationSuggestion = $"⚠️ Consider increasing iterations to ~{recommendedIterations} for better detail at zoom {Zoom:F2}x (currently {MaxIterations})";
            }
            else
            {
                // User has set iterations at or above recommendation - clear the message
                IterationSuggestion = string.Empty;
            }
        }
        else
        {
            // Auto-scale mode: Just show current status
            IterationSuggestion = $"Using {MaxIterations} iterations at zoom {Zoom:F2}x";
        }
    }
}
