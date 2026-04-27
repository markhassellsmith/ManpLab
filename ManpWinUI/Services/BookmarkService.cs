using ManpWinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace ManpWinUI.Services;

/// <summary>
/// Service for managing fractal bookmarks.
/// Handles saving/loading bookmarks from local storage and provides famous presets.
/// </summary>
public class BookmarkService : IBookmarkService
{
    private const string BookmarksFileName = "bookmarks.json";
    private List<FractalBookmark> _bookmarks = new();

    /// <summary>
    /// Gets all bookmarks (user + presets).
    /// </summary>
    public IReadOnlyList<FractalBookmark> Bookmarks => _bookmarks.AsReadOnly();

    /// <summary>
    /// Loads bookmarks from file or initializes with famous presets.
    /// </summary>
    public async Task LoadBookmarksAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.TryGetItemAsync(BookmarksFileName) as StorageFile;

            if (file != null)
            {
                var json = await FileIO.ReadTextAsync(file);
                var loaded = JsonSerializer.Deserialize<List<FractalBookmark>>(json);
                if (loaded != null)
                {
                    _bookmarks = loaded;
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading bookmarks: {ex.Message}");
        }

        // If no bookmarks file or error, initialize with famous presets
        InitializeFamousPresets();
        await SaveBookmarksAsync();
    }

    /// <summary>
    /// Saves bookmarks to file.
    /// </summary>
    public async Task SaveBookmarksAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync(BookmarksFileName, CreationCollisionOption.ReplaceExisting);

            var json = JsonSerializer.Serialize(_bookmarks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await FileIO.WriteTextAsync(file, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving bookmarks: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a new bookmark.
    /// </summary>
    public async Task AddBookmarkAsync(FractalBookmark bookmark)
    {
        _bookmarks.Add(bookmark);
        await SaveBookmarksAsync();
    }

    /// <summary>
    /// Removes a bookmark by ID.
    /// </summary>
    public async Task RemoveBookmarkAsync(string bookmarkId)
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == bookmarkId);
        if (bookmark != null && !bookmark.IsPreset) // Don't allow deleting presets
        {
            _bookmarks.Remove(bookmark);
            await SaveBookmarksAsync();
        }
    }

    /// <summary>
    /// Toggles favorite status of a bookmark.
    /// </summary>
    public async Task ToggleFavoriteAsync(string bookmarkId)
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == bookmarkId);
        if (bookmark != null)
        {
            bookmark.IsFavorite = !bookmark.IsFavorite;
            await SaveBookmarksAsync();
        }
    }

    /// <summary>
    /// Updates an existing bookmark.
    /// </summary>
    public async Task UpdateBookmarkAsync(FractalBookmark bookmark)
    {
        var existing = _bookmarks.FirstOrDefault(b => b.Id == bookmark.Id);
        if (existing != null)
        {
            var index = _bookmarks.IndexOf(existing);
            _bookmarks[index] = bookmark;
            await SaveBookmarksAsync();
        }
    }

    /// <summary>
    /// Initializes the collection with famous Mandelbrot locations.
    /// </summary>
    private void InitializeFamousPresets()
    {
        _bookmarks = new List<FractalBookmark>
        {
            // Full Mandelbrot set
            new FractalBookmark
            {
                Name = "Full Mandelbrot Set",
                Description = "The complete Mandelbrot set view",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = -0.5,
                CenterY = 0.0,
                Zoom = 1.0,
                MaxIterations = 512,
                ColorPalette = "Classic",
                IsPreset = true,
                IsFavorite = true
            },

            // Seahorse Valley
            new FractalBookmark
            {
                Name = "Seahorse Valley",
                Description = "Famous seahorse-shaped valley on the Mandelbrot boundary",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = -0.74529,
                CenterY = 0.11308,
                Zoom = 100.0,
                MaxIterations = 1024,
                ColorPalette = "Fire",
                IsPreset = true,
                IsFavorite = true
            },

            // Elephant Valley
            new FractalBookmark
            {
                Name = "Elephant Valley",
                Description = "Intricate elephant-like structures",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = 0.28693,
                CenterY = 0.01425,
                Zoom = 500.0,
                MaxIterations = 1500,
                ColorPalette = "Ocean",
                IsPreset = true,
                IsFavorite = true
            },

            // Triple Spiral
            new FractalBookmark
            {
                Name = "Triple Spiral Valley",
                Description = "Beautiful triple spiral formations",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = -0.761574,
                CenterY = -0.0847596,
                Zoom = 200.0,
                MaxIterations = 1200,
                ColorPalette = "Rainbow",
                IsPreset = true
            },

            // Scepter Valley
            new FractalBookmark
            {
                Name = "Scepter Valley",
                Description = "Delicate scepter-like structures",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = -1.74975,
                CenterY = 0.00004,
                Zoom = 1000.0,
                MaxIterations = 2000,
                ColorPalette = "Psychedelic",
                IsPreset = true
            },

            // Dendrite (Julia)
            new FractalBookmark
            {
                Name = "Julia Set - Dendrite",
                Description = "Spectacular tree-like Julia set structure",
                FractalType = "Mandelbrot",
                IterationMode = "Julia",
                CenterX = 0.0,
                CenterY = 0.0,
                Zoom = 1.5,
                MaxIterations = 512,
                ColorPalette = "Classic",
                JuliaC = new JuliaParameters { Real = -0.8, Imaginary = 0.156 },
                IsPreset = true,
                IsFavorite = true
            },

            // San Marco (Julia)
            new FractalBookmark
            {
                Name = "Julia Set - San Marco",
                Description = "Intricate circular Julia set pattern",
                FractalType = "Mandelbrot",
                IterationMode = "Julia",
                CenterX = 0.0,
                CenterY = 0.0,
                Zoom = 1.5,
                MaxIterations = 512,
                ColorPalette = "Ocean",
                JuliaC = new JuliaParameters { Real = 0.285, Imaginary = 0.01 },
                IsPreset = true
            },

            // Mini-Mandelbrot
            new FractalBookmark
            {
                Name = "Mini-Mandelbrot",
                Description = "Tiny copy of the full Mandelbrot set deep in the structure",
                FractalType = "Mandelbrot",
                IterationMode = "Standard",
                CenterX = -0.7436669,
                CenterY = 0.1318259,
                Zoom = 5000.0,
                MaxIterations = 3000,
                ColorPalette = "Fire",
                IsPreset = true,
                IsFavorite = true
            }
        };
    }
}
