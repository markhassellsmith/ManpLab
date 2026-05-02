# Week 8 Design Change: Session-Based History (Not Persistent)

**Issue**: User saw views from previous sessions when using undo, which was confusing.

**User Feedback**: "I was seeing views from previous sessions and not the session that started with my first render."

## Design Decision

**Navigation history is now session-based**, not persistent across app restarts.

### Rationale

**Original Design**: History persisted across sessions
- Pro: Can review what you did in previous sessions
- Con: Confusing when undo jumps to old session states
- Con: Unclear which history entries are from current vs previous sessions
- Con: `_currentPosition` might restore mid-history from old session

**New Design**: History clears on each app launch
- Pro: Clear mental model - undo/redo only affects current session
- Pro: Each session starts fresh at known state
- Pro: No confusion about "where am I" in history
- Pro: Simpler implementation

### User Experience

#### Before (Confusing)
```
Session 1:
  - Start at 1.0x
  - Zoom to 2.0x, 4.0x, 8.0x
  - Close app (history saved)

Session 2:
  - Start app (loads old history with position at 8.0x)
  - Zoom to 16.0x (adds to old history)
  - Press Undo → Goes to 8.0x ✅ Expected
  - Press Undo → Goes to 4.0x from Session 1 ❌ Confusing!
  - User: "Wait, I never did that in this session!"
```

#### After (Clear)
```
Session 1:
  - Start at 1.0x
  - Zoom to 2.0x, 4.0x, 8.0x
  - Close app

Session 2:
  - Start app (history EMPTY)
  - App shows default 1.0x view
  - Zoom to 2.0x (records 1.0x → history)
  - Zoom to 4.0x (records 2.0x → history)
  - Press Undo → Goes to 2.0x ✅
  - Press Undo → Goes to 1.0x (current session start) ✅
  - Press Undo → Disabled (no earlier state in this session) ✅
```

## Implementation

### Before
```csharp
public async Task LoadHistoryAsync()
{
    try
    {
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
                _currentPosition = Math.Min(data.CurrentPosition, _history.Count - 1);
                return;
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
    }

    _history.Clear();
    _currentPosition = -1;
}
```

### After
```csharp
public async Task LoadHistoryAsync()
{
    // Start fresh each session
    _history.Clear();
    _currentPosition = -1;
    System.Diagnostics.Debug.WriteLine("[NavigationHistory] Starting new session with empty history");

    await Task.CompletedTask; // Keep async signature
}
```

## What Still Persists

Navigation history is now **ephemeral**, but these are still **persistent**:

✅ **Bookmarks** - User's saved locations persist forever
✅ **Panel state** - Which panels are open/closed
✅ **Panel sizes** - Width of browser/properties panels
✅ **App settings** - User preferences

## Future Enhancement Options

If users request historical session data, we could:

1. **Session History Archive**
   - Save each session's history to dated files
   - Provide "Browse Past Sessions" feature in History tab
   - But don't load old history into active undo/redo stack

2. **Session Marker UI**
   - If we restore old history, add visual markers: "Previous Session", "Current Session"
   - Disable undo/redo past current session boundary
   - Only allow "Go To" for old session entries

3. **Import/Export History**
   - Like bookmarks, let users export/import history for sharing exploration paths

## Testing Updates

**Test 5** updated from "History Persistence" to "History Per-Session Behavior":

**Old Test**: Expected history to persist and be restorable across sessions
**New Test**: Expects history to clear on each launch, starting fresh

## Comparison to Other Apps

### Apps with Session-Based Undo
- **Most text editors** - Undo stack clears on file close
- **Image editors (Photoshop)** - Undo history lost on close
- **Browser back button** - Each tab has independent history

### Apps with Persistent Undo
- **Autodesk Fusion 360** - Full history timeline persists
- **Blender** - Undo history survives reopening (in same session file)
- **Adobe After Effects** - History tied to project file

For a fractal explorer, the **session-based model** makes more sense:
- Exploratory tool (not project-based)
- Users typically start fresh explorations each session
- Bookmarks serve the role of "save this for later"

## Related Design Patterns

**Undo/Redo**: Session-based (ephemeral)
**Bookmarks**: Persistent (curated saves)

This separation provides:
- **Short-term navigation** via undo/redo (current session)
- **Long-term navigation** via bookmarks (across sessions)

Clear distinction between temporary exploration and permanent saves.

## Files Modified

- `ManpWinUI/Services/NavigationHistoryService.cs` - `LoadHistoryAsync()` now starts fresh
- `ManpWinUI/docs/Phase2-Week8-Testing-Guide.md` - Test 5 updated

## Rollback Instructions

If we need to restore persistent history later, uncomment the code in `LoadHistoryAsync()`:

```csharp
// Uncomment this block to restore cross-session history:
try
{
    var localFolder = ApplicationData.Current.LocalFolder;
    var file = await localFolder.TryGetItemAsync(HistoryFileName) as StorageFile;
    // ... (rest of original load code)
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
}
```

## Impact Analysis

### User Impact
- ✅ **Positive**: Clearer mental model
- ✅ **Positive**: Undo/redo behaves as expected
- ⚠️ **Neutral**: Can't review previous session history (but bookmarks still work)

### Performance Impact
- ✅ **Positive**: Faster startup (no JSON load/parse)
- ✅ **Positive**: Less memory (no old history entries)

### Data Impact
- ⚠️ **Neutral**: History file still saved (for future features)
- ✅ **Positive**: No risk of corrupted old history affecting new session

## Related Issues

This fixes:
- User confusion about seeing old session states
- Unclear history position on app launch
- Undo jumping to unexpected old states

## Testing Checklist

- [x] App starts with empty history
- [x] First zoom creates first history entry
- [x] Undo returns to session start state
- [x] Close and reopen → history is empty again
- [x] Bookmarks still persist (not affected)
- [x] Build succeeds

---

**Status**: ✅ Implemented  
**Build**: Successful  
**User Feedback**: Addressed  
**Design**: Finalized - Session-based history
