# Ocean Blue Theme

## ⚠️ Current Status: Not Yet Functional

**The Ocean Blue theme colors are defined but cannot currently be selected in the UI.**

This is due to a WinUI 3 limitation: the `RequestedTheme` property only supports "Light", "Dark", and "Default" (System). Custom theme dictionaries require a more complex implementation.

**See**: `Custom-Theme-Implementation.md` for technical details and implementation plan.

**To make Ocean Blue functional**, we need to:
1. Extract theme to separate XAML file (`Themes/OceanBlue.xaml`)
2. Implement manual theme dictionary loading in `App.xaml.cs`
3. Re-enable in Settings dropdown

---

## Overview

The **Ocean Blue** theme provides a beautiful, calming color scheme with varying shades of blue, ranging from soft pastels to rich royal blues. It's designed for comfortable extended use while maintaining visual elegance.

## Color Palette

### Primary Blue Shades

- **Pastel Blue** (`#E8F4F8`) - Side panels and subtle backgrounds
- **Light Blue-White** (`#F5FAFE`) - Main background
- **Sky Blue** (`#C8E3F5`) - Selected items and highlights
- **Medium Blue** (`#4A90E2`) - Primary accent color
- **Royal Blue** (`#2E5CB8`) - Accent buttons and focus elements

### Background Colors

| Element | Color | Usage |
|---------|-------|-------|
| Side Panels | `#E8F4F8` | Browser and Properties panels |
| Main Background | `#F5FAFE` | Canvas and main content area |
| Cards/Surfaces | `#FFFFFF` | Elevated elements |
| Secondary Surfaces | `#F8FCFE` | Layered cards |

### Accent Colors

The accent color system uses **Medium Blue** (`#4A90E2`) as the base:

- **Light Variants**: `#6BA3E8`, `#8CB6EE`, `#B3D1F5`
- **Dark Variants**: `#3A7DC8`, `#2A6AAF`, `#1A5796`

These create smooth transitions and hover effects throughout the UI.

### Text Colors

- **Primary Text**: `#1A3A52` (Deep Blue-Gray) - Maximum contrast for readability
- **Secondary Text**: `#3A5A72` (Medium Blue-Gray) - Supporting text
- **Tertiary Text**: `#5A7A92` (Light Blue-Gray) - Hints and metadata
- **Disabled Text**: `#A0B8C8` (Soft Gray-Blue)

### Button Colors

**Standard Buttons**:
- Background: White
- Hover: `#F0F8FC` (Very light blue)
- Pressed: `#E3F2F9` (Light blue)
- Foreground: `#2A5A82` (Deep blue)

**Accent Buttons** (Royal Blue):
- Background: `#2E5CB8`
- Hover: `#3D6FD1`
- Pressed: `#1F4A9A`
- Foreground: White

## Design Philosophy

### Calming & Professional
The Ocean Blue theme balances professionalism with a calming aesthetic, making it ideal for:
- Extended work sessions on complex fractals
- Mathematical visualization
- Analytical tasks requiring focus
- Presentations and demonstrations

### Visual Hierarchy
The theme uses blue saturation to create visual hierarchy:
1. **Lightest Blues** - Background elements (least important)
2. **Medium Blues** - Interactive elements (clickable)
3. **Rich Blues** - Primary actions (most important)

### Contrast & Accessibility
- Text colors provide excellent contrast against backgrounds
- Selected items use `#C8E3F5` for clear visibility
- Focus states use royal blue for strong visual feedback

## UI Elements

### Side Panels (Browser & Properties)
- Background: Soft Pastel Blue `#E8F4F8`
- Creates a subtle distinction from the main canvas
- Maintains visual consistency with toolbar

### Canvas Area
- Background: Light Blue-White `#F5FAFE`
- Provides a clean, neutral space for fractal rendering
- Subtle blue tint reduces eye strain

### Selection & Hover States
- Selected Items: Sky Blue `#C8E3F5`
- Hover: Slightly darker `#B3D9F0`
- Pressed: Even darker `#9ECFEB`

### Borders & Dividers
- Soft blue-gray borders (`#B8D4E8`, `#D1E7F0`)
- Subtle visual separation without harsh lines

## Usage

### Applying the Theme

1. **Via Settings**:
   - Navigate to Settings (⚙ icon or press `S`)
   - Select "Ocean Blue" from the Theme dropdown
   - Theme applies immediately

2. **Programmatically**:
```csharp
var settingsService = serviceProvider.GetService<IAppSettingsService>();
settingsService.SetTheme("Ocean Blue");
App.Current.ApplyTheme();
```

### Theme Persistence

The Ocean Blue theme preference is automatically saved to:
- **Storage**: `LocalSettings`
- **Key**: `"AppTheme"`
- **Value**: `"Ocean Blue"`

The theme is restored automatically on app launch.

## Technical Implementation

### Theme Dictionary Location
`ManpWinUI\App.xaml` - `ResourceDictionary.ThemeDictionaries`

### Key Resources Defined
- 50+ color resources covering all WinUI controls
- Side panel backgrounds
- Accent color system
- Text colors for all states
- Button styles (standard and accent)
- Selection and hover states
- Borders and dividers

### Dynamic Application
The theme is applied dynamically via `App.ApplyTheme()`:
```csharp
if (themeName == "Ocean Blue")
{
    rootElement.RequestedTheme = ElementTheme.Default;
    // Apply OceanBlue theme dictionary
}
```

## Comparison with Other Themes

| Feature | Light | Dark | Ocean Blue |
|---------|-------|------|------------|
| Background | White | Dark Gray | Light Blue-White |
| Side Panels | Gray | Very Dark Gray | Pastel Blue |
| Primary Accent | System | System | Medium Blue |
| Button Accent | System | System | Royal Blue |
| Text | Black | White | Deep Blue-Gray |
| Best For | General Use | Night Work | Analysis & Focus |

## Design Credits

- **Inspiration**: Ocean waves, sky gradients, and professional blue UI design
- **Color Selection**: Balanced for readability, visual appeal, and reduced eye strain
- **Implementation**: Week 8.5 (Theme Support Feature)

## Future Enhancements

Potential improvements for Ocean Blue theme:
- [ ] Animated gradient transitions on hover
- [ ] Configurable accent color intensity (Light/Medium/Dark variants)
- [ ] Optional "Deep Ocean" variant with darker blues
- [ ] Glassmorphism effects for elevated surfaces
- [ ] Custom scrollbar styling

---

**Version**: 1.0  
**Created**: January 2025  
**Feature Branch**: `feature/theme-support`
