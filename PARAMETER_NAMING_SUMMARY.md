# Parameter Naming Convention - Quick Summary

**Last Updated:** 2026-05-02  
**Full Documentation:** `docs/PARAMETER_NAMING_CONVENTIONS.md`

---

## ✅ **Your Assumption is Correct!**

Yes, we **DO use Paul DeLeeuw's parameter names** from ManpWIN64 wherever applicable.

---

## 🎯 **Quick Reference**

### **Standard Parameter Names (from `ManpWIN64/fractalp.cpp`)**

| Paul's Name | Display Text | Usage |
|-------------|--------------|-------|
| `realz0` | "Real Perturbation of Z(0)" | Initial z real part |
| `imagz0` | "Imaginary Perturbation of Z(0)" | Initial z imaginary part |
| `realparm` | "Real Part of Parameter" | Generic real parameter |
| `imagparm` | "Imaginary Part of Parameter" | Generic imaginary parameter |
| `newtdegree` | "Polynomial Degree (>= 2) 5 is a diamond" | Newton polynomial degree |
| `lambda` | "Lambda" | Lambda parameter |
| `alpha` | "Alpha" | Alpha parameter |
| `beta` | "Beta" | Beta parameter |
| `gamma2` | "Gamma" | Gamma parameter |
| `omega` | "Omega" | Omega parameter |

### **View Parameters (Our Modern Convention)**

| Our Name | Display Text | Notes |
|----------|--------------|-------|
| `centerX` | "Center X" | No direct Paul equivalent (derived from `hor`) |
| `centerY` | "Center Y" | No direct Paul equivalent (derived from `vert`) |
| `zoom` | "Zoom Level" | No direct Paul equivalent (derived from `width`) |

---

## 📝 **Example: Lambda Fractal**

**From Paul's code** (`ManpWIN64/fractalp.cpp` line 273-278):
```cpp
t_lambda+1, realz0, imagz0, ES, ES, ..., 0, 0, ...
```

**Our parameter names** (Task 8):
```cpp
spec.parameters = {
    {"realz0", "Real Perturbation of Z(0)", ..., "0.0"},  // ✅ Paul's name
    {"imagz0", "Imaginary Perturbation of Z(0)", ..., "0.0"},  // ✅ Paul's name
    {"centerX", "Center X", ..., "1.0"},  // ⚠️ Our modern convention
    {"centerY", "Center Y", ..., "0.0"},  // ⚠️ Our modern convention
    {"zoom", "Zoom Level", ..., "0.375"}  // ⚠️ Our modern convention
};
```

---

## 🔑 **Key Principles**

1. **Use Paul's names for fractal-specific parameters** (`realz0`, `imagz0`, `realparm`, `imagparm`, etc.)
2. **Use Paul's display text for UI labels** (exact strings from `fractalp.cpp` lines 28-33)
3. **Match Paul's case exactly** (lowercase: `realz0`; uppercase: `A`, `B`, `C`)
4. **Use our modern convention for view parameters** (`centerX`, `centerY`, `zoom`)

---

## 📊 **Why This Matters**

✅ **Easier maintenance** - parameter names cross-reference between codebases  
✅ **User familiarity** - users migrating from ManpWIN64 see familiar names  
✅ **Algorithm porting** - parameter mappings are obvious  
✅ **Code traceability** - can reference exact line numbers in Paul's code  

---

## 🚀 **Task 8 Integration**

When implementing native parameter metadata (Task 8):
- ✅ Check `ManpWIN64/fractalp.cpp` first for parameter names
- ✅ Use Paul's names and display text exactly
- ✅ Cross-reference line numbers in comments
- ✅ Fall back to our convention only when Paul has no equivalent

**See:** `docs/TASK_8_IMPLEMENTATION_GUIDE.md` (updated with Paul's names)

---

**Your assumption was spot-on! Great instinct for code maintainability.** 👍
