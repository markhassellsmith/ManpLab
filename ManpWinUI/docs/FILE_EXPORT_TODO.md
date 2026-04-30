# File Export Feature - Implementation Note

**Status**: 🚨 **NOT IMPLEMENTED** - Missing functionality discovered during Week 7

---

## Current State

### UI Exists But Not Functional:
- ✅ Toolbar has "Save Image" button (MainPage.xaml line 88-100)
- ✅ Flyout menu with PNG/JPEG/SVG options
- ✅ Button is enabled when `FractalImage` is not null
- ❌ **Event handlers NOT implemented** in MainPage.cs:
  - `SaveImage_Click` - missing
  - `SavePNG_Click` - missing
  - `SaveJPEG_Click` - missing
  - `SaveSVG_Click` - missing (Hailstone only)

### Legacy C++ Implementation Exists:
- ✅ ManpWIN64 has full PNG/JPEG/SVG export in `Savefile.cpp`
- ✅ PNG library integrated (`pnglib/`)
- ✅ SVG generation code exists for Hailstone

---

## Required Implementation

### Week 8.5: File Export (Insert Before Week 9)

**Priority**: HIGH - Core user-facing feature

#### Task Breakdown:

**1. Create Export Service (C# Layer)**
- `IImageExportService` interface
- `ImageExportService` implementation
- PNG export using WinUI WriteableBitmap → StorageFile
- JPEG export with quality settings
- File picker integration (FileSavePicker)

**2. Implement Event Handlers (MainPage.cs)**
- `SaveImage_Click` - default PNG export
- `SavePNG_Click` - PNG with options dialog
- `SaveJPEG_Click` - JPEG with quality slider
- `SaveSVG_Click` - Hailstone SVG export (future: call C++ bridge)

**3. Export Options Dialog (Optional)**
- Resolution selection (current/custom)
- Quality settings (JPEG)
- Metadata inclusion toggle
- Filename template

**4. Native Bridge for SVG (C++/CLI)**
- Wrap existing `Savefile.cpp` SVG export
- Expose through ManpCore.Native
- Pass Hailstone sequence data to C++ for SVG generation

---

## Recommended Schedule Addition

Insert new week between current Week 8 and Week 9:

```
### Week 8: Presets & History (Current)
Save locations, navigation undo/redo

### **Week 8.5: File Export** ⬅️ NEW
**Implement image save functionality**
- PNG/JPEG export with file picker
- Export options dialog
- SVG export for Hailstone (via native bridge)
- Metadata embedding (coordinates, parameters)

### Week 9: Core Features (Renumber to Week 9.5)
- Render cancellation (ESC key) ✅ Already works!
- Deep zoom toggle (arbitrary precision)
- Enhanced status bar
```

---

## Technical Notes

### WinUI File Saving Approach:
```csharp
// Use FileSavePicker for file selection
var picker = new FileSavePicker();
picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
picker.FileTypeChoices.Add("PNG Image", new[] { ".png" });
picker.SuggestedFileName = $"fractal_{DateTime.Now:yyyyMMdd_HHmmss}";

var file = await picker.PickSaveFileAsync();
if (file != null)
{
    // Get WriteableBitmap from ViewModel.FractalImage
    // Encode to PNG using BitmapEncoder
    // Write to StorageFile
}
```

### Metadata Embedding:
- Use PNG tEXt chunks for coordinates, parameters, fractal type
- JPEG EXIF for basic metadata
- SVG `<metadata>` tags for Hailstone

### Quality Considerations:
- Default PNG: lossless, current resolution
- JPEG: quality slider (70-100), optional upsampling
- SVG: vector-based, infinite resolution (Hailstone only)

---

## Dependencies

**Blocks:**
- User satisfaction (can't save their work!)
- Week 10 Animation System (batch export needs single-image export working)

**Requires:**
- Current render pipeline (✅ working)
- FractalImage WriteableBitmap (✅ available in ViewModel)
- File system permissions (✅ MSIX package has PicturesLibrary capability)

---

## Estimated Effort

- **2-3 hours** for basic PNG/JPEG export
- **1 hour** for file picker and event handlers
- **2 hours** for SVG native bridge (if prioritized)
- **1 hour** for metadata embedding
- **1 hour** for testing and edge cases

**Total: 7-8 hours (~1 week task)**

---

## Recommendation

**Add as Week 8.5** before continuing to Phase 3. This is a critical user-facing feature that should not be deferred to Phase 4 (polish).

Users need to be able to save their discoveries while exploring fractals!

---

**Discovered**: 2025-01-XX during Week 7 Task 1  
**Priority**: HIGH  
**Status**: Awaiting scheduling decision
