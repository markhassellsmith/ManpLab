<#
.SYNOPSIS
    Fixes file encodings and line endings in the ManpLab repository.

.DESCRIPTION
    Scans source files and converts them to UTF-8 with LF line endings.
    Handles C#, C++, XAML, XML, JSON, and Markdown files.
    Safe to run multiple times - only changes files that need it.

.PARAMETER Path
    Root path to scan. Defaults to current directory.

.PARAMETER WhatIf
    Shows what would be changed without making changes.

.PARAMETER Verbose
    Shows detailed output for each file processed.

.EXAMPLE
    .\Fix-FileEncodings.ps1
    Fixes all files in current directory and subdirectories.

.EXAMPLE
    .\Fix-FileEncodings.ps1 -WhatIf
    Shows what would be changed without modifying files.

.EXAMPLE
    .\Fix-FileEncodings.ps1 -Path ".\ManpWinUI" -Verbose
    Fixes files in ManpWinUI directory with detailed output.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory=$false)]
    [string]$Path = ".",

    [Parameter(Mandatory=$false)]
    [switch]$WhatIf
)

# File extensions to process
$extensions = @(
    "*.cs",      # C# source
    "*.csproj",  # C# project
    "*.sln",     # Solution
    "*.cpp",     # C++ source
    "*.h",       # C++ header
    "*.hpp",     # C++ header
    "*.c",       # C source
    "*.xaml",    # XAML
    "*.xml",     # XML
    "*.config",  # Config
    "*.json",    # JSON
    "*.md",      # Markdown
    "*.txt",     # Text
    "*.props",   # MSBuild props
    "*.targets"  # MSBuild targets
)

# Directories to exclude
$excludeDirs = @(
    "bin",
    "obj",
    ".vs",
    "packages",
    "node_modules",
    ".git"
)

function Test-FileEncoding {
    param([string]$FilePath)

    try {
        $bytes = [System.IO.File]::ReadAllBytes($FilePath)

        # Check BOM
        if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            return "UTF8-BOM"
        }
        if ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
            return "UTF16-LE"
        }
        if ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
            return "UTF16-BE"
        }

        # Try to decode as UTF-8
        try {
            $text = [System.Text.Encoding]::UTF8.GetString($bytes)
            $roundtrip = [System.Text.Encoding]::UTF8.GetBytes($text)
            if (($bytes.Length -eq $roundtrip.Length) -and 
                ([System.Linq.Enumerable]::SequenceEqual($bytes, $roundtrip))) {
                return "UTF8"
            }
        }
        catch {
            # Not valid UTF-8
        }

        return "UNKNOWN"
    }
    catch {
        return "ERROR"
    }
}

function Test-LineEndings {
    param([string]$FilePath)

    $content = [System.IO.File]::ReadAllText($FilePath)

    $crlf = ([regex]::Matches($content, "`r`n")).Count
    $lf = ([regex]::Matches($content, "(?<!\r)`n")).Count
    $cr = ([regex]::Matches($content, "`r(?!`n)")).Count

    if ($crlf -gt 0 -and $lf -eq 0 -and $cr -eq 0) { return "CRLF" }
    if ($lf -gt 0 -and $crlf -eq 0 -and $cr -eq 0) { return "LF" }
    if ($cr -gt 0 -and $crlf -eq 0 -and $lf -eq 0) { return "CR" }
    if ($crlf -gt 0 -or $lf -gt 0 -or $cr -gt 0) { return "MIXED" }

    return "NONE"
}

function Convert-FileToUtf8Lf {
    param(
        [string]$FilePath,
        [switch]$WhatIf
    )

    $encoding = Test-FileEncoding -FilePath $FilePath
    $lineEndings = Test-LineEndings -FilePath $FilePath

    $needsConversion = ($encoding -ne "UTF8" -and $encoding -ne "UTF8-BOM") -or 
                       ($lineEndings -ne "LF")

    if ($needsConversion) {
        $relativePath = Resolve-Path -Relative $FilePath

        if ($WhatIf) {
            Write-Host "Would convert: $relativePath" -ForegroundColor Yellow
            Write-Host "  Current: $encoding / $lineEndings → Target: UTF8 / LF" -ForegroundColor Gray
        }
        else {
            try {
                # Read with detected encoding
                $content = switch ($encoding) {
                    "UTF16-LE" { [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::Unicode) }
                    "UTF16-BE" { [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::BigEndianUnicode) }
                    "UTF8-BOM" { [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::UTF8) }
                    default { [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::Default) }
                }

                # Normalize line endings to LF
                $content = $content -replace "`r`n", "`n"
                $content = $content -replace "`r", "`n"

                # Write as UTF-8 without BOM
                $utf8NoBom = New-Object System.Text.UTF8Encoding $false
                [System.IO.File]::WriteAllText($FilePath, $content, $utf8NoBom)

                Write-Host "✓ Converted: $relativePath" -ForegroundColor Green
                Write-Host "  $encoding / $lineEndings → UTF8 / LF" -ForegroundColor Gray

                return $true
            }
            catch {
                Write-Warning "Failed to convert $relativePath : $_"
                return $false
            }
        }

        return $true
    }

    if ($VerbosePreference -eq 'Continue') {
        $relativePath = Resolve-Path -Relative $FilePath
        Write-Verbose "✓ Already correct: $relativePath ($encoding / $lineEndings)"
    }

    return $false
}

# Main script
Write-Host "`n=== ManpLab File Encoding Fixer ===" -ForegroundColor Cyan
Write-Host "Scanning: $Path" -ForegroundColor Gray
if ($WhatIf) {
    Write-Host "Mode: DRY RUN (no changes will be made)" -ForegroundColor Yellow
}
Write-Host ""

$changedCount = 0
$scannedCount = 0
$errorCount = 0

# Build exclude pattern
$excludePattern = ($excludeDirs | ForEach-Object { [regex]::Escape($_) }) -join '|'

foreach ($ext in $extensions) {
    $files = Get-ChildItem -Path $Path -Filter $ext -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { 
            $_.FullName -notmatch "\\($excludePattern)\\" 
        }

    foreach ($file in $files) {
        $scannedCount++

        try {
            if (Convert-FileToUtf8Lf -FilePath $file.FullName -WhatIf:$WhatIf) {
                $changedCount++
            }
        }
        catch {
            $errorCount++
            Write-Warning "Error processing $($file.FullName): $_"
        }
    }
}

# Summary
Write-Host "`n=== Summary ===" -ForegroundColor Cyan
Write-Host "Files scanned: $scannedCount" -ForegroundColor Gray
Write-Host "Files changed: $changedCount" -ForegroundColor $(if ($changedCount -gt 0) { "Yellow" } else { "Gray" })
Write-Host "Errors: $errorCount" -ForegroundColor $(if ($errorCount -gt 0) { "Red" } else { "Gray" })

if ($WhatIf -and $changedCount -gt 0) {
    Write-Host "`nRun without -WhatIf to apply changes." -ForegroundColor Yellow
}
elseif ($changedCount -gt 0) {
    Write-Host "`n✓ Conversion complete! Files are now UTF-8 with LF line endings." -ForegroundColor Green
    Write-Host "Remember to commit these changes to Git." -ForegroundColor Gray
}
else {
    Write-Host "`n✓ All files already have correct encoding!" -ForegroundColor Green
}

Write-Host ""
