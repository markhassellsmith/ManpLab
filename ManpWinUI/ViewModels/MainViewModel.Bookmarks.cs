using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Models;
using System.Collections.ObjectModel;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Bookmark management.
/// Handles saving, loading, and managing fractal bookmarks.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // BOOKMARKS COLLECTION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Collection of fractal bookmarks displayed in the UI.
    /// </summary>
    public ObservableCollection<FractalBookmark> Bookmarks { get; } = new();

    /// <summary>
    /// Currently selected bookmark in the bookmarks panel.
    /// </summary>
    [ObservableProperty]
    public partial FractalBookmark? SelectedBookmark { get; set; }

    /// <summary>
    /// Whether the bookmarks panel is currently visible/open.
    /// </summary>
    [ObservableProperty]
    public partial bool IsBookmarksPanelOpen { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // BOOKMARK INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Refreshes the bookmarks collection from the bookmark service.
    /// Called after loading bookmarks or modifying the collection.
    /// </summary>
    private void RefreshBookmarks()
    {
        Bookmarks.Clear();
        foreach (var bookmark in _bookmarkService.Bookmarks)
        {
            Bookmarks.Add(bookmark);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // BOOKMARK COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loads a bookmark and navigates to that fractal location.
    /// Sets all fractal parameters from the bookmark and triggers a render.
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
    /// Saves current fractal view as a new bookmark.
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
    /// Deletes a bookmark from the collection.
    /// Preset bookmarks cannot be deleted.
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
    /// Toggles the favorite status of a bookmark.
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
    /// Toggles the visibility of the bookmarks panel.
    /// </summary>
    [RelayCommand]
    private void ToggleBookmarksPanel()
    {
        IsBookmarksPanelOpen = !IsBookmarksPanelOpen;
    }
}
