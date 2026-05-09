# Release Process Guide

## Quick Release (Local Build)

### Option 1: Quick Build Script
```powershell
.\Quick-Build-Release.ps1
```

This builds both MSIX and Portable ZIP packages with default settings.

### Option 2: Full Build Script
```powershell
.\Build-Release.ps1 -Configuration Release -Platform x64 -Version "1.1.1"
```

**Available Parameters:**
- `-Configuration`: Debug or Release (default: Release)
- `-Platform`: x64, x86, or ARM64 (default: x64)
- `-Version`: Version string (default: 1.1.0)
- `-SkipBuild`: Skip rebuild, use existing binaries
- `-MsixOnly`: Create only MSIX package
- `-ZipOnly`: Create only Portable ZIP
- `-SkipClean`: Don't clean output directories first

**Examples:**
```powershell
# Build only portable ZIP
.\Build-Release.ps1 -ZipOnly

# Build both packages for ARM64
.\Build-Release.ps1 -Platform ARM64

# Use existing build, just package
.\Build-Release.ps1 -SkipBuild
```

## Output Structure

```
Release-Output/
├── MSIX/
│   ├── ManpLab_1.1.1.0_x64.msix
│   └── INSTALLATION_GUIDE.txt
└── Portable-ZIP/
    ├── ManpLab-Portable-1.1.1-x64/
    │   ├── ManpWinUI.exe
    │   ├── ManpCore.Native.dll
    │   ├── (all dependencies)
    │   ├── README.txt
    │   └── LICENSE
    └── ManpLab-Portable-1.1.1-x64.zip
```

## Manual Build Process

### 1. Update Version Numbers

**Package.appxmanifest:**
```xml
<Identity Version="1.1.1.0" />
```

**AssemblyInfo (if used):**
```csharp
[assembly: AssemblyVersion("1.1.1.0")]
```

### 2. Build in Visual Studio

1. Set Configuration to **Release**
2. Set Platform to **x64**
3. Clean Solution (Build → Clean Solution)
4. Rebuild Solution (Build → Rebuild Solution)

### 3. Create MSIX Package

**Via Visual Studio:**
1. Right-click ManpWinUI project
2. Publish → Create App Packages
3. Choose "Sideloading"
4. Configure version and output location
5. Click "Create"

**Via Command Line:**
```powershell
msbuild ManpWinUI\ManpWinUI.csproj `
  /t:Publish `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:GenerateAppxPackageOnBuild=true `
  /p:AppxPackageDir="Output\MSIX\" `
  /p:UapAppxPackageBuildMode=SideloadOnly
```

### 4. Create Portable ZIP

**Publish self-contained:**
```powershell
dotnet publish ManpWinUI\ManpWinUI.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output "Output\Portable" `
  -p:PublishSingleFile=false `
  -p:WindowsPackageType=None
```

**Create ZIP:**
```powershell
Compress-Archive -Path "Output\Portable\*" `
  -DestinationPath "ManpLab-Portable-1.1.1-x64.zip"
```

## GitHub Release Process

> **📘 For detailed instructions, see [GITHUB_RELEASE_GUIDE.md](GITHUB_RELEASE_GUIDE.md)**

### 🚀 Quick Start with Helper Script (Recommended)

```powershell
# This script handles everything: build, tag, and prepare for GitHub
.\Prepare-GitHub-Release.ps1 -Version "1.1.1"
```

This will:
1. ✅ Build MSIX and ZIP packages
2. ✅ Run validation checks
3. ✅ Create Git tag
4. ✅ Push to GitHub
5. ✅ Open browser to create release

---

### 📝 Manual Process

#### Creating a New Release

1. **Build packages:**
   ```powershell
   .\Build-Release.ps1 -Version "1.1.1"
   ```

2. **Create Git tag:**
   ```bash
   git tag -a v1.1.1 -m "Release v1.1.1"
   git push origin v1.1.1
   ```

3. **Create release on GitHub:**
   - Go to: https://github.com/markhassellsmith/ManpLab/releases
   - Click "Draft a new release"
   - Select tag: `v1.1.1`
   - Add title: `ManpLab v1.1.1 - [Description]`
   - Upload files from `Release-Output\`:
     - `ManpLab-1.1.1-x64.msix`
     - `ManpLab-Portable-1.1.1-x64.zip`
   - Click "Publish release"

#### Replacing Files in Existing Release

**Option A: Edit Release (Quick Fix)**
1. Go to your release page
2. Click "Edit release" (pencil icon)
3. Delete old files (click 🗑️ icon)
4. Upload new files (drag & drop)
5. Click "Update release"

**Option B: Using GitHub CLI**
```powershell
# Delete old files
gh release delete-asset v1.1.1 ManpLab-1.1.1-x64.msix
gh release delete-asset v1.1.1 ManpLab-Portable-1.1.1-x64.zip

# Upload new files
gh release upload v1.1.1 `
  "Release-Output\MSIX\ManpLab-1.1.1-x64.msix" `
  "Release-Output\Portable-ZIP\ManpLab-Portable-1.1.1-x64.zip"
```

**Option C: Delete and Recreate**
```powershell
# On GitHub: Go to release → Delete
# Delete tag:
git push --delete origin v1.1.1
git tag -d v1.1.1

# Recreate:
git tag -a v1.1.1 -m "Release v1.1.1"
git push origin v1.1.1
# Then create new release on GitHub
```

---

### Using GitHub Actions (Automated)

1. **Create and push a tag:**
   ```bash
   git tag -a v1.1.1 -m "Release v1.1.1"
   git push origin v1.1.1
   ```

2. **Monitor workflow:**
   - Go to GitHub → Actions
   - Watch the "Release Build" workflow
   - Artifacts are automatically uploaded

3. **Manual trigger (if needed):**
   - GitHub → Actions → Release Build → Run workflow
   - Enter version number
   - Click "Run workflow"

### Manual GitHub Release

1. **Build packages locally:**
   ```powershell
   .\Build-Release.ps1 -Version "1.1.1"
   ```

2. **Create release on GitHub:**
   - Go to Releases → Draft a new release
   - Choose/create tag: `v1.1.1`
   - Release title: `ManpLab v1.1.1`
   - Add release notes (see template below)

3. **Upload artifacts:**
   - Drag and drop files from `Release-Output/`
   - MSIX package (.msix file)
   - Portable ZIP (.zip file)
   - INSTALLATION_GUIDE.txt

4. **Publish release**

## Release Notes Template

```markdown
# ManpLab v1.1.1 - Release Notes

## 🎉 Highlights

- Fixed settings persistence in Portable ZIP distribution
- Improved MSIX package installation experience
- Enhanced documentation

## 📦 Downloads

### Portable ZIP (Recommended) ✅
- **ManpLab-Portable-1.1.1-x64.zip** (XX MB)
- Extract and run - no installation needed
- Perfect for educational use and quick testing

### MSIX Package
- **ManpLab_1.1.1.0_x64.msix** (XX MB)
- Clean install/uninstall via Windows Settings
- See INSTALLATION_GUIDE.txt for setup instructions

## ✨ What's New

### Features
- Settings now persist correctly in portable distribution
- JSON-based settings storage for unpackaged apps
- Automatic detection of deployment type (MSIX vs portable)

### Improvements
- Enhanced settings service with dual storage
- Better error messages and diagnostics
- Comprehensive documentation

### Bug Fixes
- Fixed: Settings not persisting in portable ZIP version
- Fixed: Panel positions not restored on restart

## 📚 Documentation

- [Quick Start Guide](https://github.com/markhassellsmith/ManpLab#quick-start)
- [Settings Persistence](ManpWinUI/Documentation/SETTINGS_PERSISTENCE.md)
- [Keyboard Shortcuts](ManpWinUI/KEYBOARD_SHORTCUTS.md)
- [Full Documentation](ManpWinUI/README.md)

## 🔧 System Requirements

- Windows 10 (build 17763+) or Windows 11
- x64 processor
- 4 GB RAM (8 GB recommended)
- DirectX 12 capable graphics card

## 🐛 Known Issues

None reported for this version.

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete history.

---

**Full Changelog**: https://github.com/markhassellsmith/ManpLab/compare/v1.1.0...v1.1.1
```

## Testing Checklist

Before releasing, test:

### MSIX Package
- [ ] Installs without errors
- [ ] App launches successfully
- [ ] Settings persist after restart
- [ ] Uninstalls cleanly
- [ ] Works on clean Windows 10 install
- [ ] Works on Windows 11

### Portable ZIP
- [ ] Extracts without errors
- [ ] Runs without installation
- [ ] Settings saved to LocalAppData
- [ ] Settings persist after restart
- [ ] Works from USB drive
- [ ] Works without admin rights

### Both Versions
- [ ] All 300 fractals load
- [ ] Rendering works correctly
- [ ] Theme switching works
- [ ] Color palettes apply
- [ ] Panel positions save/restore
- [ ] Keyboard shortcuts work
- [ ] Animation export works (if FFmpeg available)
- [ ] Help documentation accessible

## Version Numbering

ManpLab follows semantic versioning: `MAJOR.MINOR.PATCH.BUILD`

- **MAJOR**: Breaking changes
- **MINOR**: New features, backward compatible
- **PATCH**: Bug fixes
- **BUILD**: Build number (Windows requirement for MSIX)

Examples:
- `1.0.0.0` - Initial release
- `1.1.0.0` - Added new features
- `1.1.1.0` - Bug fix release
- `2.0.0.0` - Major rewrite

## Troubleshooting Build Issues

### "MSBuild not found"
```powershell
# Install Visual Studio 2022 with:
# - .NET desktop development workload
# - Desktop development with C++ workload
```

### "NuGet restore failed"
```powershell
nuget restore ManpLab.sln
# Or in Visual Studio: Tools → NuGet Package Manager → Restore
```

### "Platform not found"
```powershell
# Ensure Platform is set correctly in Configuration Manager
# Build → Configuration Manager → Active solution platform
```

### MSIX signing errors
```powershell
# For unsigned packages:
# Set AppxPackageSigningEnabled to false in .csproj
# Users will see security warning (expected for open-source)
```

### Portable ZIP missing DLLs
```powershell
# Ensure PublishSingleFile=false
# Ensure WindowsPackageType=None
# Check that native dependencies are copied
```

## Post-Release Tasks

1. **Update documentation:**
   - Update README.md badges
   - Update version references
   - Add changelog entry

2. **Announce release:**
   - GitHub discussions
   - Project website
   - Social media (if applicable)

3. **Monitor feedback:**
   - GitHub issues
   - User reports
   - Performance metrics

4. **Plan next release:**
   - Review open issues
   - Prioritize features
   - Set milestone

---

**Last Updated:** January 2026  
**Next Scheduled Release:** TBD
