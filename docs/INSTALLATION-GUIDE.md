# ManpLab Installation Guide

## Choose Your Installation Method

### 🎯 Portable ZIP (Recommended)

**Best for**: Quick start, no admin rights, educational environments

**Steps:**
1. Download `ManpLab-v[version]-Windows-x64-Portable.zip`
2. Extract to any folder (e.g., `C:\Programs\ManpLab` or your Desktop)
3. Run `ManpWinUI.exe`
4. Start exploring fractals!

**Advantages:**
- ✅ No installation process
- ✅ No security warnings
- ✅ Portable - can run from USB drive
- ✅ No admin rights required
- ✅ Easy to remove - just delete the folder

**System Requirements:**
- Windows 10 (version 1809 or later) or Windows 11
- 64-bit (x64) processor
- 2 GB RAM minimum, 4 GB recommended
- 500 MB free disk space

---

### 📦 MSIX Package

**Best for**: Users who prefer managed apps, want automatic updates

**Important Note:** Because ManpLab is not commercially code-signed, Windows will show a security warning. This is **normal** for open-source educational software and does **not** mean the software is unsafe.

**Installation Steps:**

#### Method 1: Direct Installation
1. Download `ManpLab-v[version]-Windows-x64.msix`
2. Double-click the `.msix` file
3. Windows will show: **"Windows protected your PC"**
4. Click **"More info"**
5. Click **"Install anyway"**
6. Wait for installation to complete
7. Find ManpLab in your Start Menu

#### Method 2: PowerShell Installation
1. Download the `.msix` file
2. Right-click on it and select **"Open with PowerShell"**
3. Run this command:
   ```powershell
   Add-AppxPackage -Path "ManpLab-v[version]-Windows-x64.msix"
   ```
4. Press Enter and wait for installation

#### Method 3: Developer Mode (If above methods fail)
1. Open **Settings** → **Privacy & Security** → **For developers**
2. Enable **"Developer Mode"**
3. Double-click the `.msix` file
4. Click **"Install"**

**Why the Security Warning?**

Commercial code signing certificates cost $200-500 per year. Many open-source educational projects, including ManpLab, don't have commercial signing because:
- It's an educational tool, not commercial software
- The source code is publicly available on GitHub
- The project is maintained transparently

The warning simply means Microsoft hasn't verified the publisher - it does **not** indicate malware or security issues.

**Prefer No Warnings?** → Use the **Portable ZIP** version instead!

**Advantages:**
- ✅ Clean install/uninstall through Windows Settings
- ✅ Sandboxed security
- ✅ Ready for automatic updates (future releases)
- ✅ Integrates with Windows Start Menu

**Disadvantages:**
- ⚠️ Shows security warning during installation
- ⚠️ Requires understanding the warning is safe

**Uninstallation:**
1. Open **Settings** → **Apps** → **Installed apps**
2. Find **ManpLab**
3. Click the three dots **⋯** → **Uninstall**

---

## First Launch

After installation (either method), when you first launch ManpLab:

1. **Initial Load**: The app may take 10-15 seconds to load on first run
2. **Fractal Browser**: Opens with 300 fractals organized by category
3. **Select a Fractal**: Click any fractal to render it
4. **Explore**: Use mouse to zoom, arrow keys to pan

## Quick Start Tips

- **🖱️ Mouse**: Click and drag to pan, scroll to zoom
- **⌨️ Keyboard**: Arrow keys to pan, +/- to zoom, Spacebar to reset
- **🎨 Themes**: Try Light, Dark, or Ocean Blue themes from Settings
- **📚 Browser**: Browse 300 fractals with descriptions and formulas
- **🎬 Animation**: Create MP4 videos of fractal zooms

Full keyboard shortcuts: Press `F1` or see [KEYBOARD_SHORTCUTS.md](ManpWinUI/KEYBOARD_SHORTCUTS.md)

## Troubleshooting

### Portable ZIP Issues

**Problem**: "The application was unable to start correctly (0xc000007b)"
- **Solution**: Ensure you extracted **all files** from the ZIP
- Make sure you're running 64-bit Windows
- Try extracting to a different location (avoid OneDrive/network drives)

**Problem**: Missing DLL errors
- **Solution**: Re-extract the entire ZIP file
- Some antivirus software may quarantine files - check your AV logs

**Problem**: Slow performance
- **Solution**: Use Release build, not Debug
- Reduce max iterations in Settings
- Close other intensive applications

### MSIX Issues

**Problem**: "This app can't be installed"
- **Solution**: Try PowerShell installation method above
- Or use the Portable ZIP version instead

**Problem**: Installation hangs
- **Solution**: Restart Windows and try again
- Ensure Windows is up to date

**Problem**: App won't launch after installation
- **Solution**: Reinstall or use Portable ZIP version

### General Issues

**Problem**: Blank window or crashes
- **Solution**: 
  - Update Windows to latest version
  - Update graphics drivers
  - Try a different theme (Settings → Appearance)

**Problem**: Rendering errors
- **Solution**:
  - Reduce max iterations
  - Try a simpler fractal type first
  - Check you have at least 2 GB free RAM

## Getting Help

- **Documentation**: [GitHub Repository](https://github.com/markhassellsmith/ManpLab)
- **Issues**: [Report bugs](https://github.com/markhassellsmith/ManpLab/issues)
- **Discussions**: [Ask questions](https://github.com/markhassellsmith/ManpLab/discussions)

## System Information

**Supported Windows Versions:**
- ✅ Windows 11 (all versions)
- ✅ Windows 10 version 1809 (October 2018 Update) or later
- ❌ Windows 10 version 1803 or earlier
- ❌ Windows 8.1 or earlier

**Check Your Windows Version:**
1. Press `Win + R`
2. Type `winver` and press Enter
3. Look for "Version" number (must be 1809 or higher)

**Processor:**
- Must be 64-bit (x64)
- Intel Core i5 or AMD Ryzen 5 recommended
- Will run on slower processors but rendering will be slower

**RAM:**
- 2 GB minimum
- 4 GB recommended
- 8 GB+ ideal for deep zoom and animation rendering

**Disk Space:**
- Portable: ~500 MB
- MSIX: ~400 MB (+ Windows app cache)

## License

ManpLab is released under the MIT License - see [LICENSE](LICENSE) for details.

This project includes third-party libraries with their own licenses (MPFR, GMP, libpng, ZLib, FFmpeg).
