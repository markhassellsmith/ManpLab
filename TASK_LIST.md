# ManpWinUI Development Task List

**Active Branch:** `feature/phase3-winui-project`  
**Last Updated:** January 2026  
**Current Focus:** Phase 3 - WinUI Implementation

---

## ✅ COMPLETED TASKS

### Phase 3 - Core Features
- [x] **Basic Mandelbrot rendering** ✅ Commit: d5136c3
- [x] **Interactive zoom & pan** ✅ Commit: d5136c3
- [x] **Multiple color palettes** ✅ Commit: d5136c3
- [x] **Julia set mode** ✅ Commit: d5136c3
- [x] **Coordinate axes overlay** ✅ Commit: 905d7d8
- [x] **Auto-scale iterations** ✅ Commit: d5136c3
- [x] **Comprehensive keyboard shortcuts** ✅ Commit: ffebdca
- [x] **Resolution presets (1-4 keys)** ✅ Commit: ffebdca
- [x] **Real-time status updates** ✅ Commit: d5136c3

### Infrastructure
- [x] **MVVM architecture** ✅ Initial setup
- [x] **Dependency injection** ✅ Initial setup
- [x] **Documentation structure** ✅ Commit: aa189c6

---

## 🔄 CURRENT SPRINT - In Progress

Pick the next task from this list:

### Priority 1 - Core Functionality

#### 1. **Save/Export Images** (Ctrl+S)
**Status:** Not started  
**Priority:** HIGH  
**Effort:** Medium (4-6 hours)  

**Requirements:**
- Implement file picker dialog (SaveFileDialog)
- Export WriteableBitmap to PNG format
- Support multiple formats (PNG, JPEG, BMP)
- Update keyboard shortcut from placeholder to working
- Add toolbar button with save icon
- Show success/error messages

**Files to modify:**
- `ManpWinUI/ViewModels/MainViewModel.cs` - Add SaveImageCommand
- `ManpWinUI/Views/MainPage.xaml` - Add save button to toolbar
- `ManpWinUI/Views/MainPage.KeyboardHandling.cs` - Update Ctrl+S handler
- `ManpWinUI/Services/ImageExportService.cs` - NEW file for export logic

**Acceptance criteria:**
- [ ] Ctrl+S opens save dialog
- [ ] PNG format supported
- [ ] File saved successfully
- [ ] Status message shown
- [ ] Toolbar button added
- [ ] Tests added
- [ ] Documentation updated

---

#### 2. **Render Cancellation** (Esc key)
**Status:** Not started  
**Priority:** HIGH  
**Effort:** Medium (3-5 hours)

**Requirements:**
- Implement CancellationTokenSource in MandelbrotService
- Add Cancel button to UI (appears during rendering)
- Update Esc key handler
- Show cancellation status
- Clean up partial renders

**Files to modify:**
- `ManpWinUI/Services/MandelbrotService.cs` - Add cancellation support
- `ManpWinUI/ViewModels/MainViewModel.cs` - Add CancelRenderCommand
- `ManpWinUI/Views/MainPage.xaml` - Add cancel button
- `ManpWinUI/Views/MainPage.KeyboardHandling.cs` - Update Esc handler

**Acceptance criteria:**
- [ ] Esc key cancels render
- [ ] Cancel button appears during render
- [ ] Clean cancellation (no crashes)
- [ ] Status message shown
- [ ] Tests added
- [ ] Documentation updated

---

#### 3. **Parameter Presets**
**Status:** Not started  
**Priority:** MEDIUM  
**Effort:** Medium (4-6 hours)

**Requirements:**
- Create preset system (interesting locations)
- Add preset selector to UI
- Support custom presets
- Include default presets (seahorse valley, elephant valley, etc.)
- Save/load presets to JSON

**Files to create/modify:**
- `ManpWinUI/Models/FractalPreset.cs` - NEW preset model
- `ManpWinUI/Services/PresetService.cs` - NEW preset management
- `ManpWinUI/ViewModels/MainViewModel.cs` - Add preset commands
- `ManpWinUI/Views/MainPage.xaml` - Add preset dropdown

**Acceptance criteria:**
- [ ] 5+ default presets included
- [ ] Preset dropdown works
- [ ] Presets load correctly
- [ ] Custom presets can be saved
- [ ] JSON persistence works
- [ ] Documentation updated

---

#### 4. **Color Palette Editor**
**Status:** Not started  
**Priority:** MEDIUM  
**Effort:** Large (8-12 hours)

**Requirements:**
- Create color editor dialog
- Support gradient creation
- Add preset gradients
- Live preview
- Import/export palettes

**Files to create/modify:**
- `ManpWinUI/Views/ColorEditorDialog.xaml` - NEW dialog
- `ManpWinUI/ViewModels/ColorEditorViewModel.cs` - NEW viewmodel
- `ManpWinUI/Services/ColorScheme.cs` - Extend existing
- `ManpWinUI/Views/MainPage.xaml` - Add menu item

**Acceptance criteria:**
- [ ] Dialog opens from menu
- [ ] Gradient editor works
- [ ] Live preview visible
- [ ] Can save custom palettes
- [ ] Import/export works
- [ ] Documentation updated

---

## 🎯 BACKLOG - Planned Features

### Advanced Fractals
- [ ] Newton fractals
- [ ] Burning Ship
- [ ] Additional Mandelbrot variants
- [ ] IFS fractals

### Visualization
- [ ] Animation recording
- [ ] Orbit trap visualization
- [ ] Distance estimation rendering
- [ ] 3D rendering

### Deep Zoom
- [ ] Perturbation theory integration
- [ ] Arbitrary precision support
- [ ] BLA acceleration

### Quality of Life
- [ ] Location bookmarks
- [ ] Undo/redo navigation
- [ ] History panel
- [ ] Parameter locking

---

## 📋 WORKFLOW

### For Each Task:

1. **Before Starting:**
   - [ ] Review requirements above
   - [ ] Read related documentation
   - [ ] Check if files exist

2. **During Development:**
   - [ ] Write code following MVVM pattern
   - [ ] Add keyboard shortcuts if applicable
   - [ ] Update UI as needed
   - [ ] Test thoroughly

3. **Before Committing:**
   - [ ] Run build (`dotnet build`)
   - [ ] Test manually
   - [ ] Update this task list (move to completed)
   - [ ] Update `ManpWinUI/README.md` roadmap

4. **Commit:**
   - [ ] Stage only related files
   - [ ] Write descriptive commit message
   - [ ] Push to `feature/phase3-winui-project`

---

## 🚀 NEXT TASK

**Recommended:** Start with **Save/Export Images** (highest priority, medium effort)

Run this command to start:
```bash
# Update this task list to mark as "In Progress"
# Then begin implementation
```

---

## 📊 PROGRESS TRACKER

**Phase 3 Completion:** 9/13 tasks (69%)

**Completed:** 9  
**In Progress:** 0  
**Remaining:** 4

**Target:** Complete all "In Progress" tasks, then move to backlog

---

**Need help?** Check:
- [ManpWinUI/README.md](ManpWinUI/README.md) - Architecture overview
- [ManpWinUI/KEYBOARD_SHORTCUTS.md](ManpWinUI/KEYBOARD_SHORTCUTS.md) - Shortcut reference
- [ARCHITECTURE_FRACTAL_MODULAR_DESIGN.md](ARCHITECTURE_FRACTAL_MODULAR_DESIGN.md) - Design patterns
