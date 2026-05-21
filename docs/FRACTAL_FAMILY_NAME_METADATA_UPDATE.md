# Fractal Family and Name Metadata Update

## Summary

Updated metadata documentation to include **FractalFamily** and **FractalName** fields alongside the existing **FractalType** field, providing three levels of fractal identification in exported image metadata.

## Three-Level Identification System

### 1. FractalFamily (User-Facing Group)
- **Purpose**: Browser category/folder where the fractal is organized
- **Format**: Human-readable with spaces: `"Classic Escape-Time"`, `"Newton Method"`, `"Historical Fractals"`
- **Use Case**: Helps users understand the mathematical family and find related fractals
- **Display**: Show as "Fractal Family" in UI and metadata viewers

### 2. FractalName (User-Facing Name)
- **Purpose**: Display name of the specific fractal as shown in the browser
- **Format**: Human-readable with spaces: `"Mandelbrot Set"`, `"Burning Ship"`, `"Newton (z³-1)"`
- **Use Case**: Clear, recognizable identification for end users
- **Display**: Primary fractal identifier in UI

### 3. FractalType (Technical Identifier)
- **Purpose**: Internal registry key for programmatic reproduction
- **Format**: Code-style identifier: `"Mandelbrot"`, `"BurningShip"`, `"Newton3"`
- **Use Case**: Exact matching for code and automated reproduction
- **Source**: Matches `FractalDescriptor.Name` / registry key in codebase

## Example Relationships

### Example 1: Classic Fractal
```json
{
  "FractalFamily": "Classic Escape-Time",
  "FractalName": "Mandelbrot Set",
  "FractalType": "Mandelbrot"
}
```

### Example 2: Newton Method
```json
{
  "FractalFamily": "Newton Method",
  "FractalName": "Newton (z³-1)",
  "FractalType": "Newton3"
}
```

### Example 3: Historical Fractal
```json
{
  "FractalFamily": "Historical Fractals",
  "FractalName": "Pickover Biomorphs",
  "FractalType": "Biomorphs"
}
```

### Example 4: Burning Ship
```json
{
  "FractalFamily": "Classic Escape-Time",
  "FractalName": "Burning Ship",
  "FractalType": "BurningShip"
}
```

## Files Updated

### 1. `Docs/ImageMetadataProperties.csv`
**Added rows:**
- `FractalFamily` - With recommendation to use "Fractal Family" (with space) in UI
- `FractalName` - Display name as shown in browser
- Updated `FractalType` description to clarify it's the internal identifier

**Location in CSV:**
```csv
"Fractal Parameters","/tEXt/FractalFamily","N/A","Fractal family/category grouping...","RECOMMENDED: Browser folder name for user recognition; use 'Fractal Family' (with space) in UI",Yes,Partial
"Fractal Parameters","/tEXt/FractalName","N/A","Display name of specific fractal...","RECOMMENDED: User-friendly fractal name as shown in browser",Yes,Partial
"Fractal Parameters","/tEXt/FractalType","N/A","Internal fractal identifier...","Technical identifier matching internal registry key",Yes,Partial
```

### 2. `Docs/FractalMetadataRecommendations.md`

**Added section: "Field Definitions: Fractal Identification"**
- Complete explanation of all three fields
- Purpose and format for each
- Example relationships showing how they work together
- Placed before the JSON schema examples for clarity

**Updated all JSON schema examples:**
- Minimal Required Schema
- Extended Schema with Optional Fields
- Schema for Deep Zoom Images
- PNG Implementation Code Example

**All schemas now include:**
```json
{
  "FractalFamily": "Classic Escape-Time",
  "FractalName": "Mandelbrot Set",
  "FractalType": "Mandelbrot",
  ...
}
```

**Updated Mathematical Parameters table:**
Added FractalFamily and FractalName as CRITICAL priority fields alongside FractalType.

### 3. `Docs/ATTRIBUTION_INTEGRATION_SUMMARY.md`
**Added note about new fields:**
- Explains the three-level identification system
- Shows example values for each field
- Clarifies relationship to browser UI

## Implementation Guidance

### When Exporting Images

```csharp
var metadata = new FractalMetadata
{
    // Three-level identification
    FractalFamily = descriptor.Category,           // "Classic Escape-Time"
    FractalName = descriptor.DisplayName,          // "Mandelbrot Set"
    FractalType = descriptor.Name,                 // "Mandelbrot"

    // Mathematical parameters
    CenterX = viewModel.CenterX,
    CenterY = viewModel.CenterY,
    Zoom = viewModel.Zoom,
    MaxIterations = viewModel.MaxIterations,

    // Attribution
    Software = "ManpLab",
    Version = "1.0.0",
    SourceCode = "https://github.com/markhassellsmith/ManpLab",

    // User tagging/notes
    UserNotes = viewModel.PersonalNotes,           // From Properties panel

    // ... other fields
};
```

### Source Properties in Code

The values come from:
- **FractalFamily** ← `FractalDescriptor.Category` (internal code name)
- **FractalName** ← `FractalDescriptor.DisplayName` (UI display name)
- **FractalType** ← `FractalDescriptor.Name` (registry key)

### UI Display Recommendations

When showing metadata to users:
- Primary: Show **FractalName** (e.g., "Mandelbrot Set")
- Secondary: Show **FractalFamily** as "Fractal Family: Classic Escape-Time" (with space)
- Technical: Show **FractalType** only in developer/debug views

## Benefits

### For End Users
✅ **FractalFamily** helps them understand mathematical relationships
✅ **FractalName** provides clear, recognizable identification
✅ **UserNotes** enables tagging files for special projects and easy rediscovery
✅ Images are self-documenting and searchable

### For Developers
✅ **FractalType** ensures exact programmatic reproduction
✅ All three fields work together for robust identification
✅ No code changes needed yet - documentation defines the plan

### For Metadata Interoperability
✅ Human-readable names work in any metadata viewer
✅ Technical identifiers ensure machine reproducibility
✅ Family grouping enables filtering and organization
✅ Personal notes field supports custom categorization workflows

## Next Steps

### Phase 1: Update FractalMetadata Model
Add properties to `ManpWinUI/Models/FractalMetadata.cs`:
```csharp
[JsonPropertyName("fractalFamily")]
public string? FractalFamily { get; set; }

[JsonPropertyName("fractalName")]
public string? FractalName { get; set; }

[JsonPropertyName("userNotes")]
public string? UserNotes { get; set; }
```

### Phase 2: Update Export Service
Modify `ImageExportService.cs` to populate these fields from `FractalDescriptor` and UI:
```csharp
metadata.FractalFamily = descriptor.Category;
metadata.FractalName = descriptor.DisplayName;
metadata.FractalType = descriptor.Name;
metadata.UserNotes = viewModel.PersonalNotes;  // From Properties panel
```

### Phase 3: Update Metadata Display UI
If showing metadata to users, display:
- "Fractal: [FractalName]"
- "Family: [FractalFamily]"
- "Notes: [UserNotes]"

### Phase 4: Test and Validate
- Export various fractals and verify all fields populate correctly
- Test multi-line UserNotes with 500+ characters
- Test metadata readability in Windows Explorer and image viewers
- Verify JSON parsing works correctly
- Confirm UserNotes appears in both PNG and JPEG exports

## Backward Compatibility

✅ **No Breaking Changes**
- Existing code continues to work
- FractalType field is unchanged
- New fields (FractalFamily, FractalName, UserNotes) are additive only

✅ **Graceful Degradation**
- Old images without these fields still work
- New images are readable by old code (extra fields ignored)
- JSON parsing is forward-compatible
- UserNotes field is optional; empty/null values handled gracefully

## Terminology Consistency

### Internal Code: "Category"
- C# property: `FractalDescriptor.Category`
- Native C++ field: `spec.category`
- Class name: `FractalCategoryNode`

### User-Facing: "Fractal Family" (with space)
- JSON field name: `"FractalFamily"` (no space, camelCase)
- UI display: "Fractal Family" (with space)
- CSV column name: `/tEXt/FractalFamily`

This matches user intuition - fractals grouped into "families" like "Newton Method" or "Historical Fractals".

### User Notes Field: "Personal Notes"
- JSON field name: `"UserNotes"` (camelCase)
- UI label: "Personal Notes" (Properties panel, Info tab)
- CSV column name: `/tEXt/UserNotes`
- Purpose: Multi-line text for tagging files for special projects
- Character allowance: 500+ characters recommended
- Example use: "Deep zoom series for art exhibition. Part of seahorse valley exploration."

---

**Status**: ✅ Documentation Complete - Ready for Implementation

**Files Modified**:
- `Docs/ImageMetadataProperties.csv` (added UserNotes row)
- `Docs/FractalMetadataRecommendations.md` (added UserNotes to User Attribution section, mapping table, and JSON schemas)
- `Docs/ATTRIBUTION_INTEGRATION_SUMMARY.md`
- `Docs/FRACTAL_FAMILY_NAME_METADATA_UPDATE.md` (added UserNotes guidance)

**New File Created**:
- `Docs/FRACTAL_FAMILY_NAME_METADATA_UPDATE.md` (this document)
