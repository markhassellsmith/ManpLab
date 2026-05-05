# MSIX Deployment - Troubleshooting

## The "Project needs to be deployed" Message

If you see this message when pressing F5 in Visual Studio, the issue is typically with your local `.csproj.user` file.

### Quick Fix

Your `ManpWinUI\ManpWinUI.csproj.user` file should contain:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Tell Visual Studio that deployment is handled automatically -->
    <AppxDeployOnDebugEnabled>true</AppxDeployOnDebugEnabled>
    <DeploymentTarget>Local Machine</DeploymentTarget>
    <RemoteDebugEnabled>false</RemoteDebugEnabled>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'">
    <DebuggerFlavor>AppHostLocalDebugger</DebuggerFlavor>
    <DeployOnBuild>True</DeployOnBuild>
  </PropertyGroup>
  <ItemGroup />
</Project>
```

### Alternative: Manual Deployment

If you still see the prompt, you can:

1. **Click "Yes"** - The deployment will be instant since the package is already built
2. **Run `Deploy-And-Run.ps1`** before pressing F5 - This script manually deploys the package

### Why This Happens

- MSIX apps must be "deployed" (registered with Windows) before debugging
- Visual Studio stores deployment preferences in `.csproj.user` (which is git-ignored)
- If this file is missing or incomplete, VS prompts every time
- The project already has automatic deployment configured in the `.csproj`, but VS checks the `.user` file first

### After Fixing

1. Close Visual Studio completely
2. Reopen and press F5
3. The app should launch directly without prompts

### Verification

Check that the package is deployed:
```powershell
Get-AppxPackage -Name "6335fa26-c01e-4e88-b714-effc8096fc01"
```

This should show Status: Ok
