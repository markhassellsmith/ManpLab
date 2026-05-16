# GitHub Release Guide for ManpLab

Simple manual steps for replacing files on GitHub releases.

---

## 🔄 **REPLACE FILE (Most Common)**

**Simple 5-step process:**

1. Go to: https://github.com/markhassellsmith/ManpLab/releases
2. Find your release → Click **"Edit release"** (pencil icon)
3. Click **🗑️** next to old file → Confirm deletion
4. Drag and drop new file
5. Click **"Update release"**

**Done!** Takes 30 seconds.

---

## 📦 **File Locations**

After building:
```
Release-Output\MSIX\ManpLab-1.1.1-x64.msix
Release-Output\Portable-ZIP\ManpLab-Portable-1.1.1-x64.zip
```

Your v1.1.0 renamed MSIX:
```
AppPackages\ManpWinUI_v1.1.0_Release\ManpWinUI_1.1.0.0_x64_Test\ManpLab-1.1.0-x64.msix
```

---

## 🆕 **Create New Release**

### Step 1: Build
```powershell
.\Build-Release.ps1 -Version "1.1.1"
```

### Step 2: Tag
```powershell
git tag -a v1.1.1 -m "Release v1.1.1"
git push origin v1.1.1
```

### Step 3: Upload
1. Go to: https://github.com/markhassellsmith/ManpLab/releases/new
2. Choose tag: `v1.1.1`
3. Title: `ManpLab v1.1.1`
4. Drag files from `Release-Output\`
5. Add release notes (see template below)
6. Click **"Publish release"**

---

## 📝 **Release Notes Template**

```markdown
## What's New

- Feature or fix description

## Installation

**MSIX (Recommended):** Right-click → Install
**Portable ZIP:** Extract and run ManpWinUI.exe

## Requirements
- Windows 10 (1809+) or Windows 11
- 4 GB RAM recommended
```

---

## ⚠️ **Delete & Recreate Release**

Only if you need to start over:

1. On release page → Click **"Delete"** at bottom
2. Delete tag:
   ```powershell
   git push --delete origin v1.1.1
   git tag -d v1.1.1
   ```
3. Recreate tag and release (see above)

---

## ✅ **Naming Convention**

Use dashes (auto-enforced by build script):
- ✅ `ManpLab-1.1.1-x64.msix`
- ✅ `ManpLab-Portable-1.1.1-x64.zip`
- ❌ ~~`ManpWinUI_1.1.0_x64.msix`~~ (underscores)

---

**ManpLab Repository:** https://github.com/markhassellsmith/ManpLab
