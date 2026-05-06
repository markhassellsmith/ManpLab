# Animation Settings Interdependencies

This document describes the intelligent behavior between interdependent animation settings in the ManpWinUI application.

## Overview

The animation control panel now features smart interdependencies that automatically adjust related settings when you change one value, providing a more intuitive user experience.

## Zoom Settings Interdependencies

### 1. **Zoom Speed Preset → End Zoom**
**Trigger:** User selects a zoom speed preset from the dropdown  
**Behavior:** Automatically calculates `EndZoom` based on `StartZoom` and the selected multiplier

| Preset | Multiplier | Example (StartZoom = 1.0) |
|--------|-----------|---------------------------|
| Very Slow | 10× | EndZoom = 10 |
| Slow | 100× | EndZoom = 100 |
| Medium | 1,000× | EndZoom = 1,000 |
| Fast | 10,000× | EndZoom = 10,000 |
| Very Fast | 100,000× | EndZoom = 100,000 |
| Extreme | 1,000,000× | EndZoom = 1,000,000 |
| Custom | No change | Keeps current EndZoom |

### 2. **Start Zoom → End Zoom (when preset is active)**
**Trigger:** User changes the `StartZoom` value  
**Behavior:** Automatically recalculates `EndZoom` to maintain the current zoom speed ratio

**Example:**
- Selected preset: "Medium" (1000×)
- Change StartZoom from 1.0 → 10.0
- EndZoom automatically updates from 1,000 → 10,000

**Note:** This only applies when a preset other than "Custom" is selected.

### 3. **End Zoom → Zoom Speed Preset (auto-detection)**
**Trigger:** User manually changes the `EndZoom` value  
**Behavior:** Automatically detects which preset matches the new ratio and updates the dropdown

**Detection Logic:**
- Calculates ratio: `EndZoom / StartZoom`
- Matches to nearest preset (with 10% tolerance)
- If no preset matches → switches to "Custom"

**Example:**
- StartZoom = 1.0
- User changes EndZoom from 1,000 → 100
- Preset automatically switches from "Medium" → "Slow"

### 4. **Visual Feedback: Total Magnification Display**
**Always Visible:** Shows the current zoom ratio in human-readable format

**Format:**
- `< 1,000×`: Shows exact ratio (e.g., "500.0×")
- `1,000× - 999,999×`: Shows in thousands (e.g., "10.0K×")
- `≥ 1,000,000×`: Shows in millions (e.g., "5.0M×")

**Updates:** Real-time whenever StartZoom or EndZoom changes

## Duration/Frame Rate/Frame Count Interdependencies

### Already Implemented (Pre-existing)

1. **Duration → Frame Count**
   - When you change duration (seconds), frame count updates: `FrameCount = DurationSeconds × FrameRate`

2. **Frame Rate → Frame Count**
   - When you change frame rate (fps), frame count updates: `FrameCount = DurationSeconds × FrameRate`

3. **Frame Count → Duration**
   - When you manually change frame count, duration updates: `DurationSeconds = FrameCount / FrameRate`

## Technical Implementation

### Circular Update Prevention
All interdependent handlers use a guard flag (`_isUpdatingZoomValues`) to prevent infinite loops:

```csharp
private bool _isUpdatingZoomValues;

partial void OnStartZoomChanged(double value)
{
    if (!_isUpdatingZoomValues && SelectedZoomSpeed != ZoomSpeedPreset.Custom)
    {
        _isUpdatingZoomValues = true;
        try
        {
            // Update dependent values
        }
        finally
        {
            _isUpdatingZoomValues = false;
        }
    }
}
```

### Property Change Notifications
Uses `[NotifyPropertyChangedFor(nameof(...))]` to ensure computed properties update:

```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(ZoomRatio), nameof(ZoomRatioDisplay))]
private double startZoom = 1.0;
```

## User Experience Benefits

1. **Consistency:** Values always stay synchronized
2. **Discoverability:** Users can see the effect of presets immediately
3. **Flexibility:** Manual editing still works, but with smart detection
4. **Transparency:** Real-time feedback shows the actual magnification
5. **No Surprises:** Clear visual indication when switching to "Custom" mode

## Future Enhancements (Potential)

- **Pan Speed Presets:** Similar preset system for pan animations
- **Parameter Sweep Presets:** Common parameter ranges for artistic effects
- **Duration Presets:** Quick-select common durations (1s, 3s, 5s, 10s, 30s)
- **Export Quality Presets:** Link resolution, frame rate, and codec settings
