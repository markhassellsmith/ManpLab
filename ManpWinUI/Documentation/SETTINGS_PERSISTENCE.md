# Settings Persistence in ManpLab

## Overview

ManpLab's settings persistence system is designed to work correctly in **both distribution scenarios**:

1. **MSIX Package** - Packaged Windows app
2. **Portable ZIP** - Unpackaged self-contained distribution

## How It Works

### AppSettingsService Implementation

The `AppSettingsService` automatically detects the deployment type at runtime and uses the appropriate storage mechanism:

```csharp
public AppSettingsService()
{
    try
    {
        _localSettings = ApplicationData.Current.LocalSettings;
        _isPackaged = true;
        // Uses Windows ApplicationData for MSIX packages
    }
    catch (Exception)
    {
        _isPackaged = false;
        // Uses JSON file for portable ZIP distribution
    }
}
```

### Storage Locations

#### MSIX Package
- **Storage**: `ApplicationData.Current.LocalSettings`
- **Location**: `%LocalAppData%\Packages\[PackageFamilyName]\LocalState`
- **Management**: Automatically managed by Windows
- **Persistence**: Settings survive app updates, removed on uninstall

#### Portable ZIP
- **Storage**: JSON file (`settings.json`)
- **Location**: `%LocalAppData%\ManpLab\settings.json`
- **Management**: Manual file-based persistence
- **Persistence**: Settings survive between app sessions, even if app folder moves

## Settings Stored

The following settings are persisted across app sessions:

### UI Layout
- Browser panel width
- Properties panel width
- Browser panel visibility
- Properties panel visibility
- Properties tab selection

### Application Preferences
- Theme (Light/Dark/System/Ocean Blue)
- Default color palette
- Show axes by default
- Use smooth coloring by default
- Default antialiasing level
- Deep zoom enabled/disabled

### Per-Fractal Settings
- Last selected fractal
- Parameter values per fractal type
- User notes per fractal

## Example Usage

```csharp
// Service is automatically registered in App.xaml.cs
var settings = App.GetService<IAppSettingsService>();

// Save setting
settings.SetTheme("Dark");
settings.SetDefaultPalette("Classic");
settings.SetBrowserPanelWidth(300.0);

// Load setting
var theme = settings.GetTheme(); // Returns "Dark"
var palette = settings.GetDefaultPalette(); // Returns "Classic"
var width = settings.GetBrowserPanelWidth(); // Returns 300.0
```

## Technical Details

### JSON File Format (Portable ZIP)

```json
{
  "AppTheme": "Dark",
  "DefaultPalette": "Classic",
  "BrowserPanelWidth": 300.0,
  "PropertiesPanelWidth": 350.0,
  "ShowAxesByDefault": true,
  "UseSmoothColoringByDefault": false,
  "SelectedFractal": "Mandelbrot",
  "FractalParams_Mandelbrot": "{\"maxIterations\":1000,...}"
}
```

### Automatic File Management

- JSON file is created on first write
- Settings are loaded once at service construction
- Each write operation immediately persists to disk
- No manual save/load required from UI code

### Type Safety

The service handles type conversions automatically:
- `string` values
- `bool` values
- `double` values
- `int` values

## Testing Both Scenarios

### Test MSIX Package
1. Build as MSIX in Visual Studio
2. Deploy/install package
3. Change settings, close app, reopen
4. Settings should be restored

### Test Portable ZIP
1. Build in Release mode
2. Copy output to a new folder
3. Run `ManpWinUI.exe` directly
4. Change settings, close app, reopen
5. Check `%LocalAppData%\ManpLab\settings.json` exists
6. Settings should be restored

## Troubleshooting

### Settings Not Persisting (MSIX)
- Ensure app is properly registered via deployment
- Check that `ApplicationData.Current` is accessible
- Verify Package identity in `Package.appxmanifest`

### Settings Not Persisting (Portable ZIP)
- Check that `%LocalAppData%\ManpLab` folder is writable
- Verify `settings.json` is created after changing settings
- Check Debug output for error messages from AppSettingsService
- Ensure app has write permissions to LocalAppData

### Common Issues

**Q: Settings reset after Windows update**  
A: MSIX packages may be reset. This is Windows behavior for packaged apps.

**Q: Settings not shared between MSIX and portable versions**  
A: Correct. Each version uses different storage locations by design.

**Q: Can I manually edit settings.json?**  
A: Yes for portable ZIP. Close app first, edit JSON, then reopen.

## Best Practices

1. **Always use the service interface** - Don't access storage directly
2. **Set defaults in the service** - Each getter has a fallback default value
3. **Test both scenarios** - Verify MSIX and portable ZIP before releases
4. **Don't store large data** - Settings are for configuration, not data files
5. **Use meaningful keys** - Follow existing naming conventions

## Migration Path

If you need to migrate settings between versions:

1. Export settings from old version (if implemented)
2. Manually copy `settings.json` to new version (portable)
3. Or use Windows Settings backup (MSIX)

## Future Enhancements

Potential improvements:
- Settings import/export feature
- Settings sync across devices (cloud storage)
- Settings profiles (Light/Dark presets)
- Settings backup before reset
- Migration tool between MSIX and portable

---

**Last Updated**: January 2026  
**Applies To**: ManpLab v1.0 and later
