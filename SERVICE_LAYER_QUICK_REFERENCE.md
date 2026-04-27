# Service Layer Quick Reference
**For developers working with ManpLab service layer**

---

## 🏗️ **Architecture Overview**

```
┌─────────────────────────────────────────────────────────┐
│ ManpCore.Services (Shared, Platform-Agnostic)          │
│ ─────────────────────────────────────────────────────── │
│ • Algorithms: IHailstoneCalculator, HailstoneCalculator │
│ • Color: IColorPalette, HsvColorPalette                 │
│ • Models: HailstonePoint, HailstoneResult               │
└─────────────────────────────────────────────────────────┘
                         ▲
                         │ (references)
                         │
┌─────────────────────────────────────────────────────────┐
│ ManpWinUI.Services (WinUI-Specific)                     │
│ ─────────────────────────────────────────────────────── │
│ • IFractalRenderService → FractalRenderService          │
│ • IHailstoneService → HailstoneService                  │
│ • IHailstoneRenderService → HailstoneRenderServiceWin2D │
│ • IImageExportService → ImageExportService              │
│ • IHailstoneExportService → HailstoneExportService      │
│ • IBookmarkService → BookmarkService                    │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 **Service Catalog**

### **Shared Core Services**

#### **IHailstoneCalculator**
```csharp
// Pure algorithm - no UI dependencies
Task<HailstoneResult> CalculateSequenceAsync(
    int startX, int startY, 
    int maxIterations, 
    int colorSpread = 7);
```
**Purpose:** Calculates 2D Hailstone sequences with cycle detection  
**Implementation:** `HailstoneCalculator`  
**Location:** `ManpCore.Services.Algorithms`

#### **IColorPalette**
```csharp
// HSV spectrum generation
(byte R, byte G, byte B) GetColor(int degrees);
```
**Purpose:** Generates colors for trajectory visualization  
**Implementation:** `HsvColorPalette` (360-color spectrum)  
**Location:** `ManpCore.Services.Color`

---

### **WinUI Services**

#### **IFractalRenderService**
```csharp
// Mandelbrot/Julia rendering via native wrapper
Task<FractalRenderResult> RenderAsync(/* params */);
```
**Purpose:** Renders fractal images using C++ backend  
**Implementation:** `FractalRenderService`

#### **IHailstoneService**
```csharp
// Wraps shared calculator, adds CSV export
Task<HailstoneResult> CalculateSequenceAsync(
    int startX, int startY,
    int maxIterations,
    int colorSpread = 7,
    bool exportToCsv = false);
```
**Purpose:** UI-layer wrapper for Hailstone calculation  
**Implementation:** `HailstoneService`  
**Dependencies:** `IHailstoneCalculator`

#### **IHailstoneRenderService**
```csharp
// Renders Hailstone sequences to bitmaps
Task<HailstoneRenderResult> RenderSequenceAsync(
    HailstoneResult result,
    int width, int height,
    bool showAxes, bool showPoints, bool showLabels,
    /* viewport params */);
```
**Purpose:** Converts Hailstone sequences to visual output  
**Implementations:**
- **HailstoneRenderServiceWin2D** (default, GPU-accelerated)
- **HailstoneRenderService** (pixel-based fallback)
- **HailstoneRenderServiceRefactored** (experimental abstraction)

#### **IImageExportService**
```csharp
// Generic image export with metadata
Task<bool> SaveImageAsync(WriteableBitmap bitmap, 
    FractalMetadata metadata, ImageFormat format, IntPtr hwnd);
Task CopyToClipboardAsync(WriteableBitmap bitmap, 
    FractalMetadata metadata);
```
**Purpose:** Export fractal images to PNG/JPEG with embedded metadata  
**Implementation:** `ImageExportService`

#### **IHailstoneExportService**
```csharp
// Hailstone-specific export with overlays
Task<WriteableBitmap> CreateExportBitmapAsync(
    WriteableBitmap baseBitmap, HailstoneResult result);
Task<bool> ExportAsSvgAsync(HailstoneResult result, /* params */);
```
**Purpose:** Export Hailstone visualizations with info overlays  
**Implementation:** `HailstoneExportService`

#### **IBookmarkService**
```csharp
// Fractal bookmark management
IReadOnlyList<FractalBookmark> Bookmarks { get; }
Task LoadBookmarksAsync();
Task AddBookmarkAsync(FractalBookmark bookmark);
Task RemoveBookmarkAsync(string bookmarkId);
```
**Purpose:** Save/load fractal locations and presets  
**Implementation:** `BookmarkService`

---

## 🔌 **Dependency Injection**

### **Registration (App.xaml.cs):**
```csharp
// Shared Core
services.AddSingleton<IColorPalette, HsvColorPalette>();
services.AddSingleton<IHailstoneCalculator, HailstoneCalculator>();

// WinUI Services
services.AddSingleton<IFractalRenderService, FractalRenderService>();
services.AddSingleton<IHailstoneService, HailstoneService>();
services.AddSingleton<IHailstoneRenderService, HailstoneRenderServiceWin2D>();
services.AddSingleton<IImageExportService, ImageExportService>();
services.AddSingleton<IHailstoneExportService, HailstoneExportService>();
services.AddSingleton<IBookmarkService, BookmarkService>();

// ViewModels
services.AddTransient<MainViewModel>();
```

### **Consumption (ViewModels/Views):**
```csharp
public class MyViewModel(
    IHailstoneService hailstoneService,
    IHailstoneRenderService renderService,
    IBookmarkService bookmarkService)
{
    private readonly IHailstoneService _hailstoneService = hailstoneService;
    // ... use services via interfaces
}
```

---

## 🧪 **Testing**

### **Unit Testing (with Mocking):**
```csharp
[Fact]
public async Task Test_HailstoneCalculation()
{
    // Arrange
    var mockColorPalette = new Mock<IColorPalette>();
    mockColorPalette.Setup(p => p.GetColor(It.IsAny<int>()))
        .Returns((255, 0, 0));

    var calculator = new HailstoneCalculator(mockColorPalette.Object);

    // Act
    var result = await calculator.CalculateSequenceAsync(1, 1, 1000);

    // Assert
    Assert.True(result.HasCycle);
    Assert.NotEmpty(result.Sequence);
}
```

### **Integration Testing:**
```csharp
[Fact]
public async Task Test_EndToEnd_HailstoneRendering()
{
    // Use real services, mock only I/O
    var colorPalette = new HsvColorPalette();
    var calculator = new HailstoneCalculator(colorPalette);
    var renderService = new HailstoneRenderServiceWin2D();

    var calcResult = await calculator.CalculateSequenceAsync(7, 11, 1000);
    var renderResult = await renderService.RenderSequenceAsync(
        calcResult, 800, 600, true, true, false);

    Assert.NotNull(renderResult.Bitmap);
}
```

---

## 🔄 **Service Lifecycle**

### **Singleton vs Transient:**
- **Singletons** (one instance, app lifetime):
  - All services (stateless, reusable)
  - Color palette (static data)
  - Calculator (pure functions)

- **Transient** (new instance per request):
  - ViewModels (stateful, per-view)

---

## 🚀 **Adding New Services**

### **1. Create Interface:**
```csharp
// ManpWinUI/Services/IMyNewService.cs
public interface IMyNewService
{
    Task<MyResult> DoSomethingAsync(MyInput input);
}
```

### **2. Implement Service:**
```csharp
// ManpWinUI/Services/MyNewService.cs
public class MyNewService : IMyNewService
{
    private readonly IOtherService _otherService;

    public MyNewService(IOtherService otherService)
    {
        _otherService = otherService;
    }

    public async Task<MyResult> DoSomethingAsync(MyInput input)
    {
        // Implementation
    }
}
```

### **3. Register in DI:**
```csharp
// App.xaml.cs ConfigureServices()
services.AddSingleton<IMyNewService, MyNewService>();
```

### **4. Inject into Consumers:**
```csharp
public MyViewModel(IMyNewService myService)
{
    _myService = myService;
}
```

---

## 📚 **Related Documentation**

- **SERVICE_LAYER_ASSESSMENT.md** - Original assessment and refactoring plan
- **PHASE_0_1_IMPLEMENTATION_SUMMARY.md** - Implementation details
- **ARCHITECTURE_README.md** - Graphics rendering architecture
- **GRAPHICS_RENDERING.md** - Detailed rendering documentation

---

## ❓ **FAQ**

**Q: Why are there multiple IHailstoneRenderService implementations?**  
A: Different rendering backends for performance optimization:
- Win2D: GPU-accelerated (default, best performance)
- Pixel: CPU-based (fallback, debugging)
- Refactored: Graphics abstraction experiment

**Q: Should I use ManpWinUI.Models or ManpCore.Services.Models?**  
A: Use ManpCore.Services.Models for platform-agnostic types (HailstonePoint, HailstoneResult). Use ManpWinUI.Models for UI-specific types (FractalMetadata, HailstoneRenderResult).

**Q: Can I inject services directly in Views?**  
A: Prefer injecting into ViewModels. Views can access services via ViewModel properties or App.Current.Services.GetRequiredService<T>() if necessary.

**Q: How do I switch Hailstone render backends?**  
A: Change the registration in App.xaml.cs:
```csharp
// Win2D (default)
services.AddSingleton<IHailstoneRenderService, HailstoneRenderServiceWin2D>();

// Pixel backend
services.AddSingleton<IHailstoneRenderService, HailstoneRenderService>();
```

---

**Questions? See the implementation summary or service assessment docs.**
