using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using ManpWinUI.Services;
using ManpWinUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ManpWinUI.ViewModels;

/// <summary>
/// Main view model for the fractal explorer interface.
/// Manages fractal rendering parameters, UI state, and coordinates with the fractal engine.
/// </summary>
public partial class MainViewModel(IFractalRenderService renderService, BookmarkService bookmarkService) : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IFractalRenderService _renderService = renderService;
    private readonly BookmarkService _bookmarkService = bookmarkService;

    // Fractal rendering parameters
    [ObservableProperty]
    public partial double CenterX { get; set; } = -0.5;

    [ObservableProperty]
    public partial double CenterY { get; set; } = 0.0;

    [ObservableProperty]
    public partial double Zoom { get; set; } = 1.0;

    [ObservableProperty]
    public partial int MaxIterations { get; set; } = 512;

    [ObservableProperty]
    public partial bool AutoScaleIterations { get; set; } = true;

    [ObservableProperty]
    public partial string IterationSuggestion { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int ImageWidth { get; set; } = 1200;

    [ObservableProperty]
    public partial int ImageHeight { get; set; } = 900;

    [ObservableProperty]
    public partial string SelectedPalette { get; set; } = "Classic";

    // Fractal type selection
    [ObservableProperty]
    public partial string SelectedFractalType { get; set; } = "Mandelbrot";

    // Iteration mode selection (Standard/Julia)
    [ObservableProperty]
    public partial string SelectedIterationMode { get; set; } = "Standard";

    // Computed property: Julia mode is active when iteration mode is "Julia"
    public bool IsJuliaMode => SelectedIterationMode == "Julia";

    [ObservableProperty]
    public partial double JuliaCX { get; set; } = -0.7;

    [ObservableProperty]
    public partial double JuliaCY { get; set; } = 0.27015;

    // Computed property for total megapixels
    public string TotalPixels => $"{(ImageWidth * ImageHeight / 1_000_000.0):F2}";

    // Computed properties for current view dimensions in fractal coordinates
    public string CurrentViewWidth
    {
        get
        {
            var width = 3.0 / Zoom;
            return $"{width:F10}";
        }
    }

    public string CurrentViewHeight
    {
        get
        {
            var width = 3.0 / Zoom;
            var height = width * ((double)ImageHeight / ImageWidth);
            return $"{height:F10}";
        }
    }

    // UI state
    [ObservableProperty]
    public partial bool IsRendering { get; set; }

    [ObservableProperty]
    public partial double RenderProgress { get; set; }

    [ObservableProperty]
    public partial bool ShowCoordinateAxes { get; set; } = true;

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Ready";

    [ObservableProperty]
    public partial TimeSpan LastRenderTime { get; set; }

    // Fractal image
    [ObservableProperty]
    public partial WriteableBitmap? FractalImage { get; set; }

    // Bookmarks
    public ObservableCollection<FractalBookmark> Bookmarks { get; } = new();

    [ObservableProperty]
    public partial FractalBookmark? SelectedBookmark { get; set; }

    [ObservableProperty]
    public partial bool IsBookmarksPanelOpen { get; set; }

    /// <summary>
    /// Initializes bookmarks from storage.
    /// Call this after construction to load saved bookmarks.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _bookmarkService.LoadBookmarksAsync();
        RefreshBookmarks();
    }

    /// <summary>
    /// Refreshes the bookmarks collection from the service.
    /// </summary>
    private void RefreshBookmarks()
    {
        Bookmarks.Clear();
        foreach (var bookmark in _bookmarkService.Bookmarks)
        {
            Bookmarks.Add(bookmark);
        }
    }

    /// <summary>
    /// Renders the Mandelbrot set with current parameters.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderMandelbrotAsync()
    {
        IsRendering = true;
        RenderProgress = 0;
        var fractalName = IsJuliaMode ? $"{SelectedFractalType} Julia" : SelectedFractalType;
        StatusMessage = IsJuliaMode 
            ? $"Rendering {fractalName} set (c = {JuliaCX:F4}, {JuliaCY:F4})..." 
            : $"Rendering {fractalName} set...";

        // Auto-scale iterations based on zoom if enabled
        if (AutoScaleIterations)
        {
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                var oldIterations = MaxIterations;
                MaxIterations = recommendedIterations;
                IterationSuggestion = $"Auto-increased iterations: {oldIterations} → {MaxIterations} for zoom {Zoom:F2}x";
            }
            else
            {
                IterationSuggestion = $"Using {MaxIterations} iterations at zoom {Zoom:F2}x";
            }
        }
        else
        {
            // Check if user might need more iterations
            var recommendedIterations = CalculateRecommendedIterations(Zoom);
            if (MaxIterations < recommendedIterations)
            {
                IterationSuggestion = $"⚠️ Consider increasing iterations to ~{recommendedIterations} for better detail at zoom {Zoom:F2}x (currently {MaxIterations})";
            }
            else
            {
                IterationSuggestion = string.Empty;
            }
        }

        try
        {
            var startTime = DateTime.Now;

            // Progress reporting callback
            var progress = new Progress<double>(percentage =>
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RenderProgress = percentage * 100.0;
                });
            });

            // Call FractalRenderService to render the fractal
            var result = await _renderService.RenderMandelbrotAsync(
                CenterX,
                CenterY,
                Zoom,
                ImageWidth,
                ImageHeight,
                MaxIterations,
                SelectedPalette,
                SelectedFractalType,
                IsJuliaMode,
                JuliaCX,
                JuliaCY,
                progress);

            // Convert byte[] to WriteableBitmap on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                ConvertPixelDataToBitmap(result.PixelData, ImageWidth, ImageHeight);
            });

            LastRenderTime = DateTime.Now - startTime;

            // Show diagnostic info if escape percentage is very low
            var escapePercent = result.EscapePercentage;

            // DETAILED DIAGNOSTIC LOGGING
            var logMessage = $@"
───────────────────────────────────────────────────────────────
RENDER COMPLETE - DIAGNOSTIC INFO
───────────────────────────────────────────────────────────────
Resolution: {ImageWidth} × {ImageHeight} ({result.TotalIterations:N0} total iterations)
Render time: {LastRenderTime.TotalMilliseconds:F0} ms
Escape stats: {result.EscapedPixels:N0} / {ImageWidth * ImageHeight:N0} pixels ({escapePercent:F2}%)
Max iterations: {MaxIterations}
Zoom: {Zoom:F4}x
Center: ({CenterX:F10}, {CenterY:F10})
View dimensions: {3.0 / Zoom:F10} × {(3.0 / Zoom) * ((double)ImageHeight / ImageWidth):F10}
───────────────────────────────────────────────────────────────
";
            System.Diagnostics.Debug.WriteLine(logMessage);

            if (escapePercent < 1.0)
            {
                StatusMessage = $"⚠️ Only {escapePercent:F2}% of pixels escaped - You're inside the Mandelbrot set! Zoom to the boundary for detail.";
            }
            else if (escapePercent < 10.0)
            {
                StatusMessage = $"Low detail: {escapePercent:F1}% escaped - Try zooming to colorful boundaries";
            }
            else
            {
                StatusMessage = $"Rendered in {LastRenderTime.TotalMilliseconds:F0} ms ({escapePercent:F1}% escaped)";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsRendering = false;
        }
    }

    private bool CanRender() => !IsRendering;

    /// <summary>
    /// Resets view to default Mandelbrot parameters.
    /// </summary>
    [RelayCommand]
    private async Task ResetViewAsync()
    {
        CenterX = -0.5;
        CenterY = 0.0;
        Zoom = 1.0;
        MaxIterations = 512;
        StatusMessage = "Resetting to full Mandelbrot view...";

        // Auto-render after reset
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderMandelbrotCommand.CanExecute(null))
        {
            await RenderMandelbrotCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// Zooms in on the current center point.
    /// </summary>
    [RelayCommand]
    private async Task ZoomInAsync()
    {
        Zoom *= 2.0;
        StatusMessage = $"Zooming in to {Zoom:F2}x...";

        // Auto-render after zoom
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderMandelbrotCommand.CanExecute(null))
        {
            await RenderMandelbrotCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// Zooms out from the current center point.
    /// </summary>
    [RelayCommand]
    private async Task ZoomOutAsync()
    {
        Zoom /= 2.0;
        StatusMessage = $"Zooming out to {Zoom:F2}x...";

        // Auto-render after zoom
        await Task.Delay(10); // Small delay to ensure UI updates
        if (RenderMandelbrotCommand.CanExecute(null))
        {
            await RenderMandelbrotCommand.ExecuteAsync(null);
        }
    }

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
    }

    partial void OnMaxIterationsChanged(int value)
    {
        // Clamp max iterations to reasonable range
        // Allow higher values for very deep zooms into nodules and mini-brots
        if (value < 50) MaxIterations = 50;
        if (value > 50000) MaxIterations = 50000;
    }

    partial void OnSelectedFractalTypeChanged(string value)
    {
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
        }
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

    partial void OnZoomChanged(double value)
    {
        // Prevent zoom from going negative or too small
        if (value < 0.001) Zoom = 0.001;

        // Update computed view dimensions
        OnPropertyChanged(nameof(CurrentViewWidth));
        OnPropertyChanged(nameof(CurrentViewHeight));

        System.Diagnostics.Debug.WriteLine($"[ViewModel] Zoom changed to: {value:F10}");
    }

    partial void OnCenterXChanged(double value)
    {
        System.Diagnostics.Debug.WriteLine($"[ViewModel] CenterX changed to: {value:F10}");
    }

    partial void OnCenterYChanged(double value)
    {
        System.Diagnostics.Debug.WriteLine($"[ViewModel] CenterY changed to: {value:F10}");
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
    /// Converts pixel data byte array to WriteableBitmap for display.
    /// PixelData format is BGRA (Blue, Green, Red, Alpha - 4 bytes per pixel).
    /// This is the native format for WinUI WriteableBitmap.
    /// </summary>
    private void ConvertPixelDataToBitmap(byte[] pixelData, int width, int height)
    {
        // Create or reuse WriteableBitmap
        if (FractalImage == null || FractalImage.PixelWidth != width || FractalImage.PixelHeight != height)
        {
            FractalImage = new WriteableBitmap(width, height);
        }

        // Write pixel data to bitmap
        using (var stream = FractalImage.PixelBuffer.AsStream())
        {
            stream.Write(pixelData, 0, pixelData.Length);
        }

        // Invalidate the bitmap to trigger redraw
        FractalImage.Invalidate();
    }

    /// <summary>
    /// Creates metadata object from current fractal state.
    /// </summary>
    public Models.FractalMetadata CreateMetadata()
    {
        return Models.FractalMetadata.FromViewModel(
            fractalType: SelectedFractalType,
            iterationMode: SelectedIterationMode,
            centerX: CenterX,
            centerY: CenterY,
            zoom: Zoom,
            maxIterations: MaxIterations,
            colorPalette: SelectedPalette,
            imageWidth: ImageWidth,
            imageHeight: ImageHeight,
            autoScaleIterations: AutoScaleIterations,
            juliaCX: IsJuliaMode ? JuliaCX : null,
            juliaCY: IsJuliaMode ? JuliaCY : null,
            renderTime: LastRenderTime
        );
    }

    /// <summary>
    /// Loads a bookmark and navigates to that fractal location.
    /// </summary>
    [RelayCommand]
    private async Task LoadBookmarkAsync(FractalBookmark? bookmark)
    {
        if (bookmark == null)
            return;

        // Set all parameters from bookmark
        SelectedFractalType = bookmark.FractalType;
        SelectedIterationMode = bookmark.IterationMode;
        CenterX = bookmark.CenterX;
        CenterY = bookmark.CenterY;
        Zoom = bookmark.Zoom;
        MaxIterations = bookmark.MaxIterations;
        SelectedPalette = bookmark.ColorPalette;

        if (bookmark.JuliaC != null)
        {
            JuliaCX = bookmark.JuliaC.Real;
            JuliaCY = bookmark.JuliaC.Imaginary;
        }

        StatusMessage = $"Loaded bookmark: {bookmark.Name}";

        // Auto-render
        await Task.Delay(10);
        if (RenderMandelbrotCommand.CanExecute(null))
        {
            await RenderMandelbrotCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// Saves current view as a new bookmark.
    /// </summary>
    [RelayCommand]
    private async Task SaveCurrentAsBookmarkAsync(string? bookmarkName)
    {
        if (string.IsNullOrWhiteSpace(bookmarkName))
        {
            StatusMessage = "Please enter a bookmark name";
            return;
        }

        var bookmark = FractalBookmark.FromCurrentState(
            name: bookmarkName,
            description: $"Saved on {DateTime.Now:g}",
            fractalType: SelectedFractalType,
            iterationMode: SelectedIterationMode,
            centerX: CenterX,
            centerY: CenterY,
            zoom: Zoom,
            maxIterations: MaxIterations,
            colorPalette: SelectedPalette,
            juliaCX: IsJuliaMode ? JuliaCX : null,
            juliaCY: IsJuliaMode ? JuliaCY : null,
            isFavorite: false
        );

        await _bookmarkService.AddBookmarkAsync(bookmark);
        RefreshBookmarks();

        StatusMessage = $"Bookmark saved: {bookmarkName}";
    }

    /// <summary>
    /// Deletes a bookmark.
    /// </summary>
    [RelayCommand]
    private async Task DeleteBookmarkAsync(FractalBookmark? bookmark)
    {
        if (bookmark == null || bookmark.IsPreset)
            return;

        await _bookmarkService.RemoveBookmarkAsync(bookmark.Id);
        RefreshBookmarks();

        StatusMessage = $"Deleted bookmark: {bookmark.Name}";
    }

    /// <summary>
    /// Toggles favorite status of a bookmark.
    /// </summary>
    [RelayCommand]
    private async Task ToggleBookmarkFavoriteAsync(FractalBookmark? bookmark)
    {
        if (bookmark == null)
            return;

        await _bookmarkService.ToggleFavoriteAsync(bookmark.Id);
        RefreshBookmarks();
    }

    /// <summary>
    /// Toggles the bookmarks panel visibility.
    /// </summary>
    [RelayCommand]
    private void ToggleBookmarksPanel()
    {
        IsBookmarksPanelOpen = !IsBookmarksPanelOpen;
    }
}