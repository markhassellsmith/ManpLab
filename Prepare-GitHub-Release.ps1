# Quick Release Helper Script
# Streamlines the GitHub release process for ManpLab

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,

    [switch]$BuildOnly,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

Write-Host "`n=== ManpLab GitHub Release Helper ===" -ForegroundColor Cyan
Write-Host "Version: $Version`n" -ForegroundColor Yellow

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "ERROR: Version must be in format X.Y.Z (e.g., 1.1.1)" -ForegroundColor Red
    exit 1
}

$tagName = "v$Version"
$outputDir = "Release-Output"

# Step 1: Build packages (unless skipped)
if (-not $SkipBuild) {
    Write-Host "[1/5] Building release packages..." -ForegroundColor Yellow
    Write-Host "Running: .\Build-Release.ps1 -Version $Version" -ForegroundColor Gray

    & ".\Build-Release.ps1" -Version $Version

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Build failed!" -ForegroundColor Red
        exit 1
    }

    Write-Host "✓ Build complete`n" -ForegroundColor Green
} else {
    Write-Host "[1/5] Skipping build (using existing files)" -ForegroundColor Gray
}

# Check if files exist
$msixPath = Get-ChildItem -Path "$outputDir\MSIX" -Filter "ManpLab-$Version-*.msix" -ErrorAction SilentlyContinue | Select-Object -First 1
$zipPath = Get-ChildItem -Path "$outputDir\Portable-ZIP" -Filter "ManpLab-Portable-$Version-*.zip" -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not $msixPath -or -not $zipPath) {
    Write-Host "ERROR: Release files not found in $outputDir" -ForegroundColor Red
    Write-Host "Expected files:" -ForegroundColor Yellow
    Write-Host "  - $outputDir\MSIX\ManpLab-$Version-x64.msix" -ForegroundColor Gray
    Write-Host "  - $outputDir\Portable-ZIP\ManpLab-Portable-$Version-x64.zip" -ForegroundColor Gray
    exit 1
}

Write-Host "[2/5] Found release files:" -ForegroundColor Yellow
Write-Host "  ✓ $($msixPath.Name) ($([math]::Round($msixPath.Length / 1MB, 2)) MB)" -ForegroundColor Green
Write-Host "  ✓ $($zipPath.Name) ($([math]::Round($zipPath.Length / 1MB, 2)) MB)" -ForegroundColor Green
Write-Host ""

if ($BuildOnly) {
    Write-Host "Build complete! Files ready for manual upload." -ForegroundColor Cyan
    Write-Host "Next: Follow steps in GITHUB_RELEASE_GUIDE.md" -ForegroundColor Yellow
    exit 0
}

# Step 3: Check Git status
Write-Host "[3/5] Checking Git status..." -ForegroundColor Yellow
$gitStatus = git status --porcelain

if ($gitStatus) {
    Write-Host "WARNING: You have uncommitted changes:" -ForegroundColor Yellow
    git status --short
    Write-Host ""
    $continue = Read-Host "Continue anyway? (y/N)"
    if ($continue -ne 'y') {
        Write-Host "Aborted. Commit your changes first." -ForegroundColor Red
        exit 1
    }
}

# Check if we're on master branch
$currentBranch = git rev-parse --abbrev-ref HEAD
if ($currentBranch -ne 'master' -and $currentBranch -ne 'main') {
    Write-Host "WARNING: Not on master/main branch (current: $currentBranch)" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/N)"
    if ($continue -ne 'y') {
        Write-Host "Aborted." -ForegroundColor Red
        exit 1
    }
}

Write-Host "✓ Git status OK`n" -ForegroundColor Green

# Step 4: Create and push tag
Write-Host "[4/5] Creating Git tag..." -ForegroundColor Yellow
$tagMessage = Read-Host "Enter tag message (or press Enter for default)"
if ([string]::IsNullOrWhiteSpace($tagMessage)) {
    $tagMessage = "Release $Version"
}

# Check if tag already exists locally
$existingTag = git tag -l $tagName
if ($existingTag) {
    Write-Host "WARNING: Tag $tagName already exists locally" -ForegroundColor Yellow
    $recreate = Read-Host "Delete and recreate tag? (y/N)"
    if ($recreate -eq 'y') {
        git tag -d $tagName
        Write-Host "  Deleted local tag" -ForegroundColor Gray
    } else {
        Write-Host "Aborted." -ForegroundColor Red
        exit 1
    }
}

git tag -a $tagName -m $tagMessage
Write-Host "✓ Created tag: $tagName`n" -ForegroundColor Green

# Step 5: Push to GitHub
Write-Host "[5/5] Pushing to GitHub..." -ForegroundColor Yellow
$push = Read-Host "Push tag to GitHub? This will trigger release creation. (y/N)"
if ($push -eq 'y') {
    Write-Host "Pushing tag..." -ForegroundColor Gray
    git push origin $tagName

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Tag pushed successfully!`n" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Failed to push tag" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Tag created locally but not pushed." -ForegroundColor Yellow
    Write-Host "Push manually with: git push origin $tagName`n" -ForegroundColor Gray
}

# Summary and next steps
Write-Host "=== RELEASE PREPARATION COMPLETE ===" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Release Files Ready:" -ForegroundColor Cyan
Write-Host "  Location: $outputDir\" -ForegroundColor White
Write-Host "  - $($msixPath.Name)" -ForegroundColor White
Write-Host "  - $($zipPath.Name)" -ForegroundColor White
Write-Host ""
Write-Host "🏷️  Git Tag: $tagName" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Go to: https://github.com/markhassellsmith/ManpLab/releases" -ForegroundColor White
Write-Host "  2. Click 'Draft a new release'" -ForegroundColor White
Write-Host "  3. Select tag: $tagName" -ForegroundColor White
Write-Host "  4. Upload files from: $outputDir\" -ForegroundColor White
Write-Host "  5. Add release notes and publish!" -ForegroundColor White
Write-Host ""
Write-Host "💡 Tip: See GITHUB_RELEASE_GUIDE.md for detailed instructions" -ForegroundColor Gray
Write-Host ""

# Offer to open GitHub releases page
$openBrowser = Read-Host "Open GitHub releases page in browser? (y/N)"
if ($openBrowser -eq 'y') {
    Start-Process "https://github.com/markhassellsmith/ManpLab/releases/new"
}

# Offer to open output folder
$openFolder = Read-Host "Open release files folder? (y/N)"
if ($openFolder -eq 'y') {
    Start-Process (Resolve-Path $outputDir)
}
