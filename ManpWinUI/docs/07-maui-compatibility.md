# MAUI Compatibility - MANDATORY REQUIREMENTS

## ⚠️ IMPORTANT: This is NOT Optional Guidance

**These are MANDATORY architectural constraints, not suggestions.**  
All code must follow these patterns to ensure Android/iOS portability with minimal effort (6-8 weeks post-v1.0).

**Code reviews will REJECT any violations of these requirements.**

---

## Overview

While the immediate target is **Windows via WinUI 3**, the architecture is **designed for .NET MAUI** (Multi-platform App UI) expansion to Android, iOS, macOS, and Linux. This is a **hard requirement**, not a "nice to have."

## Why MAUI-Ready Architecture is Mandatory

- **Code Reuse:** Ensures 90%+ of business logic, ViewModels, and models are reusable
- **Android Migration:** Enables 6-8 week Android version (vs 2-3 month refactoring if ignored)
- **Platform Expansion:** Opens path to iOS/macOS tablet versions
- **Future-Proofing:** Microsoft is unifying around MAUI for cross-platform
- **Cloud Rendering:** Mobile clients leverage Azure VM for C++ engine (Windows-only interop acceptable)
- **Market Opportunity:** Fractal apps rare on mobile - potential market expansion

---

## Compliance Enforcement

**Every pull request must pass Android portability validation:**
- [ ] ViewModels have zero `Microsoft.UI.*` or `Windows.*` dependencies
- [ ] Platform services use interface abstractions
- [ ] Business logic testable without WinUI references
- [ ] Service registrations support dependency injection swapping

**See phase checklists in `03-implementation-phases.md` for detailed validation per phase.**

---

## Architecture Decisions for MAUI Compatibility

### ✅ **DO: Write Platform-Agnostic Code (MANDATORY)**

**1. Separate Business Logic from UI**
```csharp
// ✅ GOOD - Platform agnostic
public class FractalService : IFractalService
{
    public async Task<FractalResult> CalculateAsync(FractalParameters parameters)
    {
        // Pure business logic, no UI dependencies
    }
}

// ❌ BAD - WinUI specific
public class FractalService
{
    private WriteableBitmap _bitmap; // WinUI-specific type
}
```

**2. Use Abstraction Layers for Platform Services**
```csharp
// Define interfaces in shared code
public interface IFileService
{
    Task<Stream> PickFileAsync(string[] extensions);
    Task<bool> SaveFileAsync(string filename, byte[] data);
}

// Implement for WinUI
public class WinUIFileService : IFileService { ... }

// Implement for MAUI later
public class MauiFileService : IFileService { ... }
```

**3. ViewModels Should Be 100% Platform-Independent**
```csharp
// ✅ GOOD - Works in both WinUI and MAUI
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _fractalName;

    [RelayCommand]
    private async Task RenderAsync()
    {
        await _fractalService.CalculateAsync(...);
    }
}

// ❌ BAD - WinUI specific
public class MainViewModel
{
    private DispatcherQueue _dispatcher; // WinUI only
}
```

---

### ❌ **AVOID: Platform-Specific Dependencies in Shared Code**

**What to Avoid:**
- Direct WinUI control references in ViewModels
- `Microsoft.UI.Xaml.*` namespaces in business logic
- Windows-specific APIs (`Windows.Storage.*` in shared code)
- WinUI-specific threading (`DispatcherQueue`)
- Platform-specific DI container registrations mixed with business services

**Where Platform-Specific Code is OK:**
- Views (`.xaml` files) - these will be platform-specific anyway
- Platform service implementations
- App startup / bootstrapping
- Native interop wrappers

---

## Recommended Project Structure for MAUI-Ready Code

```
ManpLab.sln
├── ManpLab.Core/                    # ✅ Shared .NET Standard 2.1 or .NET 10
│   ├── Models/                      # Data structures (FractalParameters, etc.)
│   ├── ViewModels/                  # Platform-agnostic ViewModels
│   ├── Services/                    # Business logic services
│   │   ├── Interfaces/              # IFractalService, IFileService, etc.
│   │   └── Implementations/         # Platform-agnostic implementations
│   ├── Interop/                     # C++ wrapper interfaces
│   └── Utilities/                   # Helpers, converters, etc.
│
├── ManpLab.WinUI/                   # 🪟 Windows WinUI 3 app
│   ├── Views/                       # WinUI XAML views
│   ├── Services/                    # WinUI-specific implementations
│   │   ├── WinUIFileService.cs
│   │   ├── WinUIBitmapService.cs
│   │   └── WinUINavigationService.cs
│   ├── App.xaml.cs                  # WinUI app startup
│   └── Platforms/Windows/           # Windows-specific code
│
├── ManpLab.Maui/ (future)           # 📱 MAUI multi-platform app
│   ├── Views/                       # MAUI XAML views
│   ├── Services/                    # MAUI-specific implementations
│   │   ├── MauiFileService.cs
│   │   ├── MauiBitmapService.cs
│   │   └── MauiNavigationService.cs
│   ├── Platforms/                   # Platform-specific code
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── MacCatalyst/
│   │   └── Windows/
│   └── MauiProgram.cs               # MAUI app startup
│
├── ManpCore.Native/                 # C++/CLI wrapper
└── ManpWIN64/                       # Existing C++ engine
```

**Key Points:**
- **ManpLab.Core** - 100% shared between WinUI and MAUI
- **ManpLab.WinUI** - Windows-specific UI and services
- **ManpLab.Maui** (future) - Cross-platform UI, references Core
- Both UI projects reference the **same Core library**

---

## Service Interfaces to Abstract

Define these in `ManpLab.Core/Services/Interfaces/`:

```csharp
// File operations
public interface IFileService
{
    Task<Stream> OpenFileAsync(string[] fileTypes);
    Task<bool> SaveFileAsync(string path, byte[] data);
    Task<string[]> GetRecentFilesAsync();
}

// Bitmap/image handling
public interface IBitmapService
{
    object CreateBitmap(int width, int height);
    void UpdatePixels(object bitmap, byte[] pixels);
    Task<bool> SaveBitmapAsync(object bitmap, string path);
}

// Navigation
public interface INavigationService
{
    Task NavigateToAsync(string viewName, object parameter = null);
    Task GoBackAsync();
    bool CanGoBack { get; }
}

// Dialogs
public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message);
    Task<string> ShowInputAsync(string title, string prompt);
}

// Settings
public interface ISettingsService
{
    T Get<T>(string key, T defaultValue);
    void Set<T>(string key, T value);
    Task SaveAsync();
}

// Platform info
public interface IPlatformService
{
    PlatformType CurrentPlatform { get; }
    bool SupportsNativeCpp { get; }
    bool IsMobile { get; }
    int MaxImageWidth { get; }
    int MaxImageHeight { get; }
}

public enum PlatformType
{
    Windows,
    Android,
    iOS,
    MacCatalyst,
    Linux
}
```

---

## UI Controls - WinUI vs MAUI Mapping

### Controls Available in Both Platforms

| WinUI 3 Control | MAUI Control | Notes |
|-----------------|--------------|-------|
| `TextBlock` | `Label` | Minor API differences |
| `TextBox` | `Entry` | MAUI has simpler API |
| `Button` | `Button` | Nearly identical |
| `Image` | `Image` | Similar, source binding differs |
| `ListView` | `ListView`/`CollectionView` | MAUI prefers CollectionView |
| `GridView` | `CollectionView` | Use CollectionView in MAUI |
| `ProgressBar` | `ProgressBar` | Similar |
| `Slider` | `Slider` | Nearly identical |
| `CheckBox` | `CheckBox` | Similar |
| `RadioButton` | `RadioButton` | Similar |
| `ComboBox` | `Picker` | Different API |
| `Grid` | `Grid` | Identical layout |
| `StackPanel` | `StackLayout`/`VerticalStackLayout` | MAUI simplified |

### WinUI-Specific Controls (Will Need Alternatives)

| WinUI 3 Control | MAUI Alternative | Notes |
|-----------------|------------------|-------|
| `NavigationView` | Custom or `Shell` | MAUI Shell provides navigation |
| `NumberBox` | `Entry` + validation | No built-in NumberBox in MAUI |
| `CommandBar` | `Toolbar` | Different API |
| `TeachingTip` | Custom tooltip | No direct equivalent |
| `SplitView` | `FlyoutPage` or custom | Different pattern |
| `MenuBar` | `MenuBarItem` in Shell | Different approach |
| `TreeView` | Custom control | Not in MAUI standard |

**Recommendations:**
- Prefer controls that exist in both platforms
- Use `ContentView` to wrap complex controls for easy platform substitution
- Create custom controls in Core project using platform-agnostic primitives

---

## Data Binding Differences

### Both platforms support
- ✅ `INotifyPropertyChanged`
- ✅ `ICommand`
- ✅ `ObservableCollection<T>`
- ✅ CommunityToolkit.Mvvm (works in both!)

### Syntax Differences

```xaml
<!-- WinUI 3 -->
<TextBlock Text="{x:Bind ViewModel.FractalName, Mode=OneWay}" />
<Button Command="{x:Bind ViewModel.RenderCommand}" />

<!-- MAUI -->
<Label Text="{Binding FractalName}" />
<Button Command="{Binding RenderCommand}" />
```

**Recommendation:** Use `{Binding}` syntax in WinUI for MAUI compatibility, even though `{x:Bind}` is faster. Or plan to convert XAML when porting.

---

## Threading and Async Patterns

### Both platforms use
- `async`/`await`
- `Task` and `Task<T>`
- `CancellationToken`

### Dispatching to UI Thread

```csharp
// ✅ GOOD - Use abstraction
public interface IDispatcherService
{
    Task RunOnUIThreadAsync(Action action);
}

// WinUI implementation
public class WinUIDispatcherService : IDispatcherService
{
    private readonly DispatcherQueue _dispatcher;
    public async Task RunOnUIThreadAsync(Action action)
    {
        await _dispatcher.EnqueueAsync(action);
    }
}

// MAUI implementation
public class MauiDispatcherService : IDispatcherService
{
    public async Task RunOnUIThreadAsync(Action action)
    {
        await MainThread.InvokeOnMainThreadAsync(action);
    }
}

// ❌ BAD - Direct WinUI dependency
public class ViewModel
{
    private DispatcherQueue _dispatcher;
    public void UpdateUI()
    {
        _dispatcher.TryEnqueue(() => { ... });
    }
}
```

---

## File I/O Patterns

### Platform Differences
- **WinUI:** `Windows.Storage.*` APIs, `StorageFile`, `FileOpenPicker`
- **MAUI:** `Microsoft.Maui.Storage.FilePicker`, `FileSystem` helpers

### Abstraction Example

```csharp
// Shared interface
public interface IFileService
{
    Task<FileData> PickFileAsync(PickOptions options);
    Task<string> PickFolderAsync();
    Task<bool> SaveFileAsync(string filename, Stream data);
}

public class PickOptions
{
    public string[] FileTypes { get; set; }
    public string PickerTitle { get; set; }
}

public class FileData
{
    public string FileName { get; set; }
    public Stream Content { get; set; }
}

// WinUI implementation
public class WinUIFileService : IFileService
{
    public async Task<FileData> PickFileAsync(PickOptions options)
    {
        var picker = new FileOpenPicker();
        foreach (var type in options.FileTypes)
            picker.FileTypeFilter.Add(type);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return null;

        return new FileData
        {
            FileName = file.Name,
            Content = await file.OpenStreamForReadAsync()
        };
    }
}

// MAUI implementation
public class MauiFileService : IFileService
{
    public async Task<FileData> PickFileAsync(PickOptions options)
    {
        var fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, options.FileTypes }
            });

        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = fileTypes,
            PickerTitle = options.PickerTitle
        });

        if (result == null) return null;

        return new FileData
        {
            FileName = result.FileName,
            Content = await result.OpenReadAsync()
        };
    }
}
```

---

## Bitmap/Image Handling

### Challenge
Fractal rendering produces raw pixel data that needs platform-specific bitmap types.

### Solution

```csharp
// Shared interface
public interface IBitmapService
{
    IBitmapHandle CreateBitmap(int width, int height);
    void SetPixels(IBitmapHandle handle, byte[] pixelData);
    Task SaveAsync(IBitmapHandle handle, string path, ImageFormat format);
}

public interface IBitmapHandle : IDisposable
{
    int Width { get; }
    int Height { get; }
    object PlatformBitmap { get; } // For binding to Image.Source
}

// WinUI implementation
public class WinUIBitmapService : IBitmapService
{
    public IBitmapHandle CreateBitmap(int width, int height)
    {
        var bitmap = new WriteableBitmap(width, height);
        return new WinUIBitmapHandle(bitmap);
    }
}

// MAUI implementation (future)
public class MauiBitmapService : IBitmapService
{
    public IBitmapHandle CreateBitmap(int width, int height)
    {
        // Use SKBitmap from SkiaSharp (cross-platform)
        var bitmap = new SKBitmap(width, height);
        return new MauiBitmapHandle(bitmap);
    }
}
```

---

## C++ Interop Considerations

### Platform Support
- **Windows (WinUI):** Full C++/CLI support ✅
- **Android (MAUI):** Via JNI or C/C++ NDK ⚠️
- **iOS (MAUI):** Via Objective-C bridging ⚠️
- **macOS (MAUI):** Via Objective-C bridging ⚠️

### Strategies

**Option 1: Windows-Only C++ (Recommended for Phase 1)**
```csharp
public interface IFractalEngine
{
    Task<byte[]> CalculateAsync(FractalParameters params);
}

// Windows implementation - uses C++ engine
public class NativeFractalEngine : IFractalEngine { ... }

// Mobile implementation - pure C# fallback or cloud rendering
public class ManagedFractalEngine : IFractalEngine { ... }

// Or cloud-based
public class CloudFractalEngine : IFractalEngine { ... }
```

**Option 2: Cross-Compile C++ (Advanced)**
- Compile C++ engine as native library for each platform
- Use P/Invoke on all platforms
- More complex build process
- Best performance on all platforms

**Option 3: Hybrid Approach**
- Desktop (Windows/Mac): Native C++ engine
- Mobile (iOS/Android): Cloud rendering or simplified C# engine
- Mobile devices typically can't handle deep zoom anyway

**Recommendation for This Project:**
Start with **Option 1**. Mobile devices likely won't need the full power of the C++ engine. Consider:
- Mobile: Limited parameter ranges, cloud rendering for heavy work
- Desktop: Full C++ engine with all features

---

## Navigation Patterns

**WinUI:** `Frame` navigation with `NavigationView`

**MAUI:** `Shell` navigation or `NavigationPage`

### Abstraction

```csharp
public interface INavigationService
{
    Task NavigateToAsync<TViewModel>(object parameter = null);
    Task GoBackAsync();
    bool CanGoBack { get; }
}

// Register routes in startup
public static class Routes
{
    public const string MainPage = "main";
    public const string ParametersPage = "parameters";
    public const string PalettePage = "palette";
}
```

---

## Dependency Injection

### Both platforms use Microsoft.Extensions.DependencyInjection

```csharp
// Shared service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<FractalParametersViewModel>();

        // Business services
        services.AddSingleton<IFractalService, FractalService>();
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }

    public static IServiceCollection AddPlatformServices(
        this IServiceCollection services)
    {
        // Platform-specific registration done in each app
        return services;
    }
}

// WinUI App.xaml.cs
services.AddCoreServices();
services.AddSingleton<IFileService, WinUIFileService>();
services.AddSingleton<IBitmapService, WinUIBitmapService>();

// MAUI MauiProgram.cs (future)
services.AddCoreServices();
services.AddSingleton<IFileService, MauiFileService>();
services.AddSingleton<IBitmapService, MauiBitmapService>();
```

---

## Testing Benefits of MAUI-Ready Architecture

### Easier Unit Testing

```csharp
// ViewModels are platform-independent, easy to test
[Fact]
public async Task RenderCommand_ShouldCallFractalService()
{
    // Arrange
    var mockService = new Mock<IFractalService>();
    var viewModel = new MainViewModel(mockService.Object);

    // Act
    await viewModel.RenderCommand.ExecuteAsync(null);

    // Assert
    mockService.Verify(s => s.CalculateAsync(It.IsAny<FractalParameters>()));
}
```

---

## Migration Path to MAUI

### Phase 1: WinUI Development (Now)
1. Build WinUI app with Core/WinUI split
2. Keep ViewModels platform-agnostic
3. Abstract platform services

### Phase 2: MAUI Project Setup (Future)
1. Create ManpLab.Maui project
2. Reference ManpLab.Core
3. Implement MAUI platform services
4. Create MAUI-specific views

### Phase 3: Platform Optimization
1. Add Android-specific optimizations
2. Add iOS touch optimizations
3. Consider cloud rendering for mobile
4. Mobile-specific UI (simplified controls)

### Estimated Effort
- If architecture is MAUI-ready: **2-4 weeks** to get MAUI version working
- If architecture is WinUI-coupled: **2-3 months** of refactoring first

---

## Decision Checklist for Each Feature

When implementing any feature in WinUI, ask:

- [ ] Can this ViewModel be used in MAUI without changes?
- [ ] Are platform-specific APIs abstracted behind interfaces?
- [ ] Does this use controls available in both platforms?
- [ ] Is file I/O going through IFileService?
- [ ] Is bitmap handling going through IBitmapService?
- [ ] Are there no direct WinUI namespace references in Core?
- [ ] Can this be unit tested without a UI?

**If any answer is "No"**: Refactor to use abstraction layer.

---

## Performance Considerations for Mobile

### Mobile Limitations
- Less RAM (2-8 GB vs 16-64 GB desktop)
- Slower CPUs
- Battery constraints
- Smaller screens (less pixels = faster rendering)

### Design Decisions

1. **Parameter Limits:** Reduce max iterations, resolution on mobile
2. **Cloud Rendering:** Offload heavy calculations to server
3. **Simplified Features:** Mobile might not need all 240 fractal types
4. **Caching:** More aggressive caching on mobile
5. **Progressive Rendering:** Show low-res preview first

```csharp
public class FractalParameters
{
    public int MaxIterations { get; set; }

    public int GetEffectiveMaxIterations(IPlatformService platform)
    {
        if (platform.IsMobile)
            return Math.Min(MaxIterations, 1000);
        return MaxIterations;
    }
}
```

---

## Recommended Reading

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [MAUI vs WinUI Comparison](https://learn.microsoft.com/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/overall-migration-strategy)
- [Building Cross-Platform Apps](https://learn.microsoft.com/dotnet/architecture/maui/cross-platform-development)
- [CommunityToolkit.Mvvm for Cross-Platform Development](https://learn.microsoft.com/windows/communitytoolkit/mvvm/introduction)
- [Abstract Platform Services Pattern](https://learn.microsoft.com/dotnet/architecture/maui/platform-specific-features)
