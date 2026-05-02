# Custom Theme Implementation - Technical Notes

## Issue: WinUI Theme System Limitations

### Problem

WinUI 3's `RequestedTheme` property only supports three values:
- `ElementTheme.Light` → Uses "Light" ThemeDictionary
- `ElementTheme.Dark` → Uses "Dark" ThemeDictionary  
- `ElementTheme.Default` → Uses "Default" (System) ThemeDictionary

**Custom theme keys like "OceanBlue" are not automatically recognized.**

### What Doesn't Work

```csharp
// ❌ This doesn't work - WinUI ignores custom theme dictionary keys
rootElement.RequestedTheme = "OceanBlue"; // Compile error - not an ElementTheme value
```

```csharp
// ❌ This causes runtime errors (element already has parent)
Application.Current.Resources.MergedDictionaries.Clear();
Application.Current.Resources.MergedDictionaries.Add(oceanBlueDict);
```

## Solution: Advanced Theme Switching

To support custom themes like Ocean Blue, you need to implement manual theme dictionary switching:

### Approach 1: Runtime Theme Dictionary Replacement

```csharp
public void ApplyCustomTheme(string themeName)
{
    if (themeName == "Ocean Blue")
    {
        // Step 1: Set base theme to Default
        rootElement.RequestedTheme = ElementTheme.Default;

        // Step 2: Manually override Application.Current.Resources
        // This requires creating a NEW ResourceDictionary instance
        var customDict = new ResourceDictionary();
        customDict.Source = new Uri("ms-appx:///Themes/OceanBlue.xaml");

        // Step 3: Insert at the beginning of merged dictionaries
        Application.Current.Resources.MergedDictionaries.Insert(0, customDict);
    }
}
```

**Requirements:**
- Move Ocean Blue theme to separate file: `Themes/OceanBlue.xaml`
- Cannot be in `ThemeDictionaries` collection
- Must be a standalone `ResourceDictionary`

### Approach 2: Custom Attached Property

Create a custom attached property that extends theme support:

```csharp
public static class ThemeManager
{
    public static readonly DependencyProperty CustomThemeProperty =
        DependencyProperty.RegisterAttached(
            "CustomTheme",
            typeof(string),
            typeof(ThemeManager),
            new PropertyMetadata(null, OnCustomThemeChanged));

    private static void OnCustomThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is string themeName)
        {
            ApplyCustomTheme(element, themeName);
        }
    }

    private static void ApplyCustomTheme(FrameworkElement element, string themeName)
    {
        // Load and apply custom theme dictionary
        // ...
    }
}
```

**Usage in XAML:**
```xaml
<Grid local:ThemeManager.CustomTheme="OceanBlue">
    <!-- Content -->
</Grid>
```

### Approach 3: Replace Built-in Theme Slots

**Simplest but Limited:**

Replace one of the existing theme dictionaries (e.g., use "Light" slot for Ocean Blue):

```xaml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Light">
        <!-- Ocean Blue colors here instead of actual Light theme -->
        <SolidColorBrush x:Key="SidePanelBackgroundBrush" Color="#E8F4F8" />
        <!-- ... -->
    </ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

**Then in settings:**
```csharp
public List<string> AvailableThemes { get; } = new() { "Ocean Blue", "Dark", "System" };
// User selects "Ocean Blue" but it's actually mapped to Light theme slot
```

**Pros:**
- Works immediately with RequestedTheme
- No custom code needed

**Cons:**
- Loses actual Light theme
- Confusing for users
- Not scalable for multiple custom themes

## Recommended Implementation Plan

### Phase 1: Single Custom Theme (Ocean Blue)

1. **Create** `ManpWinUI/Themes/OceanBlue.xaml`:
   ```xaml
   <ResourceDictionary xmlns="...">
       <!-- All Ocean Blue colors -->
   </ResourceDictionary>
   ```

2. **Update** `App.xaml.cs` with manual dictionary loading:
   ```csharp
   private ResourceDictionary? _customThemeDict;

   public void ApplyTheme()
   {
       var themeName = _settingsService.GetTheme();

       if (themeName == "Ocean Blue")
       {
           LoadCustomTheme("ms-appx:///Themes/OceanBlue.xaml");
       }
       else
       {
           UnloadCustomTheme();
           rootElement.RequestedTheme = ThemeNameToElementTheme(themeName);
       }
   }

   private void LoadCustomTheme(string uri)
   {
       UnloadCustomTheme(); // Remove previous custom theme if any

       _customThemeDict = new ResourceDictionary { Source = new Uri(uri) };
       Application.Current.Resources.MergedDictionaries.Insert(0, _customThemeDict);
   }

   private void UnloadCustomTheme()
   {
       if (_customThemeDict != null)
       {
           Application.Current.Resources.MergedDictionaries.Remove(_customThemeDict);
           _customThemeDict = null;
       }
   }
   ```

3. **Update** `SettingsViewModel.cs`:
   ```csharp
   public List<string> AvailableThemes { get; } = new() 
       { "Light", "Dark", "Ocean Blue", "System" };
   ```

### Phase 2: Multiple Custom Themes (Future)

Consider creating a theme manager service:

```csharp
public interface IThemeService
{
    IReadOnlyList<ThemeInfo> AvailableThemes { get; }
    ThemeInfo CurrentTheme { get; }
    void ApplyTheme(string themeName);
}

public record ThemeInfo(
    string Name,
    string DisplayName,
    bool IsBuiltIn,
    string? ResourceUri = null);
```

## Current Status

- ✅ Ocean Blue theme **colors defined** in `App.xaml` (ThemeDictionaries)
- ❌ Ocean Blue theme **not selectable** in Settings UI
- ⚠️ Requires Phase 1 implementation to make functional

## Files

- `ManpWinUI/App.xaml` - Theme dictionary defined but not active
- `ManpWinUI/docs/OceanBlue-Theme.md` - Color palette documentation
- `ManpWinUI/docs/Custom-Theme-Implementation.md` - This file

## Next Steps

1. Decide on implementation approach (recommend Approach 1)
2. Extract Ocean Blue theme to separate XAML file
3. Implement custom theme loading in `App.xaml.cs`
4. Re-enable "Ocean Blue" in Settings dropdown
5. Test theme switching at runtime

---

**Created**: January 2025  
**Status**: Technical research - implementation pending  
**Branch**: `feature/theme-support`
