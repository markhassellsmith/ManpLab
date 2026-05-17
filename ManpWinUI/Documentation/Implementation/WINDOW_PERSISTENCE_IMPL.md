# Window Layout and Position Persistence - Implementation Summary

## Commit Summary
Add window layout and position persistence for both MSIX and portable deployments

## Changes Made

### 1. IAppSettingsService.cs
Added eight new methods for window bounds persistence:
- `GetWindowWidth()` / `SetWindowWidth(int)`
- `GetWindowHeight()` / `SetWindowHeight(int)`
- `GetWindowX()` / `SetWindowX(int)`
- `GetWindowY()` / `SetWindowY(int)`

All getter methods return `int?` to support first-run detection.

### 2. AppSettingsService.cs
**Constant Keys Added:**
```csharp
private const string WindowWidthKey = "WindowWidth";
private const string WindowHeightKey = "WindowHeight";
private const string WindowXKey = "WindowX";
private const string WindowYKey = "WindowY";
```

**Implementation:**
- Eight methods implemented using existing `GetValue()` / `SetValue()` pattern
- Automatic dual-mode storage:
  - **MSIX:** `ApplicationData.Current.LocalSettings`
  - **Portable:** `%LocalAppData%\ManpLab\settings.json`

### 3. App.xaml.cs
**New Methods:**
- `RestoreWindowBounds(Window, IAppSettingsService)`
  - Applies saved bounds or defaults (1200×800)
  - Called in `OnLaunched` before window activation
  - Logs restoration via Serilog

- `SaveWindowBounds(Window, IAppSettingsService)`
  - Captures current window size and position
  - Called in `OnWindowClosed` before saving navigation history
  - Logs saved values via Serilog

**Lifecycle Integration:**
- `OnLaunched`: Restore → Activate
- `OnWindowClosed`: Save window bounds → Save navigation history → Close

## Storage Format

### MSIX (ApplicationData.LocalSettings)
Platform-managed key-value storage automatically handles the window bounds.

### Portable (JSON File)
Example `settings.json`:
```json
{
  "WindowWidth": 1400,
  "WindowHeight": 900,
  "WindowX": 100,
  "WindowY": 50,
  "AppTheme": "Dark",
  "BrowserPanelWidth": 250.0
}
```

## Default Behavior
- **First Launch:** 1200×800 pixels, system-positioned
- **Subsequent Launches:** Restores exact size and position from previous session

## Testing Status
- [x] Build successful (no compilation errors)
- [x] Code follows existing patterns and conventions
- [x] Dual-mode storage implemented (MSIX + Portable)
- [x] Logging integration complete
- [ ] Runtime testing pending (MSIX deployment)
- [ ] Runtime testing pending (Portable deployment)

## API Surface
Uses WinUI 3 `Microsoft.UI.Windowing.AppWindow` API:
- `AppWindow.Resize(SizeInt32)` - Set window size
- `AppWindow.Move(PointInt32)` - Set window position
- `AppWindow.Size` - Get current size
- `AppWindow.Position` - Get current position

## Notes
- Window maximized state is NOT currently persisted (future enhancement)
- Multi-monitor support is automatic via Windows display management
- Invalid positions (off-screen) are handled by Windows automatically
- Default size of 1200×800 provides good balance for fractal viewer
