using ManpWinUI.Models;

namespace ManpWinUI.Services;

/// <summary>
/// Service interface for managing navigation history (undo/redo).
/// Tracks fractal view states for back/forward navigation.
/// </summary>
public interface INavigationHistoryService
{
    /// <summary>
    /// Gets all history entries (read-only).
    /// </summary>
    IReadOnlyList<NavigationHistoryEntry> History { get; }

    /// <summary>
    /// Gets the current position in the history stack (0-based index).
    /// -1 if no history.
    /// </summary>
    int CurrentPosition { get; }

    /// <summary>
    /// Gets whether undo is available.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Gets whether redo is available.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Records a new navigation state.
    /// Clears any forward history (redo stack) when new state is added.
    /// </summary>
    /// <param name="entry">The navigation state to record.</param>
    /// <param name="forceRecord">If true, bypasses significance check.</param>
    void RecordState(NavigationHistoryEntry entry, bool forceRecord = false);

    /// <summary>
    /// Moves back in history (undo).
    /// Returns the previous state, or null if at start of history.
    /// </summary>
    NavigationHistoryEntry? Undo();

    /// <summary>
    /// Moves forward in history (redo).
    /// Returns the next state, or null if at end of history.
    /// </summary>
    NavigationHistoryEntry? Redo();

    /// <summary>
    /// Jumps to a specific position in history.
    /// </summary>
    /// <param name="index">Index in the history list (0-based).</param>
    NavigationHistoryEntry? JumpTo(int index);

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    void Clear();

    /// <summary>
    /// Loads history from persistent storage.
    /// </summary>
    Task LoadHistoryAsync();

    /// <summary>
    /// Saves history to persistent storage.
    /// </summary>
    Task SaveHistoryAsync();
}
