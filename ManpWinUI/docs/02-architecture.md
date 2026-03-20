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

## Summary

This architecture provides:
- ✅ Clear separation of concerns (C++ math, C# UI)
- ✅ Modern MVVM pattern with data binding
- ✅ Testable ViewModels and services
- ✅ Efficient C++/C# interop
- ✅ Platform-agnostic design for future MAUI expansion
- ✅ Proper async/await patterns
- ✅ Dependency injection for loose coupling

For implementation details by phase, see [Implementation Phases](03-implementation-phases.md).

For technology stack specifics, see [Technology Stack](04-technology-stack.md).
