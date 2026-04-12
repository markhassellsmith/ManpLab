# Architecture and Design Considerations

This document describes the architectural decisions, UI component choices, MVVM patterns, and integration strategies for the ManpLab WinUI modernization project.

---

## Architecture Overview

### Hybrid C++/C# Architecture

**Decision:** Keep C++ math engine, build new WinUI interface in C#

**Architecture Layers:**
- **C++ Core:** Math engine, parsers, algorithms (existing code)
- **C# WinUI:** User interface, application logic, data binding
- **Interop Layer:** C++/CLI wrapper or native P/Invoke
- **Benefits:** Keep proven algorithms, modern UI, maintain performance

**Rationale:**
- C++ code is proven, optimized, and mathematically complex
- Rewriting 150+ source files of mathematical code is high-risk
- WinUI provides modern UI without sacrificing performance
- Clear separation between computation (C++) and presentation (C#)

---

## Recommended Project Structure

```
ManpWinUI (C# WinUI)
├── Views/ (XAML)
├── ViewModels/ (MVVM)
├── Models/ (data structures)
├── Services/ (business logic)
└── Interop/ (C++ wrapper)

ManpCore.Native (C++/CLI wrapper)
└── Wraps existing C++ libraries

ManpWIN64 (existing C++)
├── Math engine
├── Parser
└── Algorithms
```

**Key Components:**

1. **ManpWinUI (C# WinUI 3)**
   - XAML views and user interface
   - ViewModels with data binding
   - Platform-specific service implementations
   - Application startup and configuration

2. **ManpCore.Native (C++/CLI)**
   - Wrapper around existing C++ code
   - Managed/unmanaged type marshalling
   - Performance-critical bridge layer

3. **ManpWIN64 (Existing C++)**
   - Mathematical engine (unchanged)
   - Formula parser
   - High-precision arithmetic
   - Fractal calculation kernels

---

## UI Components

### Modern WinUI Controls to Use

**Navigation & Layout:**
- `NavigationView` - Main app navigation with hierarchical menu
- `SplitView` - Side panels for parameters and settings
- `Grid`, `StackPanel`, `RelativePanel` - Layout containers

**Input Controls:**
- `NumberBox` - Parameter entry with validation
- `Slider` - Real-time parameter adjustment with visual feedback
- `ComboBox` - Fractal type selection, coloring method selection
- `TextBox` - Coordinate entry, formula input
- `Button` - Command execution with icons
- `ToggleButton` - Feature toggles (perturbation, BLA, etc.)

**Display Controls:**
- `Image` - Fractal display (bound to WriteableBitmap)
- `Canvas` - Overlay for zoom rectangle, coordinate display
- `ScrollViewer` - Zoomable/scrollable content

**Feedback Controls:**
- `ProgressBar` - Rendering progress
- `ProgressRing` - Indeterminate loading
- `InfoBar` - Status messages and errors
- `TeachingTip` - Contextual help for new users

**Advanced Controls:**
- `MenuBar` - Top-level menus (File, Edit, View, etc.)
- `CommandBar` - Toolbar with grouped commands
- `ColorPicker` - Palette editor
- `ListView`/`GridView` - Fractal preset gallery
- `ContentDialog` - Modal dialogs when necessary
- `Flyout` - Context menus and quick actions

---

### Replacing Clunky Patterns

**Before (ManpWIN64) → After (ManpWinUI)**

| Old Pattern | Issues | New Pattern | Benefits |
|-------------|--------|-------------|----------|
| Modal parameter dialogs | Blocks workflow, nested dialogs | Side panel with live update | Non-blocking, immediate feedback |
| Deep menu hierarchies | Hard to discover features | CommandBar + NavigationView | Visual, organized, searchable |
| Text-only parameter entry | No validation feedback | NumberBox + Slider | Range validation, visual adjustment |
| No undo | Can't revert mistakes | Command pattern with undo stack | Safe experimentation |
| Clipboard-only sharing | Limited integration | Windows Share contract | Modern OS integration |
| INI configuration | Difficult to extend | JSON with strong typing | Structured, extensible |
| GDI rendering | Slow, no GPU | WriteableBitmap + DirectX | GPU-accelerated, smooth |

---

## Data Binding & MVVM

### ViewModels

**Core ViewModels:**

1. **MainViewModel** - Overall application state
   - Current fractal type
   - Global application commands (New, Open, Save, Exit)
   - Navigation state

2. **FractalParametersViewModel** - Current fractal settings
   - Mathematical parameters (center, zoom, iterations)
   - Precision selection (double, quad-double, MPFR)
   - Perturbation and BLA settings
   - Validation rules
   - Property change notifications

3. **RenderViewModel** - Rendering progress and controls
   - Start/Stop/Cancel commands
   - Progress percentage
   - Estimated time remaining
   - Current rendering status

4. **PaletteViewModel** - Color palette editor
   - Palette color list (ObservableCollection)
   - Color manipulation commands
   - Gradient generation
   - Import/Export palette

5. **AnimationViewModel** - Animation sequencing
   - Keyframe list
   - Timeline controls (play, pause, scrub)
   - Interpolation settings
   - Export settings (resolution, codec, framerate)

6. **PresetGalleryViewModel** - Fractal preset browser
   - Preset collection
   - Thumbnail generation
   - Search/filter functionality
   - Load preset command

---

### Observable Properties

**Bind these properties to UI controls:**

```csharp
// FractalParametersViewModel
[ObservableProperty]
private double centerX;

[ObservableProperty]
private double centerY;

[ObservableProperty]
private double zoom;

[ObservableProperty]
private int maxIterations;

[ObservableProperty]
private string selectedFractalType;

[ObservableProperty]
private PrecisionType precision;

// RenderViewModel
[ObservableProperty]
private double renderProgress;

[ObservableProperty]
private bool isRendering;

[ObservableProperty]
private string statusMessage;

// PaletteViewModel
[ObservableProperty]
private ObservableCollection<Color> paletteColors;

[ObservableProperty]
private int selectedColorIndex;
```

**Binding Examples:**

```xaml
<!-- NumberBox with two-way binding and validation -->
<NumberBox 
    Header="Max Iterations"
    Value="{x:Bind ViewModel.MaxIterations, Mode=TwoWay}"
    Minimum="1"
    Maximum="1000000"
    SpinButtonPlacementMode="Inline" />

<!-- ProgressBar bound to rendering progress -->
<ProgressBar 
    Value="{x:Bind ViewModel.RenderProgress, Mode=OneWay}"
    IsIndeterminate="{x:Bind ViewModel.IsRendering, Mode=OneWay}"
    ShowError="False" />

<!-- Button with command binding -->
<Button 
    Content="Render"
    Command="{x:Bind ViewModel.RenderCommand}"
    IsEnabled="{x:Bind ViewModel.CanRender, Mode=OneWay}" />
```

---

### Commands

**Implement commands using CommunityToolkit.Mvvm:**

```csharp
// Synchronous command
[RelayCommand]
private void ZoomIn()
{
    Zoom *= 2.0;
    NotifyParametersChanged();
}

// Asynchronous command with CanExecute
[RelayCommand(CanExecute = nameof(CanRender))]
private async Task RenderAsync()
{
    IsRendering = true;
    try
    {
        var parameters = CreateFractalParameters();
        var result = await _fractalService.CalculateAsync(parameters, _cancellationTokenSource.Token);
        UpdateBitmap(result.PixelData);
    }
    finally
    {
        IsRendering = false;
    }
}

private bool CanRender() => !IsRendering;

// Command with parameter
[RelayCommand]
private void LoadPreset(FractalPreset preset)
{
    ApplyPreset(preset);
    RenderCommand.Execute(null);
}

// Cancellable command
[RelayCommand]
private void CancelRender()
{
    _cancellationTokenSource?.Cancel();
}
```

**Key Commands:**

| Command | Purpose | Scope |
|---------|---------|-------|
| `RenderCommand` | Start fractal calculation | RenderViewModel |
| `CancelRenderCommand` | Stop rendering | RenderViewModel |
| `ZoomInCommand` / `ZoomOutCommand` | Adjust zoom level | FractalParametersViewModel |
| `ExportImageCommand` | Save PNG | MainViewModel |
| `ExportAnimationCommand` | Save video | AnimationViewModel |
| `LoadPresetCommand` | Load .MAP/.PAR file | MainViewModel |
| `SavePresetCommand` | Save current state | MainViewModel |
| `UndoCommand` / `RedoCommand` | Undo/redo parameters | MainViewModel |
| `AddKeyframeCommand` | Add animation keyframe | AnimationViewModel |
| `SelectColorCommand` | Pick palette color | PaletteViewModel |

---

## Integration Points

### C++ to C# Bridge

**Option 1: C++/CLI Wrapper (Recommended)**

**Advantages:**
- Easier marshalling of complex types (Complex, BigDouble)
- Direct memory access for image buffers
- Mixed-mode debugging (step from C# into C++)
- Better performance for frequent calls
- Can gradually refactor if needed

**Disadvantages:**
- Slightly larger binary size
- Windows-only (no cross-platform)
- Requires /clr compilation

**Implementation:**

```cpp
// ManpCore.Native (C++/CLI)
public ref class FractalEngine
{
public:
    FractalEngine();
    
    void Calculate(FractalParameters^ parameters);
    array<Byte>^ GetImageData();
    void Cancel();
    
    event EventHandler<ProgressEventArgs^>^ ProgressChanged;
    
private:
    // Native C++ engine
    std::unique_ptr<::FractalEngine> m_nativeEngine;
};

// Complex type marshalling
public value struct Complex
{
    double Real;
    double Imaginary;
    
    // Constructor from native Complex
    Complex(const ::Complex& native)
    {
        Real = native.x;
        Imaginary = native.y;
    }
    
    // Convert to native Complex
    ::Complex ToNative()
    {
        return ::Complex(Real, Imaginary);
    }
};
```

**Usage in C#:**

```csharp
// Consume C++/CLI wrapper
using ManpCore.Native;

var engine = new FractalEngine();
engine.ProgressChanged += OnProgressChanged;

var parameters = new FractalParameters
{
    Center = new Complex { Real = -0.5, Imaginary = 0.0 },
    Zoom = 1.0,
    MaxIterations = 1000
};

engine.Calculate(parameters);
byte[] imageData = engine.GetImageData();
```

---

**Option 2: P/Invoke (Alternative)**

**Advantages:**
- Standard .NET interop mechanism
- More portable (works on any .NET platform)
- Smaller binary size
- No /clr compilation required

**Disadvantages:**
- Requires careful marshalling of complex types
- Manual memory management for buffers
- More boilerplate code
- Harder to debug across boundary

**Implementation:**

```cpp
// Export C functions from C++ DLL
extern "C" {
    __declspec(dllexport) void* CreateFractalEngine();
    __declspec(dllexport) void DestroyFractalEngine(void* engine);
    __declspec(dllexport) void CalculateFractal(void* engine, FractalParams* params);
    __declspec(dllexport) int GetImageData(void* engine, unsigned char* buffer, int size);
}
```

```csharp
// P/Invoke declarations in C#
[DllImport("ManpCore.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern IntPtr CreateFractalEngine();

[DllImport("ManpCore.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern void DestroyFractalEngine(IntPtr engine);

[DllImport("ManpCore.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern void CalculateFractal(IntPtr engine, ref FractalParams parameters);

[DllImport("ManpCore.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int GetImageData(IntPtr engine, byte[] buffer, int size);

// Wrapper class
public class FractalEngine : IDisposable
{
    private IntPtr _nativeEngine;
    
    public FractalEngine()
    {
        _nativeEngine = CreateFractalEngine();
    }
    
    public void Calculate(FractalParameters parameters)
    {
        var nativeParams = ConvertToNative(parameters);
        CalculateFractal(_nativeEngine, ref nativeParams);
    }
    
    public byte[] GetImageData(int width, int height)
    {
        byte[] buffer = new byte[width * height * 4];
        GetImageData(_nativeEngine, buffer, buffer.Length);
        return buffer;
    }
    
    public void Dispose()
    {
        if (_nativeEngine != IntPtr.Zero)
        {
            DestroyFractalEngine(_nativeEngine);
            _nativeEngine = IntPtr.Zero;
        }
    }
}
```

**Recommendation:** Use **C++/CLI** for this project. The easier debugging and better type marshalling outweigh the portability concerns (Windows-only is acceptable for this desktop app).

---

### Key Interfaces to Expose

Define these interfaces in C# for abstraction:

```csharp
// Core fractal calculation
public interface IFractalEngine
{
    Task<FractalResult> CalculateAsync(
        FractalParameters parameters, 
        CancellationToken cancellationToken = default);
    
    event EventHandler<ProgressEventArgs> ProgressChanged;
}

public class FractalResult
{
    public byte[] PixelData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public TimeSpan RenderTime { get; set; }
    public long IterationCount { get; set; }
}

// Formula parsing
public interface IFormulaParser
{
    bool ParseFormula(string formula, out string errorMessage);
    FractalType[] GetAvailableTypes();
    string[] GetBuiltInFunctions();
}

// High-precision arithmetic (if needed in C#)
public interface IArithmeticProvider
{
    void SetPrecision(int decimals);
    IComplexNumber Add(IComplexNumber a, IComplexNumber b);
    IComplexNumber Multiply(IComplexNumber a, IComplexNumber b);
    // ... other operations
}

// Progress reporting
public class ProgressEventArgs : EventArgs
{
    public double Percentage { get; set; }
    public int CurrentLine { get; set; }
    public int TotalLines { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public TimeSpan EstimatedRemaining { get; set; }
}
```

---

### Performance Considerations

**Efficient Data Transfer:**

```csharp
// ✅ GOOD - Use Memory<byte> for large buffers
public async Task<Memory<byte>> GetImageDataAsync()
{
    // Avoid copying large arrays
    return new Memory<byte>(_imageBuffer);
}

// ✅ GOOD - Use Span<T> for zero-copy operations
public void UpdateBitmap(Span<byte> pixelData)
{
    using var buffer = _bitmap.PixelBuffer.AsStream();
    pixelData.CopyTo(buffer);
}

// ❌ BAD - Creating unnecessary copies
public byte[] GetImageData()
{
    byte[] copy = new byte[_imageBuffer.Length];
    Array.Copy(_imageBuffer, copy, _imageBuffer.Length);
    return copy; // Wasteful copy
}
```

**Async/Await for Long Calculations:**

```csharp
// Run calculation on background thread
public async Task<FractalResult> CalculateAsync(
    FractalParameters parameters, 
    CancellationToken cancellationToken)
{
    return await Task.Run(() =>
    {
        // Call into C++ engine
        _nativeEngine.Calculate(parameters);
        
        // Check cancellation periodically
        cancellationToken.ThrowIfCancellationRequested();
        
        return new FractalResult
        {
            PixelData = _nativeEngine.GetImageData(),
            RenderTime = _stopwatch.Elapsed
        };
    }, cancellationToken);
}
```

**Progress Reporting with IProgress<T>:**

```csharp
public async Task CalculateAsync(
    FractalParameters parameters,
    IProgress<ProgressEventArgs> progress,
    CancellationToken cancellationToken)
{
    await Task.Run(() =>
    {
        for (int line = 0; line < height; line++)
        {
            // Render line in C++
            _nativeEngine.RenderLine(line);
            
            // Report progress every 10 lines
            if (line % 10 == 0)
            {
                progress?.Report(new ProgressEventArgs
                {
                    Percentage = (line * 100.0) / height,
                    CurrentLine = line,
                    TotalLines = height
                });
            }
            
            cancellationToken.ThrowIfCancellationRequested();
        }
    }, cancellationToken);
}

// Usage in ViewModel
var progress = new Progress<ProgressEventArgs>(e =>
{
    RenderProgress = e.Percentage;
    StatusMessage = $"Line {e.CurrentLine} of {e.TotalLines}";
});

await _fractalService.CalculateAsync(parameters, progress, _cancellationTokenSource.Token);
```

**CancellationToken for Stop Functionality:**

```csharp
// ViewModel
private CancellationTokenSource _cancellationTokenSource;

[RelayCommand]
private async Task RenderAsync()
{
    _cancellationTokenSource = new CancellationTokenSource();
    
    try
    {
        await _fractalEngine.CalculateAsync(Parameters, _cancellationTokenSource.Token);
    }
    catch (OperationCanceledException)
    {
        StatusMessage = "Rendering cancelled";
    }
}

[RelayCommand]
private void CancelRender()
{
    _cancellationTokenSource?.Cancel();
}

// In C++ wrapper
void FractalEngine::Calculate(FractalParameters^ params)
{
    m_cancelled = false;
    
    for (int y = 0; y < height; y++)
    {
        // Check cancellation flag
        if (m_cancelled)
            throw gcnew OperationCanceledException();
        
        RenderLine(y);
    }
}

void FractalEngine::Cancel()
{
    m_cancelled = true;
}
```

---

## Platform-Agnostic Design (MAUI-Ready)

For future cross-platform expansion, follow these guidelines:

**✅ DO:**
- Keep ViewModels 100% platform-independent (no WinUI references)
- Use interfaces for platform services (IFileService, IBitmapService)
- Separate business logic from UI code
- Use CommunityToolkit.Mvvm (works in both WinUI and MAUI)

**❌ AVOID:**
- Direct WinUI control references in ViewModels
- `Microsoft.UI.Xaml.*` namespaces in shared code
- Windows-specific APIs in business logic
- WinUI-specific threading (DispatcherQueue) in ViewModels

**Example Abstraction:**

```csharp
// Platform-agnostic interface (in ManpLab.Core)
public interface IFileService
{
    Task<Stream> OpenFileAsync(string[] fileTypes);
    Task<bool> SaveFileAsync(string path, byte[] data);
}

// WinUI implementation (in ManpWinUI)
public class WinUIFileService : IFileService
{
    public async Task<Stream> OpenFileAsync(string[] fileTypes)
    {
        var picker = new FileOpenPicker();
        foreach (var type in fileTypes)
            picker.FileTypeFilter.Add(type);
        
        var file = await picker.PickSingleFileAsync();
        return file != null ? await file.OpenStreamForReadAsync() : null;
    }
}

// ViewModel uses interface only (platform-agnostic)
public class MainViewModel
{
    private readonly IFileService _fileService;
    
    public MainViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var stream = await _fileService.OpenFileAsync(new[] { ".map", ".par" });
        if (stream != null)
            await LoadParametersAsync(stream);
    }
}
```

See [MAUI Compatibility](07-maui-compatibility.md) for detailed guidance.

---

## Dependency Injection

Use Microsoft.Extensions.DependencyInjection for service management:

**App.xaml.cs:**

```csharp
public partial class App : Application
{
    private IServiceProvider _serviceProvider;
    
    public App()
    {
        this.InitializeComponent();
        
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    private void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<FractalParametersViewModel>();
        services.AddTransient<RenderViewModel>();
        services.AddTransient<PaletteViewModel>();
        
        // Services
        services.AddSingleton<IFractalEngine, NativeFractalEngine>();
        services.AddSingleton<IFileService, WinUIFileService>();
        services.AddSingleton<IBitmapService, WinUIBitmapService>();
        services.AddSingleton<ISettingsService, JsonSettingsService>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddFile("Logs/app-{Date}.txt");
        });
    }
    
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = _serviceProvider.GetRequiredService<MainWindow>();
        window.Activate();
    }
}
```

**ViewModel Injection:**

```csharp
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
    }
}

public class MainViewModel
{
    private readonly IFractalEngine _fractalEngine;
    private readonly IFileService _fileService;
    private readonly ILogger<MainViewModel> _logger;
    
    public MainViewModel(
        IFractalEngine fractalEngine,
        IFileService fileService,
        ILogger<MainViewModel> logger)
    {
        _fractalEngine = fractalEngine;
        _fileService = fileService;
        _logger = logger;
    }
}
```

---

## Error Handling

**Global Exception Handling:**

```csharp
public App()
{
    this.InitializeComponent();
    
    // Catch unhandled exceptions
    this.UnhandledException += OnUnhandledException;
    AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
}

private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    _logger.LogCritical(e.Exception, "Unhandled exception");
    
    // Show error dialog
    var dialog = new ContentDialog
    {
        Title = "Unexpected Error",
        Content = $"An error occurred: {e.Exception.Message}",
        CloseButtonText = "OK"
    };
    dialog.ShowAsync();
}
```

**Service-Level Error Handling:**

```csharp
public async Task<FractalResult> CalculateAsync(
    FractalParameters parameters,
    CancellationToken cancellationToken)
{
    try
    {
        _logger.LogInformation("Starting fractal calculation");
        var result = await _nativeEngine.CalculateAsync(parameters, cancellationToken);
        _logger.LogInformation("Calculation completed in {Time}ms", result.RenderTime.TotalMilliseconds);
        return result;
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("Calculation cancelled by user");
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Calculation failed: {Message}", ex.Message);
        throw new FractalCalculationException("Failed to calculate fractal", ex);
    }
}
```

---

## Testing Strategy

**Unit Tests for ViewModels:**

```csharp
public class FractalParametersViewModelTests
{
    [Fact]
    public void ZoomIn_DoublesZoom()
    {
        // Arrange
        var viewModel = new FractalParametersViewModel();
        viewModel.Zoom = 1.0;
        
        // Act
        viewModel.ZoomInCommand.Execute(null);
        
        // Assert
        Assert.Equal(2.0, viewModel.Zoom);
    }
    
    [Fact]
    public async Task RenderCommand_CallsFractalEngine()
    {
        // Arrange
        var mockEngine = new Mock<IFractalEngine>();
        var viewModel = new RenderViewModel(mockEngine.Object);
        
        // Act
        await viewModel.RenderCommand.ExecuteAsync(null);
        
        // Assert
        mockEngine.Verify(e => e.CalculateAsync(
            It.IsAny<FractalParameters>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

**Integration Tests for C++ Interop:**

```csharp
public class FractalEngineIntegrationTests
{
    [Fact]
    public void Calculate_StandardMandelbrot_ReturnsValidImage()
    {
        // Arrange
        var engine = new NativeFractalEngine();
        var parameters = new FractalParameters
        {
            FractalType = "Mandelbrot",
            Width = 800,
            Height = 600,
            MaxIterations = 1000
        };
        
        // Act
        var result = engine.Calculate(parameters);
        
        // Assert
        Assert.NotNull(result.PixelData);
        Assert.Equal(800 * 600 * 4, result.PixelData.Length);
    }
}
```

---

## Android Portability Constraints (MANDATORY)

**These are NOT optional guidelines - every design decision must validate against these constraints.**

The architecture is designed for future .NET MAUI Android portability with minimal effort (6-8 weeks migration timeline). All code written during Windows development MUST follow these rules to ensure 90%+ code reuse for mobile versions.

### ✅ DO (Required for All New Code):

**ViewModels:**
- ✅ Zero platform-specific dependencies (no `Microsoft.UI.Xaml.*` namespaces)
- ✅ Use `CommunityToolkit.Mvvm` exclusively (cross-platform compatible)
- ✅ Accept platform services via constructor injection (`IFileService`, `IBitmapService`, etc.)
- ✅ Keep ViewModels in separate project/folder that could compile without WinUI references

**Platform Services:**
- ✅ Define interfaces for ALL platform-specific functionality:
  - `IFileService` - File picking, saving, directory access
  - `IBitmapService` - Bitmap creation, WriteableBitmap abstraction
  - `INavigationService` - Page navigation, modal dialogs
  - `IDispatcherService` - UI thread dispatching
  - `IShareService` - Social media sharing (future Android feature)
- ✅ Implement services in platform-specific projects (`ManpWinUI`, future `ManpAndroid`)
- ✅ Register services via dependency injection

**Business Logic:**
- ✅ Keep fractal calculation, file I/O, and algorithms in shared code
- ✅ Use standard .NET types (`Stream`, `byte[]`, not platform-specific types)
- ✅ Separate data models from UI controls

**Data Binding:**
- ✅ Use MVVM patterns compatible with both WinUI and MAUI
- ✅ Observable properties with `[ObservableProperty]` attribute
- ✅ Commands with `[RelayCommand]` attribute
- ✅ Avoid code-behind in views (except platform-specific gestures)

### ❌ DON'T (Prohibited - Will Break Android Portability):

**ViewModels:**
- ❌ `DispatcherQueue` in ViewModels → Use `IDispatcherService` interface
- ❌ `WriteableBitmap` in business logic → Use `IBitmapService.CreateBitmap(byte[])`
- ❌ `ContentDialog` in ViewModels → Use `IDialogService.ShowAsync()`
- ❌ `Microsoft.UI.Xaml.*` references in ViewModels

**Platform APIs:**
- ❌ `Windows.Storage.*` in shared code → Use `IFileService`
- ❌ `Windows.System.*` in business logic → Use abstractions
- ❌ Direct WinUI control references in models/services

**C++/CLI for Android:**
- ❌ C++/CLI is Windows-only - Android requires different strategy:
  - **Option 1 (Recommended):** Cloud rendering (Azure VM hosts C++ engine, Android displays results)
  - **Option 2:** Cross-compile C++ with Android NDK + JNI wrapper (complex)
  - **Option 3:** Simplified C# engine for mobile (limited features)

### 📋 Android Portability Validation Checklist

**Before merging any code to `development`, verify:**

- [ ] All ViewModels have zero `Microsoft.UI.*` or `Windows.*` namespace references
- [ ] Platform-specific functionality uses interface abstractions
- [ ] Services registered via dependency injection (can be swapped for Android implementations)
- [ ] Business logic could compile in hypothetical `ManpLab.Core` shared library
- [ ] No `DispatcherQueue`, `WriteableBitmap`, or `ContentDialog` in non-UI code
- [ ] Code follows patterns from `07-maui-compatibility.md`

### 🎯 Android Migration Strategy (Post-v1.0)

**Timeline:** 6-8 weeks after Windows v1.0 completion

**Approach (Recommended):**
1. **Cloud Rendering Architecture:**
   - C++ math engine remains on Azure Windows VM (~$20/month)
   - Android app sends parameters via REST API
   - Server returns rendered images (JPEG compressed)
   - Local caching for offline browsing of previous renders

2. **Code Reuse Breakdown:**
   - **90% reusable:** ViewModels, business logic, data models, services (interfaces)
   - **10% platform-specific:** XAML → MAUI XAML, `WriteableBitmap` → MAUI `ImageSource`, file pickers

3. **New Platform Services for Android:**
   - `AndroidFileService : IFileService` (uses Android file picker)
   - `AndroidBitmapService : IBitmapService` (uses MAUI `ImageSource`)
   - `AndroidNavigationService : INavigationService` (uses MAUI `Shell`)
   - `AndroidDispatcherService : IDispatcherService` (uses `MainThread.BeginInvokeOnMainThread`)

**Alternative (If Cloud Not Desired):**
- Cross-compile C++ with Android NDK (ARM64)
- Create JNI wrapper (Java Native Interface)
- Much more complex, 2-3 months effort

### 🔒 Enforcement

**All code reviews must validate Android portability.**  
Violations of these constraints will block merge to `development` branch.

See [07-maui-compatibility.md](07-maui-compatibility.md) for complete cross-platform implementation patterns.

---

## Color Palette System Architecture

### Overview

The color palette system supports both **procedural** (math-generated) and **lookup table** (array-based) palettes, enabling flexibility for algorithmic color schemes and user-imported custom palettes.

**Design Goals:**
- Support both procedural and lookup table palettes
- Flexible import/export with multiple format support
- Community palette sharing capabilities
- "Baking" procedural palettes to lookup tables for export
- Extensible format provider architecture
- Performance-optimized native C++ palette interpolation

---

### Palette Types

**1. Procedural Palettes (Math-Generated)**

Colors computed algorithmically using mathematical formulas:

```cpp
// Example: Rainbow palette using HSV to RGB conversion
ColorRGB GetRainbowColor(double iteration, int maxIter)
{
    double t = (iteration / maxIter);
    double hue = t * 360.0;  // Full spectrum
    return HSVtoRGB(hue, 1.0, 1.0);
}
```

**Characteristics:**
- ✅ Infinite resolution (smooth at any zoom level)
- ✅ Minimal memory footprint (just code, no data)
- ✅ Mathematically precise gradients
- ❌ Cannot be exported directly (requires "baking")
- ❌ Limited to algorithmic patterns

**Use Cases:**
- Built-in classic palettes (Grayscale, Classic, Fire, Ocean, Rainbow, Psychedelic)
- Algorithmic color schemes (wave functions, fractals, spectrums)
- Real-time parameter-driven color generation

---

**2. Lookup Table Palettes (Array-Based)**

Discrete color arrays with interpolation:

```csharp
// Example: User's Spectrum360 palette (360 HSL colors generated in Excel)
public static ColorStruct[] Spectrum360 = {
    new ColorStruct { Red = 255, Green = 0, Blue = 0 },      // Pure red
    new ColorStruct { Red = 255, Green = 2, Blue = 0 },      // Red-orange
    new ColorStruct { Red = 255, Green = 4, Blue = 0 },      // ...
    // ... 360 total colors
};
```

**Characteristics:**
- ✅ Can represent any color sequence (not limited to formulas)
- ✅ Easy import/export (JSON, CSV, images, .MAP files)
- ✅ Consistent across platforms (exact RGB values)
- ✅ Community sharing (palette galleries)
- ❌ Fixed resolution (requires interpolation for smooth rendering)
- ❌ Higher memory usage (array storage)

**Use Cases:**
- User-created palettes in external tools (Excel, Photoshop, GIMP)
- Community-shared palettes (.MAP format from other fractal software)
- Gradient images extracted from photographs
- Classic fractal palette libraries

---

### Architecture Design

**Core Interfaces:**

```csharp
// Unified palette abstraction
public interface IPalette
{
    string Name { get; }
    string Author { get; }
    PaletteType Type { get; }  // Procedural or LookupTable

    ColorRGB GetColor(double iteration, int maxIterations);
    int GetColorCount();  // -1 for procedural (infinite)
}

// Procedural palette implementation
public class ProceduralPalette : IPalette
{
    public PaletteType Type => PaletteType.Procedural;

    private Func<double, int, ColorRGB> _colorFunction;

    public ProceduralPalette(string name, Func<double, int, ColorRGB> colorFunc)
    {
        Name = name;
        _colorFunction = colorFunc;
    }

    public ColorRGB GetColor(double iteration, int maxIterations)
    {
        return _colorFunction(iteration, maxIterations);
    }

    public int GetColorCount() => -1;  // Infinite resolution
}

// Lookup table palette implementation
public class LookupTablePalette : IPalette
{
    public PaletteType Type => PaletteType.LookupTable;

    private ColorRGB[] _colors;

    public LookupTablePalette(string name, ColorRGB[] colors)
    {
        Name = name;
        _colors = colors;
    }

    public ColorRGB GetColor(double iteration, int maxIterations)
    {
        // Smooth interpolation between discrete colors
        double index = (iteration / maxIterations) * (_colors.Length - 1);
        int i1 = (int)Math.Floor(index);
        int i2 = Math.Min(i1 + 1, _colors.Length - 1);
        double t = index - i1;

        return InterpolateColors(_colors[i1], _colors[i2], t);
    }

    public int GetColorCount() => _colors.Length;
}
```

---

### Format Provider System

**Extensible import/export architecture:**

```csharp
public interface IPaletteFormatProvider
{
    string Name { get; }                    // "JSON", "CSV", "Image", "MAP"
    string[] FileExtensions { get; }         // [".json"], [".csv"], [".png", ".jpg"]
    bool CanRead { get; }
    bool CanWrite { get; }

    Task<PaletteData> ImportAsync(Stream stream);
    Task ExportAsync(PaletteData palette, Stream stream);
}

public class PaletteData
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string[] Tags { get; set; }          // "fire", "cool", "spectrum"
    public DateTime CreatedDate { get; set; }
    public ColorEntry[] Colors { get; set; }
}

public struct ColorEntry
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public double Position { get; set; }  // 0.0 to 1.0 for gradient positions
}
```

**Supported Formats:**

| Format | Extension | Import | Export | Use Case |
|--------|-----------|--------|--------|----------|
| **JSON** | `.json` | ✅ | ✅ | Structured metadata, human-readable, web-friendly |
| **CSV** | `.csv` | ✅ | ✅ | Excel import/export, simple text format |
| **Image** | `.png`, `.jpg` | ✅ | ✅ | Extract colors from gradient images |
| **MAP** | `.map` | ✅ | ✅ | UltraFractal/Fractal eXtreme compatibility |
| **Photoshop ACO** | `.aco` | ✅ | ❌ | Import Photoshop swatches |

**Format Provider Examples:**

```csharp
// JSON format provider
public class JsonPaletteProvider : IPaletteFormatProvider
{
    public string Name => "JSON";
    public string[] FileExtensions => new[] { ".json" };
    public bool CanRead => true;
    public bool CanWrite => true;

    public async Task<PaletteData> ImportAsync(Stream stream)
    {
        return await JsonSerializer.DeserializeAsync<PaletteData>(stream);
    }

    public async Task ExportAsync(PaletteData palette, Stream stream)
    {
        await JsonSerializer.SerializeAsync(stream, palette, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}

// Image format provider (extract horizontal gradient)
public class ImagePaletteProvider : IPaletteFormatProvider
{
    public async Task<PaletteData> ImportAsync(Stream stream)
    {
        var bitmap = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
        var pixelProvider = await bitmap.GetPixelDataAsync();
        byte[] pixels = pixelProvider.DetachPixelData();

        // Sample colors from middle row of image
        var colors = new List<ColorEntry>();
        int width = (int)bitmap.PixelWidth;
        int y = (int)bitmap.PixelHeight / 2;

        for (int x = 0; x < width; x++)
        {
            int offset = (y * width + x) * 4;
            colors.Add(new ColorEntry
            {
                R = pixels[offset],
                G = pixels[offset + 1],
                B = pixels[offset + 2],
                Position = (double)x / width
            });
        }

        return new PaletteData
        {
            Name = "Imported from Image",
            Colors = colors.ToArray()
        };
    }
}
```

---

### Palette Manager

**Central palette management service:**

```csharp
public class PaletteManager
{
    private readonly List<IPaletteFormatProvider> _formatProviders;
    private readonly Dictionary<string, IPalette> _palettes;

    public PaletteManager()
    {
        _formatProviders = new List<IPaletteFormatProvider>
        {
            new JsonPaletteProvider(),
            new CsvPaletteProvider(),
            new ImagePaletteProvider(),
            new MapPaletteProvider()
        };

        _palettes = new Dictionary<string, IPalette>();
        RegisterBuiltInPalettes();
    }

    // Auto-detect format and import
    public async Task<IPalette> ImportPaletteAsync(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        var provider = _formatProviders.FirstOrDefault(p => 
            p.CanRead && p.FileExtensions.Contains(extension));

        if (provider == null)
            throw new NotSupportedException($"No provider for {extension}");

        using var stream = File.OpenRead(filePath);
        var paletteData = await provider.ImportAsync(stream);

        var palette = new LookupTablePalette(paletteData.Name, 
            paletteData.Colors.Select(c => new ColorRGB(c.R, c.G, c.B)).ToArray());

        _palettes[palette.Name] = palette;
        return palette;
    }

    // Export palette (bake if procedural)
    public async Task ExportPaletteAsync(IPalette palette, string filePath, int colorCount = 256)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        var provider = _formatProviders.FirstOrDefault(p => 
            p.CanWrite && p.FileExtensions.Contains(extension));

        if (provider == null)
            throw new NotSupportedException($"No provider for {extension}");

        // "Bake" procedural palettes to lookup table for export
        var paletteData = palette.Type == PaletteType.Procedural
            ? BakePalette(palette, colorCount)
            : ConvertToData(palette as LookupTablePalette);

        using var stream = File.Create(filePath);
        await provider.ExportAsync(paletteData, stream);
    }

    // Convert procedural palette to lookup table ("baking")
    private PaletteData BakePalette(IPalette palette, int colorCount)
    {
        var colors = new ColorEntry[colorCount];

        for (int i = 0; i < colorCount; i++)
        {
            double iteration = i;
            var color = palette.GetColor(iteration, colorCount);

            colors[i] = new ColorEntry
            {
                R = color.R,
                G = color.G,
                B = color.B,
                Position = (double)i / (colorCount - 1)
            };
        }

        return new PaletteData
        {
            Name = palette.Name,
            Author = palette.Author,
            Description = $"Baked from procedural palette with {colorCount} colors",
            Colors = colors
        };
    }

    private void RegisterBuiltInPalettes()
    {
        // Register procedural palettes
        _palettes["Grayscale"] = new ProceduralPalette("Grayscale", 
            (iter, max) => ColorPalette.GetGrayscaleColor(iter, max));

        _palettes["Classic"] = new ProceduralPalette("Classic", 
            (iter, max) => ColorPalette.GetClassicColor(iter, max));

        _palettes["Fire"] = new ProceduralPalette("Fire", 
            (iter, max) => ColorPalette.GetFireColor(iter, max));

        _palettes["Ocean"] = new ProceduralPalette("Ocean", 
            (iter, max) => ColorPalette.GetOceanColor(iter, max));

        _palettes["Rainbow"] = new ProceduralPalette("Rainbow", 
            (iter, max) => ColorPalette.GetRainbowColor(iter, max));

        _palettes["Psychedelic"] = new ProceduralPalette("Psychedelic", 
            (iter, max) => ColorPalette.GetPsychedelicColor(iter, max));
    }
}
```

---

### Native C++ Palette Support

**High-performance palette interpolation in native code:**

```cpp
// Native palette structure for C++
struct NativePalette
{
    ColorRGB* colors;
    int colorCount;

    // Fast interpolated lookup
    ColorRGB GetColor(double iteration, int maxIterations) const
    {
        double t = (iteration / maxIterations) * (colorCount - 1);
        int i1 = static_cast<int>(t);
        int i2 = std::min(i1 + 1, colorCount - 1);
        double frac = t - i1;

        // Linear interpolation
        const ColorRGB& c1 = colors[i1];
        const ColorRGB& c2 = colors[i2];

        return ColorRGB(
            static_cast<unsigned char>(c1.r + (c2.r - c1.r) * frac),
            static_cast<unsigned char>(c1.g + (c2.g - c1.g) * frac),
            static_cast<unsigned char>(c1.b + (c2.b - c1.b) * frac)
        );
    }
};

// C++/CLI marshalling
void FractalEngine::SetPalette(array<Byte>^ colorData)
{
    pin_ptr<Byte> pinnedData = &colorData[0];

    int colorCount = colorData->Length / 3;
    m_palette.colors = new ColorRGB[colorCount];
    m_palette.colorCount = colorCount;

    for (int i = 0; i < colorCount; i++)
    {
        m_palette.colors[i].r = pinnedData[i * 3];
        m_palette.colors[i].g = pinnedData[i * 3 + 1];
        m_palette.colors[i].b = pinnedData[i * 3 + 2];
    }
}
```

---

### "Baking" Concept

**Converting procedural palettes to lookup tables for export:**

Procedural palettes (like Rainbow) cannot be exported directly because they are mathematical formulas, not data. The "baking" process samples the procedural function at regular intervals to create a discrete lookup table:

```csharp
// Example: Bake Rainbow palette to 256 colors
var rainbowPalette = paletteManager.GetPalette("Rainbow");  // Procedural

// Sample at 256 points
var bakedColors = new ColorRGB[256];
for (int i = 0; i < 256; i++)
{
    bakedColors[i] = rainbowPalette.GetColor(i, 256);
}

// Now exportable as lookup table
var lookupPalette = new LookupTablePalette("Rainbow (Baked)", bakedColors);
await paletteManager.ExportPaletteAsync(lookupPalette, "rainbow.json");
```

**Baking Resolution:**
- **Low (64 colors):** Fast, small files, visible banding
- **Medium (256 colors):** Standard, good quality, reasonable size
- **High (1024+ colors):** Smooth gradients, larger files, imperceptible banding

---

### Implementation Phases

**Phase 2 (Current - Core Preparation):**
- ✅ 6 procedural palettes (Grayscale, Classic, Fire, Ocean, Rainbow, Psychedelic)
- ✅ Basic PaletteType enum in FractalParameters
- ✅ Native C++ ColorPalette class with mathematical color generation

**Phase 4 (WinUI Interface - Deferred):**
- Palette picker UI component
- Import palette from JSON/CSV/Image
- Basic palette management (add, remove, rename)
- Palette preview rendering

**Phase 6 (File Operations - Deferred):**
- Full format provider system
- Export palettes (with baking for procedural)
- .MAP format compatibility (UltraFractal/Fractal eXtreme)
- Batch import/export

**Phase 7 (Polish & Optimization - Deferred):**
- Community palette gallery
- Online palette sharing/download
- Palette tags and search
- Palette editor (gradient stops, color manipulation)
- Advanced interpolation modes (cubic, cosine)

---

### Recommended Next Steps

For **Phase 2**, keep implementation simple:
- Use existing 6 procedural palettes
- No import/export functionality yet
- Focus on C++/CLI wrapper completion

For **Phase 4+**, implement comprehensive palette system:
- Build format provider infrastructure
- Add palette import UI
- Implement lookup table palette support
- Add "bake and export" functionality

**Documentation Complete:** This architecture design is preserved for future implementation. Phase 2 continues with current simple palette approach (procedural only).

---

## Summary

This architecture provides:
- ✅ Clear separation of concerns (C++ math, C# UI)
- ✅ Modern MVVM pattern with data binding
- ✅ Testable ViewModels and services
- ✅ Efficient C++/C# interop
- ✅ **Platform-agnostic design ensuring 90% code reuse for Android (MANDATORY)**
- ✅ Proper async/await patterns
- ✅ Dependency injection for loose coupling
- ✅ **Service abstractions enabling 6-8 week Android migration**

For implementation details by phase, see [Implementation Phases](03-implementation-phases.md).

For technology stack specifics, see [Technology Stack](04-technology-stack.md).

**For mandatory cross-platform compliance, see [MAUI Compatibility](07-maui-compatibility.md).**
