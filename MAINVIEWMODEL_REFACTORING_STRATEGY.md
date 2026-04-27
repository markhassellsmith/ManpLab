# MainViewModel Refactoring Strategy

**Goal:** Split monolithic MainViewModel into focused, composable ViewModels

**Current State:**
- MainViewModel.cs: 759 lines (includes core + some Mandelbrot)
- MainViewModel.StandardFractals.cs: 150 lines (partial class)
- MainViewModel.Hailstone.cs: 130 lines (partial class)
- **Total:** ~1,039 lines across 3 files

---

## 📊 Current Structure Analysis

### MainViewModel.cs (759 lines)
Contains mixed responsibilities:
- Image resolution/output
- Color palette selection
- Fractal type selection
- UI state (IsRendering, Progress)
- Bookmarks management
- Rendering coordination
- **Problem:** Too many concerns in one file

### MainViewModel.StandardFractals.cs (150 lines)
Contains Mandelbrot/Julia parameters:
- CenterX, CenterY, Zoom
- MaxIterations
- Julia mode toggle
- **Good:** Already separated, but as partial class

### MainViewModel.Hailstone.cs (130 lines)
Contains Hailstone parameters:
- StartX, StartY
- Sequence calculation
- Viewport management
- **Good:** Already separated, but as partial class

---

## 🎯 Refactoring Strategy

### Approach: Convert Partial Classes → Composition

**Current (Partial Classes):**
```
MainViewModel (partial)
├── MainViewModel.cs                    (759 lines)
├── MainViewModel.StandardFractals.cs   (150 lines) 
└── MainViewModel.Hailstone.cs          (130 lines)
```

**Target (Composition):**
```
ViewModels/
├── MainViewModel.cs                    (~150 lines - coordinator)
│   ├── References to child VMs
│   ├── Mode switching
│   ├── Shared UI state
│   └── Rendering coordination
│
└── Features/
    ├── MandelbrotViewModel.cs          (~200 lines)
    │   ├── From MainViewModel.StandardFractals.cs
    │   └── Mandelbrot-specific logic
    │
    ├── HailstoneViewModel.cs           (~200 lines)
    │   ├── From MainViewModel.Hailstone.cs
    │   └── Hailstone-specific logic
    │
    ├── RenderingViewModel.cs           (~150 lines)
    │   ├── Image resolution
    │   ├── IsRendering, Progress
    │   └── Render coordination
    │
    └── BookmarksViewModel.cs           (~100 lines)
        ├── Bookmark collection
        └── Bookmark management
```

---

## 📋 Step-by-Step Plan

### Phase 1: Create Feature ViewModels (Extract Logic)

#### Step 1.1: Create MandelbrotViewModel
- **Source:** `MainViewModel.StandardFractals.cs`
- **Extract:**
  - CenterX, CenterY, Zoom properties
  - MaxIterations
  - Julia mode toggle
  - RenderMandelbrotCommand logic
- **Keep in MainViewModel:**
  - Reference to MandelbrotViewModel
  - Mode switching logic

#### Step 1.2: Create HailstoneViewModel
- **Source:** `MainViewModel.Hailstone.cs`
- **Extract:**
  - StartX, StartY properties
  - Sequence result
  - Viewport management
  - RenderHailstoneCommand logic
- **Keep in MainViewModel:**
  - Reference to HailstoneViewModel
  - Mode switching logic

#### Step 1.3: Create RenderingViewModel
- **Source:** `MainViewModel.cs` (extract rendering state)
- **Extract:**
  - ImageWidth, ImageHeight
  - IsRendering, RenderProgress
  - StatusMessage, LastRenderTime
  - FractalImage (WriteableBitmap)
- **Keep in MainViewModel:**
  - Reference to RenderingViewModel
  - Delegation logic

#### Step 1.4: Create BookmarksViewModel
- **Source:** `MainViewModel.cs` (extract bookmarks)
- **Extract:**
  - Bookmarks collection
  - SelectedBookmark
  - IsBookmarksPanelOpen
  - Bookmark management methods
- **Keep in MainViewModel:**
  - Reference to BookmarksViewModel

### Phase 2: Update MainViewModel (Coordinator)

#### Step 2.1: Refactor to Composition
```csharp
public partial class MainViewModel : ObservableObject
{
    // Child ViewModels (composition)
    public MandelbrotViewModel Mandelbrot { get; }
    public HailstoneViewModel Hailstone { get; }
    public RenderingViewModel Rendering { get; }
    public BookmarksViewModel Bookmarks { get; }

    // App-level state
    [ObservableProperty]
    private string _selectedFractalType = "Mandelbrot";

    [ObservableProperty]
    private string _selectedPalette = "Classic";

    // Computed properties
    public bool IsMandelbrotMode => SelectedFractalType == "Mandelbrot";
    public bool IsHailstoneMode => SelectedFractalType == "Hailstone";

    // Mode switching
    partial void OnSelectedFractalTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsMandelbrotMode));
        OnPropertyChanged(nameof(IsHailstoneMode));
    }
}
```

### Phase 3: Update XAML Bindings

#### Step 3.1: Identify All Bindings
Search MainPage.xaml for patterns:
- `{Binding CenterX}` → `{Binding Mandelbrot.CenterX}`
- `{Binding ImageWidth}` → `{Binding Rendering.ImageWidth}`
- `{Binding IsRendering}` → `{Binding Rendering.IsRendering}`

#### Step 3.2: Update Systematically
- Search and replace by category
- Test each category before moving to next
- Use Find All References to catch code-behind

### Phase 4: Update Dependencies

#### Step 4.1: Constructor Injection
```csharp
public MainViewModel(
    IFractalRenderService renderService,
    BookmarkService bookmarkService,
    IHailstoneService hailstoneService)
{
    // Create child ViewModels
    Mandelbrot = new MandelbrotViewModel(renderService);
    Hailstone = new HailstoneViewModel(hailstoneService);
    Rendering = new RenderingViewModel();
    Bookmarks = new BookmarksViewModel(bookmarkService);
}
```

#### Step 4.2: Service Registration (App.xaml.cs)
```csharp
services.AddTransient<MainViewModel>();
services.AddTransient<MandelbrotViewModel>();
services.AddTransient<HailstoneViewModel>();
services.AddTransient<RenderingViewModel>();
services.AddTransient<BookmarksViewModel>();
```

---

## ⚠️ Risk Areas

### High Risk
1. **XAML Bindings** - Miss one and UI breaks silently
2. **Property Change Notifications** - Cross-VM dependencies
3. **Command References** - Buttons calling wrong commands

### Medium Risk
1. **Service Dependencies** - Ensuring correct injection
2. **Initialization Order** - Child VMs may depend on each other
3. **Event Subscriptions** - Memory leaks if not unsubscribed

### Low Risk
1. **Compile Errors** - Easy to catch and fix
2. **Logic Extraction** - Code is already well-separated

---

## ✅ Testing Strategy

### Phase Testing
After each phase:
1. **Build succeeds** - No compile errors
2. **App launches** - No runtime exceptions
3. **Mandelbrot renders** - Core functionality works
4. **Hailstone renders** - Secondary functionality works
5. **UI responds** - Buttons, sliders work correctly

### Comprehensive Testing
After complete refactoring:
1. **All fractal modes** - Mandelbrot, Julia, Hailstone
2. **All interactions** - Mouse zoom, pan, keyboard shortcuts
3. **All UI controls** - Resolution, palette, parameters
4. **Bookmarks** - Save, load, delete
5. **Edge cases** - Invalid input, rapid clicks

---

## 📝 Implementation Order

### Order of Operations (Safest Path)

1. **Create ViewModels/Features/ directory**
2. **Create BookmarksViewModel** (simplest, least dependencies)
3. **Test bookmarks still work**
4. **Create RenderingViewModel** (shared state)
5. **Test rendering still works**
6. **Create MandelbrotViewModel** (convert partial)
7. **Update XAML for Mandelbrot bindings**
8. **Test Mandelbrot mode**
9. **Create HailstoneViewModel** (convert partial)
10. **Update XAML for Hailstone bindings**
11. **Test Hailstone mode**
12. **Refactor MainViewModel to coordinator**
13. **Comprehensive testing**
14. **Remove old partial files**

---

## 🎯 Success Criteria

### Code Quality
- [ ] MainViewModel < 200 lines
- [ ] All feature VMs < 250 lines
- [ ] No partial classes (full composition)
- [ ] Clear single responsibility per VM

### Functionality
- [ ] All features work as before
- [ ] No regression in behavior
- [ ] Build succeeds with zero warnings
- [ ] All tests pass (when tests exist)

### Architecture
- [ ] Clean composition pattern
- [ ] Dependency injection throughout
- [ ] Event aggregator if needed
- [ ] Proper lifetime management

---

## 📚 Rollback Plan

If refactoring fails:
1. **Abandon branch** - Don't merge
2. **Learn from attempt** - Document what went wrong
3. **Revise strategy** - Try incremental approach
4. **Alternative:** Keep partial classes, just split MainViewModel.cs smaller

---

**Status:** Strategy Defined - Ready to Implement  
**Estimated Effort:** 4-6 hours (careful, methodical)  
**Risk Level:** Medium (core component, but well-planned)
