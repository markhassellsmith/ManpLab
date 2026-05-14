# MSIX Publishing Guide for ManpLab

## Overview

This guide explains how to create distributable MSIX packages for ManpLab using the included publish profiles.

---

## Quick Start

### Using Visual Studio (Recommended)

1. **Switch to Release Configuration**
   - Solution Configuration dropdown → **Release**
   - Solution Platform dropdown → **x64** (or x86/ARM64)

2. **Right-click ManpWinUI project** in Solution Explorer

3. **Select "Publish"**

4. **Choose a publish profile:**
   - `MSIX-Release-x64` (for 64-bit Windows - most common)
   - `MSIX-Release-x86` (for 32-bit Windows)
   - `MSIX-Release-ARM64` (for ARM devices like Surface Pro X)

5. **Click "Publish" button**

6. **Find your package:**
   ```
   AppPackages\ManpWinUI_<Version>_Test\
   └── ManpWinUI_<Version>_<Architecture>.msix
   ```

---

## Using Command Line

### Build Release MSIX Package (x64)

```powershell
# From solution root directory

# 1. Clean previous builds
msbuild ManpWinUI.sln /t:Clean /p:Configuration=Release /p:Platform=x64

# 2. Restore NuGet packages
dotnet restore ManpWinUI.sln

# 3. Build solution
msbuild ManpWinUI.sln /p:Configuration=Release /p:Platform=x64

# 4. Create MSIX package
msbuild ManpWinUI\ManpWinUI.csproj /t:Publish /p:Configuration=Release /p:Platform=x64 /p:PublishProfile=MSIX-Release-x64
```

### Build for All Architectures

```powershell
# x64 (most common)
msbuild ManpWinUI\ManpWinUI.csproj /t:Publish /p:Configuration=Release /p:Platform=x64 /p:PublishProfile=MSIX-Release-x64

# x86 (32-bit)
msbuild ManpWinUI\ManpWinUI.csproj /t:Publish /p:Configuration=Release /p:Platform=x86 /p:PublishProfile=MSIX-Release-x86

# ARM64 (Surface Pro X, etc.)
msbuild ManpWinUI\ManpWinUI.csproj /t:Publish /p:Configuration=Release /p:Platform=ARM64 /p:PublishProfile=MSIX-Release-ARM64
```

---

## Publish Profile Settings

Each profile is configured with:

| Setting | Value | Purpose |
|---------|-------|---------|
| **Configuration** | Release | Optimized build with no debug symbols |
| **Platform** | x64/x86/ARM64 | Target CPU architecture |
| **PublishDir** | `AppPackages\` | Output location (solution root) |
| **SelfContained** | true | Includes .NET runtime (no installation required) |
| **AppxPackageSigningEnabled** | false | Unsigned (for testing/development) |
| **AppxBundle** | Never | Separate package per architecture |

---

## Output Structure

After publishing, you'll find:

```
AppPackages\
└── ManpWinUI_1.4.0.0_Test\
    ├── ManpWinUI_1.4.0.0_x64.msix      ← The distributable package
    ├── Dependencies\                    ← Required runtimes
    │   └── x64\
    │       └── Microsoft.WindowsAppRuntime.*.appx
    ├── Add-AppDevPackage.ps1            ← Installer script
    └── Install.ps1                      ← Alternative installer
```

---

## Distribution

### For Testing (Unsigned Package)

**Share with testers:**
1. The entire folder: `ManpWinUI_<Version>_Test\`
2. Or just the `.msix` file + `Dependencies\` folder

**Installation Requirements:**
- Windows 10 version 1809+ or Windows 11
- **Developer Mode enabled** (Settings → Update & Security → For developers)
- Internet connection (for dependency download if not included)

**Installation Steps:**
```powershell
# Option 1: PowerShell script (handles dependencies automatically)
.\Add-AppDevPackage.ps1

# Option 2: Manual double-click
# Double-click ManpWinUI_<Version>_<Arch>.msix
```

---

### For Public Release (Signed Package)

**Before distribution:**

1. **Get a code signing certificate**
   - Purchase from DigiCert, Sectigo, etc. (~$200-500/year)
   - Or submit to Microsoft Store (Microsoft signs it for you)

2. **Enable signing in publish profile**

   Edit the `.pubxml` file:
   ```xml
   <AppxPackageSigningEnabled>true</AppxPackageSigningEnabled>
   <PackageCertificateKeyFile>ManpLab_Certificate.pfx</PackageCertificateKeyFile>
   <PackageCertificateThumbprint>CERT_THUMBPRINT_HERE</PackageCertificateThumbprint>
   ```

3. **Publish with signing enabled**

**Benefits:**
- No "Do you trust this publisher?" warning
- No Developer Mode required for installation
- Required for Microsoft Store submission
- Builds user trust

---

## GitHub Releases Distribution

### Recommended Workflow

1. **Update version** in `Version.props`:
   ```xml
   <FileVersion>1.5.0.0</FileVersion>
   ```

2. **Commit version change**
   ```bash
   git add Version.props
   git commit -m "Bump version to 1.5.0"
   git tag v1.5.0
   git push origin development --tags
   ```

3. **Build all architectures** (x64, x86, ARM64)

4. **Create GitHub Release**
   - Go to https://github.com/markhassellsmith/ManpLab/releases
   - Click "Draft a new release"
   - Select tag `v1.5.0`
   - Title: `ManpLab v1.5.0`
   - Description: Release notes

5. **Upload packages:**
   - `ManpWinUI_1.5.0.0_x64.msix`
   - `ManpWinUI_1.5.0.0_x86.msix`
   - `ManpWinUI_1.5.0.0_ARM64.msix`
   - ZIP the entire `ManpWinUI_1.5.0.0_Test\` folder for each architecture

6. **Include installation instructions:**
   ```markdown
   ## Installation

   ### For Windows 11 / Windows 10 (version 1809+)

   **x64 (64-bit Intel/AMD)** - Most common
   - Download `ManpWinUI_1.5.0.0_x64.msix`
   - Double-click to install
   - Or use the PowerShell installer from the full package

   **x86 (32-bit)**
   - Download `ManpWinUI_1.5.0.0_x86.msix`

   **ARM64 (Surface Pro X, etc.)**
   - Download `ManpWinUI_1.5.0.0_ARM64.msix`

   **Note:** Developer Mode must be enabled for unsigned packages.
   Settings → Update & Security → For developers → Developer Mode
   ```

---

## Versioning

### Version Number Format

```
Major.Minor.Patch.Revision
  1  .  4  .  0  .   0
```

**Where:**
- **Major** - Breaking changes
- **Minor** - New features
- **Patch** - Bug fixes
- **Revision** - Build number (usually 0)

### Updating Version

Edit `Version.props` at solution root:

```xml
<PropertyGroup>
    <!-- Update this for new releases -->
    <FileVersion>1.5.0.0</FileVersion>

    <!-- Optional: Semantic version for NuGet packages -->
    <VersionPrefix>1.5.0</VersionPrefix>
</PropertyGroup>
```

This automatically updates:
- Assembly version
- File version
- MSIX package version
- Display version in Windows

---

## Troubleshooting

### Issue: "Could not find a part of the path 'AppPackages'"

**Solution:** Create the folder manually:
```powershell
New-Item -Path "AppPackages" -ItemType Directory -Force
```

---

### Issue: "Native DLL not found"

**Solution:** Build the entire solution first:
```powershell
msbuild ManpWinUI.sln /p:Configuration=Release /p:Platform=x64
```

The ManpCore.Native project must build before ManpWinUI.

---

### Issue: "Package validation failed"

**Check:**
1. Version number is valid (4-part: `1.2.3.4`)
2. All assets exist in `Assets\` folder
3. `Package.appxmanifest` is valid XML
4. No special characters in version

---

### Issue: "Can't install on user's machine"

**Checklist:**
- Windows 10 version 1809+ or Windows 11?
- Developer Mode enabled? (for unsigned packages)
- Dependencies included? (`Add-AppDevPackage.ps1` handles this)
- Correct architecture? (x64 package on x64 Windows)

---

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build MSIX Packages

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest

    strategy:
      matrix:
        platform: [x64, x86, ARM64]

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'

    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1

    - name: Restore NuGet packages
      run: dotnet restore ManpWinUI.sln

    - name: Build Solution
      run: msbuild ManpWinUI.sln /p:Configuration=Release /p:Platform=${{ matrix.platform }}

    - name: Publish MSIX
      run: |
        msbuild ManpWinUI\ManpWinUI.csproj `
          /t:Publish `
          /p:Configuration=Release `
          /p:Platform=${{ matrix.platform }} `
          /p:PublishProfile=MSIX-Release-${{ matrix.platform }}

    - name: Upload Artifacts
      uses: actions/upload-artifact@v3
      with:
        name: ManpLab-${{ matrix.platform }}
        path: AppPackages\**\*.msix
```

---

## Best Practices

1. **Always build Release configuration** for distribution
2. **Test each architecture** if possible
3. **Sign packages** before public release
4. **Include Dependencies folder** for offline installation
5. **Document system requirements** clearly
6. **Test on clean Windows installations**
7. **Use semantic versioning** consistently
8. **Create GitHub releases** with clear notes
9. **Archive old versions** for rollback capability
10. **Test update scenarios** (old version → new version)

---

## Microsoft Store Submission

If you plan to distribute via Microsoft Store:

1. **Create Partner Center account** ($19 one-time fee)
2. **Reserve app name** "ManpLab"
3. **Prepare store assets:**
   - Screenshots (1280x720 or larger)
   - App description
   - Privacy policy URL
   - Support contact

4. **Create submission:**
   - Upload `.msix` or `.msixupload` file
   - Select pricing (Free recommended)
   - Set availability (all markets or specific)
   - Age rating
   - System requirements

5. **Certification:** Usually 1-3 days

**Benefits:**
- Automatic updates for users
- No Developer Mode required
- Trusted distribution
- Payment processing (if paid app)

---

**Last Updated:** January 2026  
**Version:** 1.0
