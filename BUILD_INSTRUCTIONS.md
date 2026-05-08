# RELEASE BUILD INSTRUCTIONS

## The Problem
PowerShell scripts cannot build C++ projects properly because they don't have
the Visual Studio Developer environment variables loaded.

## THE CORRECT WORKFLOW

### Step 1: Build in Visual Studio
1. **Open Visual Studio** (not PowerShell yet)
2. Open `ManpLab.sln`
3. Set **Configuration to Release**
4. Set **Platform to x64**
5. **Build > Rebuild Solution** (or Ctrl+Shift+B)
6. Wait for build to complete (~2-5 minutes)
7. **Close Visual Studio** (or keep it open)

### Step 2: Package the Build
Now in PowerShell:
```powershell
.\Build-Package.ps1
```

This will:
- Verify the build exists
- Copy all binaries to a package folder
- Create README and LICENSE files
- Compress into `ManpLab-Portable-1.1.1-x64.zip`

### Step 3: Test
1. Navigate to `Release-Output\Portable-ZIP\`
2. Extract `ManpLab-Portable-1.1.1-x64.zip`  
3. Run `ManpWinUI.exe`
4. Test settings persistence (change theme, close, reopen)

### Step 4: Release
1. If tests pass, upload ZIP to GitHub releases
2. Tag as `v1.1.1`

---

## Why This Workflow?

**Visual Studio builds properly** because it:
- Loads Developer Command Prompt environment
- Sets VCTargetsPath and other C++ variables
- Has full C++ build toolchain in PATH

**PowerShell/MSBuild alone fails** because:
- Missing Visual Studio environment variables
- Can't find C++ targets and props files
- Even `vswhere.exe` can't fix this from outside VS

---

## Alternative: Developer PowerShell

If you want to build from PowerShell, use **Developer PowerShell for VS 2022**:
1. Start Menu > "Developer PowerShell for VS 2022"
2. Navigate to solution root
3. Run: `msbuild ManpLab.sln /p:Configuration=Release /p:Platform=x64`
4. Run: `.\Build-Package.ps1`

---

## Scripts Overview

**Build-Package.ps1** - ✅ USE THIS
- Packages already-built binaries
- Fast (seconds, not minutes)
- Always works

**Build-Portable.ps1** - ❌ BROKEN
- Tries to build from regular PowerShell
- Fails on C++ projects
- DO NOT USE (kept for reference)

**Build-Release.ps1** - ❌ BROKEN  
- Same issue as Build-Portable
- DO NOT USE (kept for reference)

---

Last Updated: $(Get-Date -Format 'yyyy-MM-dd')
