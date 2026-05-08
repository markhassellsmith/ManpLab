# Distribution Guide for ManpLab

This guide explains how to create and distribute ManpLab releases.

## Distribution Strategy

ManpLab offers **two distribution methods** to accommodate different user preferences:

### 1. Portable ZIP (Recommended for Most Users) ✅
- **No installation required** - extract and run
- **No security warnings** - runs immediately
- **Self-contained** - includes all dependencies
- **Best for**: Educational environments, quick testing, users without admin rights

### 2. MSIX Package (Optional)
- **Modern Windows app** - clean install/uninstall
- **Automatic updates** - ready for future releases
- **Security sandbox** - additional protection
- **⚠️ Caveat**: Shows security warning (unsigned package)
- **Best for**: Users comfortable with sideloading, prefer managed apps

## Creating Release Packages

### Prerequisites

- Visual Studio 2022 with .NET 10 SDK
- Windows 10/11 SDK (22621 or later)
- PowerShell 5.1 or later

### Quick Package (Portable Only)

```powershell
cd Build
.\Package-Portable.ps1 -Version "1.0.0"
```

This creates:
- `ManpLab-v1.0.0-Portable.zip` - Ready to distribute

### Full Package (Both Methods)

```powershell
cd Build
.\Package-Release.ps1 -Version "1.0.0"
```

This creates:
- `ManpLab-v1.0.0-Windows-x64-Portable.zip` - Portable distribution
- `ManpLab-v1.0.0-Windows-x64.msix` - MSIX package
- `MSIX-Installation-Guide.txt` - Instructions for MSIX users

### Package Options

```powershell
# Different configurations
.\Package-Release.ps1 -Configuration Release -Platform x64 -Version "1.0.0"

# Debug build (for testing)
.\Package-Release.ps1 -Configuration Debug -Version "1.0.0-beta"
```

## Testing Distribution Packages

### Testing Portable ZIP

**On a clean Windows 10/11 machine:**

1. Extract ZIP to `C:\Temp\ManpLab-Test\`
2. Run `ManpWinUI.exe` directly
3. Verify application launches without errors
4. Test basic fractal rendering
5. Check all dependencies are present

**If it fails:**
- Check Windows version (must be 1809 or later)
- Verify all files extracted (some files may be large)
- Ensure .NET 10 runtime is not required (should be self-contained)

### Testing MSIX Package

**Important: Test on a machine where ManpLab is NOT already installed**

1. Double-click the `.msix` file
2. **Expected**: Security warning appears
3. Click "Install anyway" or "More info" → "Install anyway"
4. Verify installation completes
5. Find app in Start Menu
6. Test basic functionality
7. Uninstall via Settings → Apps

## Uploading to GitHub Releases

### 1. Create GitHub Release

```bash
# Tag the release
git tag -a v1.0.0 -m "Release 1.0.0"
git push origin v1.0.0
```

### 2. Create Release on GitHub

1. Go to https://github.com/markhassellsmith/ManpLab/releases
2. Click "Create a new release"
3. Select tag: `v1.0.0`
4. Release title: `ManpLab v1.0.0 - Modern Fractal Explorer`

### 3. Upload Distribution Files

Attach both files:
- `ManpLab-v1.0.0-Windows-x64-Portable.zip`
- `ManpLab-v1.0.0-Windows-x64.msix`
- `MSIX-Installation-Guide.txt`

### 4. Release Notes Template

```markdown
## ManpLab v1.0.0 - Modern Fractal Explorer

A modern WinUI 3 fractal explorer powered by Paul de Leeuw's production-grade rendering engine.

### Download

**Recommended: Portable ZIP** (No security warnings, no installation)
- 📦 [ManpLab-v1.0.0-Windows-x64-Portable.zip](#) (XX MB)
  - Extract and run `ManpWinUI.exe`
  - No installation required
  - Includes all dependencies

**Alternative: MSIX Package** (Modern app installation)
- 📦 [ManpLab-v1.0.0-Windows-x64.msix](#) (XX MB)
  - ⚠️ Shows security warning (unsigned - this is normal)
  - Read: [MSIX-Installation-Guide.txt](#)
  - Provides clean install/uninstall

### Requirements

- Windows 10 (version 1809 or later) or Windows 11
- 64-bit (x64) processor
- 2 GB RAM minimum, 4 GB recommended

### Features

- 🎨 300 fractal types (246 original + 54 new)
- 🔬 Deep zoom with perturbation theory (10^100+ magnification)
- ⚡ BLA acceleration for extreme performance
- 🎬 Animation rendering with FFmpeg
- 📚 Fractal browser with metadata and bookmarks
- 🎨 Theme system (Light, Dark, Ocean Blue, System)

### What's New in v1.0.0

- Initial educational fork release
- Modern WinUI 3 interface
- Complete integration of Paul de Leeuw's native engine
- Extended from 246 to 300 fractal types
- Comprehensive fractal metadata system
- Self-contained portable distribution

[Full Documentation](https://github.com/markhassellsmith/ManpLab)
```

## Distribution Best Practices

### For README.md

Update the Quick Start section to emphasize the portable option:

```markdown
### Pre-built Executable

**Portable ZIP (Recommended)** - No installation or security warnings
- [Download Latest Release](https://github.com/markhassellsmith/ManpLab/releases/latest)
- Extract and run `ManpWinUI.exe`
- Includes all dependencies

**MSIX Package** - Modern Windows app (shows security warning)
- See [MSIX Installation Guide](docs/MSIX-Installation-Guide.md)
```

### For Educational Institutions

When distributing to students/faculty:

**Email/Announcement Template:**

```
Subject: ManpLab Fractal Explorer - Download Instructions

Hello,

ManpLab is now available for download. This is an educational fractal 
exploration tool with 300 fractal types and deep zoom capabilities.

DOWNLOAD (Recommended):
1. Download: ManpLab-v1.0.0-Windows-x64-Portable.zip
2. Extract to your preferred location
3. Run ManpWinUI.exe
4. No installation needed!

REQUIREMENTS:
- Windows 10/11 (64-bit)
- No admin rights required
- Works from USB drives

DOCUMENTATION:
Full guide: https://github.com/markhassellsmith/ManpLab

Questions? Contact: [your email]
```

## Troubleshooting Distribution

### Common Issues

**Issue: "WindowsAppRuntime not found"**
- Cause: Publish not set to self-contained
- Fix: Ensure `-p:SelfContained=true` in publish command

**Issue: Missing DLL errors**
- Cause: Not all dependencies copied
- Fix: Check publish output for warnings, ensure `PublishTrimmed=false`

**Issue: MSIX won't install**
- Cause: Unsigned package + restrictive policies
- Fix: Direct users to portable ZIP instead

**Issue: Large file size**
- Expected: 100-200 MB (includes Windows App SDK runtime)
- This is normal for self-contained WinUI 3 apps

## Code Signing (Optional - Future Enhancement)

If you decide to purchase a code signing certificate:

1. Obtain certificate from DigiCert, Sectigo, etc. (~$200-500/year)
2. Update `ManpWinUI.csproj`:
   ```xml
   <AppxPackageSigningEnabled>true</AppxPackageSigningEnabled>
   <PackageCertificateKeyFile>YourCertificate.pfx</PackageCertificateKeyFile>
   ```
3. MSIX will install without warnings

## Version Management

### Semantic Versioning

- **Major.Minor.Patch** (e.g., 1.0.0)
- **Major**: Breaking changes
- **Minor**: New features
- **Patch**: Bug fixes

### Pre-release Versions

```powershell
# Beta releases
.\Package-Release.ps1 -Version "1.1.0-beta.1"

# Release candidates
.\Package-Release.ps1 -Version "1.1.0-rc.1"
```

## Checklist for Release

- [ ] Update version in `ManpWinUI.csproj` (AssemblyVersion, FileVersion)
- [ ] Update `CHANGELOG.md` with release notes
- [ ] Run full test suite
- [ ] Build Release configuration
- [ ] Create distribution packages
- [ ] Test portable ZIP on clean machine
- [ ] Test MSIX installation
- [ ] Create GitHub release and tag
- [ ] Upload distribution files
- [ ] Announce release (website, social media, email)

## Support Resources

- GitHub Issues: https://github.com/markhassellsmith/ManpLab/issues
- Documentation: https://github.com/markhassellsmith/ManpLab/wiki
- Discussions: https://github.com/markhassellsmith/ManpLab/discussions
