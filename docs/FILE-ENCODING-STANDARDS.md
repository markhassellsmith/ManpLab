# File Encoding and Line Ending Standards

This document explains the safeguards in place to prevent character encoding and line ending issues in the ManpLab repository.

## 📋 Overview

The repository uses three configuration files to ensure consistency:

1. **`.gitattributes`** - Git-level rules for line endings and encodings
2. **`.editorconfig`** - IDE/editor settings for consistent formatting
3. **`Fix-FileEncodings.ps1`** - Utility script to fix existing files

## 🛡️ Safeguards in Place

### Line Endings
- **LF (Unix-style)** for all source code, XML, JSON, and documentation
- **CRLF (Windows-style)** only for batch files and PowerShell scripts
- Git automatically normalizes line endings on commit

### Character Encoding
- **UTF-8** for all text files (with BOM only where required)
- **Binary** handling for images, archives, and compiled files
- Prevents UTF-16, ANSI, or other encoding issues

### File Types Covered
- C# source files (`.cs`, `.csproj`, `.sln`)
- C++ source files (`.cpp`, `.h`, `.hpp`)
- XAML and XML files
- JSON configuration files
- Markdown documentation
- Scripts (`.ps1`, `.bat`, `.cmd`, `.sh`)

## 🔧 How It Works

### Git Attributes (`.gitattributes`)
```gitattributes
*.cs text eol=lf encoding=UTF-8
*.png binary
```
- Tells Git how to handle each file type
- Normalizes line endings on commit
- Marks binary files to prevent text processing

### Editor Config (`.editorconfig`)
```ini
[*.cs]
charset = utf-8
end_of_line = lf
indent_style = space
indent_size = 4
```
- Automatically configures Visual Studio and other editors
- Ensures new files use correct settings
- Provides consistent formatting rules

## 🚀 Using the Fix Script

If you encounter encoding issues with existing files, run:

```powershell
.\Fix-FileEncodings.ps1
```

This script will:
1. Scan all source files in the repository
2. Convert any non-UTF-8 files to UTF-8
3. Normalize line endings to LF
4. Report what was changed

### Options
```powershell
# Dry run (check only, don't modify)
.\Fix-FileEncodings.ps1 -WhatIf

# Process specific directory
.\Fix-FileEncodings.ps1 -Path ".\ManpWinUI"

# Verbose output
.\Fix-FileEncodings.ps1 -Verbose
```

## ⚙️ Visual Studio Integration

Visual Studio 2022+ automatically respects `.editorconfig` settings. No additional configuration needed!

### Verify Settings
1. Open any `.cs` file
2. Check status bar (bottom right): should show `UTF-8` and `LF`
3. If not, VS will auto-convert on save

### Manual Check
- **View** → **Advanced** → **Show Line Endings**
- **View** → **Advanced** → **Show Encoding**

## 🔍 Common Issues and Solutions

### Issue: File shows different encoding
**Solution:** Run `Fix-FileEncodings.ps1` or re-save in Visual Studio

### Issue: Line endings mixed in single file
**Solution:** 
```powershell
git rm --cached -r .
git reset --hard
```

### Issue: Git shows entire file as changed
**Solution:** Line ending normalization in progress. Commit once to fix.

## 📝 Best Practices

1. **Always commit through Git** - Let Git normalize files
2. **Don't manually change encodings** - Use the fix script
3. **Check before pushing** - Run `git diff` to verify changes
4. **New files auto-configured** - `.editorconfig` handles this

## 🔗 References

- [EditorConfig](https://editorconfig.org/)
- [Git Attributes](https://git-scm.com/docs/gitattributes)
- [UTF-8 Everywhere](http://utf8everywhere.org/)

## ✅ Verification Checklist

After setup, verify:
- [ ] `.gitattributes` exists and is committed
- [ ] `.editorconfig` exists and is committed
- [ ] Visual Studio shows UTF-8 encoding for `.cs` files
- [ ] New files automatically use LF line endings
- [ ] PNG/binary files not corrupted in Git

## 🆘 Support

If you encounter persistent encoding issues:
1. Run `Fix-FileEncodings.ps1 -Verbose`
2. Check Git config: `git config --list | Select-String encoding`
3. Verify `.gitattributes` is not ignored
4. Re-clone repository if issues persist
