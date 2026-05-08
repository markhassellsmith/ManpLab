# ManpWinUI Developer Guide

**Last Updated:** May 2026 | **Target:** .NET 10, WinUI 3, Windows 11

This guide covers the architecture, extension points, and development workflow for ManpWinUI.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Adding New Fractals](#adding-new-fractals)
4. [Extending the UI](#extending-the-ui)
5. [Theme System](#theme-system)
6. [Testing & Debugging](#testing--debugging)
7. [Build & Deployment](#build--deployment)

---

## Architecture Overview

### Technology Stack

- **.NET 10** - Latest framework with C# 13
- **WinUI 3** - Modern Windows UI with Fluent Design
- **MVVM Pattern** - Clean separation of UI and logic
- **Native Engine** - C++17 fractal computation (ManpCore.Native)
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

### Design Principles

**MVVM Architecture:**
```
View (XAML)
  ↕ Data Binding
ViewModel (C# Logic)
  ↕ Service Calls
Services (Business Logic)
  ↕ P/Invoke
Native Engine (C++ Performance)
```

**Key Patterns:**
- **Commands** - `RelayCommand` for all user actions
- **Async/Await** - Non-blocking UI operations
- **Partial Classes** - Organize large files by concern
- **Resource Dictionaries** - Themeable UI resources

---

## Project Structure

```
ManpWinUI/
├── App.xaml.cs                 # DI container, theme loading, startup
├── ViewModels/
│   ├── MainViewModel.cs        # Core application state
│   ├── MainViewModel.UI.cs     # UI-specific properties
│   ├── SettingsViewModel.cs    # Settings management
│   └── AnimationViewModel.cs   # Animation rendering
├── Views/
│   ├── MainPage.xaml           # Main UI layout
│   ├── MainPage.xaml.cs        # Page initialization
│   └── [Feature]View.xaml      # Feature-specific views
├── Services/
│   ├── IFractalRenderService.cs           # Rendering abstraction
│   ├── StandardFractalRenderService.cs    # Standard fractals
│   ├── HailstoneRenderService.cs          # Special rendering
│   ├── IAppSettingsService.cs             # Settings persistence
│   ├── INavigationHistoryService.cs       # History tracking
│   ├── IFractalMetadataService.cs         # Fractal information
│   └── IImageExportService.cs             # Image saving
├── Themes/
│   └── OceanBlue.xaml          # Custom theme resources
└── docs/
    ├── DEVELOPER_GUIDE.md      # This file
    └── [Feature docs]
```

### Key Files

**App.xaml.cs** - Application lifecycle, DI setup, theme management
**MainViewModel.cs** - Central state manager, coordinates all services
**MainPage.xaml** - UI layout with data binding
**FractalRegistry.cpp** - Native fractal registration (ManpCore.Native)

---

## Adding New Fractals

### Step 1: Register in Native Engine

**Location:** `ManpCore.Native/[FamilyName]Family.cpp`

```cpp
// Example: Adding "CustomMandel" to ExoticFormulasFamily.cpp
void RegisterExoticFormulasFamily()
{
    FractalSpec spec;

    spec.name = "CustomMandel";
    spec.displayName = "Custom Mandelbrot Variant";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Brief description of the fractal";
    spec.formula = "z = f(z) + c";  // Plain text
    spec.formulaLatex = R"(z_{n+1} = f(z_n) + c)";  // TeX

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                        ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // Your fractal formula here
            z = /* ... */ param;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);
}
```

### Step 2: Update Family Registration

**Location:** `ManpCore.Native/FractalRegistry.cpp`

Ensure your family function is declared and called in `InitializeBuiltins()`:

```cpp
extern void RegisterExoticFormulasFamily();  // Declaration

void FractalRegistry::InitializeBuiltins()
{
    // ... other families
    RegisterExoticFormulasFamily();  // Call
    // ...
}
```

### Step 3: Rebuild Native Project

```bash
# Build ManpCore.Native project
# The DLL will be automatically copied to ManpWinUI output
```

### Step 4: Populate Metadata (Optional but Recommended)

**Location:** `ManpWinUI/Services/FractalMetadataService.cs`

Add rich metadata for your fractal:

```csharp
new FractalMetadata
{
    Name = "CustomMandel",
    DisplayName = "Custom Mandelbrot Variant",
    Category = "Exotic",
    Description = "Detailed description for users",
    Formula = "z = f(z) + c",
    FormulaLatex = @"z_{n+1} = f(z_n) + c",
    DefaultParams = new Dictionary<string, double>
    {
        { "power", 2.0 },
        { "bailout", 256.0 }
    },
    ParameterDescriptions = new Dictionary<string, string>
    {
        { "power", "Exponent for iteration" },
        { "bailout", "Escape threshold" }
    }
}
```

### Step 5: Test

1. Launch ManpWinUI
2. Open Browser panel (Ctrl+B)
3. Search for "Custom"
4. Select and render your fractal

**See:** `ManpCore.Native/ADDING_FRACTALS.md` for more details

---

## Extending the UI

### Adding a New View/Panel

**1. Create View Files:**

```
ManpWinUI/Views/[Feature]/
  ├── [Feature]View.xaml      # UI layout
  └── [Feature]View.xaml.cs   # Code-behind
```

**2. Create ViewModel:**

```csharp
// ViewModels/[Feature]ViewModel.cs
public class FeatureViewModel : INotifyPropertyChanged
{
    private readonly IServiceDependency _service;

    public FeatureViewModel(IServiceDependency service)
    {
        _service = service;
        // Initialize commands
    }

    // Properties, commands, event handlers
}
```

**3. Register in DI Container:**

```csharp
// App.xaml.cs - ConfigureServices()
services.AddTransient<FeatureViewModel>();
```

**4. Add to MainPage:**

```xml
<!-- MainPage.xaml -->
<TabViewItem Header="Feature">
    <feature:FeatureView />
</TabViewItem>
```

### Adding a Service

**1. Define Interface:**

```csharp
// Services/IFeatureService.cs
public interface IFeatureService
{
    Task<Result> DoSomethingAsync(Params params);
}
```

**2. Implement Service:**

```csharp
// Services/FeatureService.cs
public class FeatureService : IFeatureService
{
    public async Task<Result> DoSomethingAsync(Params params)
    {
        // Implementation
    }
}
```

**3. Register in DI:**

```csharp
// App.xaml.cs
services.AddSingleton<IFeatureService, FeatureService>();
```

**4. Inject into ViewModel:**

```csharp
public class SomeViewModel
{
    private readonly IFeatureService _featureService;

    public SomeViewModel(IFeatureService featureService)
    {
        _featureService = featureService;
    }
}
```

---

## Theme System

### Architecture

**Built-in Themes:** Light, Dark, System (follows Windows)
**Custom Themes:** OceanBlue (example of app-specific theme)

**Key Principle:** Only override what you need. Let WinUI 3 handle control styling.

### Adding a Custom Theme

**1. Create Theme Resource Dictionary:**

```xml
<!-- Themes/MyTheme.xaml -->
<ResourceDictionary>
    <!-- Only override application-specific visuals -->
    <SolidColorBrush x:Key="ApplicationPageBackgroundThemeBrush" Color="#..." />
    <SolidColorBrush x:Key="SidePanelBackgroundBrush" Color="#..." />

    <!-- If custom toolbar background, must provide button foregrounds -->
    <SolidColorBrush x:Key="AppBarBackground" Color="#..." />
    <SolidColorBrush x:Key="AppBarButtonForeground" Color="#..." />
    <SolidColorBrush x:Key="AppBarButtonForegroundPointerOver" Color="#..." />
    <SolidColorBrush x:Key="AppBarButtonForegroundPressed" Color="#..." />

    <!-- Accent color (optional) -->
    <Color x:Key="SystemAccentColor">#...</Color>
</ResourceDictionary>
```

**2. Load Theme in App.xaml.cs:**

```csharp
// App.xaml.cs - ApplyTheme()
if (themeName == "MyTheme")
{
    LoadCustomTheme("ms-appx:///Themes/MyTheme.xaml");
    rootFrame.RequestedTheme = ElementTheme.Light; // Or Dark
}
```

**3. Add to Settings:**

```csharp
// SettingsViewModel.cs
public List<string> AvailableThemes => new()
{
    "Light", "Dark", "System", "Ocean Blue", "My Theme"
};
```

**Important:** When setting a custom Background, always provide matching Foreground. See `docs/WINUI3_THEME_BEST_PRACTICES.md`.

---

## Testing & Debugging

### Build Configurations

**Debug:**
- Full symbols, assertions enabled
- Native engine runs slower
- Use for development

**Release:**
- Optimized, native engine at full speed
- Use for testing performance and final builds

### Common Issues

**Native DLL Not Found:**
- Check `ManpCore.Native.dll` is in output directory
- Rebuild ManpCore.Native project
- Check post-build copy events

**Theme Changes Reset State:**
- Ensure `ApplyTheme()` doesn't recreate Frame
- Theme changes should only modify `RequestedTheme`

**Fractal Not Appearing in Browser:**
- Verify fractal is registered in FractalRegistry.cpp
- Check `InitializeBuiltins()` calls your family function
- Rebuild native project

### Debugging Native Code

1. Set ManpWinUI as startup project
2. Enable native debugging: Project Properties → Debug → Enable Native Code Debugging
3. Set breakpoints in `.cpp` files
4. F5 to debug C# and C++ simultaneously

---

## Build & Deployment

### Development Build

```bash
# Visual Studio
F5 (or Ctrl+F5 without debugging)

# Command line
cd ManpWinUI
dotnet build
dotnet run
```

### Release Build

```bash
# Visual Studio
Build → Configuration Manager → Release
Build → Build Solution

# Command line
cd ManpWinUI
dotnet build -c Release
```

### MSIX Packaging (For Distribution)

**1. Configure Package:**

Edit `ManpWinUI/Package.appxmanifest`:
- Update version number
- Set publisher identity
- Configure capabilities

**2. Build Package:**

```bash
# Visual Studio
Project → Publish → Create App Packages

# Command line
msbuild ManpWinUI.csproj /p:Configuration=Release /p:Platform=x64 /p:AppxPackageDir=..\AppPackages
```

**3. Test Package:**

```powershell
# Install locally
Add-AppxPackage -Path .\AppPackages\ManpWinUI_1.0.0.0_x64.msix
```

**See:** `docs/MSIX_DEPLOYMENT.md` for detailed deployment guide

---

## Additional Resources

### Documentation
- **ADDING_FRACTALS.md** - Detailed fractal development guide
- **WINUI3_THEME_BEST_PRACTICES.md** - Theme system guidelines
- **THEME_FIX_SUMMARY_MAY_2026.md** - Recent theme fixes explained
- **ANIMATION_FEATURE_PLAN.md** - Animation system architecture

### Code Examples
- **ExoticFormulasFamily.cpp** - Examples of fractal calculators
- **OceanBlue.xaml** - Custom theme example
- **MainViewModel.cs** - Service coordination patterns
- **AnimationViewModel.cs** - Complex async workflows

### External Resources
- [WinUI 3 Documentation](https://docs.microsoft.com/windows/apps/winui/winui3/)
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/)
- [Fractal Forums](https://fractalforums.org/)

---

## Contributing

When extending the codebase:

1. **Follow MVVM** - Keep UI logic in ViewModels
2. **Use DI** - Register services, inject dependencies
3. **Async Everything** - Use async/await for I/O and compute
4. **Document Public APIs** - XML comments for interfaces
5. **Test Both Configs** - Debug and Release builds
6. **Theme-Aware** - Test Light/Dark/Custom themes
7. **Commit Atomically** - One feature/fix per commit

**Questions?** Open a GitHub issue or discussion.

---

*Last updated: May 2026 - Covers 300 fractal milestone, animation system, theme fixes, and complete architecture*
