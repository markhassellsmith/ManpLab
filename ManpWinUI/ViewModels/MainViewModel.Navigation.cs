using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Models;
using System;
using System.Collections.ObjectModel;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Navigation and view manipulation commands.
/// Handles zoom, pan, reset, resolution presets, and navigation history (undo/redo).
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // NAVIGATION HISTORY
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Collection of navigation history entries displayed in the UI.
    /// </summary>
    public ObservableCollection<NavigationHistoryEntry> NavigationHistory { get; } = new();

    /// <summary>
    /// Gets whether undo (go back) is available.
    /// </summary>
    public bool CanUndo => _navigationHistoryService.CanUndo;

    /// <summary>
    /// Gets whether redo (go forward) is available.
    /// </summary>
    public bool CanRedo => _navigationHistoryService.CanRedo;

    /// <summary>
    /// Flag to prevent recording history during restore operations.
    /// </summary>
    private bool _isRestoringFromHistory = false;

    /// <summary>
    /// Refreshes the navigation history collection from the service.
    /// </summary>
    private void RefreshNavigationHistory()
    {
        NavigationHistory.Clear();
        foreach (var entry in _navigationHistoryService.History)
        {
            NavigationHistory.Add(entry);
        }

        // Notify that CanUndo/CanRedo may have changed
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));

        // Notify commands to re-evaluate their CanExecute state
        UndoNavigationCommand.NotifyCanExecuteChanged();
        RedoNavigationCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Records current navigation state to history.
    /// Call this after user-initiated zoom, pan, or parameter changes.
    /// </summary>
    private void RecordNavigationState(string? customDescription = null)
    {
        // Don't record if we're currently restoring from history
        if (_isRestoringFromHistory)
            return;

        // Don't record for Hailstone mode (uses different navigation model)
        if (IsHailstoneMode)
            return;

        var entry = NavigationHistoryEntry.FromCurrentState(
            fractalType: SelectedFractalType,
            iterationMode: SelectedIterationMode,
            centerX: CenterX,
            centerY: CenterY,
            zoom: Zoom,
            maxIterations: MaxIterations,
            colorPalette: SelectedPalette,
            juliaCX: IsJuliaMode ? JuliaCX : null,
            juliaCY: IsJuliaMode ? JuliaCY : null,
            customDescription: customDescription
        );

        _navigationHistoryService.RecordState(entry);
        RefreshNavigationHistory();
    }

    /// <summary>
    /// Restores a navigation state from history.
    /// </summary>
    private async Task RestoreNavigationStateAsync(NavigationHistoryEntry? entry)
    {
        if (entry == null)
            return;

        _isRestoringFromHistory = true;

        try
        {
            // Restore all parameters
            SelectedFractalType = entry.FractalType;
            SelectedIterationMode = entry.IterationMode;
            CenterX = entry.CenterX;
            CenterY = entry.CenterY;
            Zoom = entry.Zoom;
            MaxIterations = entry.MaxIterations;
            SelectedPalette = entry.ColorPalette;

            if (entry.JuliaC != null)
            {
                JuliaCX = entry.JuliaC.Real;
                JuliaCY = entry.JuliaC.Imaginary;
            }

            StatusMessage = $"Navigated to: {entry.Description}";

            // Auto-render on UI thread
            await Task.Delay(10);
            _dispatcherQueue.TryEnqueue(async () =>
            {
                if (!IsRendering && RenderCommand.CanExecute(null))
                {
                    await RenderCommand.ExecuteAsync(null);
                }
            });
        }
        finally
        {
            _isRestoringFromHistory = false;
        }

        RefreshNavigationHistory();
    }

    /// <summary>
    /// Undo navigation (go back in history).
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private async Task UndoNavigationAsync()
    {
        var entry = _navigationHistoryService.Undo();
        await RestoreNavigationStateAsync(entry);
    }

    /// <summary>
    /// Redo navigation (go forward in history).
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    private async Task RedoNavigationAsync()
    {
        var entry = _navigationHistoryService.Redo();
        await RestoreNavigationStateAsync(entry);
    }

    /// <summary>
    /// Jumps to a specific position in navigation history.
    /// </summary>
    [RelayCommand]
    private async Task JumpToHistoryAsync(int index)
    {
        var entry = _navigationHistoryService.JumpTo(index);
        await RestoreNavigationStateAsync(entry);
    }

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    [RelayCommand]
    private void ClearNavigationHistory()
    {
        _navigationHistoryService.Clear();
        RefreshNavigationHistory();
        StatusMessage = "Navigation history cleared";
    }
    // ═══════════════════════════════════════════════════════════════════════════════
    // VIEW RESET
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Resets view to default parameters based on current fractal type.
    /// </summary>
    [RelayCommand]
    private async Task ResetViewAsync()
    {
        if (IsHailstoneMode)
        {
            // Reset Hailstone parameters to default starting point
            HailstoneStartX = -10;
            HailstoneStartY = 6;
            HailstoneMaxIterations = 150;
            UseFixedHailstoneViewport = false; // Use auto-scale
            ResetHailstoneViewport(); // Clear any custom viewport
            StatusMessage = "Resetting to default Hailstone view...";
        }
        else
        {
            // Reset standard fractal parameters to default Mandelbrot view
            CenterX = -0.5;
            CenterY = 0.0;
            Zoom = 1.0;
            MaxIterations = 512;
            StatusMessage = "Resetting to full Mandelbrot view...";
        }

        // Auto-render after reset on UI thread
        await Task.Delay(10); // Small delay to ensure UI updates
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (!IsRendering && RenderCommand.CanExecute(null))
            {
                await RenderCommand.ExecuteAsync(null);
            }
        });
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ZOOM COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Zooms in on the current center point (for standard fractals only).
    /// Hailstone doesn't support zoom - it's determined by the sequence itself.
    /// </summary>
    [RelayCommand]
    private async Task ZoomInAsync()
    {
        if (IsHailstoneMode)
        {
            StatusMessage = "Zoom not applicable to Hailstone sequences - adjust starting point or iterations instead";
            return;
        }

        Zoom *= 2.0;
        StatusMessage = $"Zooming in to {Zoom:F2}x...";

        // Auto-render after zoom on UI thread
        await Task.Delay(10); // Small delay to ensure UI updates
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (!IsRendering && RenderCommand.CanExecute(null))
            {
                await RenderCommand.ExecuteAsync(null);
            }
        });
        // Note: RecordNavigationState() is called by render completion
    }

    /// <summary>
    /// Zooms out from the current center point (for standard fractals only).
    /// Hailstone doesn't support zoom - it's determined by the sequence itself.
    /// </summary>
    [RelayCommand]
    private async Task ZoomOutAsync()
    {
        if (IsHailstoneMode)
        {
            StatusMessage = "Zoom not applicable to Hailstone sequences - adjust starting point or iterations instead";
            return;
        }

        Zoom /= 2.0;
        StatusMessage = $"Zooming out to {Zoom:F2}x...";

        // Auto-render after zoom on UI thread
        await Task.Delay(10); // Small delay to ensure UI updates
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (!IsRendering && RenderCommand.CanExecute(null))
            {
                await RenderCommand.ExecuteAsync(null);
            }
        });
        // Note: RecordNavigationState() is called by render completion
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ZOOM FINE-TUNING
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Fine-tune adjustment for zoom (range: -0.5 to +0.5 zoom steps).
    /// Slider value of 0 = no adjustment (center position)
    /// Slider value of -0.5 = zoom in by sqrt(2) ≈ 1.414x (left - increases zoom)
    /// Slider value of +0.5 = zoom out by sqrt(2) ≈ 0.707x (right - decreases zoom)
    /// </summary>
    [ObservableProperty]
    private double _zoomFineTune = 0.0;

    /// <summary>
    /// Flag to suppress immediate rendering during slider drag.
    /// When true, the slider value changes update the zoom but don't trigger render.
    /// </summary>
    public bool IsZoomSliderDragging { get; set; }

    partial void OnZoomFineTuneChanged(double value)
    {
        if (IsHailstoneMode || IsRendering)
        {
            // Reset slider to center if not applicable
            if (value != 0.0)
            {
                ZoomFineTune = 0.0;
            }
            return;
        }

        // Only apply adjustment if value is non-zero
        if (Math.Abs(value) > 0.001)
        {
            // Negate the value so that:
            // - Negative slider value (left) becomes positive exponent = zoom in (increase zoom)
            // - Positive slider value (right) becomes negative exponent = zoom out (decrease zoom)
            // At value = -0.5, factor = 2^0.5 = sqrt(2) ≈ 1.414 (zoom in - increases zoom factor)
            // At value = +0.5, factor = 2^-0.5 = 1/sqrt(2) ≈ 0.707 (zoom out - decreases zoom factor)
            double adjustmentFactor = Math.Pow(2.0, -value);

            // Apply the adjustment
            Zoom *= adjustmentFactor;

            StatusMessage = value < 0 
                ? $"Fine zoom in to {Zoom:F2}x..." 
                : $"Fine zoom out to {Zoom:F2}x...";

            // Reset slider to center for next adjustment
            ZoomFineTune = 0.0;

            // Only auto-render if NOT currently dragging the slider
            if (!IsZoomSliderDragging)
            {
                // Auto-render after adjustment on UI thread
                _dispatcherQueue.TryEnqueue(async () =>
                {
                    await Task.Delay(10); // Small delay to ensure UI updates
                    if (!IsRendering && RenderCommand.CanExecute(null))
                    {
                        await RenderCommand.ExecuteAsync(null);
                    }
                });
            }
            // Note: RecordNavigationState() is called by render completion
        }
    }

    /// <summary>
    /// Called when the zoom fine-tune slider drag is completed.
    /// Triggers the render with the accumulated zoom changes.
    /// </summary>
    public async Task ApplyZoomFineTuneAsync()
    {
        if (IsHailstoneMode || IsRendering)
        {
            return;
        }

        // Render with the current zoom value
        _dispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Delay(10); // Small delay to ensure UI updates
            if (!IsRendering && RenderCommand.CanExecute(null))
            {
                await RenderCommand.ExecuteAsync(null);
            }
        });
    }

    /// <summary>
    /// Applies a discrete zoom step adjustment.
    /// Step is 1/10th of the full range (-0.5 to +0.5), so each click is 0.1 zoom step.
    /// Zoom in (left) increases zoom factor, zoom out (right) decreases it.
    /// </summary>
    [RelayCommand]
    private async Task ZoomFineTuneStepAsync(string direction)
    {
        if (IsHailstoneMode || IsRendering)
        {
            return;
        }

        // Each discrete step is 0.1 (1/10th of the full -0.5 to +0.5 range)
        double stepSize = 0.1;
        // Zoom IN increases zoom value (positive exponent), zoom OUT decreases it (negative exponent)
        double adjustmentValue = direction == "in" ? stepSize : -stepSize;

        // Calculate adjustment factor: 2^value
        double adjustmentFactor = Math.Pow(2.0, adjustmentValue);

        // Apply the adjustment
        Zoom *= adjustmentFactor;

        StatusMessage = direction == "in"
            ? $"Fine zoom in to {Zoom:F2}x..."
            : $"Fine zoom out to {Zoom:F2}x...";

        // Auto-render after adjustment on UI thread
        await Task.Delay(10); // Small delay to ensure UI updates
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (!IsRendering && RenderCommand.CanExecute(null))
            {
                await RenderCommand.ExecuteAsync(null);
            }
        });
        // Note: RecordNavigationState() is called by render completion
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // RESOLUTION PRESETS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Sets image resolution to a preset value.
    /// </summary>
    [RelayCommand]
    private void SetResolution(string preset)
    {
        switch (preset)
        {
            case "HD":
                ImageWidth = 1280;
                ImageHeight = 720;
                StatusMessage = "Resolution set to HD (1280×720)";
                break;
            case "FullHD":
                ImageWidth = 1920;
                ImageHeight = 1080;
                StatusMessage = "Resolution set to Full HD (1920×1080)";
                break;
            case "2K":
                ImageWidth = 2560;
                ImageHeight = 1440;
                StatusMessage = "Resolution set to 2K (2560×1440)";
                break;
            case "4K":
                ImageWidth = 3840;
                ImageHeight = 2160;
                StatusMessage = "Resolution set to 4K (3840×2160)";
                break;
            case "4K+":
                ImageWidth = 4096;
                ImageHeight = 2160;
                StatusMessage = "Resolution set to 4K+ (4096×2160)";
                break;
            default:
                StatusMessage = "Unknown resolution preset";
                break;
        }

        // Notify that computed properties have changed
        OnPropertyChanged(nameof(TotalPixels));
        OnPropertyChanged(nameof(CurrentViewWidth));
        OnPropertyChanged(nameof(CurrentViewHeight));
        OnPropertyChanged(nameof(IsHDResolution));
        OnPropertyChanged(nameof(IsFullHDResolution));
        OnPropertyChanged(nameof(Is2KResolution));
        OnPropertyChanged(nameof(Is4KResolution));
        OnPropertyChanged(nameof(Is4KPlusResolution));
    }
}
