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
/// Service for managing navigation history (undo/redo).
/// Maintains a stack of fractal view states for back/forward navigation.
/// Works for both MSIX packages and portable ZIP distributions.
/// </summary>
public class NavigationHistoryService : INavigationHistoryService
{
    private const string HistoryFileName = "navigation_history.json";
    private const int MaxHistorySize = 50;

    private readonly List<NavigationHistoryEntry> _history = new();
    private int _currentPosition = -1;

    private readonly bool _isPackaged;
    private readonly string? _historyFilePath;

    public NavigationHistoryService()
    {
        // Detect if running as packaged (MSIX) or unpackaged (portable ZIP)
        try
        {
            _ = ApplicationData.Current.LocalFolder;
            _isPackaged = true;
            System.Diagnostics.Debug.WriteLine("[NavigationHistoryService] Running as packaged app (MSIX)");
        }
        catch (Exception)
        {
            _isPackaged = false;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localAppData, "ManpLab");
            Directory.CreateDirectory(appFolder);
            _historyFilePath = Path.Combine(appFolder, HistoryFileName);
            System.Diagnostics.Debug.WriteLine($"[NavigationHistoryService] Running as unpackaged app (portable) - using: {_historyFilePath}");
        }
    }

    /// <summary>
    /// Gets all history entries (read-only).
    /// </summary>
    public IReadOnlyList<NavigationHistoryEntry> History => _history.AsReadOnly();

    /// <summary>
    /// Gets the current position in the history stack.
    /// -1 = no history
    /// 0 to Count-1 = at a specific history entry
    /// Count = at "present" (past all history entries)
    /// </summary>
    public int CurrentPosition => _currentPosition;

    /// <summary>
    /// Gets whether undo is available (can go back).
    /// </summary>
    public bool CanUndo => _currentPosition > 0;

    /// <summary>
    /// Gets whether redo is available (can go forward).
    /// </summary>
    public bool CanRedo => _currentPosition >= 0 && _currentPosition < _history.Count - 1;

    /// <summary>
    /// Records a new navigation state.
    /// Clears any forward history (redo stack) when new state is added.
    /// Position is set to "present" (past all history) after recording.
    /// </summary>
    public void RecordState(NavigationHistoryEntry entry, bool forceRecord = false)
    {
        // Check if this is a significant change from the current state
        if (!forceRecord && _currentPosition >= 0 && _currentPosition < _history.Count)
        {
            var currentEntry = _history[_currentPosition];
            if (!entry.IsSignificantChangeFrom(currentEntry))
            {
                System.Diagnostics.Debug.WriteLine("[NavigationHistory] Skipping insignificant change");
                return;
            }
        }

        // Clear any forward history (redo entries)
        if (_currentPosition < _history.Count - 1)
        {
            var removeCount = _history.Count - _currentPosition - 1;
            _history.RemoveRange(_currentPosition + 1, removeCount);
            System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Cleared {removeCount} forward entries");
        }

        // Add new entry and point to it
        _history.Add(entry);
        _currentPosition = _history.Count - 1;

        // Enforce max size (remove oldest entries if needed)
        if (_history.Count > MaxHistorySize)
        {
            var removeCount = _history.Count - MaxHistorySize;
            _history.RemoveRange(0, removeCount);
            _currentPosition -= removeCount;
            System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Pruned {removeCount} old entries");
        }

        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Recorded: {entry.Description} (position {_currentPosition + 1}/{_history.Count})");
    }

    /// <summary>
    /// Moves back in history (undo).
    /// Returns the previous state, or null if at start of history.
    /// </summary>
    public NavigationHistoryEntry? Undo()
    {
        if (!CanUndo)
        {
            System.Diagnostics.Debug.WriteLine("[NavigationHistory] Cannot undo - at start of history");
            return null;
        }

        _currentPosition--;
        var entry = _history[_currentPosition];
        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Undo to: {entry.Description} (position {_currentPosition + 1}/{_history.Count})");
        return entry;
    }

    /// <summary>
    /// Moves forward in history (redo).
    /// Returns the next state, or null if at end of history.
    /// </summary>
    public NavigationHistoryEntry? Redo()
    {
        if (!CanRedo)
        {
            System.Diagnostics.Debug.WriteLine("[NavigationHistory] Cannot redo - at end of history");
            return null;
        }

        _currentPosition++;
        var entry = _history[_currentPosition];
        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Redo to: {entry.Description} (position {_currentPosition + 1}/{_history.Count})");
        return entry;
    }

    /// <summary>
    /// Jumps to a specific position in history.
    /// </summary>
    public NavigationHistoryEntry? JumpTo(int index)
    {
        if (index < 0 || index >= _history.Count)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Invalid jump index: {index}");
            return null;
        }

        _currentPosition = index;
        var entry = _history[_currentPosition];
        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Jumped to: {entry.Description} (position {_currentPosition + 1}/{_history.Count})");
        return entry;
    }

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    public void Clear()
    {
        _history.Clear();
        _currentPosition = -1;
        System.Diagnostics.Debug.WriteLine("[NavigationHistory] Cleared all history");
    }

    /// <summary>
    /// Loads history from persistent storage.
    /// NOTE: Currently, we don't load previous session history to avoid confusion.
    /// Each session starts with empty history for better user experience.
    /// Old implementation persisted history but users found it confusing when
    /// undo/redo jumped to previous session states.
    /// </summary>
    public async Task LoadHistoryAsync()
    {
        // Start fresh each session
        _history.Clear();
        _currentPosition = -1;
        System.Diagnostics.Debug.WriteLine("[NavigationHistory] Starting new session with empty history");

        // If we want to restore old history later, uncomment this:
        /*
        try
        {
            if (_isPackaged)
            {
                // MSIX: Use Windows Storage API
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.TryGetItemAsync(HistoryFileName) as StorageFile;

                if (file != null)
                {
                    var json = await FileIO.ReadTextAsync(file);
                    var data = JsonSerializer.Deserialize<HistoryData>(json);

                    if (data != null && data.Entries != null)
                    {
                        _history.Clear();
                        _history.AddRange(data.Entries);
                        _currentPosition = _history.Count - 1; // Start at end for new session

                        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Loaded {_history.Count} entries from MSIX storage");
                        return;
                    }
                }
            }
            else
            {
                // Portable ZIP: Use file system
                if (!string.IsNullOrEmpty(_historyFilePath) && File.Exists(_historyFilePath))
                {
                    var json = File.ReadAllText(_historyFilePath);
                    var data = JsonSerializer.Deserialize<HistoryData>(json);

                    if (data != null && data.Entries != null)
                    {
                        _history.Clear();
                        _history.AddRange(data.Entries);
                        _currentPosition = _history.Count - 1; // Start at end for new session

                        System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Loaded {_history.Count} entries from file: {_historyFilePath}");
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigationHistory] Error loading history: {ex.Message}");
        }
        */

        await Task.CompletedTask; // Keep async signature
    }

    /// <summary>
    /// Saves history to persistent storage.
    /// </summary>
    public async Task SaveHistoryAsync()
    {
        try
        {
            var data = new HistoryData
            {
                Entries = _history,
                CurrentPosition = _currentPosition
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            if (_isPackaged)
            {
                // MSIX: Use Windows Storage API
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(HistoryFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
                System.Diagnostics.Debug.WriteLine($"[NavigationHistoryService] Saved {_history.Count} entries to MSIX storage");
            }
            else
            {
                // Portable ZIP: Use file system
                if (!string.IsNullOrEmpty(_historyFilePath))
                {
                    File.WriteAllText(_historyFilePath, json);
                    System.Diagnostics.Debug.WriteLine($"[NavigationHistoryService] Saved {_history.Count} entries to file: {_historyFilePath}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigationHistoryService] Error saving history: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper class for JSON serialization of history data.
    /// </summary>
    private class HistoryData
    {
        public List<NavigationHistoryEntry> Entries { get; set; } = new();
        public int CurrentPosition { get; set; }
    }
}
