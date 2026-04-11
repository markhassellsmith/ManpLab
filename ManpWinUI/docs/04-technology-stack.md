# Technology Stack

## WinUI / .NET

- **.NET 10** - Latest runtime
- **WinUI 3** - Modern Windows UI framework
- **CommunityToolkit.Mvvm** - MVVM helpers
- **CommunityToolkit.WinUI** - Additional controls
- **Microsoft.Extensions.DependencyInjection** - DI container
- **System.Text.Json** - Settings serialization
- **Serilog / NLog** - Logging framework

---

## C++ Interop Options

### Option 1: C++/CLI (Recommended)

- Direct CLR integration
- Easier type marshalling
- Mixed-mode debugging
- Slightly larger binary

**When to use:**
- Complex type marshalling needed (BigDouble, BigComplex)
- Direct memory access for image buffers required
- Mixed C++/C# debugging is important
- Gradual refactoring from C++ to C# planned

### Option 2: Native P/Invoke

- Standard .NET interop
- Requires careful marshalling
- More portable
- Smaller binary

**When to use:**
- Simple C function interfaces
- Cross-platform compatibility desired
- Smaller binary size is priority
- No need for mixed-mode debugging

---

## Existing C++ Libraries (Keep)

### Mathematical Libraries

- **MPFR/MPIR** - Arbitrary precision arithmetic (100+ decimal places)
  - Used via existing ManpWIN64 code
  - Wrapped by BigDouble/BigComplex
  - Essential for deep zoom calculations

- **QD Library** - Quad-double precision (~64 decimal digits)
  - Standard library implementation
  - Don't modify
  - Provides intermediate precision level

### Utility Libraries

- **zlib** - PNG compression
  - Standard library
  - Don't modify
  - Used for image export

- **MPEG Library** - Video encoding for animation export
  - Keep for backward compatibility
  - May supplement with modern codecs

### Custom Libraries

- **Parser** - Formula compilation
  - Custom-built compiler for user-defined fractals
  - Compiles formulas to bytecode
  - Supports 240+ built-in fractal types
  - Critical component - don't rewrite

- **Custom Math Libraries**
  - `BigDouble`, `BigComplex` - MPFR wrappers
  - `DDComplex`, `DDMatrix` - Double-double implementations
  - `floatexp`, `ExpComplex` - Extended range floating-point
  - `Complex` - Standard double complex numbers

---

## New Libraries to Add

### Video/Media Libraries

**Primary Option: Windows Media Foundation**
- Native to Windows
- Hardware acceleration support
- H.264, H.265 encoding
- Well-integrated with WinUI

**Alternative: FFmpeg**
- More codec options (VP9, AV1)
- Cross-platform (useful if considering MAUI later)
- Larger binary size
- More complex integration

**Recommendation:** Start with Windows Media Foundation, add FFmpeg later if cross-platform support needed.

### Graphics Libraries (Optional)

**SkiaSharp**
- Advanced 2D graphics
- Cross-platform compatibility
- Useful for custom rendering effects
- Good MAUI compatibility

**When to use:**
- Custom coloring effects beyond standard algorithms
- Vector graphics export (SVG)
- Advanced anti-aliasing
- Cross-platform rendering consistency

**Recommendation:** Optional - only add if specific rendering features needed.

### UI Enhancement Libraries

**Windows Community Toolkit**
- Additional WinUI controls
- Layout helpers
- Converters and behaviors
- Animation helpers

**Microsoft.Toolkit.Uwp.UI.Controls** (if targeting Windows 10 compatibility)
- Extended control library
- Data grid, color picker, etc.

---

## Library Decision Matrix

| Library | Phase | Priority | Reason |
|---------|-------|----------|--------|
| CommunityToolkit.Mvvm | Phase 3 | ⭐⭐⭐⭐ High | Essential for MVVM pattern |
| CommunityToolkit.WinUI | Phase 4 | ⭐⭐⭐ Medium | Additional UI controls |
| Microsoft.Extensions.DependencyInjection | Phase 3 | ⭐⭐⭐⭐ High | Dependency injection |
| System.Text.Json | Phase 3 | ⭐⭐⭐⭐ High | Settings serialization |
| Serilog / NLog | Phase 3 | ⭐⭐⭐ Medium | Logging |
| Windows Media Foundation | Phase 6 | ⭐⭐⭐ Medium | Modern video export |
| SkiaSharp | Phase 7+ | ⭐ Low | Optional advanced graphics |
| Windows Community Toolkit | Phase 4 | ⭐⭐ Low-Medium | Nice-to-have controls |

---

## Package Installation Commands

### Phase 3: Foundation Setup

```powershell
# MVVM Toolkit
dotnet add package CommunityToolkit.Mvvm

# Dependency Injection
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions

# Configuration
dotnet add package System.Text.Json

# Logging (choose one)
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
# OR
dotnet add package NLog
dotnet add package NLog.Extensions.Logging
```

### Phase 4: UI Development

```powershell
# WinUI Community Toolkit
dotnet add package CommunityToolkit.WinUI
dotnet add package CommunityToolkit.WinUI.UI.Controls
dotnet add package CommunityToolkit.WinUI.UI.Animations
```

### Phase 6: File Operations

```powershell
# Windows Media Foundation (native, no package needed)
# Just add platform API references in project file

# Alternative: FFmpeg (if needed)
dotnet add package FFmpeg.AutoGen
```

### Optional Packages

```powershell
# SkiaSharp (advanced graphics)
dotnet add package SkiaSharp
dotnet add package SkiaSharp.Views.WinUI

# Testing libraries
dotnet add package xunit
dotnet add package Moq
dotnet add package FluentAssertions
```

---

## Version Compatibility

### .NET Version Requirements

- **Minimum:** .NET 10
- **Recommended:** Latest .NET 10 patch version
- **Future:** Ready for .NET 11+ migration

### WinUI Version Requirements

- **Minimum:** WinUI 3.0
- **Recommended:** WinUI 3.2+ (for latest controls)
- **Target:** Windows 10 version 1809 (build 17763) or later

### C++ Compiler Requirements

- **Visual Studio:** 2022 or later
- **Platform Toolset:** v143 (VS 2022) or later
- **C++ Standard:** C++17 or C++20
- **Windows SDK:** 10.0.19041.0 or later

---

## Build Configuration

### Project References

```xml
<!-- ManpWinUI.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
    <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
    <Platforms>x64;ARM64</Platforms>
    <RuntimeIdentifiers>win10-x64;win10-arm64</RuntimeIdentifiers>
  </PropertyGroup>

  <!-- WinUI 3 -->
  <ItemGroup>
    <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.5.*" />
    <PackageReference Include="Microsoft.Windows.SDK.BuildTools" Version="10.0.*" />
  </ItemGroup>

  <!-- MVVM & DI -->
  <ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.*" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.*" />
  </ItemGroup>

  <!-- C++/CLI Interop -->
  <ItemGroup>
    <ProjectReference Include="..\ManpCore.Native\ManpCore.Native.vcxproj" />
  </ItemGroup>
</Project>
```

---

## Runtime Dependencies

### Redistributables Required

**For End Users:**
- .NET 10 Runtime (Windows Desktop)
- Windows App SDK Runtime
- Visual C++ Redistributable (for C++ components)

**Distribution Options:**
1. **Self-contained:** Bundle .NET runtime with app
   - Larger download (~150 MB)
   - No .NET installation required
   - Better user experience

2. **Framework-dependent:** Require .NET 10 installed
   - Smaller download (~10 MB)
   - Requires .NET 10 Runtime pre-installed
   - Smaller disk footprint

**Recommendation:** Self-contained for initial release (easier deployment).

---

## Development Tools

### Required

- Visual Studio 2022 or later
- .NET 10 SDK
- Windows SDK 10.0.19041.0 or later
- C++ desktop development workload

### Recommended

- Visual Studio extensions:
  - XAML Designer
  - WinUI 3 templates
  - C++/CLI support
- Git for version control
- NuGet Package Manager

### Optional

- Visual Studio Code (for quick edits)
- LINQPad (for testing LINQ queries)
- ILSpy / dnSpy (for .NET inspection)
- Dependency Walker (for native DLL debugging)

---

## Platform Support

### Target Platforms

| Platform | Support Level | Notes |
|----------|---------------|-------|
| Windows 11 | ✅ Full support | Primary target |
| Windows 10 (1809+) | ✅ Full support | Minimum version |
| Windows 10 (older) | ❌ Not supported | Missing WinUI 3 APIs |
| Windows ARM64 | ✅ Supported | Via WinUI 3 |

### Future Platform Considerations

**MAUI Expansion:**
- macOS - Possible via .NET MAUI
- iOS/Android - Possible with cloud rendering backend
- Linux - Possible via Avalonia (alternative to WinUI)

**See:** [MAUI Compatibility Considerations](07-maui-compatibility.md)

---

## Performance Characteristics

### Binary Sizes (Estimated)

| Build Type | App Size | With .NET Runtime | Notes |
|------------|----------|-------------------|-------|
| Debug | ~5 MB | ~155 MB | Development only |
| Release (Framework-dependent) | ~3 MB | N/A | Requires .NET 10 installed |
| Release (Self-contained) | ~150 MB | N/A | Includes .NET runtime |
| Release (ReadyToRun) | ~160 MB | N/A | Faster startup |

### Startup Performance

- **Cold start:** 1-2 seconds (self-contained)
- **Warm start:** <500ms
- **With ReadyToRun:** ~30% faster cold start

### Memory Usage

- **Baseline (idle):** ~50-80 MB
- **With fractal loaded:** ~100-200 MB
- **During render:** Varies by fractal complexity and precision

---

## CI/CD Considerations

### Build Pipeline Requirements

**Build Agents:**
- Windows-based agents (for WinUI 3)
- Visual Studio 2022 build tools
- .NET 10 SDK installed

**Build Steps:**
1. Restore NuGet packages
2. Build C++ projects (ManpCore.Native)
3. Build C# projects (ManpWinUI)
4. Run unit tests
5. Create MSIX package
6. Sign package (for distribution)

**Example GitHub Actions:**
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v3
  with:
    dotnet-version: '10.0.x'

- name: Setup MSBuild
  uses: microsoft/setup-msbuild@v1

- name: Restore dependencies
  run: dotnet restore

- name: Build
  run: dotnet build --configuration Release

- name: Test
  run: dotnet test --configuration Release
```

---

## Summary

**Core Stack:**
- .NET 10 + WinUI 3 for modern Windows UI
- C++/CLI for interop with existing C++ math engine
- MVVM pattern with CommunityToolkit.Mvvm
- Dependency injection with Microsoft.Extensions.DependencyInjection

**Key Libraries:**
- Keep existing: MPFR, QD, zlib, parser
- Add new: Windows Media Foundation, logging, UI toolkit

**Platform:**
- Windows 10 (1809+) and Windows 11
- x64 and ARM64 support
- Future MAUI expansion possible
