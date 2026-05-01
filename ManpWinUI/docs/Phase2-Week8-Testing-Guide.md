# Week 8: User Acceptance Testing Guide

**Purpose**: Verify all Bookmarks and Navigation History features work correctly  
**Duration**: ~15-20 minutes  
**Prerequisites**: Build and run ManpWinUI in Visual Studio

---

## 🎯 Test Scenarios

### Test 1: Basic Navigation History (Undo/Redo)
**Goal**: Verify undo/redo works correctly

1. **Launch app** - Should show default Mandelbrot view
2. **Render initial image** - Click "Render" or press F5
3. **Zoom in** - Click "Zoom In" button or press + key
   - ✅ **VERIFY**: Back button becomes enabled in toolbar
   - ✅ **VERIFY**: Status shows "Zooming in to 2.00x..."
4. **Zoom in again** - Click "Zoom In" again
   - ✅ **VERIFY**: Back button still enabled
5. **Click Back button** (⬅)
   - ✅ **VERIFY**: View returns to previous zoom level
   - ✅ **VERIFY**: Forward button becomes enabled
   - ✅ **VERIFY**: Status shows "Navigated to: Zoomed in 2x to 2.00x"
6. **Click Back button again**
   - ✅ **VERIFY**: View returns to original state
   - ✅ **VERIFY**: Back button becomes disabled
7. **Click Forward button** (➡)
   - ✅ **VERIFY**: View moves forward in history
8. **Press Ctrl+Z**
   - ✅ **VERIFY**: Undoes navigation
9. **Press Ctrl+Y**
   - ✅ **VERIFY**: Redoes navigation

**Expected Result**: All undo/redo operations work smoothly ✅

---

### Test 2: History Panel Interface
**Goal**: Verify history UI displays correctly

1. **Perform several zoom operations** - Zoom in 3-4 times
2. **Open Properties panel** - Press Ctrl+P if not visible
3. **Navigate to History tab** - Click on "History" tab (Clock icon)
   - ✅ **VERIFY**: History tab opens
   - ✅ **VERIFY**: Instructions visible: "Use Ctrl+Z to undo and Ctrl+Y to redo"
   - ✅ **VERIFY**: "Total entries: X" shows correct count
   - ✅ **VERIFY**: List shows all navigation entries
4. **Check entry details** - Look at each history entry
   - ✅ **VERIFY**: Description (e.g., "Zoomed in 2x to 4.00x")
   - ✅ **VERIFY**: Coordinates display (e.g., "(-0.500000, 0.000000) @ 4.00x")
   - ✅ **VERIFY**: Timestamp (e.g., "2:45:30 PM")
   - ✅ **VERIFY**: "Go" button (➡) visible on each entry
5. **Click "Go" button** on an older entry
   - ✅ **VERIFY**: View jumps to that historical state
   - ✅ **VERIFY**: Fractal renders with those parameters
6. **Click "Clear All" button** in header
   - ✅ **VERIFY**: History list empties
   - ✅ **VERIFY**: "Total entries: 0" displayed
   - ✅ **VERIFY**: Back/Forward buttons become disabled

**Expected Result**: History panel displays and functions correctly ✅

---

### Test 3: Bookmarks System
**Goal**: Verify existing bookmark system still works

1. **Click Bookmarks button** in toolbar (⭐) or press B key
   - ✅ **VERIFY**: Bookmarks panel slides in from left
2. **Load a preset** - Click "Load" (➡) button on "Seahorse Valley"
   - ✅ **VERIFY**: View navigates to Seahorse Valley coordinates
   - ✅ **VERIFY**: Fractal renders automatically
   - ✅ **VERIFY**: Status shows "Loaded bookmark: Seahorse Valley"
3. **Zoom in on the loaded view** - Zoom in 2-3 times
4. **Save as new bookmark**
   - Enter name: "My Seahorse Detail"
   - Click "Save Bookmark" button
   - ✅ **VERIFY**: New bookmark appears in list
   - ✅ **VERIFY**: Status shows "Bookmark saved: My Seahorse Detail"
5. **Navigate away** - Reset view (Space key) or load different preset
6. **Reload your bookmark** - Click "Load" on "My Seahorse Detail"
   - ✅ **VERIFY**: Returns to saved view exactly
7. **Try to delete preset** - Click delete (🗑) button on "Seahorse Valley"
   - ✅ **VERIFY**: Preset cannot be deleted (button hidden for presets)
8. **Delete user bookmark** - Click delete on "My Seahorse Detail"
   - ✅ **VERIFY**: Bookmark is removed from list
   - ✅ **VERIFY**: Status shows "Deleted bookmark: ..."
9. **Toggle favorite** - Click star icon on any bookmark
   - ✅ **VERIFY**: Star changes from outline (☆) to filled (⭐)
10. **Close bookmarks panel** - Press B key or click X
    - ✅ **VERIFY**: Panel slides out

**Expected Result**: All bookmark features work as before ✅

---

### Test 4: Keyboard Shortcuts
**Goal**: Verify all shortcuts work from any control

1. **Press F1** (Help)
   - ✅ **VERIFY**: Help dialog appears
   - ✅ **VERIFY**: Shows "NAVIGATION HISTORY (Week 8)" section
   - ✅ **VERIFY**: Lists Ctrl+Z and Ctrl+Y shortcuts
2. **Close help dialog** - Press Escape or click outside
3. **Click in Parameters tab** - Click on any NumberBox
4. **Zoom with keyboard** - Press + key multiple times
5. **Undo with Ctrl+Z** while NumberBox focused
   - ✅ **VERIFY**: Undo works even with focus in NumberBox
6. **Test all shortcuts**:
   - Ctrl+R → Render
   - Space → Reset View
   - \+ → Zoom In
   - \- → Zoom Out
   - Ctrl+Z → Undo
   - Ctrl+Y → Redo
   - Ctrl+B → Toggle Browser
   - Ctrl+P → Toggle Properties
   - B → Toggle Bookmarks
   - ✅ **VERIFY**: All shortcuts work

**Expected Result**: Shortcuts work globally ✅

---

### Test 5: History Persistence
**Goal**: Verify history survives app restart

1. **Build up history** - Zoom in/out 5-6 times
2. **Open History tab** - Verify entries visible
3. **Note the entry count** - Remember how many entries exist
4. **Close application** - Click X button to exit
5. **Relaunch application** - Start app again in Visual Studio
6. **Open History tab** immediately
   - ✅ **VERIFY**: Previous history entries are restored
   - ✅ **VERIFY**: Entry count matches what you had before
7. **Press Ctrl+Z**
   - ✅ **VERIFY**: Can undo to previous session's state
   - ✅ **VERIFY**: Fractal renders with saved parameters

**Expected Result**: History persists across sessions ✅

---

### Test 6: Smart History Compression
**Goal**: Verify micro-movements aren't recorded

1. **Clear history** - Use "Clear All" button
2. **Zoom in once** - Should record entry
3. **Zoom in 5 more times rapidly** - Within a few seconds
4. **Check History tab**
   - ✅ **VERIFY**: NOT all 6 zooms recorded (smart compression)
   - ✅ **VERIFY**: Only significant changes appear (zoom doubling)
5. **Make tiny coordinate change**
   - Go to Parameters tab
   - Change Center X from -0.5 to -0.501 (very small)
   - Render
6. **Check History tab**
   - ✅ **VERIFY**: Small change not recorded (< 5% of view)

**Expected Result**: Only significant changes recorded ✅

---

### Test 7: Edge Cases
**Goal**: Verify proper behavior at boundaries

1. **Clear history**
2. **Verify Back button disabled** when no history
   - ✅ **VERIFY**: Back button grayed out
3. **Zoom in once** → **Undo**
   - ✅ **VERIFY**: Back button becomes disabled at start
4. **Redo** → **Zoom in twice more**
   - ✅ **VERIFY**: Forward button disabled at end of history
5. **Undo twice** → **Zoom in once**
   - ✅ **VERIFY**: Forward history cleared (can't redo past new action)
6. **Switch to Hailstone mode**
   - Select "Hailstone" fractal type
   - Click Render
7. **Try to zoom**
   - ✅ **VERIFY**: Zoom buttons disabled or message shown
   - ✅ **VERIFY**: History NOT recorded for Hailstone (expected)

**Expected Result**: Edge cases handled gracefully ✅

---

## 🐛 Bug Tracking

### Issues Found
| # | Issue | Severity | Status |
|---|-------|----------|--------|
| 1 | | | |
| 2 | | | |
| 3 | | | |

### Notes
- Use this section to document any unexpected behavior
- Include steps to reproduce
- Note severity: Critical / High / Medium / Low

---

## ✅ Final Checklist

- [ ] Test 1: Basic Undo/Redo - PASS
- [ ] Test 2: History Panel UI - PASS
- [ ] Test 3: Bookmarks System - PASS
- [ ] Test 4: Keyboard Shortcuts - PASS
- [ ] Test 5: History Persistence - PASS
- [ ] Test 6: Smart Compression - PASS
- [ ] Test 7: Edge Cases - PASS

**Overall Status**: ☐ PASS / ☐ FAIL

---

## 📝 Tester Sign-Off

**Tested By**: ___________________________  
**Date**: ___________________________  
**Build Version**: ___________________________  

**Comments**:
```
(Add any additional observations or suggestions here)
```

---

## 🚀 If All Tests Pass

**Ready to merge!** 🎉

Next steps:
1. Merge `feature/phase2-week8-presets-history` → `development`
2. Update PROJECT_PLAN.md to mark Week 8 complete
3. Begin Week 9 planning

---

**Testing Guide Version**: 1.0  
**Week 8 Features**: Bookmarks (existing) + Navigation History (new)
