# Settings Persistence in ManpLab

## Overview

ManpLab's **complete data persistence system** is designed to work correctly in **both distribution scenarios**:

1. **MSIX Package** - Packaged Windows app
2. **Portable ZIP** - Unpackaged self-contained distribution

**All persistence mechanisms** (settings, bookmarks, navigation history, fractal parameters, animation preferences) automatically detect the runtime mode and use the appropriate storage.

## How It Works

### Dual-Mode Storage Pattern

All services that handle persistence follow the same pattern - runtime detection with fallback:

```csharp
public MyService()
{
    try
    {
        _ = ApplicationData.Current.LocalFolder; // or LocalSettings
        _isPackaged = true;
        // Uses Windows Storage API for MSIX packages
    }
    catch (Exception)
    {
        _isPackaged = false;
        // Uses file system in %LocalAppData%\ManpLab for portable ZIP
    }
}
```

### Storage Locations

#### MSIX Package
- **Settings**: `ApplicationData.Current.LocalSettings`
- **Files**: `ApplicationData.Current.LocalFolder`
- **Location**: `%LocalAppData%\Packages\[PackageFamilyName]\LocalState`
- **Management**: Automatically managed by Windows
- **Persistence**: Data survives app updates, removed on uninstall

#### Portable ZIP
- **Settings**: JSON files in `%LocalAppData%\ManpLab\`
- **Files**: Same location as settings
- **Location**: `C:\Users\[Username]\AppData\Local\ManpLab\`
- **Management**: Manual file-based persistence
- **Persistence**: Data survives between app sessions, even if app folder moves

## All Persisted Data

### 1. Application Settings (`AppSettingsService`)
**File:** `settings.json` (portable) or `LocalSettings` (MSIX)

- UI Layout (panel widths, visibility, tab selection)
- Theme (Light/Dark/System/Ocean Blue)
- Default color palette
- Show axes by default
- Use smooth coloring by default
- Default antialiasing level
- Deep zoom enabled/disabled
- Animation last export directory
- Selected fractal name
- Per-fractal parameters (JSON)
- Per-fractal user notes

### 2. Bookmarks (`BookmarkService`)
**File:** `bookmarks.json` (portable) or `LocalFolder` (MSIX)

- User-created bookmarks
- Famous preset locations (Seahorse Valley, Elephant Valley, etc.)
- Bookmark metadata (name, description, parameters, favorite status)

### 3. Navigation History (`NavigationHistoryService`)
**File:** `navigation_history.json` (portable) or `LocalFolder` (MSIX)

- Undo/redo history for current session
- Note: History starts fresh each session by default
- Can be uncommented to persist across sessions

### 4. Fractal Parameters (`FractalParameterSet`)
**Storage:** Via `AppSettingsService` with key `FractalParams_[FractalName]`

- Per-fractal parameter values
- Automatically restored when fractal is selected
- Independent values for each fractal type

## Services Using Dual-Mode Storage

All these services correctly handle both MSIX and portable modes:

1. **AppSettingsService** ✅ - Application settings and preferences
2. **BookmarkService** ✅ - Fractal bookmarks and presets
3. **NavigationHistoryService** ✅ - Undo/redo navigation history
4. **FractalParameterSet** ✅ - Per-fractal parameter values
5. **AnimationViewModel** ✅ - Last export directory (via AppSettingsService)

## Example Usage

```csharp
// Settings service is automatically registered in App.xaml.cs
var settings = App.GetService<IAppSettingsService>();

// Save setting
settings.SetTheme("Dark");
settings.SetDefaultPalette("Classic");
settings.SetBrowserPanelWidth(300.0);
settings.SetAnimationLastDirectory(@"C:\Users\...\Videos");

// Load setting
var theme = settings.GetTheme(); // Returns "Dark"
var palette = settings.GetDefaultPalette(); // Returns "Classic"
var width = settings.GetBrowserPanelWidth(); // Returns 300.0

// Bookmarks service
var bookmarks = App.GetService<IBookmarkService>();
await bookmarks.LoadBookmarksAsync(); // Loads from appropriate storage
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

### Automatic File Management

- JSON files are created on first write
- Settings/data are loaded once at service construction
- Each write operation immediately persists to disk
- No manual save/load required from UI code
- All services handle missing files gracefully (use defaults)

### Type Safety

The services handle type conversions automatically:
- `string` values
- `bool` values
- `double` values
- `int` values
- Complex objects (serialized as JSON strings)

## Testing Both Scenarios

### Test MSIX Package
1. Build as MSIX in Visual Studio
2. Deploy/install package
3. Change settings, add bookmarks, navigate
4. Close app, reopen
5. All data should be restored
6. Check: `%LocalAppData%\Packages\[PackageFamilyName]\LocalState`

### Test Portable ZIP
1. Build in Release mode (unpackaged)
2. Copy output to a new folder
3. Run `ManpWinUI.exe` directly
4. Change settings, add bookmarks, navigate
5. Close app, reopen
6. Check `%LocalAppData%\ManpLab\` for JSON files
7. All data should be restored

## Troubleshooting

### Data Not Persisting (MSIX)
- Ensure app is properly registered via deployment
- Check that `ApplicationData.Current` is accessible
- Verify Package identity in `Package.appxmanifest`
- Look for exceptions in Debug output

### Data Not Persisting (Portable ZIP)
- **Check folder exists**: `%LocalAppData%\ManpLab` should be created automatically
- **Check files exist**: Look for `settings.json`, `bookmarks.json`, etc.
- **Check write permissions**: Ensure app can write to LocalAppData
- **Check Debug output**: Look for `[ServiceName] Running as unpackaged app` messages
- **Check for exceptions**: Look for `InvalidOperationException` related to ApplicationData
- **Verify storage mode**: Debug output should show "portable" or "JSON file storage"

### Common Issues

**Q: "InvalidOperationException: Operation is not valid due to the current state of the object"**  
A: This means a service is trying to access `ApplicationData.Current` in portable mode. All services have been fixed to handle both modes, but if you see this, a service may need updating.

**Q: Settings reset after Windows update**  
A: MSIX packages may be reset. This is Windows behavior for packaged apps.

**Q: Settings not shared between MSIX and portable versions**  
A: Correct. Each version uses different storage locations by design.

**Q: Can I manually edit JSON files?**  
A: Yes for portable ZIP. Close app first, edit JSON, then reopen.

**Q: Bookmarks/history disappeared**  
A: Check if you switched between MSIX and portable builds. They use different storage locations.

## Best Practices

1. **Always use the service interfaces** - Don't access ApplicationData directly
2. **Set defaults in the services** - Each getter has a fallback default value
3. **Test both scenarios** - Verify MSIX and portable ZIP before releases
4. **Don't store large data** - Use these for configuration/metadata, not large files
5. **Use meaningful keys** - Follow existing naming conventions
6. **Handle missing data gracefully** - Services should work even if files don't exist
7. **Log storage mode** - Debug output should indicate which mode is active

## Architecture Notes

### Why This Pattern?

The dual-mode pattern exists because:
- **MSIX apps** must use Windows Storage API (ApplicationData)
- **Portable ZIP apps** cannot access ApplicationData (throws exception)
- **Same codebase** needs to work for both distribution types
- **No conditional compilation** - runtime detection is cleaner

### Adding New Persisted Data

To add new data that needs persistence:

1. Add to `IAppSettingsService` interface if it's a simple setting
2. Or create a new service following the dual-mode pattern:
   ```csharp
   private readonly bool _isPackaged;
   private readonly string? _filePath;

   public MyService()
   {
       try
       {
           _ = ApplicationData.Current.LocalFolder;
           _isPackaged = true;
       }
       catch
       {
           _isPackaged = false;
           var path = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               "ManpLab",
               "mydata.json");
           Directory.CreateDirectory(Path.GetDirectoryName(path)!);
           _filePath = path;
       }
   }
   ```
3. Implement load/save with branching logic for both modes
4. Test in both MSIX and portable builds
Potential improvements:
- Settings import/export feature
- Settings sync across devices (cloud storage)
- Settings profiles (Light/Dark presets)
- Settings backup before reset
- Migration tool between MSIX and portable

---

**Last Updated**: January 2026  
**Applies To**: ManpLab v1.0 and later
