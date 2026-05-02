using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using ManpWinUI.Services;
using ManpCore.Services.Models;
using ManpWinUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ManpWinUI.ViewModels;

/// <summary>
/// Main view model for the fractal explorer interface (Core).
/// Coordinates services and provides initialization.
/// 
/// Split into partial classes for maintainability:
/// - MainViewModel.cs (this file): Core initialization and service coordination
/// - MainViewModel.UI.cs: UI state, visual settings, fractal type selection
/// - MainViewModel.Rendering.cs: Image resolution, render state, fractal image output
/// - MainViewModel.StandardFractals.cs: Mandelbrot/Julia parameters
/// - MainViewModel.Hailstone.cs: Hailstone sequence parameters
/// - MainViewModel.Bookmarks.cs: Bookmark management
/// - MainViewModel.Commands.cs: Rendering commands (Mandelbrot, Hailstone)
/// - MainViewModel.Navigation.cs: View manipulation (zoom, pan, reset)
/// - MainViewModel.Metadata.cs: Metadata creation for export
/// </summary>
public partial class MainViewModel(
    IFractalRenderService renderService, 
    IBookmarkService bookmarkService,
    INavigationHistoryService navigationHistoryService,
    IHailstoneService hailstoneService,
    IHailstoneRenderService hailstoneRenderService,
    IAppSettingsService settingsService) : ObservableObject
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // SERVICE DEPENDENCIES
    // ═══════════════════════════════════════════════════════════════════════════════

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IFractalRenderService _renderService = renderService;
    private readonly IBookmarkService _bookmarkService = bookmarkService;
    private readonly INavigationHistoryService _navigationHistoryService = navigationHistoryService;
    private readonly IHailstoneService _hailstoneService = hailstoneService;
    private readonly IHailstoneRenderService _hailstoneRenderService = hailstoneRenderService;
    private readonly IAppSettingsService _settingsService = settingsService;

    // ═══════════════════════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Initializes bookmarks and navigation history from storage and restores panel state.
    /// Call this after construction to load saved state and UI settings.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _bookmarkService.LoadBookmarksAsync();
        RefreshBookmarks();

        await _navigationHistoryService.LoadHistoryAsync();
        RefreshNavigationHistory();

        RestorePanelState();
    }
}
