# File Export Feature - Manual Testing Guide

## Prerequisites
- ManpWinUI application running
- At least one fractal rendered

---

## Test 1: PNG Export with Metadata

### Steps:
1. Launch ManpWinUI
2. Render a Mandelbrot fractal (default view or navigate to an interesting area)
3. Click "Save Image" button in toolbar
4. Select "Save as PNG" from the flyout menu
5. In the file picker:
   - Verify suggested filename format: `Mandelbrot_YYYYMMDD_HHMMSS.png`
   - Choose a save location
   - Click "Save"
6. Verify status bar shows: "Image saved as PNG with embedded metadata!"

### Verification:
- [ ] File saved successfully
- [ ] Image opens in default image viewer
- [ ] Image quality matches rendered fractal
- [ ] Resolution matches current render (default: 1280×720)

### Metadata Check (Optional):
Use ExifTool or similar to verify PNG tEXt chunks:
```powershell
# Install ExifTool from https://exiftool.org/
exiftool Mandelbrot_20250115_123456.png
```

Expected metadata fields:
- Software: "ManpWinUI 1.0"
- FractalType: "Mandelbrot"
- Center: coordinates
- Zoom: zoom level
- MaxIterations: iteration count
- ColorPalette: palette name
- ManpLabMetadata: Complete JSON

---

## Test 2: JPEG Export

### Steps:
1. Render a Julia fractal (use "Fractal Browser" → select Julia → use preset)
2. Click "Save Image" → "Save as JPEG"
3. In the file picker:
   - Verify suggested filename: `Julia_YYYYMMDD_HHMMSS.jpg`
   - Save file
4. Verify status bar message

### Verification:
- [ ] File saved successfully
- [ ] Image opens correctly
- [ ] Compression artifacts minimal (high quality)
- [ ] Julia parameters preserved in metadata

---

## Test 3: SVG Export (Hailstone Only)

### Steps:
1. Switch to Hailstone mode:
   - Select "Hailstone 2D" from Fractal Browser
   - Or change SelectedFractalType to a Hailstone variant
2. Enter starting coordinates (e.g., -10, 6)
3. Click "Render" to compute sequence
4. Verify "Save as SVG" option is visible in Save flyout
5. Click "Save as SVG"
6. In the file picker:
   - Verify filename: `Hailstone_YYYYMMDD_HHMMSS.svg`
   - Save file
7. Open SVG in browser or vector editor

### Verification:
- [ ] SVG saved successfully
- [ ] Vector graphics render correctly
- [ ] Trajectory visible
- [ ] Points labeled (if enabled)
- [ ] Cycle detection shown (if applicable)
- [ ] Metadata embedded in `<metadata>` tags
- [ ] Infinite zoom works (vector benefits)

---

## Test 4: Clipboard Copy

### Steps:
1. Render any fractal
2. Click "Save Image" → "Copy to Clipboard"
3. Verify status bar: "Image copied to clipboard with metadata!"
4. Open Paint, Photoshop, or GIMP
5. Paste (Ctrl+V)

### Verification:
- [ ] Image pastes successfully
- [ ] Quality matches original
- [ ] Resolution preserved
- [ ] No visible artifacts

---

## Test 5: Edge Cases

### Test 5.1: No Image Rendered
1. Launch app without rendering
2. Verify "Save Image" button is **disabled**

### Test 5.2: Cancel File Picker
1. Render fractal
2. Click "Save Image" → "Save as PNG"
3. In file picker, click "Cancel"
4. Verify status bar: "Save cancelled"
5. No error message or crash

### Test 5.3: SVG in Wrong Mode
1. Ensure you're NOT in Hailstone mode (e.g., Mandelbrot)
2. Verify "Save as SVG" option is **NOT visible** in flyout

### Test 5.4: File Overwrite
1. Save a fractal as PNG
2. Render a different view
3. Save again with same filename
4. Verify Windows asks for overwrite confirmation
5. Click "Yes" and verify new image replaces old

---

## Test 6: Metadata Accuracy

### Test 6.1: Mandelbrot Standard
1. Render Mandelbrot at default view
2. Save as PNG
3. Check metadata includes:
   - [ ] FractalType: "Mandelbrot"
   - [ ] IterationMode: "Standard"
   - [ ] Center: (-0.5, 0.0) approximately
   - [ ] Zoom: 0.6 approximately
   - [ ] MaxIterations: 256 (default)
   - [ ] ColorPalette: current palette
   - [ ] ImageWidth/Height: 1280×720

### Test 6.2: Julia Set
1. Render Julia with preset (e.g., Classic Spiral)
2. Save as PNG
3. Check metadata includes:
   - [ ] FractalType: "Mandelbrot" (base type)
   - [ ] IterationMode: "Julia"
   - [ ] JuliaC: { real: -0.7, imaginary: 0.27015 }
   - [ ] Center: (0, 0)

### Test 6.3: Different Resolutions
1. Change resolution to 1920×1080 (Full HD preset)
2. Render fractal
3. Save as PNG
4. Verify metadata shows:
   - [ ] ImageWidth: 1920
   - [ ] ImageHeight: 1080
5. Open image and verify actual pixel dimensions match

### Test 6.4: Different Palettes
1. Switch to "Fire" palette (Color Editor)
2. Render fractal
3. Save as PNG
4. Check metadata:
   - [ ] ColorPalette: "Fire"
5. Switch to "Ocean" palette
6. Save as PNG
7. Check metadata:
   - [ ] ColorPalette: "Ocean"

---

## Test 7: Filename Generation

Verify suggested filenames for different scenarios:

| Fractal Type | Mode | Expected Filename Pattern |
|--------------|------|---------------------------|
| Mandelbrot | Standard | `Mandelbrot_YYYYMMDD_HHMMSS.png` |
| Mandelbrot | Julia | `Mandelbrot_Julia_YYYYMMDD_HHMMSS.png` |
| Phoenix | Standard | `Phoenix_YYYYMMDD_HHMMSS.png` |
| Burning Ship | Standard | `BurningShip_YYYYMMDD_HHMMSS.png` |
| Hailstone 2D | N/A (SVG) | `Hailstone_YYYYMMDD_HHMMSS.svg` |

---

## Test 8: Performance

### Large Resolution Export
1. Set resolution to 4K (3840×2160)
2. Render fractal (may take longer)
3. Save as PNG
4. Measure time from click to "saved" message
5. Verify no UI freezing during save

**Expected**: < 3 seconds for 4K PNG save

---

## Test 9: Cross-Platform Compatibility

### Test 9.1: Windows Photo Viewer
- [ ] PNG opens correctly
- [ ] JPEG opens correctly

### Test 9.2: Adobe Photoshop / GIMP
- [ ] Import PNG with metadata preserved
- [ ] Verify tEXt chunks readable (if tool supports)

### Test 9.3: Web Browsers (Chrome, Firefox, Edge)
- [ ] SVG renders correctly
- [ ] Vector zoom works smoothly
- [ ] Metadata visible in SVG source code

---

## Test 10: Error Handling

### Test 10.1: Read-Only Location
1. Try saving to a read-only folder (e.g., C:\Windows\System32)
2. Verify error message in status bar
3. No application crash

### Test 10.2: Disk Full (Advanced)
1. Use a nearly-full drive
2. Try saving large image
3. Verify graceful error handling

### Test 10.3: Invalid Characters in Filename
1. Try saving with filename: `Test<>:"/\|?*.png`
2. Verify Windows file picker prevents invalid names
3. Or application sanitizes filename

---

## Success Criteria

✅ **All tests pass** = File Export feature fully functional

### Critical Tests (Must Pass):
- [ ] Test 1: PNG Export
- [ ] Test 2: JPEG Export
- [ ] Test 3: SVG Export (Hailstone)
- [ ] Test 4: Clipboard Copy
- [ ] Test 5.1: Disabled when no image

### Important Tests (Should Pass):
- [ ] Test 5.2: Cancel file picker
- [ ] Test 6.1-6.4: Metadata accuracy
- [ ] Test 7: Filename generation
- [ ] Test 9: Cross-platform compatibility

### Nice-to-Have Tests (Optional):
- [ ] Test 8: Performance
- [ ] Test 10: Error handling

---

## Troubleshooting

### Issue: "Save Image" button disabled
- **Cause**: No fractal rendered yet
- **Solution**: Click "Render" button first

### Issue: SVG not visible in flyout
- **Cause**: Not in Hailstone mode
- **Solution**: Switch to Hailstone 2D fractal type

### Issue: Metadata not showing in image viewer
- **Cause**: Viewer doesn't support PNG tEXt or JPEG EXIF
- **Solution**: Use ExifTool or specialized metadata viewer

### Issue: File picker doesn't appear
- **Cause**: Window handle initialization issue
- **Solution**: Check logs, ensure WinUI 3 runtime is up-to-date

### Issue: Image quality poor
- **Cause**: JPEG compression or low resolution
- **Solution**: Use PNG for lossless, or increase render resolution

---

## Reporting Issues

If any test fails, please report with:
1. Test number and name
2. Steps to reproduce
3. Expected behavior
4. Actual behavior
5. Screenshots (if applicable)
6. Log files (check `%LocalAppData%\ManpWinUI\logs\`)

---

**Testing Guide Version**: 1.0
**Last Updated**: Week 8.5 completion
**Related Document**: `Phase2-Week8.5-Summary.md`
