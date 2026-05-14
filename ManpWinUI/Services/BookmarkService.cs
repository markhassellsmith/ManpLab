using ManpWinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ManpWinUI.Services;

/// <summary>
/// Service for managing fractal bookmarks.
/// Handles saving/loading bookmarks from local storage and provides famous presets.
/// Works for both MSIX packages and portable ZIP distributions.
/// </summary>
public class BookmarkService : IBookmarkService
{
    private const string BookmarksFileName = "bookmarks.json";
    private List<FractalBookmark> _bookmarks = new();

    private readonly bool _isPackaged;
    private readonly string? _bookmarksFilePath;

    public BookmarkService()
    {
        // Detect if running as packaged (MSIX) or unpackaged (portable ZIP)
        try
        {
            _ = ApplicationData.Current.LocalFolder;
            _isPackaged = true;
            System.Diagnostics.Debug.WriteLine("[BookmarkService] Running as packaged app (MSIX)");
        }
        catch (Exception)
        {
            _isPackaged = false;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localAppData, "ManpLab");
            Directory.CreateDirectory(appFolder);
            _bookmarksFilePath = Path.Combine(appFolder, BookmarksFileName);
            System.Diagnostics.Debug.WriteLine($"[BookmarkService] Running as unpackaged app (portable) - using: {_bookmarksFilePath}");
        }
    }

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
            if (_isPackaged)
            {
                // MSIX: Use Windows Storage API
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.TryGetItemAsync(BookmarksFileName) as StorageFile;

                if (file != null)
                {
                    var json = await FileIO.ReadTextAsync(file);
                    var loaded = JsonSerializer.Deserialize<List<FractalBookmark>>(json);
                    if (loaded != null)
                    {
                        _bookmarks = loaded;
                        System.Diagnostics.Debug.WriteLine($"[BookmarkService] Loaded {_bookmarks.Count} bookmarks from MSIX storage");
                        return;
                    }
                }
            }
            else
            {
                // Portable ZIP: Use file system
                if (!string.IsNullOrEmpty(_bookmarksFilePath) && File.Exists(_bookmarksFilePath))
                {
                    var json = File.ReadAllText(_bookmarksFilePath);
                    var loaded = JsonSerializer.Deserialize<List<FractalBookmark>>(json);
                    if (loaded != null)
                    {
                        _bookmarks = loaded;
                        System.Diagnostics.Debug.WriteLine($"[BookmarkService] Loaded {_bookmarks.Count} bookmarks from file: {_bookmarksFilePath}");
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BookmarkService] Error loading bookmarks: {ex.Message}");
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
            var json = JsonSerializer.Serialize(_bookmarks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            if (_isPackaged)
            {
                // MSIX: Use Windows Storage API
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(BookmarksFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
                System.Diagnostics.Debug.WriteLine($"[BookmarkService] Saved {_bookmarks.Count} bookmarks to MSIX storage");
            }
            else
            {
                // Portable ZIP: Use file system
                if (!string.IsNullOrEmpty(_bookmarksFilePath))
                {
                    File.WriteAllText(_bookmarksFilePath, json);
                    System.Diagnostics.Debug.WriteLine($"[BookmarkService] Saved {_bookmarks.Count} bookmarks to file: {_bookmarksFilePath}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BookmarkService] Error saving bookmarks: {ex.Message}");
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

    /// <summary>
    /// Exports bookmarks to a JSON file chosen by the user.
    /// </summary>
    public async Task<bool> ExportBookmarksAsync()
    {
        try
        {
            var savePicker = new FileSavePicker();

            // Get the window handle for the picker
            var window = App.Current.MainWindow;
            if (window == null)
                return false;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("JSON Files", new List<string>() { ".json" });
            savePicker.SuggestedFileName = $"ManpLab_Bookmarks_{DateTime.Now:yyyyMMdd_HHmmss}";

            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return false;

            // Export user bookmarks only (exclude presets)
            var userBookmarks = _bookmarks.Where(b => !b.IsPreset).ToList();

            var json = JsonSerializer.Serialize(userBookmarks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await FileIO.WriteTextAsync(file, json);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting bookmarks: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Imports bookmarks from a JSON file chosen by the user.
    /// </summary>
    public async Task<bool> ImportBookmarksAsync()
    {
        try
        {
            var openPicker = new FileOpenPicker();

            // Get the window handle for the picker
            var window = App.Current.MainWindow;
            if (window == null)
                return false;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".json");

            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
                return false;

            var json = await FileIO.ReadTextAsync(file);
            var importedBookmarks = JsonSerializer.Deserialize<List<FractalBookmark>>(json);

            if (importedBookmarks != null && importedBookmarks.Count > 0)
            {
                // Add imported bookmarks, marking them as non-preset
                foreach (var bookmark in importedBookmarks)
                {
                    bookmark.IsPreset = false;
                    bookmark.Id = Guid.NewGuid().ToString(); // Generate new ID to avoid conflicts
                    _bookmarks.Add(bookmark);
                }

                await SaveBookmarksAsync();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error importing bookmarks: {ex.Message}");
            return false;
        }
    }
}
