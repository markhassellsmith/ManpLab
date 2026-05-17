# Window Layout and Position Persistence

## Overview
ManpLab automatically saves and restores the main window's size and position across application sessions. This feature works seamlessly for both MSIX installer and portable ZIP deployments.

## Implementation

### Storage Locations

**MSIX (Packaged) Deployment:**
- Stored in: `ApplicationData.Current.LocalSettings`
- Platform-managed by Windows
- Automatically roams across devices (if enabled in Windows settings)

**Portable (Unpackaged) Deployment:**
- Stored in: `%LocalAppData%\ManpLab\settings.json`
- Example: `C:\Users\[Username]\AppData\Local\ManpLab\settings.json`
- Portable across machines when copied with the settings file

### Persisted Data

The following window properties are saved:
- **Width** (pixels)
- **Height** (pixels)
- **X Position** (pixels from left edge of primary display)
- **Y Position** (pixels from top edge of primary display)

### Default Behavior

On first launch or when no saved state exists:
- **Default Size:** 1200 × 800 pixels
- **Default Position:** System-determined (typically centered)

## Architecture

### Service Layer
- **IAppSettingsService** interface defines window bounds methods
- **AppSettingsService** implements storage using existing dual-mode pattern:
  - Packaged: `ApplicationData.Current.LocalSettings`
  - Unpackaged: JSON file in `LocalApplicationData`

### Application Lifecycle
1. **OnLaunched** (App.xaml.cs)
   - Window created
   - Settings service retrieved
   - `RestoreWindowBounds()` applies saved or default size/position
   - Window activated

2. **OnWindowClosed** (App.xaml.cs)
   - `SaveWindowBounds()` captures current window state
   - Settings persisted to appropriate storage
   - Navigation history also saved

## Code Structure

### New Interface Methods (IAppSettingsService.cs)
```csharp
int? GetWindowWidth();
void SetWindowWidth(int width);
int? GetWindowHeight();
void SetWindowHeight(int height);
int? GetWindowX();
void SetWindowX(int x);
int? GetWindowY();
void SetWindowY(int y);
```

### Storage Keys (AppSettingsService.cs)
```csharp
private const string WindowWidthKey = "WindowWidth";
private const string WindowHeightKey = "WindowHeight";
private const string WindowXKey = "WindowX";
private const string WindowYKey = "WindowY";
```

### Window Management (App.xaml.cs)
- `RestoreWindowBounds(Window, IAppSettingsService)` - Applies saved bounds at startup
- `SaveWindowBounds(Window, IAppSettingsService)` - Captures bounds at shutdown
- Uses WinUI 3 `AppWindow` API via existing `GetAppWindowForWindow()` helper

## User Experience

### First Launch
- Application opens with default 1200×800 window
- Position is system-determined (typically centered)
- User can resize and reposition window

### Subsequent Launches
- Window opens with exact size and position from previous session
- Multi-monitor configurations are supported
- Invalid positions (e.g., disconnected monitor) fall back gracefully

### Settings File Example (Portable Mode)
```json
{
  "WindowWidth": 1400,
  "WindowHeight": 900,
  "WindowX": 100,
  "WindowY": 50,
  "AppTheme": "Dark",
  "BrowserPanelWidth": 250.0,
  "PropertiesPanelWidth": 300.0
}
```

## Logging

Window operations are logged via Serilog:
```
[INF] Restored window bounds: 1400x900 at (100, 50)
[INF] Saved window bounds: 1400x900 at (100, 50)
```

## Multi-Monitor Support

The implementation handles multi-monitor scenarios:
- Window position is stored as absolute screen coordinates
- Windows handles off-screen detection automatically
- If saved position is invalid (monitor disconnected), Windows moves window on-screen

## Future Enhancements

Potential improvements:
- [ ] Persist maximized state separately
- [ ] Add DPI-awareness for high-DPI displays
- [ ] Validate saved position is within current display bounds before applying
- [ ] Support per-monitor DPI scaling

## Testing Checklist

### MSIX Deployment
- [x] Build successful
- [ ] Install via MSIX package
- [ ] Verify window restores size and position across restarts
- [ ] Check ApplicationData.LocalSettings contains window bounds
- [ ] Uninstall and verify settings are cleared

### Portable Deployment
- [ ] Build and extract portable ZIP
- [ ] Launch application
- [ ] Resize and reposition window
- [ ] Close and relaunch
- [ ] Verify `%LocalAppData%\ManpLab\settings.json` contains window bounds
- [ ] Copy settings file to new machine and verify restoration

### Edge Cases
- [ ] First launch (no saved state)
- [ ] Invalid saved coordinates (negative values, off-screen)
- [ ] Multi-monitor: disconnect monitor with saved position
- [ ] Extremely small window size (verify minimum enforced by OS)
- [ ] Window maximized (currently not explicitly handled)

## Related Features
- Panel width persistence (Browser, Properties)
- Panel visibility persistence
- Theme preference persistence
- Fractal parameter persistence
- Navigation history persistence

All features use the same `AppSettingsService` infrastructure and dual-mode storage strategy.
