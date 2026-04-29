# Phase 2 Week 5: Fractal Browser Integration

**Status**: 🚧 In Progress  
**Branch**: `feature/phase2-week5-browser-integration`  
**Started**: January 2025

---

## Overview

**Goal**: Connect the Week 4 browser UI to the native FractalRegistry, replacing stub data with real fractal metadata from the C++ engine.

**Why**: The browser panel created in Week 4 displayed hardcoded test data. Week 5 bridges the WinUI frontend to the native registry so users can browse and select from all 14 registered fractals (and eventually 240+).

---

## Architecture

### Data Flow
```
Native C++ FractalRegistry (ManpCore.Native)
    ↓ (C++/CLI Wrapper)
ManpCore.Native.FractalRegistryWrapper
    ↓ (C# Service Layer)
FractalBrowserViewModel
    ↓ (XAML Binding)
FractalBrowserView UI
```

### Key Components

1. **Native Layer** (`ManpCore.Native`)
   - `FractalRegistry.h/.cpp` - Native registry (already exists)
   - `FractalRegistryWrapper.h/.cpp` - **NEW** C++/CLI bridge to expose registry to C#

2. **Managed Types**
   - `FractalInfo` - Managed ref class with Name, DisplayName, Category, Description, etc.
   - `FractalRegistryWrapper` - Static managed wrapper with methods like `GetCategories()`, `GetFractalsByCategory()`, `GetFractalInfo()`

3. **WinUI Layer** (`ManpWinUI`)
   - `App.xaml.cs` - Initializes registry at startup via `FractalRegistryWrapper.Initialize()`
   - `FractalBrowserViewModel.cs` - Loads real data from registry instead of stub data
   - `FractalBrowserView.xaml` - Displays categories and fractals with DisplayName + Description tooltips

---

## Tasks

### ✅ Task 1: Create C++/CLI Wrapper (COMPLETE)

**Files Created**:
- ✅ `ManpCore.Native/FractalRegistryWrapper.h` - Managed wrapper interface
- ✅ `ManpCore.Native/FractalRegistryWrapper.cpp` - Implementation with `msclr/marshal_cppstd.h`

**Key Features**:
- `Initialize()` - Calls `FractalRegistry::InitializeBuiltins()`
- `GetCategories()` - Returns `List<String^>` of category names
- `GetFractalsByCategory(category)` - Returns `List<FractalInfo^>` for a category
- `GetFractalInfo(name)` - Returns single `FractalInfo^` by name
- `IsRegistered(name)` - Check if fractal exists
- `GetCount()` - Total registered fractals

**Managed Type: FractalInfo**
Properties: Name, DisplayName, Category, Description, SupportsJulia, DefaultCenterX, DefaultCenterY, DefaultZoom

---

### ✅ Task 2: Initialize Registry at Startup (COMPLETE)

**File Modified**: `ManpWinUI/App.xaml.cs`

**Changes**:
- Added `InitializeFractalRegistry()` method called in `App()` constructor
- Calls `ManpCore.Native.FractalRegistryWrapper.Initialize()`
- Logs fractal count on success: `"FractalRegistry initialized with {Count} fractals"`
- Catches and logs errors if initialization fails

**Result**: Registry is ready before any UI components load.

---

### ✅ Task 3: Load Registry Data in Browser ViewModel (COMPLETE)

**File Modified**: `ManpWinUI/ViewModels/Browser/FractalBrowserViewModel.cs`

**Changes**:
- Replaced `LoadStubData()` with `LoadFromRegistry()`
- Calls `FractalRegistryWrapper.GetCategories()` to get all category names
- For each category, calls `GetFractalsByCategory()` to get fractals
- Populates `FractalCategoryNode` and `FractalNode` collections
- Expands "Classic Fractals" category by default
- Graceful error handling - falls back to empty state if registry fails

**FractalNode Model Updated**:
- Added `DisplayName` property for UI-friendly names (e.g., "Mandelbrot Set" instead of "Mandelbrot")
- Existing properties: `Name`, `Category`, `Description`, `ThumbnailPath`

---

### ✅ Task 4: Update Browser View (COMPLETE)

**File Modified**: `ManpWinUI/Views/Browser/FractalBrowserView.xaml`

**Changes**:
- Changed fractal button binding from `{x:Bind Name}` to `{x:Bind DisplayName}`
- Added tooltip binding: `ToolTipService.ToolTip="{x:Bind Description}"`

**Result**: Browser now displays human-readable names like "Mandelbrot Set" with descriptions on hover.

---

### ✅ Task 5: Build Verification (COMPLETE)

**Build Status**: ✅ **Successful**

**Project Changes**:
- `ManpCore.Native.vcxproj` updated with `FractalRegistryWrapper.h/.cpp`
- All C++/CLI interop marshalling compiles correctly
- WinUI app compiles with new registry integration

---

## Current State

### What Works
- ✅ FractalRegistry initializes at app startup
- ✅ Browser panel loads real categories and fractals from registry
- ✅ All 14 registered fractals visible in browser (Classic Fractals, Julia Sets, Multibrot Family, Newton Method, Magnet Fractals)
- ✅ DisplayName shows friendly names in UI
- ✅ Descriptions appear as tooltips when hovering over fractals
- ✅ Build compiles successfully

### Known Limitations (Week 5 Scope)
- ⏳ Clicking fractals doesn't load them yet (Week 5 Task 6)
- ⏳ No search/filtering implementation (Week 5 Task 7)
- ⏳ No fractal selection persistence (Week 5 Task 8)

---

## Next Steps

### Task 6: Fractal Selection & Loading
**Goal**: Wire fractal selection to actually load and render the selected fractal

**Files to Modify**:
- `FractalBrowserViewModel.cs` - Add `FractalSelected` event or command
- `MainViewModel.cs` - Handle fractal selection and trigger render
- `FractalRenderService.cs` - Update to support any fractal by name (not just Mandelbrot/Hailstone)

**Implementation**:
1. Add command or event to `FractalBrowserViewModel` when fractal button clicked
2. Pass selected fractal name to `MainViewModel`
3. Update render service to accept fractal name parameter
4. Load fractal metadata (default center/zoom) from registry
5. Trigger render with selected fractal

---

### Task 7: Search & Filtering
**Goal**: Make the search box functional

**Files to Modify**:
- `FractalBrowserViewModel.cs` - Implement `OnSearchQueryChanged` filtering logic

**Implementation**:
1. Store master category/fractal list
2. Filter fractals by name/displayName/description when `SearchQuery` changes
3. Update `Categories` observable collection with filtered results
4. Clear search button already exists (works)

---

### Task 8: Selection State & Persistence
**Goal**: Remember which fractal is selected across app restarts

**Files to Modify**:
- `FractalBrowserViewModel.cs` - Add selected fractal state
- `IAppSettingsService.cs` - Add `GetSelectedFractal()` / `SetSelectedFractal()`
- `AppSettingsService.cs` - Implement persistence

**Implementation**:
1. Save selected fractal name when selection changes
2. Restore selection on browser initialization
3. Highlight selected fractal in browser UI (visual state)

---

## Success Criteria for Week 5

- [x] FractalRegistry accessible from C# via C++/CLI wrapper
- [x] Registry initialized at application startup
- [x] Browser panel loads real categories and fractals from registry
- [x] DisplayName and Description visible in UI
- [ ] Clicking a fractal loads and renders it (Task 6)
- [ ] Search box filters fractals by name/description (Task 7)
- [ ] Selected fractal persists across app restarts (Task 8)
- [ ] All 14 registered fractals accessible from browser
- [ ] Clean build with no warnings/errors

**Current Progress**: 5/9 criteria complete (56%)

---

## Testing Plan

### Manual Testing (Task 6+)
1. Launch app → Browser should show 5 categories
2. Expand "Classic Fractals" → Should see Mandelbrot Set, Burning Ship, Tricorn, Phoenix
3. Expand "Julia Sets" → Should see San Marco, Douady Rabbit, Siegel Disk
4. Hover over fractals → Tooltips should show descriptions
5. Click a fractal → Should load and render (Task 6)
6. Type in search box → Categories/fractals should filter (Task 7)
7. Close and reopen app → Last selected fractal should be remembered (Task 8)

### Automated Testing (Future)
- Unit tests for `FractalRegistryWrapper` marshalling
- UI tests for browser selection workflow

---

## Notes

### Design Decisions

**Why C++/CLI Wrapper Instead of P/Invoke?**
- C++/CLI allows direct use of C++ types (std::vector, std::string) without manual marshalling
- Simpler memory management for complex types (FractalSpec)
- Consistent with existing `FractalEngineWrapper` pattern

**Why Initialize in App Constructor?**
- Registry must be ready before any UI components load
- Startup is one-time cost (~14 fractals register quickly)
- Logged initialization helps debug if registry fails

**Why DisplayName vs Name?**
- `Name` is unique identifier for code ("Mandelbrot", "BurningShip")
- `DisplayName` is human-readable for UI ("Mandelbrot Set", "Burning Ship")
- Allows refactoring internal names without breaking UI

---

## Related Documentation

- `Phase2-Week4-Progress.md` - 3-panel layout foundation (prerequisite)
- `PROJECT_PLAN.md` - Overall 12-week roadmap
- `ManpCore.Native/FractalRegistry.h` - Native registry API documentation
- `docs/Phase1-Week1-Progress.md` - FractalRegistry creation (Phase 1)

---

**Last Updated**: January 2025  
**Next Milestone**: Task 6 - Fractal Selection & Loading
