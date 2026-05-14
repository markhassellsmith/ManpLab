# Debug vs Release Build Configuration in ManpLab

## Quick Reference: The Four Build Scenarios

| | **Debug Configuration** | **Release Configuration** |
|---|------------------------|---------------------------|
| **🟢 F5 (Solid Arrow)**<br>Start Debugging | **Development Mode**<br>Full debugging, slower<br>✅ Breakpoints work<br>✅ Variable inspection<br>🐢 10-30% slower than normal | **Pre-Release Testing**<br>Debug optimized code<br>⚠️ Breakpoints may skip<br>⚠️ Variables may be optimized away<br>⚡ Fast but still attached |
| **▷ Ctrl+F5 (Outline)**<br>Start Without Debugging | **Fast Development Testing**<br>No debugging, faster<br>❌ No breakpoints<br>❌ No variable inspection<br>⚡ Faster than F5 debug mode | **Production Simulation**<br>Exactly what users experience<br>❌ No debugging capability<br>⚡ Maximum performance<br>✅ True user experience |

**→ Most Common:** Debug + F5 (top-left) for daily work  
**→ Before Release:** Release + Ctrl+F5 (bottom-right) to test what users will see

---

## Overview
When you switch between **Debug** and **Release** configurations in Visual Studio and perform a build, the compiler produces different optimized outputs with varying levels of debugging support and performance characteristics.

---

## Running Your Application: The Green Arrows

Visual Studio provides **two different ways** to run your application:

### **🟢 Solid Green Arrow (▶️) - "Start Debugging" (F5)**

**What it does:**
- Builds the solution (if needed)
- **Attaches the Visual Studio debugger**
- Launches the application
- Enables breakpoints, step-through debugging, variable inspection
- Performance impact: ~10-30% slower due to debugger overhead

**When to use:**
- ✅ Investigating bugs
- ✅ Setting breakpoints to inspect code flow
- ✅ Examining variable values at runtime
- ✅ Step-through debugging (F10/F11)
- ✅ Catching exceptions with detailed information
- ✅ Daily development work

**Behavior:**
```
[Build] → [Deploy MSIX] → [Attach Debugger] → [Launch App] → [Wait for breakpoints]
```

---

### **▷ Outline Green Arrow (▷) - "Start Without Debugging" (Ctrl+F5)**

**What it does:**
- Builds the solution (if needed)
- **Does NOT attach the debugger**
- Launches the application at full speed
- No breakpoints, no step-through, no variable inspection
- Performance: Native speed (no debugger overhead)

**When to use:**
- ✅ Testing application performance
- ✅ Measuring actual startup times
- ✅ Benchmarking fractal rendering speed
- ✅ Testing user experience without debugging artifacts
- ✅ Reproducing issues that don't occur when debugging
- ✅ Final testing before release

**Behavior:**
```
[Build] → [Deploy MSIX] → [Launch App] → [Run at full speed]
```

---

### **Key Differences Summary**

| Aspect | Solid Arrow (F5) | Outline Arrow (Ctrl+F5) |
|--------|------------------|-------------------------|
| **Debugger** | ✅ Attached | ❌ Not attached |
| **Breakpoints** | ✅ Work | ❌ Ignored |
| **Performance** | 🐢 10-30% slower | ⚡ Full speed |
| **Exception Handling** | Breaks on exceptions | Shows error dialog |
| **Console Output** | Debug Output window | Separate console (if applicable) |
| **Use Case** | Development/Debugging | Testing/Performance |
| **Stop Behavior** | Detaches debugger | Must close app manually |

---

### **Special Note for MSIX Apps**

Both buttons trigger the **Deploy** target in your project, which:
1. Registers the MSIX package with Windows
2. Updates the installed app
3. Launches the registered app

The command executed (from your .csproj):
```powershell
Add-AppxPackage -Register 'AppxManifest.xml' -ForceApplicationShutdown -ForceUpdateFromAnyVersion
```

---

### **Pro Tips**

**🔍 Debugging Performance Issues:**
- Use **F5 (solid arrow)** with Release configuration
- This gives you debugging capability while testing optimized code
- Breakpoints may not work perfectly due to optimization, but exceptions are caught

**⚡ Testing Real Performance:**
- Use **Ctrl+F5 (outline arrow)** with Release configuration
- This is what end-users experience
- No debugger overhead, full optimizations active

**🐛 Investigating Heisenbug (bug that disappears when debugging):**
- Try **Ctrl+F5** to reproduce the issue
- Add extensive logging instead of breakpoints
- Check Serilog output in `%LocalAppData%\ManpWinUI\logs\`

---

## What Happens When You Build

### **Build Process Flow**

```
[Switch Configuration] → [Build Solution] → [Compile Projects] → [Copy Dependencies] → [Create Output]
```

1. **Switch Configuration** (Debug/Release dropdown in VS)
2. **Build Solution** (F6 or Build → Build Solution)
3. Projects compile in order:
   - Native C++ projects (ManpCore.Native, ManpWIN64, etc.)
   - .NET projects (ManpCore.Services, ManpWinUI)
4. Dependencies copied to output directories
5. MSIX package structure created (for packaged deployments)

---

## Output Directory Structure

### **Base Path**
```
ManpWinUI\bin\x64\<Configuration>\net10.0-windows10.0.22621.0\win-x64\
```

Where `<Configuration>` is either:
- **Debug** - Development builds with debugging symbols
- **Release** - Production builds with optimizations

### **Full Path Examples**

**Debug:**
```
C:\Users\Mark\source\repos\ManpLab\ManpWinUI\bin\x64\Debug\net10.0-windows10.0.22621.0\win-x64\
```

**Release:**
```
C:\Users\Mark\source\repos\ManpLab\ManpWinUI\bin\x64\Release\net10.0-windows10.0.22621.0\win-x64\
```

---

## Files Produced

### **1. Core Application Files**

| File | Size (approx) | Purpose |
|------|--------------|---------|
| `ManpWinUI.exe` | 200-400 KB | Main application executable |
| `ManpWinUI.dll` | 5-15 MB | Application logic assembly |
| `ManpWinUI.pdb` | Debug: 5-20 MB<br>Release: Optional | Debug symbols |
| `ManpWinUI.deps.json` | ~50 KB | Dependency manifest |
| `ManpWinUI.runtimeconfig.json` | ~1 KB | Runtime configuration |

### **2. Native Dependencies (from x64\\<Configuration>\\)**

| File | Size | Purpose |
|------|------|---------|
| `ManpCore.Native.dll` | 2-5 MB | Native fractal engine wrapper |
| `ManpWIN64.dll` | 5-15 MB | Core fractal algorithms (C++) |
| `zlibd1.dll` (Debug) | ~200 KB | Compression library (debug) |
| `zlib1.dll` (Release) | ~100 KB | Compression library (release) |
| `mpeg.dll` | ~1 MB | MPEG encoding support |
| `qdlib.dll` | ~500 KB | Quad-double precision math |

### **3. Windows App SDK Runtime**

| File | Size | Purpose |
|------|------|---------|
| `Microsoft.ui.xaml.dll` | ~15 MB | WinUI 3 framework |
| `Microsoft.WindowsAppRuntime.*.dll` | ~20 MB total | App runtime components |
| `WinRT.Runtime.dll` | ~500 KB | WinRT interop layer |
| `Microsoft.Web.WebView2.*.dll` | ~3 MB | WebView2 control |

### **4. .NET Runtime & Dependencies**

| Category | Count | Total Size |
|----------|-------|------------|
| System.*.dll | ~150 files | ~50 MB |
| Language resource folders | ~75 folders | ~200 MB |
| NuGet packages | ~30 assemblies | ~40 MB |

**Key Dependencies:**
- `System.Text.Json.dll` (1.9 MB) - JSON serialization
- `CommunityToolkit.Mvvm.dll` (~200 KB) - MVVM framework
- `Serilog.*.dll` (~500 KB total) - Logging
- `Microsoft.Extensions.*.dll` (~2 MB total) - DI & Logging
- `Xabe.FFmpeg.dll` (~150 KB) - Video encoding
- `Microsoft.Graphics.Win2D.dll` (~3 MB) - GPU rendering

### **5. Application Assets**

| Item | Location | Purpose |
|------|----------|---------|
| `Assets\*.png` | `Assets\` | App icons, splash screen |
| `Themes\OceanBlue.xaml` | `Themes\` | Custom theme |
| `AppxManifest.xml` | Root | MSIX package manifest |
| `resources.pri` | Root | Compiled XAML resources |

### **6. MSIX Package Files (when WindowsPackageType=MSIX)**

| Folder | Contents |
|--------|----------|
| `AppX\` | Complete packaged app layout |
| `AppxMetadata\` | Package metadata |
| `Dependencies\` | Framework dependencies |

---

## Debug vs Release: Key Differences

### **Debug Configuration**

**Purpose:** Development, troubleshooting, step-through debugging

| Aspect | Setting |
|--------|---------|
| **Optimizations** | Disabled |
| **Debug Symbols (.pdb)** | Always generated |
| **Code Generation** | Unoptimized, preserves stack frames |
| **Assertions** | Enabled (`DEBUG` constant defined) |
| **IL Trimming** | Disabled |
| **Ready-to-Run** | Disabled |
| **Binary Size** | Larger (~20-30% more) |
| **Performance** | Slower (2-10x) |
| **Error Checking** | Maximum |

**Special Debug Features:**
```csharp
#if DEBUG
    // Extra validation, logging, test hooks
#endif
```

**File Characteristics:**
- ✅ `ManpWinUI.pdb` - Full debug symbols (~15-20 MB)
- ✅ `zlibd1.dll` - Debug version of zlib with asserts
- ✅ Unobfuscated IL code
- ✅ Line number information preserved
- ✅ Local variable names preserved

---

### **Release Configuration**

**Purpose:** Distribution, end-user deployment, performance testing

| Aspect | Setting |
|--------|---------|
| **Optimizations** | Aggressive |
| **Debug Symbols (.pdb)** | Optional (not included by default) |
| **Code Generation** | Highly optimized, inlined |
| **Assertions** | Disabled |
| **IL Trimming** | ~~Enabled~~ (Currently disabled in your project) |
| **Ready-to-Run** | ~~Enabled~~ (Currently disabled in your project) |
| **Binary Size** | Smaller |
| **Performance** | Optimized (2-10x faster) |
| **Error Checking** | Minimal |

**Optimizations Applied:**
- Method inlining
- Loop unrolling
- Dead code elimination
- Constant folding
- Register allocation optimization
- Branch prediction hints

**File Characteristics:**
- ❌ No `.pdb` files (unless explicitly requested)
- ✅ `zlib1.dll` - Release version (smaller, faster)
- ✅ Obfuscated/optimized IL code
- ❌ No line numbers
- ❌ No local variable names
- ✅ Stripped metadata

---

## Native DLL Compilation

Your project includes **native C++ projects** that also compile separately:

### **x64\\Debug\\ vs x64\\Release\\**

Located at: `C:\Users\Mark\source\repos\ManpLab\x64\<Configuration>\`

**Debug Native DLLs:**
- Compiled with `/Od` (no optimization)
- Include debug symbols
- Use debug runtime (`/MDd`)
- Enable runtime checks (`/RTC1`)
- Example: `zlibd1.dll` (~200 KB)

**Release Native DLLs:**
- Compiled with `/O2` (maximum speed) or `/Ox`
- Stripped symbols
- Use release runtime (`/MD`)
- No runtime checks
- Example: `zlib1.dll` (~100 KB)

**Custom Build Target (CopyNativeWrapper):**
```xml
<Target Name="CopyNativeWrapper" AfterTargets="Build">
    <Copy SourceFiles="$(SolutionDir)x64\$(Configuration)\*.dll" 
          DestinationFolder="$(OutDir)" />
</Target>
```

This ensures the correct native DLLs (Debug or Release) are copied to match your current configuration.

---

## MSIX Packaging (WindowsPackageType=MSIX)

When building as an **MSIX package**, additional steps occur:

### **AppX Folder Structure**

```
ManpWinUI\bin\x64\<Configuration>\net10.0-windows10.0.22621.0\win-x64\AppX\
├── ManpWinUI.exe
├── ManpWinUI.dll
├── *.dll (all dependencies)
├── AppxManifest.xml
├── resources.pri
├── Assets\
└── Dependencies\
    └── x64\
        └── Microsoft.WindowsAppRuntime.*.appx
```

### **Custom Targets for MSIX**

1. **CopyNativeToAppX** - Copies native DLLs to `AppX\` folder
2. **EnsureNativeDllBeforeRun** - Pre-build verification
3. **CopyNativeDllsAfterLayout** - Post-manifest generation copy
4. **Deploy** - PowerShell registration for F5 debugging

```powershell
Add-AppxPackage -Register 'AppxManifest.xml' -ForceApplicationShutdown
```

---

## Configuration Comparison Table

| File/Folder | Debug | Release | Notes |
|-------------|-------|---------|-------|
| **ManpWinUI.exe** | ✅ 400 KB | ✅ 250 KB | Smaller in Release |
| **ManpWinUI.dll** | ✅ 15 MB | ✅ 10 MB | 30% reduction |
| **ManpWinUI.pdb** | ✅ 20 MB | ❌ - | Not generated |
| **zlibd1.dll** | ✅ 207 KB | ❌ - | Debug only |
| **zlib1.dll** | ❌ - | ✅ ~100 KB | Release only |
| **Total Size** | ~400 MB | ~350 MB | Includes all deps |
| **Language Folders** | ✅ ~75 | ✅ ~75 | Same count |
| **Startup Time** | ~3-5s | ~1-2s | Faster in Release |
| **Debugging** | ✅ Full | ⚠️ Limited | Breakpoints may not work in Release |

---

## What Changes When You Switch

### **Switching from Debug → Release**

1. ✅ **Solution rebuilds** native projects in Release mode
2. ✅ **Different DLLs copied** from `x64\Release\`
3. ✅ **Optimizations enabled** in C# compiler
4. ✅ **Debug symbols excluded** (no .pdb files)
5. ✅ **Smaller binaries** produced
6. ⚠️ **Debugging capability reduced**

### **Switching from Release → Debug**

1. ✅ **Solution rebuilds** native projects in Debug mode
2. ✅ **Different DLLs copied** from `x64\Debug\`
3. ✅ **Optimizations disabled**
4. ✅ **Debug symbols generated** (.pdb files)
5. ✅ **Larger binaries** produced
6. ✅ **Full debugging support**

---

## Common Build Issues & Solutions

### **Issue: Native DLL not found**

**Symptom:**
```
[CopyNativeWrapper] ManpCore.Native.dll not found
```

**Solution:**
Build the native projects first:
```
Build → Batch Build → Select all x64/Debug or x64/Release → Build
```

---

### **Issue: Wrong version of DLL loaded**

**Symptom:** App crashes with `DllNotFoundException` or behaves unexpectedly

**Cause:** Mixed Debug/Release binaries

**Solution:**
1. Clean solution (Build → Clean Solution)
2. Rebuild all projects in same configuration
3. Verify `x64\<Configuration>\` contains correct DLLs

---

### **Issue: AppX deployment fails**

**Symptom:**
```
Error deploying MSIX package
```

**Solution:**
1. Close any running instances of ManpWinUI
2. Unregister old package:
   ```powershell
   Get-AppxPackage *ManpWinUI* | Remove-AppxPackage
   ```
3. Rebuild and redeploy

---

## Practical Examples

### **Example 1: "My fractal renders slowly"**

**Wrong approach:** Test in Debug + F5
- You're measuring Debug code with debugger overhead
- Could be 10x slower than reality

**Right approach:**
1. Switch to **Release** configuration
2. Press **Ctrl+F5** (outline arrow)
3. Now you're measuring actual performance

---

### **Example 2: "The app crashes randomly"**

**Right approach:**
1. Switch to **Debug** configuration
2. Press **F5** (solid arrow)
3. Set breakpoints near the crash location
4. Step through code to find the issue

**If it doesn't crash in Debug:**
- It might be an optimization-related bug (Heisenbug)
- Try **Release + F5** to debug optimized code
- Add extensive logging to track the issue

---

### **Example 3: "I want to show the app to someone"**

**Best approach:**
1. Switch to **Release** configuration
2. Press **Ctrl+F5** (outline arrow)
3. App runs at full speed with no debugger pop-ups

**Why not Debug + F5?**
- Slower performance makes bad impression
- Debug assertions might pop up
- Debugger artifacts visible in taskbar

---

### **Example 4: "Testing before releasing v1.5"**

**Checklist:**
1. ✅ Switch to **Release** configuration
2. ✅ Clean Solution (Build → Clean Solution)
3. ✅ Rebuild All (Build → Rebuild Solution)
4. ✅ Press **Ctrl+F5** (outline arrow)
5. ✅ Test all major features
6. ✅ Verify performance is acceptable
7. ✅ Check startup time
8. ✅ Test on fresh machine (or after uninstalling dev version)

---

## Common Pitfalls

### **❌ Pitfall 1: Testing Performance in Debug Mode**
```
"Why is fractal rendering so slow? It takes 10 seconds!"
→ Check: Are you in Debug mode? Switch to Release!
```

### **❌ Pitfall 2: Distributing Debug Builds**
```
"Users are complaining the app is huge and slow"
→ Check: Did you send them a Debug build? Always use Release!
```

### **❌ Pitfall 3: Assuming Release Works Like Debug**
```
"It works perfectly on my machine!"
→ Check: Did you test in Release + Ctrl+F5? Optimizations can change behavior!
```

### **❌ Pitfall 4: Debugging Release Builds with F5**
```
"My breakpoints aren't working!"
→ Expected: Release builds optimize away some code, making breakpoints unreliable
→ Solution: Use Debug mode for breakpoints, or add logging to Release builds
```

---

## Performance Impact

### **Typical Performance Differences**

| Scenario | Debug | Release | Improvement |
|----------|-------|---------|-------------|
| **App Startup** | 3-5s | 1-2s | 2-3x faster |
| **Fractal Render** | 5-10s | 1-2s | 5x faster |
| **UI Response** | 50-100ms | 10-20ms | 5x faster |
| **Memory Usage** | +20-30% | Baseline | More efficient |

---

## Summary

### **Configuration Guide**

| Task | Configuration | Run Method | Why |
|------|---------------|------------|-----|
| **Daily Development** | Debug | F5 (Solid Arrow) | Full debugging capability |
| **Investigating Bugs** | Debug | F5 (Solid Arrow) | Breakpoints and variable inspection |
| **Testing UX** | Debug | Ctrl+F5 (Outline) | See app without debugger artifacts |
| **Performance Testing** | Release | Ctrl+F5 (Outline) | True end-user performance |
| **Final QA** | Release | Ctrl+F5 (Outline) | Exactly what users will experience |
| **Pre-Release Debugging** | Release | F5 (Solid Arrow) | Debug optimized code issues |

### **When to Use Debug Configuration**
- ✅ Daily development
- ✅ Troubleshooting crashes
- ✅ Step-through debugging
- ✅ Investigating logic errors
- ✅ Testing new features

### **When to Use Release Configuration**
- ✅ Performance testing
- ✅ End-user builds
- ✅ Distribution (MSIX/ZIP)
- ✅ Benchmarking
- ✅ Final quality assurance
- ✅ Reproducing optimization-related bugs

### **When to Use F5 (Solid Arrow)**
- ✅ Need breakpoints
- ✅ Need to inspect variables
- ✅ Investigating exceptions
- ✅ Step-through code execution
- ✅ Most development work

### **When to Use Ctrl+F5 (Outline Arrow)**
- ✅ Testing actual performance
- ✅ Measuring render times
- ✅ User experience testing
- ✅ Investigating Heisenbugs (bugs that disappear when debugging)
- ✅ Final validation before release

### **Key Takeaways**

1. **Debug builds** are for developers. **Release builds** are for users.
2. **F5 (solid arrow)** is for debugging. **Ctrl+F5 (outline)** is for performance.
3. Always test in **Release + Ctrl+F5** before distribution to see what users will experience.
4. The behavior can differ significantly between configurations and run modes due to:
   - Compiler optimizations
   - Debugger overhead
   - Different runtime checks
   - Timing differences

---

**Last Updated:** May 2026  
**Applies To:** ManpLab .NET 10.0 / Visual Studio 2026
