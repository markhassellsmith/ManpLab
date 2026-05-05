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
/// - MainViewModel.Parameters.cs: Flexible parameter system integration (Task 5)
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
    IAppSettingsService settingsService,
    IFractalParameterService fractalParameterService,
    ViewModels.Properties.RenderSettingsViewModel renderSettingsViewModel) : ObservableObject
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
    private readonly IFractalParameterService _fractalParameterService = fractalParameterService;
    private readonly ViewModels.Properties.RenderSettingsViewModel _renderSettingsViewModel = renderSettingsViewModel;

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL METADATA (INFO TAB)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Currently selected fractal information for display in the Info tab.
    /// Populated when a fractal is selected from the browser.
    /// </summary>
    [ObservableProperty]
    private ManpCore.Native.FractalInfo? selectedFractalInfo;

    /// <summary>
    /// User notes for the currently selected fractal.
    /// Editable text that persists separately from native metadata.
    /// </summary>
    [ObservableProperty]
    private string userNotes = string.Empty;

    /// <summary>
    /// Updates the selected fractal info when a fractal is selected from the browser.
    /// </summary>
    public void UpdateSelectedFractalInfo(string fractalName)
    {
        SelectedFractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(fractalName);
        UserNotes = _settingsService.GetFractalNotes(fractalName) ?? string.Empty;
    }

    /// <summary>
    /// Saves user notes for the currently selected fractal.
    /// </summary>
    [RelayCommand]
    private void SaveUserNotes()
    {
        if (SelectedFractalInfo == null)
            return;

        _settingsService.SetFractalNotes(SelectedFractalInfo.Name, UserNotes);
    }

    /// <summary>
    /// Clears user notes for the currently selected fractal.
    /// </summary>
    [RelayCommand]
    private void ClearUserNotes()
    {
        UserNotes = string.Empty;

        if (SelectedFractalInfo != null)
        {
            _settingsService.SetFractalNotes(SelectedFractalInfo.Name, null);
        }
    }

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
