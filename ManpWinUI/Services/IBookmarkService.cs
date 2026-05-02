using ManpWinUI.Models;

namespace ManpWinUI.Services;

/// <summary>
/// Service interface for managing fractal bookmarks.
/// Handles saving/loading bookmarks from local storage and provides famous presets.
/// </summary>
public interface IBookmarkService
{
    /// <summary>
    /// Gets all bookmarks (user + presets).
    /// </summary>
    IReadOnlyList<FractalBookmark> Bookmarks { get; }

    /// <summary>
    /// Loads bookmarks from file or initializes with famous presets.
    /// </summary>
    Task LoadBookmarksAsync();

    /// <summary>
    /// Saves bookmarks to file.
    /// </summary>
    Task SaveBookmarksAsync();

    /// <summary>
    /// Adds a new bookmark.
    /// </summary>
    Task AddBookmarkAsync(FractalBookmark bookmark);

    /// <summary>
    /// Removes a bookmark by ID.
    /// </summary>
    Task RemoveBookmarkAsync(string bookmarkId);

    /// <summary>
    /// Toggles favorite status of a bookmark.
    /// </summary>
    Task ToggleFavoriteAsync(string bookmarkId);

    /// <summary>
    /// Updates an existing bookmark.
    /// </summary>
    Task UpdateBookmarkAsync(FractalBookmark bookmark);

    /// <summary>
    /// Exports bookmarks to a JSON file chosen by the user.
    /// </summary>
    Task<bool> ExportBookmarksAsync();

    /// <summary>
    /// Imports bookmarks from a JSON file chosen by the user.
    /// </summary>
    Task<bool> ImportBookmarksAsync();
}
