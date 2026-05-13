# ManpLab Build Utilities
# Common functions for build scripts

function Get-VersionFromProps {
    <#
    .SYNOPSIS
    Reads version information from Version.props file

    .DESCRIPTION
    Parses Version.props XML to extract VersionPrefix, VersionSuffix, and constructs full version strings

    .PARAMETER PropsPath
    Path to Version.props file (defaults to solution root)

    .OUTPUTS
    Hashtable containing: VersionPrefix, VersionSuffix, Version (with suffix if present), FileVersion (4-part)

    .EXAMPLE
    $version = Get-VersionFromProps
    Write-Host "Building version $($version.Version)"
    #>

    param(
        [string]$PropsPath = "Version.props"
    )

    if (-not (Test-Path $PropsPath)) {
        throw "Version.props not found at: $PropsPath"
    }

    try {
        [xml]$props = Get-Content $PropsPath -Raw

        $versionPrefix = $props.Project.PropertyGroup.VersionPrefix
        $versionSuffix = $props.Project.PropertyGroup.VersionSuffix

        if ([string]::IsNullOrWhiteSpace($versionPrefix)) {
            throw "VersionPrefix not found in Version.props"
        }

        # Build full version string
        if ([string]::IsNullOrWhiteSpace($versionSuffix)) {
            $version = $versionPrefix
        } else {
            $version = "$versionPrefix-$versionSuffix"
        }

        # Build 4-part version for MSIX (required format)
        $fileVersion = "$versionPrefix.0"

        return @{
            VersionPrefix = $versionPrefix
            VersionSuffix = $versionSuffix
            Version = $version
            FileVersion = $fileVersion
        }
    }
    catch {
        throw "Failed to parse Version.props: $_"
    }
}

function Write-VersionInfo {
    <#
    .SYNOPSIS
    Displays version information in a formatted way

    .PARAMETER VersionInfo
    Hashtable returned from Get-VersionFromProps
    #>

    param(
        [hashtable]$VersionInfo
    )

    Write-Host "`n=== Version Information ===" -ForegroundColor Cyan
    Write-Host "  Version Prefix: $($VersionInfo.VersionPrefix)" -ForegroundColor White
    if (-not [string]::IsNullOrWhiteSpace($VersionInfo.VersionSuffix)) {
        Write-Host "  Version Suffix: $($VersionInfo.VersionSuffix)" -ForegroundColor Yellow
    }
    Write-Host "  Full Version:   $($VersionInfo.Version)" -ForegroundColor Green
    Write-Host "  File Version:   $($VersionInfo.FileVersion)" -ForegroundColor White
    Write-Host "==========================`n" -ForegroundColor Cyan
}
