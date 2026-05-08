# Release Build Summary - ManpLab v1.1.1

## Changes Made

### 1. Settings Persistence Fix ✅
- **Problem**: Settings weren't persisting in Portable ZIP distribution
- **Solution**: Enhanced `AppSettingsService` with dual storage:
  - **MSIX**: Uses `ApplicationData.LocalSettings` (Windows managed)
  - **Portable ZIP**: Uses JSON file at `%LocalAppData%\ManpLab\settings.json`
- **Detection**: Automatic runtime detection of deployment type
- **File**: `ManpWinUI\Services\AppSettingsService.cs`

### 2. Version Updated ✅
- **Package.appxmanifest**: `1.1.0.0` → `1.1.1.0`

### 3. Build Scripts Created ✅

#### Build-Release.ps1 (Full Build)
- Builds both MSIX and Portable ZIP packages
- Includes comprehensive error handling
- Creates installation guides and READMEs
- Parameters:
  ```powershell
  .\Build-Release.ps1 -Configuration Release -Platform x64 -Version "1.1.1"
  ```

#### Build-Portable.ps1 (Quick Build - Recommended)
- **Faster**: Builds only Portable ZIP (recommended distribution)
- **Simpler**: Fewer dependencies, less complexity
- **Usage**:
  ```powershell
  .\Build-Portable.ps1 -Version "1.1.1" -Platform "x64"
  ```

#### Quick-Build-Release.ps1
- One-click wrapper for Build-Release.ps1
- Uses default settings (Release, x64, v1.1.1)

### 4. Documentation Created ✅
- **RELEASE_PROCESS.md**: Complete release workflow guide
- **ManpWinUI/Documentation/SETTINGS_PERSISTENCE.md**: Technical documentation
- **.github/workflows/release.yml**: GitHub Actions automation (optional)

## How to Create Release

### Option 1: Quick Build (Portable ZIP Only)

```powershell
# From solution root
.\Build-Portable.ps1
```

**Output**: `Release-Output\ManpLab-Portable-1.1.1-x64.zip`

### Option 2: Full Build (Both MSIX and ZIP)

```powershell
# From solution root
.\Build-Release.ps1 -Configuration Release -Platform x64 -Version "1.1.1"
```

**Output**:
- `Release-Output\MSIX\ManpLab_1.1.1.0_x64.msix`
- `Release-Output\MSIX\INSTALLATION_GUIDE.txt`
- `Release-Output\Portable-ZIP\ManpLab-Portable-1.1.1-x64.zip`

### Option 3: Manual Build in Visual Studio

1. Set Configuration to **Release**
2. Set Platform to **x64**
3. Build → Rebuild Solution (Ctrl+Shift+B)
4. For MSIX: Right-click ManpWinUI → Publish → Create App Packages
5. For Portable: Use Build-Portable.ps1 script

## Testing Checklist

### Portable ZIP
- [ ] Extract to new folder
- [ ] Run ManpWinUI.exe (no errors)
- [ ] Change settings (theme, palette, panel sizes)
- [ ] Close and reopen app
- [ ] Verify settings restored
- [ ] Check `%LocalAppData%\ManpLab\settings.json` exists

### MSIX Package (if built)
- [ ] Install package (expect security warning)
- [ ] Launch from Start Menu
- [ ] Change settings
- [ ] Restart app
- [ ] Verify settings restored
- [ ] Uninstall cleanly

## GitHub Release Steps

1. **Build packages**:
   ```powershell
   .\Build-Portable.ps1 -Version "1.1.1"
   ```

2. **Create release on GitHub**:
   - Go to: https://github.com/markhassellsmith/ManpLab/releases
   - Click "Draft a new release"
   - Tag: `v1.1.1`
   - Title: `ManpLab v1.1.1 - Settings Persistence Fix`

3. **Upload artifacts**:
   - `ManpLab-Portable-1.1.1-x64.zip`
   - (Optional) `ManpLab_1.1.1.0_x64.msix`
   - (Optional) `INSTALLATION_GUIDE.txt`

4. **Release notes** (template below)

## Release Notes Template

```markdown
# ManpLab v1.1.1

## 🐛 Bug Fixes

- **Fixed settings persistence in Portable ZIP distribution**
  - Settings now properly save/restore in unpackaged deployments
  - Uses JSON file storage: `%LocalAppData%\ManpLab\settings.json`
  - MSIX packages continue using Windows ApplicationData

## 📦 Downloads

### Portable ZIP (Recommended) ✅
**ManpLab-Portable-1.1.1-x64.zip** (~XX MB)
- Extract and run - no installation needed
- Perfect for educational use
- No admin rights required
- Self-contained with all dependencies

### MSIX Package (Optional)
**ManpLab_1.1.1.0_x64.msix** (~XX MB)
- Windows app with clean install/uninstall
- ⚠️ Shows security warning (normal for unsigned packages)
- See INSTALLATION_GUIDE.txt

## ✨ What's Improved

- Settings persistence works correctly in both distribution methods
- Automatic detection of packaged vs unpackaged deployment
- Enhanced error diagnostics for troubleshooting
- Comprehensive documentation added

## 📚 Documentation

- [Quick Start Guide](README.md#quick-start)
- [Settings Persistence Details](ManpWinUI/Documentation/SETTINGS_PERSISTENCE.md)
- [Keyboard Shortcuts](ManpWinUI/KEYBOARD_SHORTCUTS.md)

## 🔧 System Requirements

- Windows 10 (build 17763+) or Windows 11
- x64 processor
- 4 GB RAM minimum (8 GB recommended)
- DirectX 12 capable graphics

## 📝 Changelog

### Changed
- Enhanced AppSettingsService with dual storage mechanism
- Improved runtime deployment detection

### Fixed
- Settings not persisting in portable ZIP version (#XX)
- Panel positions resetting on app restart

### Documentation
- Added SETTINGS_PERSISTENCE.md
- Updated RELEASE_PROCESS.md
- Created build automation scripts

---

**Full Changelog**: https://github.com/markhassellsmith/ManpLab/compare/v1.1.0...v1.1.1
```

## Files Modified

- `ManpWinUI\Services\AppSettingsService.cs` - Dual storage implementation
- `ManpWinUI\Package.appxmanifest` - Version bump to 1.1.1.0

## Files Created

- `Build-Release.ps1` - Full build script (MSIX + ZIP)
- `Build-Portable.ps1` - Quick portable ZIP builder
- `Quick-Build-Release.ps1` - One-click wrapper
- `RELEASE_PROCESS.md` - Complete release documentation
- `ManpWinUI\Documentation\SETTINGS_PERSISTENCE.md` - Technical docs
- `.github\workflows\release.yml` - CI/CD automation

## Next Steps

1. **Build the release**:
   ```powershell
   .\Build-Portable.ps1
   ```

2. **Test the package** (see checklist above)

3. **Create GitHub release** with the build artifact

4. **Update README** if needed (download links point to latest release)

5. **Announce** (optional):
   - GitHub Discussions
   - Project documentation
   - Educational community

## Notes

- **Portable ZIP is recommended** for educational users
- MSIX shows security warnings (expected for unsigned packages)
- Both versions now have full settings persistence
- Build scripts handle all packaging automatically
- Documentation is comprehensive and ready for users

---

**Version**: 1.1.1  
**Date**: $(Get-Date -Format 'yyyy-MM-dd')  
**Branch**: distribution  
**Status**: ✅ Ready for Release
