# WinUI 3 Theme Implementation - Best Practices

## Date: May 6, 2026

## Core Principle

**Only override what you need to customize. Let WinUI 3's built-in theme system handle everything else.**

## The Problem We Had

We were defining custom `AppBarButtonForeground`, `AppBarButtonIconForeground`, and dozens of other resources manually. This approach:
- ❌ Was fragile and incomplete
- ❌ Broke when WinUI 3's internal templates changed
- ❌ Required maintaining dozens of resource definitions
- ❌ Didn't work with WinUI 3's resource resolution system

## The Correct Approach

### Built-in Themes (Light, Dark, Default)

**Only override custom application-specific resources:**

```xaml
<ResourceDictionary x:Key="Dark">
    <!-- Only override what we need for our custom panels -->
    <SolidColorBrush x:Key="SidePanelBackgroundBrush" Color="#121212" />
    <SolidColorBrush x:Key="CommandBarBackground" Color="#1E1E1E" />
</ResourceDictionary>
```

**What WinUI 3 automatically provides:**
- ✅ All `AppBarButton` foreground colors (normal, hover, pressed, disabled)
- ✅ All icon foreground colors (`SymbolIcon`, `FontIcon`)
- ✅ All control template resources
- ✅ Proper contrast and accessibility
- ✅ Automatic theme switching

### Custom Themes (e.g., Ocean Blue)

**For custom themes, only define visual appearance changes:**

```xaml
<!-- Ocean Blue Theme - Maritime color palette -->
<!-- This theme is based on Light theme with custom background colors -->
<!-- WinUI 3's Light theme handles all button/icon foreground automatically -->

<!-- Central Canvas Area -->
<SolidColorBrush x:Key="ApplicationPageBackgroundThemeBrush" Color="#C8DFF0" />

<!-- Toolbar / CommandBar -->
<SolidColorBrush x:Key="CommandBarBackground" Color="#B3D9F0" />

<!-- Side Panels -->
<SolidColorBrush x:Key="SidePanelBackgroundBrush" Color="#E8F4F8" />
```

**What NOT to do:**
- ❌ Don't define `AppBarButtonForeground` unless you need a specific color
- ❌ Don't define icon-specific foreground resources
- ❌ Don't try to replicate the entire WinUI 3 theme resource dictionary

## How Theme Loading Works

### Built-in Themes
1. App.xaml.cs sets `rootFrame.RequestedTheme = ElementTheme.Dark`
2. WinUI 3 loads its built-in Dark theme resources
3. Then applies your `ThemeDictionaries["Dark"]` overrides
4. Result: System resources + your custom panel colors

### Custom Themes (Ocean Blue)
1. App.xaml.cs loads custom ResourceDictionary: `LoadCustomTheme("ms-appx:///Themes/OceanBlue.xaml")`
2. Sets base theme: `rootFrame.RequestedTheme = ElementTheme.Light`
3. WinUI 3 applies Light theme resources
4. Then merges your custom OceanBlue resources
5. Result: Light theme buttons + your custom blue backgrounds

## Resources You Should Override

### Always Safe to Override
- `ApplicationPageBackgroundThemeBrush` - Main app background
- `CommandBarBackground` - Toolbar background
- `SidePanelBackgroundBrush` - Custom application panels
- `SystemAccentColor` - Accent color for highlights

### Usually Don't Need to Override
- `AppBarButtonForeground` - WinUI 3 handles this
- `AppBarButtonIconForeground` - WinUI 3 handles this
- Any `*PointerOver`, `*Pressed`, `*Disabled` variants - WinUI 3 provides these
- Icon-specific resources - Inherited from parent controls

### When You MIGHT Need to Override
If your custom background color is very unusual and WinUI 3's automatic contrast doesn't work:
```xaml
<!-- Only if absolutely necessary -->
<SolidColorBrush x:Key="AppBarButtonForeground" Color="#YourSpecificColor" />
```

## Current Implementation

### App.xaml Theme Dictionaries
```
Light:   SidePanelBackgroundBrush + CommandBarBackground (system defaults)
Dark:    SidePanelBackgroundBrush (#121212) + CommandBarBackground (#1E1E1E)
Default: SidePanelBackgroundBrush + CommandBarBackground (system defaults)
```

### Themes/OceanBlue.xaml
```
ApplicationPageBackgroundThemeBrush: #C8DFF0 (light blue)
CommandBarBackground: #B3D9F0 (lighter blue)
SidePanelBackgroundBrush: #E8F4F8 (very light blue)
+ Uses Light theme for all button/icon foregrounds
```

## Testing Checklist

When implementing or modifying themes:

1. ☐ **Test all four themes** (Light, Dark, Default, Ocean Blue)
2. ☐ **Test after clean rebuild** (`dotnet clean && dotnet build`)
3. ☐ **Check toolbar visibility** in each theme
4. ☐ **Hover over buttons** - verify hover states work
5. ☐ **Check disabled button states** - should have reduced contrast
6. ☐ **Switch themes at runtime** - verify smooth transitions
7. ☐ **Check custom panels** - verify your custom backgrounds apply

## Why This Works

### Resource Resolution Order
1. WinUI 3 loads its system theme resources (Light/Dark/HighContrast)
2. Your ThemeDictionary overrides are merged on top
3. Custom ResourceDictionaries (like OceanBlue.xaml) are merged last
4. Controls inherit from this combined resource dictionary

### The Beauty of This Approach
- ✅ **Minimal code** - Only define what you customize
- ✅ **Automatic updates** - Benefit from WinUI 3 improvements
- ✅ **Consistent behavior** - All controls work the same way
- ✅ **Accessibility** - System handles contrast requirements
- ✅ **Maintainable** - No massive resource definitions to maintain

## Common Pitfalls to Avoid

### ❌ Pitfall 1: Over-defining Resources
```xaml
<!-- DON'T DO THIS -->
<SolidColorBrush x:Key="AppBarButtonForeground" Color="#FFFFFF" />
<SolidColorBrush x:Key="AppBarButtonForegroundPointerOver" Color="#E0E0E0" />
<SolidColorBrush x:Key="AppBarButtonForegroundPressed" Color="#C0C0C0" />
<!-- ... and 20 more variants -->
```

### ✅ Solution: Let WinUI 3 Handle It
```xaml
<!-- Only override your custom backgrounds -->
<SolidColorBrush x:Key="CommandBarBackground" Color="#1E1E1E" />
```

### ❌ Pitfall 2: Implicit Styles Without ThemeResource
```xaml
<!-- DON'T DO THIS - Breaks theme switching -->
<Style TargetType="AppBarButton">
    <Setter Property="Foreground" Value="White" />
</Style>
```

### ✅ Solution: Use ThemeResource or Don't Set It
```xaml
<!-- Either use ThemeResource (only if needed) -->
<Style TargetType="AppBarButton">
    <Setter Property="Foreground" Value="{ThemeResource AppBarButtonForeground}" />
</Style>

<!-- Or better: Don't define it at all - let WinUI 3 handle it -->
```

### ❌ Pitfall 3: Setting Foreground on Controls Directly
```xaml
<!-- DON'T DO THIS IN XAML -->
<AppBarButton Foreground="White" Label="Render" />
```

### ✅ Solution: Let Theme Resources Apply
```xaml
<!-- Just define the button - theme handles foreground -->
<AppBarButton Label="Render" Icon="Play" />
```

## Documentation References

- WinUI 3 Theme Resources: https://learn.microsoft.com/en-us/windows/apps/design/style/xaml-theme-resources
- Custom Themes: https://learn.microsoft.com/en-us/windows/apps/design/style/app-theme
- This implementation: ManpWinUI/App.xaml and ManpWinUI/Themes/OceanBlue.xaml

## Change History

- **May 6, 2026**: Simplified all themes to follow WinUI 3 best practices
  - Removed 50+ unnecessary resource definitions
  - Let system handle all button/icon foregrounds
  - Fixed Dark mode toolbar visibility issue
  - Cleaned up OceanBlue.xaml to only define visual colors

## Summary

**The key insight**: WinUI 3's theme system is designed to handle the complexity for you. Your job is to define your app's unique visual identity (background colors, accent colors), not to replicate the entire control template system. Trust the framework!
