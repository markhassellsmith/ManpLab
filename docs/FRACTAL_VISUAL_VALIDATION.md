# Fractal Visual Validation Checklist

**Purpose:** Systematic verification that all registered fractals display correctly with their default parameters.

**Status:** Deferred until after Task 8 (Native Parameter Metadata Integration)

---

## 🎯 **Validation Criteria**

For each fractal, verify:
1. ✅ **Renders without error**
2. ✅ **Shows visible structure** (not all black/white)
3. ✅ **Default view shows interesting features**
4. ✅ **Escape percentage is reasonable** (typically 40-90% for escape-time fractals)
5. ✅ **Matches expected appearance** (compare to Fractint/ManpWIN64 if possible)

---

## 📋 **Fractal Test Matrix**

### **Classic Fractals**
| Fractal | Renders | Visible | Interesting | Escape % | Notes |
|---------|---------|---------|-------------|----------|-------|
| **Mandelbrot** | ✅ | ✅ | ✅ | ~60% | **WORKING** |
| **Julia** | ✅ | ✅ | ✅ | ~70% | **WORKING** (with good c values) |
| **Lambda** | ✅ | ❌ | ❌ | <1% | **BLACK SCREEN** - needs view/bailout tuning |
| **Mandel-Lambda** | ✅ | ✅ | ✅ | ~50% | **WORKING** |
| **Tetrate** | ✅ | ✅ | ✅ | ~55% | **WORKING** (55 sec render) |
| **Unity** | ✅ | ✅ | ✅ | ~65% | **WORKING** |

### **Mandelbrot Variants**
| Fractal | Renders | Visible | Interesting | Escape % | Notes |
|---------|---------|---------|-------------|----------|-------|
| **Tricorn** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Burning Ship** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Manowar** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Spider** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Marks Mandel** | ⏳ | ⏳ | ⏳ | ⏳ | Placeholder - needs implementation |

### **Multibrot Family**
| Fractal | Renders | Visible | Interesting | Escape % | Notes |
|---------|---------|---------|-------------|----------|-------|
| **Multibrot (z^3)** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Multibrot (z^4)** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Multibrot (z^5)** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |

### **Other Families**
| Fractal | Renders | Visible | Interesting | Escape % | Notes |
|---------|---------|---------|-------------|----------|-------|
| **Barnsley** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Phoenix** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Newton** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Magnet** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Sierpinski** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |
| **Mandelbar** | ⏳ | ⏳ | ⏳ | ⏳ | Not yet tested |

---

## 🔍 **Known Issues**

### **Lambda Fractal (Black Screen)**

**Symptom:** Renders all black, escape % < 1%

**Attempted Fixes:**
1. ✅ Changed bailout from 256.0 → 4.0
2. ✅ Changed center from (0.0, 0.0) → (1.0, 0.0)
3. ✅ Changed zoom from 0.8 → 0.375 (viewWidth = 8.0)

**Reference (from ManpWIN64/fractalp.cpp line 274):**
```cpp
// Original Fractint defaults for LAMBDA:
// xmin = -3.0, xmax = 5.0, ymin = -3.0, ymax = 3.0
// This gives: center = (1.0, 0.0), viewWidth = 8.0
```

**Status:** Still shows black screen after fixes. Needs deeper investigation:
- Verify iteration formula: `z = λ * z * (1 - z)`
- Check if view window calculation is correct
- Compare with ManpWIN64 Lambda rendering
- May need different default maxIterations or palette

**Deferred until:** After Task 8 completion (systematic fractal validation)

---

## 🧪 **Testing Process**

### **Phase 1: Quick Smoke Test (After Task 8)**
1. Click each fractal in browser
2. Note if it renders or crashes
3. Note if output is all black/white
4. Record escape percentage from status bar

### **Phase 2: Visual Comparison (After Phase 1)**
For fractals with issues:
1. Run same fractal in ManpWIN64 with same parameters
2. Compare visual output
3. Check iteration counts and escape percentages
4. Identify parameter mismatches

### **Phase 3: Systematic Fixes (After Phase 2)**
For each broken fractal:
1. Find original Fractint/ManpWIN64 definition
2. Verify iteration formula
3. Verify default view window (xmin/xmax/ymin/ymax)
4. Verify bailout radius
5. Test and document fix

---

## 📅 **Timeline**

- **Now:** Document Lambda issue, defer validation
- **After Task 8:** Run Phase 1 smoke test on all fractals
- **Week 9+:** Systematic fractal validation and fixes

---

## 🎯 **Success Criteria**

- [ ] All registered fractals render without errors
- [ ] All fractals show visible structure (not all black/white)
- [ ] Default views match Fractint/ManpWIN64 where applicable
- [ ] Escape percentages are reasonable for fractal type
- [ ] Julia modes work for fractals that support them

---

## 📝 **Notes**

- Lambda fractal may have unique requirements (different bailout, different center)
- Some fractals may need palette adjustments to show structure
- Trigonometric fractals may need special handling
- 3D attractors are out of scope for 2D escape-time validation
