using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Models;
using System.Collections.ObjectModel;
using System.Linq;

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
    /// Filtered collection of bookmarks based on ShowFavoritesOnly filter.
    /// </summary>
    public ObservableCollection<FractalBookmark> FilteredBookmarks { get; } = new();

    /// <summary>
    /// Currently selected bookmark in the bookmarks panel.
    /// </summary>
    [ObservableProperty]
    public partial FractalBookmark? SelectedBookmark { get; set; }

    /// <summary>
    /// Whether the bookmarks panel is currently visible/open.
    /// Now used for persistent panel visibility (like Browser/Properties).
    /// </summary>
    [ObservableProperty]
    public partial bool IsBookmarksPanelOpen { get; set; }

    /// <summary>
    /// Whether to show only favorited bookmarks or all bookmarks.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BookmarkFilterLabel))]
    private bool _showFavoritesOnly;

    /// <summary>
    /// Label for the current bookmark filter state.
    /// </summary>
    public string BookmarkFilterLabel => _showFavoritesOnly ? "⭐ Favorites" : "📚 All";

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

        ApplyBookmarkFilter();
    }

    /// <summary>
    /// Applies the current filter to the bookmarks collection.
    /// </summary>
    private void ApplyBookmarkFilter()
    {
        FilteredBookmarks.Clear();

        var source = _showFavoritesOnly 
            ? Bookmarks.Where(b => b.IsFavorite) 
            : Bookmarks;

        foreach (var bookmark in source)
        {
            FilteredBookmarks.Add(bookmark);
        }
    }

    partial void OnShowFavoritesOnlyChanged(bool value)
    {
        ApplyBookmarkFilter();
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

        // Set the current visualization name to the bookmark name
        CurrentVisualizationName = bookmark.Name;
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
    /// Toggles the favorites filter on/off.
    /// </summary>
    [RelayCommand]
    private void ToggleFavoritesFilter()
    {
        ShowFavoritesOnly = !ShowFavoritesOnly;
        StatusMessage = ShowFavoritesOnly ? "Showing favorites only" : "Showing all bookmarks";
    }

    /// <summary>
    /// Exports bookmarks to a JSON file.
    /// </summary>
    [RelayCommand]
    private async Task ExportBookmarksAsync()
    {
        var success = await _bookmarkService.ExportBookmarksAsync();
        if (success)
        {
            StatusMessage = "Bookmarks exported successfully";
        }
        else
        {
            StatusMessage = "Export cancelled or failed";
        }
    }

    /// <summary>
    /// Imports bookmarks from a JSON file.
    /// </summary>
    [RelayCommand]
    private async Task ImportBookmarksAsync()
    {
        var success = await _bookmarkService.ImportBookmarksAsync();
        if (success)
        {
            RefreshBookmarks();
            StatusMessage = "Bookmarks imported successfully";
        }
        else
        {
            StatusMessage = "Import cancelled or failed";
        }
    }

    /// <summary>
    /// Promotes a navigation history entry to a bookmark.
    /// </summary>
    [RelayCommand]
    private async Task PromoteHistoryToBookmarkAsync(NavigationHistoryEntry? entry)
    {
        if (entry == null)
            return;

        // Use a simple prompt for the bookmark name
        // In a real app, this would be a dialog
        var name = $"{entry.FractalType} - {DateTime.Now:g}";
        var bookmark = FractalBookmark.FromHistoryEntry(entry, name, entry.Description);

        await _bookmarkService.AddBookmarkAsync(bookmark);
        RefreshBookmarks();

        StatusMessage = $"History entry promoted to bookmark: {name}";
    }
}
