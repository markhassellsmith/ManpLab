# GitHub Release Quick Reference

## 🔄 **REPLACE FILE** (30 seconds)

1. Go to: https://github.com/markhassellsmith/ManpLab/releases
2. Find release → **"Edit release"**
3. Click **🗑️** next to old file
4. Drag & drop new file
5. **"Update release"**

---

## 🆕 **CREATE RELEASE**

### Build:
```powershell
.\Build-Release.ps1 -Version "1.1.1"
```

### Tag:
```powershell
git tag -a v1.1.1 -m "Release v1.1.1"
git push origin v1.1.1
```

### Upload:
1. Go to: https://github.com/markhassellsmith/ManpLab/releases/new
2. Select tag `v1.1.1`
3. Drag files from `Release-Output\`
4. Publish

---

## 📁 **File Locations**

After building:
```
Release-Output\
├── MSIX\ManpLab-1.1.1-x64.msix
└── Portable-ZIP\ManpLab-Portable-1.1.1-x64.zip
```

Your v1.1.0 renamed MSIX:
```
AppPackages\...\ManpLab-1.1.0-x64.msix
```

---

## ✅ **Naming**

- ✅ `ManpLab-1.1.1-x64.msix` (dashes)
- ❌ `ManpWinUI_1.1.0_x64.msix` (underscores)

---

## 🗑️ **Delete Release**

```powershell
# On GitHub: release page → Delete
git push --delete origin v1.1.1
git tag -d v1.1.1
```

---

**Link:** https://github.com/markhassellsmith/ManpLab/releases
