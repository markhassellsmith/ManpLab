# Fractal Type Expansion - Task Definition

**Branch:** `feature/fractal-type-expansion`  
**Status:** Planning  
**Date:** January 2025  
**Priority:** High  

---

## 📋 Task Overview

**Goal:** Expand ManpWinUI from 2 fractal types (Mandelbrot, Julia) to support the full catalog of 240+ fractal types available in ManpWIN64, leveraging the existing FractalRegistry system already present in `ManpCore.Native`.

---

## 🎯 Current State Analysis

### What We Have
✅ **Deep zoom implementation** - Perturbation theory with BLA (just merged to `development`)  
✅ **FractalRegistry in native wrapper** - Already implemented in `ManpCore.Native/FractalRegistry.cpp`  
✅ **Family-based organization** - Fractals grouped into logical families  
✅ **240+ fractal implementations** - Complete ManpWIN64 catalog  

### What's Missing
❌ **UI integration** - No way to select from the full catalog in ManpWinUI  
❌ **Type discovery** - `GetAvailableFractalTypes()` returns placeholder data  
❌ **Family browsing** - No UI for browsing by family (Mandelbrot, Newton, IFS, etc.)  
❌ **Per-type parameters** - No support for type-specific settings (power, Julia constant, etc.)  
❌ **Type persistence** - Selected type not saved in settings  

---

## 🏗️ Implementation Plan

### Phase 1: Registry Integration (1-2 days)

#### Task 1.1: Expose Registry to Managed Code
**Files:**
- `ManpCore.Native/FractalRegistryWrapper.h/cpp` (already exists)
- `ManpCore.Native/FractalEngineWrapper.cpp` (update `GetAvailableFractalTypes()`)

**Changes:**
1. Remove placeholder implementation in `GetAvailableFractalTypes()`
2. Query actual FractalRegistry for all registered types
3. Return managed array of fractal names
4. Add `GetFractalsByFamily()` method
5. Add `GetFractalInfo(typeName)` method for metadata

**Example API:**
```cpp
// Get all fractals
array<String^>^ GetAvailableFractalTypes();

// Get fractals by family
array<String^>^ GetFractalsByFamily(String^ familyName);

// Get families
array<String^>^ GetFractalFamilies();

// Get detailed info
FractalTypeInfo^ GetFractalInfo(String^ typeName);
```

#### Task 1.2: Create FractalTypeInfo Class
**File:** `ManpCore.Native/FractalEngineWrapper.h`

**Add managed class:**
```cpp
public ref class FractalTypeInfo
{
public:
    property String^ Name;           // e.g., "Mandelbrot Power 2"
    property String^ Family;         // e.g., "Mandelbrot Family"
    property String^ Description;    // Brief description
    property bool SupportsJulia;     // Can generate Julia variant
    property bool SupportsPerturbation; // Works with deep zoom
    property int DefaultPower;       // Default exponent
    property int MaxIterations;      // Recommended max iterations
};
```

---

### Phase 2: UI Integration (2-3 days)

#### Task 2.1: Fractal Type Selector UI
**File:** `ManpWinUI/Views/Properties/FractalTypeView.xaml`

**Create new view:**
- ComboBox for family selection
- ListView of fractals in selected family
- Search/filter textbox
- Preview thumbnail (future enhancement)
- Description text

**Layout:**
```
┌─────────────────────────────────────┐
│ Family: [Mandelbrot ▼]              │
├─────────────────────────────────────┤
│ Search: [_________________] 🔍      │
├─────────────────────────────────────┤
│ ☑ Mandelbrot Power 2                │
│ ☐ Mandelbrot Power 3                │
│ ☐ Mandelbrot Power 4                │
│ ☐ Burning Ship                      │
│ ☐ Celtic Mandelbrot                 │
│   ...                               │
├─────────────────────────────────────┤
│ Description:                        │
│ Classic quadratic Mandelbrot set    │
│ z → z² + c                          │
└─────────────────────────────────────┘
```

#### Task 2.2: ViewModel Updates
**File:** `ManpWinUI/ViewModels/Properties/FractalTypeViewModel.cs`

**Add properties:**
- `ObservableCollection<string> FractalFamilies`
- `string SelectedFamily`
- `ObservableCollection<FractalTypeInfo> FractalsInFamily`
- `FractalTypeInfo SelectedFractalType`
- `string SearchFilter`

**Add commands:**
- `SelectFamilyCommand`
- `SelectFractalTypeCommand`
- `SearchCommand`

#### Task 2.3: Settings Persistence
**Files:**
- `ManpWinUI/Services/AppSettingsService.cs`
- `ManpWinUI/Services/IAppSettingsService.cs`

**Add settings:**
```csharp
string SelectedFractalType { get; set; }
string SelectedFractalFamily { get; set; }
```

---

### Phase 3: Parameter Support (2-3 days)

#### Task 3.1: Dynamic Parameter System
**Challenge:** Different fractal types need different parameters:
- **Mandelbrot Power N:** Needs `power` parameter
- **Newton fractals:** Need polynomial coefficients
- **Magnet fractals:** Need `order` parameter
- **IFS fractals:** Need transformation matrices

**Solution:** Extensible parameter system

**File:** `ManpCore.Native/FractalEngineWrapper.h`

**Add to FractalParameters:**
```cpp
// Generic parameter storage for type-specific settings
property Dictionary<String^, Object^>^ ExtendedParameters;
```

#### Task 3.2: UI for Type-Specific Parameters
**File:** `ManpWinUI/Views/Properties/FractalParametersView.xaml`

**Add dynamic parameter panel:**
- Shows/hides based on selected fractal type
- Generates UI controls from parameter metadata
- Validates input ranges

**Example for Mandelbrot Power N:**
```
Power: [2▼]  (2-16)
```

**Example for Newton fractals:**
```
Polynomial: z³ - 1
Root Color Mode: [Closest Root ▼]
```

---

### Phase 4: Rendering Integration (1-2 days)

#### Task 4.1: Update Calculate() Method
**File:** `ManpCore.Native/FractalEngineWrapper.cpp`

**Changes:**
1. Query FractalRegistry for selected type
2. Extract type-specific parameters from ExtendedParameters
3. Set up fractal calculation context
4. Handle perturbation compatibility check
5. Render using appropriate algorithm

#### Task 4.2: Perturbation Compatibility
**Challenge:** Only Mandelbrot and Power types support full BLA

**Solution:**
- Check `SupportspertPerturbation` flag
- Fall back to standard (non-BLA) rendering for unsupported types
- Show warning if user attempts deep zoom on unsupported type

---

### Phase 5: Documentation & Testing (1-2 days)

#### Task 5.1: Update Documentation
**Files to create/update:**
- `ManpWinUI/docs/FRACTAL_TYPE_CATALOG.md` - List all 240+ types
- `ManpWinUI/docs/ADDING_FRACTAL_TYPES.md` - Guide for adding new types
- `ManpWinUI/KEYBOARD_SHORTCUTS.md` - Add fractal switching shortcuts

#### Task 5.2: Testing Plan
**Test each fractal family:**
- [ ] Mandelbrot Family (15+ variants)
- [ ] Julia Sets
- [ ] Burning Ship Family
- [ ] Newton Fractals
- [ ] Phoenix Curves
- [ ] Magnet Fractals
- [ ] IFS Fractals
- [ ] Exotic Types

**Validation:**
- Renders without crashes
- Parameters apply correctly
- Persistence works
- Deep zoom compatibility checked
- UI responsive

---

## 🚀 Success Criteria

### Must Have (MVP)
- ✅ All 240+ fractal types accessible in UI
- ✅ Family-based browsing
- ✅ Search/filter functionality
- ✅ Type selection persisted
- ✅ Basic parameter support (power, Julia constant)
- ✅ Perturbation compatibility flagged

### Nice to Have (Future)
- ⏳ Preview thumbnails in type selector
- ⏳ Favorites/bookmarks system
- ⏳ Full parameter UI for Newton/IFS types
- ⏳ Formula editor for custom fractals
- ⏳ Batch rendering across types

---

## 📊 Effort Estimate

| Phase | Tasks | Estimated Days | Priority |
|-------|-------|----------------|----------|
| Phase 1: Registry Integration | 2 | 1-2 | 🔴 Critical |
| Phase 2: UI Integration | 3 | 2-3 | 🔴 Critical |
| Phase 3: Parameter Support | 2 | 2-3 | 🟡 High |
| Phase 4: Rendering Integration | 2 | 1-2 | 🔴 Critical |
| Phase 5: Documentation & Testing | 2 | 1-2 | 🟢 Medium |
| **Total** | **11 tasks** | **7-12 days** | |

---

## 🔗 Related Work

### Completed (from development branch)
- ✅ Deep zoom with perturbation theory
- ✅ Adaptive BLA tile sizing
- ✅ FractalRegistry implementation
- ✅ Native wrapper architecture

### Blocked Until This Completes
- Julia set deep zoom testing (needs Julia selection UI)
- Non-Mandelbrot deep zoom validation
- Full catalog showcase/gallery

---

## 🎯 First Step

**Start with Phase 1, Task 1.1:**  
Implement real `GetAvailableFractalTypes()` by querying FractalRegistry and returning actual type names instead of placeholder data.

**File to edit first:** `ManpCore.Native/FractalEngineWrapper.cpp`  
**Method to replace:** `FractalEngineWrapper::GetAvailableFractalTypes()`

---

**Ready to begin?** 🚀
