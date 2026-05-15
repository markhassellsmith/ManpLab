# Application Icon Fix for Portable Builds

## Issue
The ManpWinUI.exe file in **portable/unpackaged builds** had no visible icon in Windows Explorer. The application appeared with the default generic executable icon.

**Important:** MSIX packages were working correctly and never had this issue.

## Root Cause
The ManpWinUI project was only configured for MSIX packaging, which uses PNG icon assets. Portable builds require different icon handling:

**Missing for portable builds:**
1. A `.ico` file (Windows icon format)
2. The `ApplicationIcon` property in the `.csproj` file

**Icon systems by build type:**
- **MSIX packages** ✅ Use PNG assets defined in `Package.appxmanifest` (already working)
- **Portable/unpackaged builds** ❌ Require a traditional `.ico` file embedded in the executable (was missing)

## Solution

### 1. Added Icon File
- Copied `ManpWIN64\MANPWIN.ICO` to `ManpWinUI\Assets\ManpLab.ico`
- This is the same icon used in the original Win32 application

### 2. Updated Project File
Added to `ManpWinUI.csproj`:

```xml
<!-- In PropertyGroup -->
<ApplicationIcon>Assets\ManpLab.ico</ApplicationIcon>

<!-- In ItemGroup -->
<Content Include="Assets\ManpLab.ico">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```

### 3. Verification
- ✅ Icon is now embedded in `ManpWinUI.exe`
- ✅ Build succeeds without errors
- ✅ Icon displays in Windows Explorer for portable builds

## Files Changed
- `ManpWinUI\Assets\ManpLab.ico` (added)
- `ManpWinUI\ManpWinUI.csproj` (modified)

## Testing
After rebuilding:
1. Navigate to the published output folder
2. Check `ManpWinUI.exe` in Windows Explorer
3. Verify the ManpLab icon is displayed

## Notes
- The icon file is small (766 bytes) containing a 32x32 icon
- For future releases, consider creating a modern multi-resolution .ico file with 16x16, 32x32, 48x48, and 256x256 sizes
- MSIX packages continue to use the PNG assets in the Assets folder
- Both icon types are now present and functional

## Related
- Release: v1.4.0
- Affected: Portable ZIP builds only
- MSIX: Not affected (was already working)
- Issue discovered: User reported missing icon in portable .exe
- Fix date: January 18, 2025

## Historical Context
Previous v1.3 and earlier portable builds also had this issue. The MSIX packages have always displayed icons correctly because they use the comprehensive PNG icon set via the app manifest.
