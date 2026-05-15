# MSIX Packaging in ManpLab

## What is MSIX?

**MSIX** is Microsoft's modern application package format for Windows. Think of it as a **ZIP file on steroids** that contains:
- Your application files
- Dependencies
- Installation instructions
- Security metadata
- App identity and permissions

It's the successor to older formats like MSI (Windows Installer) and AppX (UWP apps).

---

## MSIX vs Traditional EXE Installation

| Aspect | Traditional EXE/MSI | MSIX Package |
|--------|---------------------|--------------|
| **Installation** | Scatters files across system | Isolated container |
| **Uninstall** | May leave files behind | Clean removal guaranteed |
| **Updates** | Full reinstall often needed | Delta updates (only changed files) |
| **Permissions** | Can do anything | Sandboxed with declared capabilities |
| **Location** | `C:\Program Files\` | `C:\Program Files\WindowsApps\` |
| **User Data** | Mixed with app files | Isolated in `%LocalAppData%\Packages\` |
| **Store Distribution** | No | Yes (Microsoft Store) |
| **Automatic Updates** | Manual implementation | Built-in |

---

## Build vs Publish: The Crucial Difference

### **🔨 Build (F6 or Build → Build Solution)**

**What it creates:**
- Compiles your code into assemblies
- Creates an **unpackaged layout** in `bin\x64\<Configuration>\`
- Generates the **AppX folder** with all files
- **Does NOT create a .msix file**

**Output Location:**
```
ManpWinUI\bin\x64\Release\net10.0-windows10.0.22621.0\win-x64\
├── ManpWinUI.exe
├── ManpWinUI.dll
├── AppX\                    ← MSIX package layout (not a .msix file yet!)
│   ├── ManpCore.Native.dll
│   ├── AppxManifest.xml
│   └── (all dependencies)
└── (loose files for debugging)
```

**What you can do with it:**
- ✅ Run and debug in Visual Studio (F5)
- ✅ Test during development
- ❌ **Cannot distribute to users** (not a package yet)
- ❌ Cannot double-click install

**Behind the scenes:**
When you press F5, Visual Studio runs:
```powershell
Add-AppxPackage -Register 'AppxManifest.xml' -ForceApplicationShutdown
```
This **registers** the unpacked files as an installed app temporarily.

---

### **📦 Publish (Project → Publish / Package → Create App Packages)**

**What it creates:**
- Takes the built files
- **Packages them into a .msix file** (compressed archive)
- Optionally signs the package (required for Store)
- Creates installer bundle with dependencies

**Output Location:**
```
AppPackages\ManpWinUI_<Version>_Test\
├── ManpWinUI_1.4.0.0_x64.msix         ← This is the distributable package!
├── Dependencies\                       ← Required runtime packages
│   └── x64\
│       └── Microsoft.WindowsAppRuntime.*.appx
├── Add-AppDevPackage.ps1              ← PowerShell installer script
└── Install.ps1                         ← Alternative installer
```

**Note:** The `AppPackages` folder is created at the **solution root level**, not under the project folder.

**What you can do with it:**
- ✅ **Distribute to users** (email, website, GitHub releases)
- ✅ Double-click to install
- ✅ Submit to Microsoft Store
- ✅ Deploy via Group Policy or Intune
- ✅ Install on other machines

---

## Your Project's Current Configuration

From your `.csproj`:

```xml
<EnableMsixTooling>true</EnableMsixTooling>
<WindowsPackageType>MSIX</WindowsPackageType>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

This means:
- ✅ Every **Build** creates the AppX folder structure
- ✅ F5 (Debug) automatically registers the app
- ⚠️ **Build does NOT create a .msix file**
- 📦 You must **Publish** to create the distributable .msix package

---

## How to Create a Distributable MSIX Package

### **Method 1: Visual Studio Publish Wizard** ⭐ Recommended

1. Right-click **ManpWinUI** project in Solution Explorer
2. Select **Publish**
3. Choose **MSIX** package type
4. Select target:
   - **Sideloading** (for distribution outside Store)
   - **Microsoft Store** (for Store submission)
5. Configure settings:
   - Version number
   - Architecture (x64, x86, ARM64)
   - Update settings
6. Click **Create** or **Publish**

**Output:** `AppPackages\...\ManpWinUI_x.x.x.x_x64.msix`

---

### **Method 2: Command Line (MSBuild)**

```powershell
# Restore packages
dotnet restore ManpWinUI.sln

# Build in Release mode
msbuild ManpWinUI.sln /p:Configuration=Release /p:Platform=x64

# Create MSIX package
msbuild ManpWinUI\ManpWinUI.csproj /t:Publish /p:Configuration=Release /p:Platform=x64 /p:PublishProfile=MSIX
```

---

### **Method 3: Package from Command Prompt** (Manual)

```powershell
# Use MakeAppx.exe tool (part of Windows SDK)
MakeAppx.exe pack /d "ManpWinUI\bin\x64\Release\...\win-x64\AppX" /p "ManpLab.msix"
```

---

## MSIX Package Contents

When you **Publish**, the .msix file contains:

```
ManpWinUI_1.4.0.0_x64.msix (compressed)
│
├── AppxManifest.xml              ← Package identity and capabilities
├── AppxSignature.p7x             ← Digital signature (if signed)
├── [Content_Types].xml           ← MIME types
│
├── ManpWinUI.exe                 ← Your application
├── ManpWinUI.dll                 ← Your code
├── ManpCore.Native.dll           ← Native dependencies
├── zlib1.dll, gmp-10.dll, etc.   ← Native libs
│
├── Microsoft.UI.Xaml.dll         ← WinUI 3 framework
├── System.*.dll                  ← .NET runtime (self-contained)
│
├── Assets\                       ← App icons, splash screen
│   ├── ManpLab-Square150x150Logo.png
│   ├── ManpLab-SplashScreen.png
│   └── ...
│
├── Themes\                       ← Custom themes
│   └── OceanBlue.xaml
│
├── resources.pri                 ← Compiled resources
│
└── Language folders (75+)        ← Localized resources
    ├── en-US\
    ├── es-ES\
    └── ...
```

---

## Where MSIX Apps Get Installed

### **System Location (Protected)**
```
C:\Program Files\WindowsApps\
└── [PackageFamilyName]_[Version]_x64__[PublisherId]\
    └── (all app files - read-only)
```

Example:
```
C:\Program Files\WindowsApps\6335fa26-c01e-4e88-b714-effc8096fc01_1.4.0.0_x64__8wekyb3d8bbwe\
```

**Note:** This folder is **hidden and protected** by Windows. Users cannot easily access it.

---

### **User Data Location**
```
%LocalAppData%\Packages\[PackageFamilyName]\
├── LocalState\                    ← Your ApplicationData.LocalFolder
│   ├── bookmarks.json            ← Saved bookmarks
│   ├── navigation_history.json   ← Navigation history
│   └── logs\                      ← Serilog logs
├── LocalCache\
├── RoamingState\
└── Settings\
    └── settings.dat               ← ApplicationData.LocalSettings
```

Example:
```
C:\Users\Mark\AppData\Local\Packages\6335fa26-c01e-4e88-b714-effc8096fc01_8wekyb3d8bbwe\
```

This is where your **AppSettingsService** stores data when running as MSIX!

---

## MSIX Signing

### **Unsigned Packages (Current State)**

Your project has:
```xml
<AppxPackageSigningEnabled>False</AppxPackageSigningEnabled>
```

**Implications:**
- ❌ Cannot submit to Microsoft Store
- ⚠️ Users get warning: "Do you trust this publisher?"
- ✅ Can install for testing (sideloading)
- ✅ Good for development and internal testing

**To install unsigned package:**
1. Enable Developer Mode on Windows
2. Right-click .msix file
3. Install (or run `Add-AppDevPackage.ps1`)

---

### **Signed Packages (Required for Distribution)**

To sign your package:

1. **Get a code signing certificate:**
   - Option A: Purchase from Certificate Authority (DigiCert, Sectigo, etc.)
   - Option B: Create self-signed cert for testing (not for public distribution)
   - Option C: Microsoft Store handles signing automatically

2. **Enable signing in project:**
   ```xml
   <AppxPackageSigningEnabled>True</AppxPackageSigningEnabled>
   <PackageCertificateKeyFile>ManpLab_TemporaryKey.pfx</PackageCertificateKeyFile>
   ```

3. **Publish creates signed package**

**Benefits of signing:**
- ✅ No warning during installation
- ✅ Required for Microsoft Store
- ✅ Builds user trust
- ✅ Proves authenticity

---

## Distribution Methods

### **1. Sideloading (Direct Installation)**

Share the MSIX package directly:

**What to distribute:**
```
ManpLab_1.4.0.0_x64.msix              ← Main package
Add-AppDevPackage.ps1                 ← Installer script
Dependencies\                          ← Required runtimes
└── x64\
    └── Microsoft.WindowsAppRuntime.*.appx
```

**User installation:**
```powershell
# Method A: PowerShell script (handles dependencies)
.\Add-AppDevPackage.ps1

# Method B: Manual double-click (may fail if dependencies missing)
# Double-click ManpLab_1.4.0.0_x64.msix
```

**Requirements:**
- Windows 10 version 1809 or later
- Developer Mode enabled (for unsigned packages)
- Internet connection (for dependency download)

---

### **2. Microsoft Store**

**Benefits:**
- ✅ Automatic updates
- ✅ Trusted distribution
- ✅ Reach millions of users
- ✅ No Developer Mode needed
- ✅ Microsoft handles signing

**Requirements:**
- Developer account ($19 one-time fee)
- App submission and review
- Package signed by Microsoft
- Store compliance policies

**Steps:**
1. Create Microsoft Partner Center account
2. Reserve app name
3. Create Store listing
4. Upload MSIX package
5. Submit for certification

---

### **3. Enterprise Deployment**

**Via Group Policy:**
- Deploy MSIX via Active Directory
- Automatic installation on domain computers

**Via Microsoft Intune:**
- Mobile Device Management (MDM)
- Deploy to managed devices

**Via SCCM/Configuration Manager:**
- Large-scale enterprise deployment

---

## AppxManifest.xml - The Package Identity

Your `Package.appxmanifest` defines:

```xml
<Identity
  Name="6335fa26-c01e-4e88-b714-effc8096fc01"
  Publisher="CN=User Name"
  Version="1.4.0.0" />
```

**What these mean:**

| Field | Purpose | Example |
|-------|---------|---------|
| **Name** | Unique package identifier (GUID) | `6335fa26-c01e-4e88-b714-effc8096fc01` |
| **Publisher** | Certificate subject name | `CN=User Name` |
| **Version** | 4-part version number | `1.4.0.0` |

**Package Family Name** (Generated):
```
[Name]_[PublisherHash]
6335fa26-c01e-4e88-b714-effc8096fc01_8wekyb3d8bbwe
```

This is used by Windows to:
- Track installed version
- Store user data
- Manage app identity
- Handle updates

---

## Build vs Publish Comparison Table

| Aspect | Build (F6) | Publish |
|--------|-----------|---------|
| **Creates .msix file** | ❌ No | ✅ Yes |
| **Output** | Loose files in `bin\` | Packaged `.msix` file |
| **Can distribute** | ❌ No | ✅ Yes |
| **Can debug** | ✅ Yes | ⚠️ After installing |
| **AppX folder** | ✅ Yes | ✅ Yes (inside .msix) |
| **Signing** | N/A | Optional |
| **Size** | ~400 MB (unpacked) | ~150-200 MB (compressed) |
| **Dependencies included** | Separate | Bundled or separate |
| **Purpose** | Development | Distribution |

---

## Common Workflows

### **Development Workflow (Daily)**

1. Switch to **Debug** configuration
2. Press **F6** (Build Solution)
3. Press **F5** (Start Debugging)
   - VS automatically registers AppX layout
   - App runs as if installed
4. Make changes, repeat

**Result:** No .msix file created, just testing the app

---

### **Testing Workflow (Before Release)**

1. Switch to **Release** configuration
2. Press **F6** (Build Solution)
3. Press **Ctrl+F5** (Start Without Debugging)
   - Tests optimized build
   - Still no .msix file created
4. Verify performance and functionality

**Result:** No .msix file created, validating the app

---

### **Distribution Workflow (Release)**

1. Update version in `Version.props`:
   ```xml
   <FileVersion>1.5.0.0</FileVersion>
   ```

2. Switch to **Release** configuration

3. **Publish** the project:
   - Right-click project → Publish
   - Configure MSIX settings
   - Click Create

4. Locate output:
   ```
   AppPackages\ManpWinUI_1.5.0.0_Test\
   └── ManpWinUI_1.5.0.0_x64.msix   ← Distribute this!
   ```

5. Distribute:
   - Upload to GitHub Releases
   - Share via OneDrive/Dropbox
   - Submit to Microsoft Store
   - Deploy via enterprise tools

**Result:** ✅ .msix file ready for distribution

---

## Troubleshooting

### **Issue: "I built my app but there's no .msix file"**

**Cause:** You used **Build**, not **Publish**

**Solution:**
- Build creates the AppX layout for debugging
- Use **Publish** to create the .msix package file

---

### **Issue: "Users can't install my .msix file"**

**Possible causes:**
1. Package is unsigned and Developer Mode not enabled
2. Missing dependencies (WindowsAppRuntime)
3. Incompatible Windows version
4. Corrupted package

**Solutions:**
- Sign the package with a valid certificate
- Include `Add-AppDevPackage.ps1` (handles dependencies)
- Ensure Windows 10 1809+ or Windows 11
- Verify package integrity

---

### **Issue: "App won't start after MSIX installation"**

**Possible causes:**
1. Missing native DLLs (ManpCore.Native.dll, etc.)
2. Missing Windows App SDK runtime
3. Wrong CPU architecture (x64 package on x86 system)

**Solutions:**
- Verify `CopyNativeToAppX` target ran during build
- Check `AppX\` folder contains all DLLs
- Ensure `WindowsAppSDKSelfContained=true`
- Match architecture (x64 package for x64 Windows)

---

### **Issue: "Settings not persisting in MSIX package"**

**Cause:** Your `AppSettingsService` should detect MSIX mode and use `ApplicationData.Current`

**Verify:**
```csharp
// In AppSettingsService.cs
try
{
    _ = ApplicationData.Current.LocalSettings;
    _isPackaged = true;  // MSIX mode
}
catch
{
    _isPackaged = false; // Portable mode
}
```

**User data location (MSIX):**
```
%LocalAppData%\Packages\[PackageFamilyName]\LocalState\
```

---

## Summary

### **Quick Answers:**

**Q: Is MSIX the output of Publish?**  
**A:** Yes! **Build** creates the layout, **Publish** creates the .msix file.

**Q: Do I need to Publish for daily development?**  
**A:** No. Use **Build** + **F5** for development. Only **Publish** when distributing to users.

**Q: Can I email the .msix file to users?**  
**A:** Yes! The .msix file from **Publish** is a complete, installable package.

**Q: What's the difference between the AppX folder and .msix file?**  
**A:** AppX folder is the unpackaged layout (for debugging). The .msix file is the compressed, distributable package.

---

### **Key Takeaways:**

1. **MSIX is a modern Windows app package format** - secure, isolated, clean install/uninstall
2. **Build creates AppX layout** (for debugging) - not distributable
3. **Publish creates .msix file** (for distribution) - ready for users
4. **F5 registers the AppX layout** temporarily for development
5. **Users install the .msix file** by double-clicking or using the PowerShell script
6. **Signing is optional for testing**, required for Store and public trust

---

**Last Updated:** January 2026  
**Applies To:** ManpLab .NET 10.0 / Visual Studio 2026
