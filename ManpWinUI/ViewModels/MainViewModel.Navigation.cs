using CommunityToolkit.Mvvm.Input;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Navigation and view manipulation commands.
/// Handles zoom, pan, reset, and resolution preset commands.
/// </summary>
public partial class MainViewModel
{
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

        // Auto-render after reset
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderCommand.CanExecute(null))
        {
            await RenderCommand.ExecuteAsync(null);
        }
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

        // Auto-render after zoom
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderCommand.CanExecute(null))
        {
            await RenderCommand.ExecuteAsync(null);
        }
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

        // Auto-render after zoom
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderCommand.CanExecute(null))
        {
            await RenderCommand.ExecuteAsync(null);
        }
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
            default:
                StatusMessage = "Unknown resolution preset";
                break;
        }

        // Notify that TotalPixels has changed
        OnPropertyChanged(nameof(TotalPixels));
        OnPropertyChanged(nameof(CurrentViewWidth));
        OnPropertyChanged(nameof(CurrentViewHeight));
    }
}
