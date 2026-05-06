# Animation State Persistence Fix

## Problem
When starting an animation and switching to a different tab in the properties panel, the animation rendering would stop or be forgotten. The UI would not show progress, and the animation state would be lost.

## Root Cause
The issue was caused by the lifecycle management of `AnimationViewModel`:

1. **Transient Lifetime**: `AnimationViewModel` was registered as **Transient** in the DI container
2. **New Instance Per Load**: Each time `AnimationControlPanel` was loaded (e.g., switching tabs), it would call `GetRequiredService<AnimationViewModel>()`, creating a **new instance**
3. **State Loss**: The new instance had no knowledge of the ongoing animation, progress, or cancellation token from the previous instance
4. **Tab Switching**: Depending on `TabView` behavior, the control could be unloaded/reloaded, triggering instance recreation

### Lifecycle Flow (Before Fix)
```
User clicks "Render Animation"
→ AnimationViewModel instance #1 starts rendering
→ Animation progress: 25%
→ User switches to "Colors" tab
→ AnimationControlPanel potentially unloaded
→ User switches back to "Animation" tab
→ AnimationControlPanel reloaded
→ GetRequiredService<AnimationViewModel>() creates instance #2 ← NEW INSTANCE!
→ Instance #2 has no state from instance #1
→ User sees no animation progress
→ Rendering may continue in background (instance #1) but UI is disconnected
```

## Solution

### 1. Changed ViewModel Lifetime to Singleton
**File**: `ManpWinUI\App.xaml.cs`

```csharp
// Before:
services.AddTransient<AnimationViewModel>();

// After:
services.AddSingleton<AnimationViewModel>(); 
// Singleton preserves state across tab switches
```

**Why Singleton?**
- Same instance is reused throughout the application lifetime
- Animation state (progress, cancellation token, settings) is preserved
- Tab switching doesn't create new instances
- Consistent with other panel ViewModels (`RenderSettingsViewModel`, `ColorEditorViewModel`)

### 2. Refactored MainViewModel Injection
Since `MainViewModel` is **Transient** and `AnimationViewModel` is now **Singleton**, we can't inject MainViewModel directly in the constructor.

**Solution**: Added `SetMainViewModel()` method for late binding

**File**: `ManpWinUI\ViewModels\AnimationViewModel.cs`

```csharp
// Made field mutable instead of readonly
private MainViewModel? _mainViewModel;

// Removed MainViewModel from constructor
public AnimationViewModel(
    AnimationService animationService,
    ILogger<AnimationViewModel> logger)
{
    // ...
}

// Added setter method
public void SetMainViewModel(MainViewModel? mainViewModel)
{
    _mainViewModel = mainViewModel;
}
```

### 3. Updated AnimationControlPanel Initialization
**File**: `ManpWinUI\Views\Animation\AnimationControlPanel.xaml.cs`

**Added `Loaded` event handler**:
```csharp
private void OnLoaded(object sender, RoutedEventArgs e)
{
    // Inject MainViewModel reference when control loads
    var mainViewModel = App.Current.Services.GetService<MainViewModel>();
    ViewModel.SetMainViewModel(mainViewModel);
}
```

**File**: `ManpWinUI\Views\Animation\AnimationControlPanel.xaml`

```xaml
<UserControl
    ...
    Loaded="OnLoaded"
    ...>
```

### 4. Updated Sync Button Handler
Changed from using cached `_mainViewModel` field to fetching from DI each time:

```csharp
private void SyncFromCurrentViewButton_Click(object sender, RoutedEventArgs e)
{
    // Get fresh MainViewModel reference from DI
    var mainViewModel = App.Current.Services.GetService<MainViewModel>();

    if (mainViewModel == null)
    {
        ShowError("MainViewModel not available", "Cannot sync from current view.");
        return;
    }

    // ... sync logic
}
```

## Benefits

### 1. **State Persistence**
- Animation progress preserved across tab switches
- Cancellation token remains valid
- User can switch tabs and return to see progress

### 2. **Consistent UX**
- Progress bar continues updating
- Status messages remain visible
- Render/cancel buttons maintain correct enabled state

### 3. **Resource Management**
- Single ViewModel instance = less memory
- No duplicate animation renders
- Proper cancellation token lifecycle

### 4. **Alignment with Other Panels**
- Matches lifetime pattern of `RenderSettingsViewModel`, `ColorEditorViewModel`
- Consistent DI pattern across the application

## Lifecycle Flow (After Fix)
```
User clicks "Render Animation"
→ AnimationViewModel singleton instance starts rendering
→ Animation progress: 25%
→ User switches to "Colors" tab
→ AnimationControlPanel may be unloaded (but ViewModel persists!)
→ User switches back to "Animation" tab
→ AnimationControlPanel reloaded
→ Loaded event fires → SetMainViewModel() called
→ GetRequiredService<AnimationViewModel>() returns SAME singleton instance
→ UI rebinds to existing instance
→ User sees animation progress at current state (e.g., 45%)
→ Progress continues smoothly
```

## Testing Scenarios

### Scenario 1: Tab Switch During Render
1. Start animation render
2. Switch to "Colors" tab
3. Wait 5 seconds
4. Switch back to "Animation" tab
5. **Expected**: Progress bar shows current progress, not reset to 0%

### Scenario 2: Multiple Tab Switches
1. Start animation render
2. Switch through all tabs: Colors → Render → Info → Bookmarks → Animation
3. **Expected**: Each return to Animation tab shows updated progress

### Scenario 3: Cancellation Persistence
1. Start animation render
2. Switch to different tab
3. Switch back to Animation tab
4. Click "Cancel"
5. **Expected**: Animation cancels properly, cancellation token still valid

### Scenario 4: Completion State
1. Start animation render
2. Switch away from Animation tab
3. Wait for completion
4. Switch back to Animation tab
5. **Expected**: Completion message and clickable file path visible

## Alternative Approaches Considered

### ❌ Option A: Keep Transient, Store State Externally
- Store animation state in a separate singleton service
- **Rejected**: Adds complexity, duplicates state management

### ❌ Option B: Prevent Tab Switching During Render
- Disable other tabs while rendering
- **Rejected**: Poor UX, user should be able to adjust colors, etc.

### ❌ Option C: Save/Restore State on Load/Unload
- Serialize state when unloading, restore when loading
- **Rejected**: Complex, error-prone, doesn't handle background tasks well

### ✅ Option D: Singleton ViewModel (Chosen)
- Simple, standard pattern
- Aligns with existing ViewModels
- Natural state persistence

## Additional Notes

### MainViewModel Injection Pattern
The late-binding `SetMainViewModel()` approach allows:
- AnimationViewModel to be a singleton
- MainViewModel to remain transient (each window/page gets its own instance)
- Proper separation of concerns
- Easy testing (can inject mock MainViewModel)

### Memory Considerations
Since AnimationViewModel is now a singleton:
- Lives for the application lifetime
- Consider adding `Dispose()` pattern if memory becomes a concern
- Current implementation is lightweight (just properties and settings)

### Thread Safety
Animation rendering uses `CancellationTokenSource`:
- Properly disposed after each render
- No thread safety issues with singleton pattern
- UI updates marshaled to dispatcher thread

## Future Enhancements

1. **Persistent Animation Queue**
   - Queue multiple animations
   - Survive tab switches and even app restarts

2. **Background Rendering Indicator**
   - Global indicator (e.g., status bar) when animation is rendering
   - Visible even when Animation tab not active

3. **Render History**
   - Keep list of recently completed animations
   - Quick re-export with different settings

4. **Multi-Window Support**
   - If app supports multiple windows
   - May need scoped ViewModel per window
