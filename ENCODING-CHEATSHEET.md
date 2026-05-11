# 🛡️ Encoding Safeguards - Quick Reference

## ✅ What's Protected

Your repository now has automatic protection against:
- ❌ UTF-16 file corruption
- ❌ Mixed line ending issues  
- ❌ ANSI/Latin-1 encoding problems
- ❌ Binary file corruption in Git

## 📁 Files Added

| File | Purpose |
|------|---------|
| `.gitattributes` | Git-level rules for all file types |
| `.editorconfig` | IDE settings (auto-applied in VS) |
| `Fix-FileEncodings.ps1` | Utility to fix existing files |
| `docs/FILE-ENCODING-STANDARDS.md` | Full documentation |

## 🚀 Quick Commands

### Check if files need fixing
```powershell
.\Fix-FileEncodings.ps1 -WhatIf
```

### Fix all encoding issues
```powershell
.\Fix-FileEncodings.ps1
```

### Fix specific directory
```powershell
.\Fix-FileEncodings.ps1 -Path ".\ManpWinUI"
```

## 🔍 Verify Settings

### In Visual Studio
1. Open any `.cs` file
2. Check bottom-right corner: should show **UTF-8** and **LF**
3. If wrong, save file again (VS will auto-fix)

### In Git
```powershell
# View current line endings
git config core.autocrlf

# Should be: false (we use .gitattributes instead)

# Check file attributes
git check-attr -a *.cs
```

## 🎯 Standard Settings

| File Type | Encoding | Line Endings |
|-----------|----------|--------------|
| `.cs`, `.cpp`, `.xaml` | UTF-8 | LF |
| `.json`, `.xml` | UTF-8 | LF |
| `.md`, `.txt` | UTF-8 | LF |
| `.bat`, `.cmd`, `.ps1` | UTF-8 | CRLF |
| `.png`, `.dll`, `.exe` | Binary | N/A |

## ⚡ How It Works

1. **Git commits** - `.gitattributes` normalizes files automatically
2. **Opening files** - `.editorconfig` tells VS to use UTF-8 + LF
3. **Saving files** - VS applies settings from `.editorconfig`
4. **Existing files** - Run `Fix-FileEncodings.ps1` once

## 🆘 Troubleshooting

### Issue: VS shows "File encoding changed"
**Solution:** Normal! Save file to apply new encoding.

### Issue: Git shows entire file as changed
**Solution:** One-time normalization. Commit once and future commits will be clean.

### Issue: Script finds no issues but Git diff shows changes
**Solution:** Git is normalizing on commit. Run:
```powershell
git add -A
git commit -m "Normalize line endings"
```

## 📚 More Info

See full documentation: `docs/FILE-ENCODING-STANDARDS.md`

## ✨ Benefits

✅ No more corrupted PNGs  
✅ No more "file changed but content same" in Git  
✅ Consistent across all team members  
✅ Works on Windows, Mac, Linux  
✅ IDE automatically applies settings
