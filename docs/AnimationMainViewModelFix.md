# Animation MainViewModel Instance Fix

## Problem
When starting an animation, it would render the base fractal with the standard blue color palette instead of using the current viewport size and custom color palette.

## Root Cause
The issue was caused by **MainViewModel lifetime management**:

1. **Transient Lifetime**: `MainViewModel` is registered as **Transient** in the DI container
2. **Different Instances**: 
   - MainPage has its own `MainViewModel` instance with the user's current fractal view, zoom, and color settings
   - AnimationControlPanel was calling `App.Current.Services.GetService<MainViewModel>()` in its `OnLoaded` event
   - This created a **brand new instance** with default values (blue palette, standard viewport)
3. **Wrong State**: The animation used the new instance's default state instead of the current fractal state

### Instance Flow (Before Fix)
```
MainPage.cs:
→ Gets MainViewModel instance #1 from DI
→ User adjusts colors, zoom, viewport
→ Instance #1 has custom purple palette, zoom 500×, center (-0.75, 0.1)

AnimationControlPanel.OnLoaded:
→ Calls GetService<MainViewModel>()
→ Gets MainViewModel instance #2 (NEW!)
→ Instance #2 has defaults: blue palette, zoom 1×, center (-0.5, 0)

Animation renders:
→ Uses instance #2's defaults
→ User sees blue palette and standard viewport
❌ User's custom settings ignored!
```

## Solution

### Change: Pass MainPage's MainViewModel to AnimationPanel
Instead of letting `AnimationControlPanel` fetch its own `MainViewModel` from DI, MainPage now explicitly passes its own instance.

**File**: `ManpWinUI\Views\MainPage.cs`

Added after browser initialization:
```csharp
// Week 12: Inject MainViewModel into AnimationPanel so it uses the correct instance
// This ensures the animation uses the current viewport, colors, and settings
AnimationPanel.ViewModel.SetMainViewModel(ViewModel);
```

**Additional Enhancement**: The default animation filename now uses the current fractal or bookmark name instead of "ManpWinUI". When `SetMainViewModel` is called, it automatically updates the filename to include the fractal/bookmark name plus timestamp (e.g., `Mandelbrot_20240115_143052.mp4` or `MyFavorite_20240115_143052.mp4`).

**File**: `ManpWinUI\Views\Animation\AnimationControlPanel.xaml.cs`

Removed the problematic code from `OnLoaded`:
```csharp
// Before (WRONG):
private void OnLoaded(object sender, RoutedEventArgs e)
{
    // This creates a NEW instance!
    var mainViewModel = App.Current.Services.GetService<MainViewModel>();
    ViewModel.SetMainViewModel(mainViewModel);
}

// After (CORRECT):
private void OnLoaded(object sender, RoutedEventArgs e)
{
    // MainViewModel is already injected by MainPage
    // No need to fetch from DI here
}
```

### Instance Flow (After Fix)
```
MainPage.cs:
→ Gets MainViewModel instance #1 from DI
→ User adjusts colors, zoom, viewport
→ Instance #1 has custom purple palette, zoom 500×, center (-0.75, 0.1)
→ Passes instance #1 to AnimationPanel via SetMainViewModel()

AnimationControlPanel:
→ Receives instance #1 (SAME INSTANCE!)
→ Has access to: purple palette, zoom 500×, center (-0.75, 0.1)

Animation renders:
→ Uses instance #1's current state
→ BuildRenderParametersFromMainView() reads actual values
✓ User sees their custom purple palette and viewport!
```

## Benefits

### 1. **Correct State Capture**
- Animation uses the **actual** current fractal view
- Colors, zoom, center, max iterations all match what the user sees
- Extended parameters (Julia C, algorithm settings) properly captured

### 2. **Consistent UX**
- "What you see is what you animate"
- No mysterious reversion to defaults
- Predictable behavior

### 3. **Proper DI Pattern**
- MainPage owns the primary MainViewModel instance
- Child components receive it via explicit injection
- Clear ownership and lifetime semantics

### 4. **Maintains Transient Pattern**
- MainViewModel remains Transient (important for potential multi-window scenarios)
- Each window/page gets its own instance
- Explicit passing avoids accidental cross-instance pollution

### 5. **Smart Default Filenames**
- Filenames now include the fractal or bookmark name
- Example: `Mandelbrot_20240115_143052.mp4` instead of `ManpWinUI_Animation_20240115_143052.mp4`
- Makes exported files easier to identify and organize
- Uses bookmark/visualization name if available, otherwise fractal type

## Technical Details

### Why MainViewModel is Transient
From `App.xaml.cs` line 135:
```csharp
services.AddTransient<MainViewModel>();
```

Transient is correct for MainViewModel because:
- Each page/window should have its own fractal state
- Supports potential multi-window scenarios
- Clear separation of concerns

### Why AnimationViewModel is Singleton
From `App.xaml.cs` line 139:
```csharp
services.AddSingleton<AnimationViewModel>();
```

Singleton is correct for AnimationViewModel because:
- Animation state must persist across tab switches
- Single background render task at a time
- Settings and output path shared across all uses

### Late-Binding Pattern
`AnimationViewModel` uses late binding for `MainViewModel`:
- Constructor doesn't require MainViewModel (avoids circular dependency)
- `SetMainViewModel(MainViewModel?)` called after construction
- Allows singleton AnimationViewModel to reference transient MainViewModel
- Testable (can inject mock MainViewModel)

## Code References

### AnimationViewModel.BuildRenderParametersFromMainView()
Lines 565-638 in `AnimationViewModel.cs`:
- Reads `_mainViewModel.SelectedFractalType`
- Reads `_mainViewModel.CenterX`, `CenterY`, `Zoom`
- Reads `_mainViewModel.SelectedPalette`
- Reads `_mainViewModel.ColorCycleSpeed`, `ColorOffset`, `UseSmoothColoring`
- Reads `_mainViewModel.CurrentParameters` for extended parameters
- **This is where the user's custom settings are captured**

### Fallback Behavior
If `_mainViewModel` is `null` (lines 567-580):
- Uses sensible defaults
- Logs warning: "MainViewModel not available, using default parameters"
- Returns basic Mandelbrot render with Classic palette

## Testing Scenarios

### Scenario 1: Custom Colors
1. Select "Sunset" or custom color palette
2. Start animation
3. **Expected**: Animation renders with selected colors, not blue

### Scenario 2: Custom Viewport
1. Zoom into interesting feature (zoom 500×, center (-0.75, 0.1))
2. Start animation
3. **Expected**: Animation starts from current viewport, not default (-0.5, 0)

### Scenario 3: Custom Iterations
1. Set max iterations to 2048
2. Start animation
3. **Expected**: Animation uses 2048 iterations, not default 512

### Scenario 4: Julia Mode
1. Enable Julia mode, set C = (0.285, 0.01)
2. Start animation
3. **Expected**: Animation renders Julia set with correct C value

### Scenario 5: Extended Parameters
1. Select fractal with custom parameters (e.g., Newton's bailout)
2. Adjust parameter values
3. Start animation
4. **Expected**: Animation uses adjusted parameter values

## Alternative Approaches Considered

### ❌ Option A: Make MainViewModel Singleton
- **Rejected**: Breaks multi-window scenarios
- Each window needs its own fractal state
- Global singleton would share state across windows

### ❌ Option B: Store Settings in Separate Service
- Store viewport/colors in AnimationSettingsService
- **Rejected**: Duplicates state, adds complexity
- Would need continuous sync between MainViewModel and service

### ❌ Option C: Clone MainViewModel State
- Copy all properties when animation starts
- **Rejected**: Fragile, easy to miss properties
- Extended parameters make this error-prone

### ✅ Option D: Pass MainViewModel Reference (Chosen)
- Simple, direct, explicit
- No state duplication
- Clear ownership model
- Leverages existing `SetMainViewModel()` pattern

## Future Considerations

### Multi-Window Support
If the app adds multiple windows in the future:
- Each window has its own MainPage with its own MainViewModel
- Each MainPage passes its MainViewModel to its AnimationPanel
- Animations in window A use window A's state
- Animations in window B use window B's state
- Pattern scales naturally

### Animation Queue
If multiple animations can be queued:
- Capture MainViewModel state at queue-time
- Clone RenderParameters into queue entry
- Queue entry is immutable snapshot
- Allows user to change view while queue processes

## Related Documentation

- See `AnimationStatePersistenceFix.md` for tab-switching persistence
- See `AnimationMainViewIntegration.md` for "Sync from Current View" feature
- See `AnimationCompactCancelControl.md` for cancel button UX

## Commit Message

```
Fix animation to use current viewport and color palette

PROBLEM: Animations were rendering with default blue palette and
standard viewport instead of user's custom colors and zoom level.

ROOT CAUSE: AnimationControlPanel was fetching a new MainViewModel
instance from DI, which had default values. MainPage's MainViewModel
instance (with user's settings) was not being shared.

SOLUTION: MainPage now passes its MainViewModel instance directly to
AnimationControlPanel via SetMainViewModel() during initialization.
Removed problematic GetService call from AnimationControlPanel.OnLoaded.

RESULT: Animations now correctly capture:
- Current color palette
- Current viewport (center, zoom)
- Current max iterations
- Julia mode settings
- Extended parameters

Related: AnimationViewModel is singleton (for tab persistence)
but MainViewModel is transient (for multi-window support).
```
