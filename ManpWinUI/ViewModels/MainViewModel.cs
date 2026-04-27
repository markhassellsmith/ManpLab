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
/// Main view model for the fractal explorer interface (Core).
/// Manages rendering state, image output, bookmarks, and coordinates with services.
/// 
/// Split into partial classes for maintainability:
/// - MainViewModel.cs (this file): Core state, rendering, bookmarks
/// - MainViewModel.StandardFractals.cs: Mandelbrot/Julia parameters
/// - MainViewModel.Hailstone.cs: Hailstone sequence parameters
/// </summary>
public partial class MainViewModel(
    IFractalRenderService renderService, 
    BookmarkService bookmarkService,
    IHailstoneService hailstoneService) : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IFractalRenderService _renderService = renderService;
    private readonly BookmarkService _bookmarkService = bookmarkService;
    private readonly IHailstoneService _hailstoneService = hailstoneService;
    private readonly HailstoneRenderServiceWin2D _hailstoneRenderService = new();

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDERING STATE & IMAGE OUTPUT
    // Moved to: MainViewModel.Rendering.cs
    // - ImageWidth, ImageHeight, TotalPixels
    // - IsRendering, RenderProgress, LastRenderTime
    // - FractalImage, ConvertPixelDataToBitmap()
    // ═══════════════════════════════════════════════════════════════════════════════

    // ═══════════════════════════════════════════════════════════════════════════════
    // UI STATE & VISUAL SETTINGS
    // Moved to: MainViewModel.UI.cs
    // - SelectedPalette
    // - SelectedFractalType
    // - ShowCoordinateAxes, OnShowCoordinateAxesChanged()
    // - StatusMessage
    // ═══════════════════════════════════════════════════════════════════════════════

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
                try
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        RenderProgress = percentage * 100.0;
                    });
                }
                catch (InvalidCastException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"InvalidCastException in Progress callback: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            });

            // Call FractalRenderService to render the fractal
            FractalRenderResult result;
            try
            {
                result = await _renderService.RenderMandelbrotAsync(
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
            }
            catch (InvalidCastException ex)
            {
                StatusMessage = $"InvalidCastException during render: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"InvalidCastException in RenderMandelbrotAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return;
            }

            // Convert byte[] to WriteableBitmap on UI thread
            try
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        ConvertPixelDataToBitmap(result.PixelData, ImageWidth, ImageHeight);
                    }
                    catch (InvalidCastException ex)
                    {
                        StatusMessage = $"InvalidCastException in ConvertPixelDataToBitmap: {ex.Message}";
                        System.Diagnostics.Debug.WriteLine($"InvalidCastException in ConvertPixelDataToBitmap: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                });
            }
            catch (InvalidCastException ex)
            {
                StatusMessage = $"InvalidCastException in TryEnqueue: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"InvalidCastException in TryEnqueue: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

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


    // ═══════════════════════════════════════════════════════════════════════════════
    // NOTE: CanRender() moved to MainViewModel.Rendering.cs
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Unified render command that routes to the appropriate render method.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderAsync()
    {
        System.Diagnostics.Debug.WriteLine($"[RenderAsync] Called - IsHailstoneMode={IsHailstoneMode}, SelectedFractalType={SelectedFractalType}");

        if (IsHailstoneMode)
        {
            System.Diagnostics.Debug.WriteLine("[RenderAsync] Routing to RenderHailstoneAsync");
            await RenderHailstoneAsync();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[RenderAsync] Routing to RenderMandelbrotAsync");
            await RenderMandelbrotAsync();
        }
    }

    /// <summary>
    /// Renders the Hailstone 2D sequence with current parameters.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderHailstoneAsync()
    {
        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Called - IsHailstoneMode={IsHailstoneMode}");

        if (!IsHailstoneMode)
        {
            System.Diagnostics.Debug.WriteLine("[RenderHailstoneAsync] EARLY EXIT - Not in Hailstone mode!");
            StatusMessage = "Please select Hailstone fractal type first.";
            return;
        }

        IsRendering = true;
        RenderProgress = 0;
        StatusMessage = $"Calculating Hailstone sequence from ({HailstoneStartX}, {HailstoneStartY})...";

        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Starting render - ({HailstoneStartX}, {HailstoneStartY}), MaxIter={HailstoneMaxIterations}");

        try
        {
            var startTime = DateTime.Now;

            // Calculate sequence with color spread and optional CSV export
            var result = await _hailstoneService.CalculateSequenceAsync(
                HailstoneStartX,
                HailstoneStartY,
                HailstoneMaxIterations,
                colorSpread: 7,  // Default color spread
                exportToCsv: false);  // Set to true if you want CSV export

            System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Sequence calculated - {result.Sequence.Count} points");

            StatusMessage = $"Rendering Hailstone sequence ({result.Sequence.Count} points)...";
            RenderProgress = 50;

            // Render to bitmap with optional custom viewport
            var renderResult = await _hailstoneRenderService.RenderSequenceAsync(
                result,
                ImageWidth,
                ImageHeight,
                ShowHailstoneAxes,
                ShowHailstonePoints,
                ShowHailstoneLabels,
                UseFixedHailstoneViewport,
                HailstoneViewportMinX,
                HailstoneViewportMaxX,
                HailstoneViewportMinY,
                HailstoneViewportMaxY);

            System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Got render result with {(renderResult.PixelData != null ? renderResult.PixelData.Length : 0)} bytes of pixel data");

            // Create bitmap on UI thread (WriteableBitmap must be created on UI thread!)
            WriteableBitmap? bitmap = null;
            _dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (renderResult.PixelData != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Creating WriteableBitmap on UI thread ({renderResult.Width}x{renderResult.Height})");
                        bitmap = new WriteableBitmap(renderResult.Width, renderResult.Height);
                        using (var stream = bitmap.PixelBuffer.AsStream())
                        {
                            stream.Write(renderResult.PixelData, 0, renderResult.PixelData.Length);
                        }
                        System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] Bitmap created successfully");
                    }
                    else if (renderResult.Bitmap != null)
                    {
                        // Legacy path: bitmap already created
                        bitmap = renderResult.Bitmap;
                    }

                    FractalImage = bitmap;
                    CurrentHailstoneResult = result;
                    HailstoneScaleX = renderResult.ScaleX;
                    HailstoneScaleY = renderResult.ScaleY;
                    HailstoneOffsetX = renderResult.OffsetX;
                    HailstoneOffsetY = renderResult.OffsetY;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[RenderHailstoneAsync] ERROR creating bitmap on UI thread: {ex.Message}");
                    StatusMessage = $"Error creating bitmap: {ex.Message}";
                }
            });

            LastRenderTime = DateTime.Now - startTime;
            RenderProgress = 100;

            // Build status message
            string cycleInfo = result.HasCycle
                ? $" | Cycle detected at step {result.CycleStartIndex} (length {result.CycleLength})"
                : " | No cycle detected";

            StatusMessage = $"Rendered {result.Sequence.Count} points in {LastRenderTime.TotalMilliseconds:F0} ms{cycleInfo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Hailstone render error: {ex}");
        }
        finally
        {
            IsRendering = false;
        }
    }

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
    // NOTE: ConvertPixelDataToBitmap() moved to MainViewModel.Rendering.cs
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Creates metadata object from current fractal state.
    /// </summary>
    public Models.FractalMetadata CreateMetadata()
    {
        Models.HailstoneParameters? hailstoneParams = null;

        // Include Hailstone parameters if in Hailstone mode
        if (IsHailstoneMode && CurrentHailstoneResult != null)
        {
            hailstoneParams = new Models.HailstoneParameters
            {
                StartX = HailstoneStartX,
                StartY = HailstoneStartY,
                MaxIterations = HailstoneMaxIterations,
                TotalPoints = CurrentHailstoneResult.Sequence.Count,
                HasCycle = CurrentHailstoneResult.HasCycle,
                CycleStartIndex = CurrentHailstoneResult.HasCycle ? CurrentHailstoneResult.CycleStartIndex : null,
                CycleLength = CurrentHailstoneResult.HasCycle ? CurrentHailstoneResult.CycleLength : null,
                BoundsMinX = CurrentHailstoneResult.MinX,
                BoundsMaxX = CurrentHailstoneResult.MaxX,
                BoundsMinY = CurrentHailstoneResult.MinY,
                BoundsMaxY = CurrentHailstoneResult.MaxY,
                UseFixedViewport = UseFixedHailstoneViewport
            };
        }

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
            renderTime: LastRenderTime,
            hailstone: hailstoneParams
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