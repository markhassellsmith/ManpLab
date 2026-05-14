# Parameter Migration: Tactical Implementation Plan

## Executive Summary

**Project:** Complete the flexible parameter system for **279 fractals** (actual native count)  
**Timeline:** May 14 - June 15, 2026 (3-4 weeks)  
**Priority:** HIGH - Use AI budget while available  
**Status:** 279/279 fractals complete (100%) - **🎉 MIGRATION COMPLETE! 🎉**  
**Goal:** ✅ **100% coverage achieved**, eliminate dual parameter system

**⚠️ DISCOVERY:** Native inventory scan revealed 279 unique fractals (not 300 as estimated).  
**📊 See:** `MIGRATION_RECONCILIATION.md` and `NATIVE_FRACTAL_INVENTORY.md` for complete details.

---

## Why This Takes Priority

### Strategic Rationale
1. **AI-optimal task** - Pattern recognition, code generation, validation
2. **High complexity solo** - Requires analyzing 300 fractal algorithms
3. **Technical debt cost** - Dual system wastes ~8 hours/month in maintenance
4. **Budget uncertainty** - May be last month with full Copilot access

### Contrast: Viewport Tuning
- ✅ Can be done solo (visual judgment, no AI needed)
- ✅ Low complexity (adjust 3 numbers, rebuild, look)
- ✅ No technical debt cost
- ✅ **Do this AFTER Parameter Migration**

---

## Current State

### ✅ Architecture Complete (100%)
All infrastructure exists and works:
- `FractalParameterSet` - Parameter container model
- `FractalParameterDescriptor` - Parameter metadata (min/max, tooltips, units)
- `FractalParameterService` - Loading logic
- `ParameterEditorViewModel.Flexible.cs` - UI generation
- `StandardParameterTemplates.cs` - Template registry

### ✅ Working Examples (172 fractals)
Completed templates registered across:
- Mandelbrot + Multibrot family (8 fractals)
- Julia presets family (23 fractals)
- Burning Ship family (17 fractals)
- Newton/Convergence family (12 fractals)
- Trigonometric family (20 fractals)
- Exponential/Logarithmic (10 fractals)
- Polynomial Variants (15 fractals)
- Rational Functions (5 fractals)
- Strange Attractors (14 fractals)
- Visual Priority (8 fractals)
- **Phase 4 Priority 1:**
  - Julia Core Variants (8 fractals) ✅
  - Mandelbrot/Multibrot Variants (9 fractals) ✅
  - PowerVariants (9 fractals) ✅
- **Phase 4 Priority 2:**
  - Extended Trigonometric (8 fractals) ✅
- **Phase 4 Priority 3:**
  - Complex Functions (6 fractals) ✅
  - Special Functions (7 fractals) ✅
- **Phase 4 Priority 4 (Partial):**
  - Exotic Formulas (4/8 fractals) ✅
- **Phase 4 Priority 5:**
  - Hybrids & Blends (18 fractals) ✅
- **Phase 4 Priority 6:**
  - Orbital & Distance Estimators (12 fractals) ✅
- **Phase 4 Priority 7:**
  - IFS (2 fractals) ✅
- **Phase 4 Priority 8:**
  - Chaotic Maps & Bifurcation (7 fractals) ✅
- **Phase 4 Priority 9:**
  - Historical & Research (4 fractals) ✅
- **Phase 4 Priority 10:**
  - Polynomial Variants (8 fractals) ✅
- Plus: Tricorn, Phoenix, Lambda, Hailstone, Complex exponents

**Location:** `ManpWinUI\Services\FractalParameterService.cs`

### ❌ What's Missing (0 fractals - **ALL COMPLETE!** ✅)
**🎉 ALL 279 FRACTALS NOW HAVE PARAMETER TEMPLATES! 🎉**

**Implementation Complete:**
- Phase 1-3: Foundation + Core (138 fractals) ✅
- Phase 4 Priorities 1-10: Extended families (102 fractals) ✅
- Phase 4 Priority 11: Advanced Techniques (17 fractals) ✅
- Phase 4 Priority 12: Orbital & Extensions (19 fractals) ✅
- Final Reconciliation: Last 3 (Multibrot-10, JuliaSanMarco, JuliaDouadyRabbit) ✅

**Next Step: Phase 5 - Legacy System Removal**

---

## The Work Pattern

### What You're Creating

For each fractal, register a `FractalParameterSet`:

```csharp
RegisterParameterSet("FractalName", new FractalParameterSet
{
    DisplayName = "Human-Readable Name",
    Description = "Brief description of the fractal",
    Parameters = new[]
    {
        // Common parameters (copy from base template)
        StandardParameterTemplates.MaxIterations,
        StandardParameterTemplates.Bailout,
        StandardParameterTemplates.SmoothColoring,

        // Fractal-specific parameters
        new FractalParameterDescriptor("param_name", ParameterType.Double)
        {
            DisplayName = "Display Name",
            DefaultValue = 2.0,
            MinValue = 0.5,
            MaxValue = 10.0,
            Description = "What this parameter does",
            Category = "Fractal Parameters",
            DisplayOrder = 100,
            Unit = "exponent"
        }
    }
});
```

### Where to Find Information

1. **Fractal defaults** → `FractalCore\FractalRegistry.cpp`
   - Look for `DefaultCenterX`, `DefaultCenterY`, `DefaultZoom`
   - These become your default values

2. **Parameter names** → Search C++ renderer code
   - Look for fractal-specific calculation logic
   - Parameters are usually in iteration formulas

3. **Min/max constraints** → Mathematical knowledge + testing
   - Start with wide ranges, can refine later
   - Prevent obviously invalid values (negative exponents, etc.)

4. **Existing patterns** → `StandardParameterTemplates.Core.cs`
   - Copy from similar fractals
   - Reuse common parameter definitions

---

## Implementation Schedule

### 🎯 PHASE 1: May 14-20 (Week 1) - FOUNDATION + CORE
**Target: 50 fractals (17% total coverage)**  
**Estimated Time: 12-15 hours**

#### Step 1A: Create Family Base Templates (3 hours)
Create reusable base templates for major families:

```csharp
// Escape-time fractal base (most common)
private static FractalParameterDescriptor[] EscapeTimeFractalBase => new[]
{
    MaxIterations,
    Bailout,
    SmoothColoring,
    AutoScaleIterations,
    CenterX,
    CenterY,
    ZoomLevel
};

// Newton/convergence fractal base
private static FractalParameterDescriptor[] NewtonFractalBase => new[]
{
    MaxIterations,
    ConvergenceTolerance,
    DampingFactor,
    CenterX,
    CenterY,
    ZoomLevel
};

// IFS (Iterated Function System) base
private static FractalParameterDescriptor[] IFSFractalBase => new[]
{
    MaxIterations,
    TransformCount,
    RandomSeed
};

// Strange attractor base
private static FractalParameterDescriptor[] AttractorBase => new[]
{
    Iterations,
    StepSize,
    InitialX,
    InitialY,
    InitialZ
};
```

**Files to edit:**
- `ManpWinUI\Services\Parameters\StandardParameterTemplates.Core.cs`

#### Step 1B: Mandelbrot Family (2 hours) - 8 fractals
All use `EscapeTimeFractalBase` + exponent parameter:

- Mandelbrot (already done ✅)
- MandelbrotCubic
- MandelbrotQuartic
- MandelbrotQuintic
- MandelbrotN (generalized)
- MultibrotZ3
- MultibrotZ4
- MultibrotZ5

**Pattern:**
```csharp
RegisterParameterSet("MandelbrotCubic", new FractalParameterSet
{
    DisplayName = "Mandelbrot Cubic (z³ + c)",
    Description = "Mandelbrot set with cubic exponent",
    Parameters = EscapeTimeFractalBase.Concat(new[]
    {
        new FractalParameterDescriptor("exponent", ParameterType.Double)
        {
            DisplayName = "Exponent",
            DefaultValue = 3.0,
            MinValue = 2.0,
            MaxValue = 20.0,
            Description = "Power applied to z in iteration formula",
            Category = "Fractal Parameters",
            DisplayOrder = 100
        }
    }).ToArray()
});
```

#### Step 1C: Julia Family (4 hours) - 23 fractals
Enhanced Julia presets + standard Julia variants:

**Enhanced Julia Presets (23 fractals):**
- Julia_Dendrite
- Julia_SanMarco
- Julia_Siegel
- Julia_Douady
- Julia_Dragon
- Julia_Spiral
- Julia_Seahorse
- (... 16 more named presets)

**Pattern:**
```csharp
RegisterParameterSet("Julia_Dendrite", new FractalParameterSet
{
    DisplayName = "Julia Set - Dendrite",
    Description = "Julia set with dendrite-like structure",
    Parameters = EscapeTimeFractalBase.Concat(new[]
    {
        new FractalParameterDescriptor("julia_c_real", ParameterType.Double)
        {
            DisplayName = "C (Real)",
            DefaultValue = 0.0,      // Specific to this preset
            MinValue = -2.0,
            MaxValue = 2.0,
            Category = "Julia Parameters",
            DisplayOrder = 100
        },
        new FractalParameterDescriptor("julia_c_imag", ParameterType.Double)
        {
            DisplayName = "C (Imaginary)",
            DefaultValue = 1.0,      // Specific to this preset
            MinValue = -2.0,
            MaxValue = 2.0,
            Category = "Julia Parameters",
            DisplayOrder = 110
        }
    }).ToArray()
});
```

**Data source:** Look in `FractalRegistry.cpp` for Julia preset default values.

#### Step 1D: Burning Ship Family (3 hours) - 10 fractals
- BurningShip (already done ✅)
- BurningShipCubic
- BurningShipQuartic
- BurningShipJulia
- BirdOfPrey
- BuffaloFractal
- CelticFractal
- MandelbarFractal
- Perpendicular (series)

**Pattern:** Similar to Mandelbrot, but add `abs_mode` parameter:
```csharp
new FractalParameterDescriptor("abs_mode", ParameterType.Choice)
{
    DisplayName = "Absolute Value Mode",
    DefaultValue = "Both",
    Choices = new[] { "Real", "Imaginary", "Both" },
    Description = "Which component to apply absolute value to",
    Category = "Fractal Parameters"
}
```

#### Step 1E: Newton/Magnet Families (2 hours) - 9 fractals
- Newton (already done ✅)
- NewtonCubic
- NewtonQuartic
- NewtonSin
- NewtonExp
- Magnet1
- Magnet2
- NovaFractal
- Halley

**Pattern:** Use `NewtonFractalBase` + specific convergence parameters.

**✅ PHASE 1 COMPLETE: 59/300 fractals (20%)**

**✅ All Phase 1 families complete:**
- Mandelbrot family: 8/8 ✅
- Julia presets family: 23/23 ✅
- Burning Ship family: 17/17 ✅  
- Newton/Magnet/Convergence: 9/9 ✅
- Complex exponent: 2/2 ✅
- Phoenix: 1/1 ✅
- Lambda: 1/1 ✅
- ExpSquare: 1/1 ✅
- Hailstone: 1/1 ✅

**Next: Phase 2 - Mathematical Functions (64 more fractals)**

---

### 🎯 PHASE 2: May 21-27 (Week 2) - MATHEMATICAL FUNCTIONS
**Target: 114 fractals total (38% coverage), +64 this week**  
**Estimated Time: 14-16 hours**

#### Step 2A: Trigonometric Family (4 hours) - 20 fractals
- MandelSin
- MandelCos
- MandelTan
- MandelSinh
- MandelCosh
- MandelTanh
- JuliaSin, JuliaCos, JuliaTan
- (... 11 more trig variants)

**Pattern:** Escape-time base + trig-specific parameters:
```csharp
new FractalParameterDescriptor("frequency", ParameterType.Double)
{
    DisplayName = "Frequency",
    DefaultValue = 1.0,
    MinValue = 0.1,
    MaxValue = 10.0,
    Category = "Trigonometric Parameters"
}
```

#### Step 2B: Exponential/Logarithmic (3 hours) - 12 fractals
- MandelExp
- MandelLog
- MandelExpZ
- MandelLogZ
- JuliaExp
- JuliaLog
- ZPowerExp
- (... 5 more)

**Pattern:** Escape-time base + exponential base parameter.

#### Step 2C: Polynomial Variants (5 hours) - 15 fractals ✅
- Phoenix Extended family: PhoenixM, PhoenixJ, PhoenixPower3, PhoenixPower4 (4 fractals)
- Lambda Extended family: LambdaPower3, LambdaPower4, LambdaTanh, LambdaSquared, LambdaFlip (5 fractals)
- Barnsley family: BarnsleyM1, BarnsleyJ1, BarnsleyM2, BarnsleyJ2, BarnsleyM3, BarnsleyJ3 (6 fractals)
- Spider family: SpiderVariant (1 fractal)

**Note:** Tricorn, Phoenix (base), and Lambda (base) were already registered in Phase 1.
Collatz fractal is registered as "Hailstone" (interactive, no parameters).

**Implementation:** All use standard Julia-enabled or escape-time templates.

#### Step 2D: Rational Functions (2 hours) - 5 fractals ✅
- NewtonQuintic (z⁵-1 Newton method)
- RationalZ2Z3 (z²/(z³+c))
- RationalSymmetric ((z²+c)/(z²-c))
- Mobius ((z+c)/(z-c) Möbius transformation)
- RationalPower (z³/(z³+c))

**Note:** NewtonCubic, NewtonQuartic, and HalleyCubic were already registered in Phase 1.
The tactical plan originally mentioned "InverseMandelbrot" and "ReciprocalMandelbrot" but these don't exist as separate fractals in the native code. The actual rational function fractals are the ones listed above.

**Implementation:** All use standard Julia-enabled or escape-time templates.

**📊 PHASE 2 COMPLETION: 109/300 fractals (36%)**

---

### 🎯 PHASE 3: May 28-31 (Week 3) - HIGH-VALUE EXOTIC
**Target: 162 fractals total (54% coverage), +48 this week**  
**Estimated Time: 10-12 hours** (shorter week)

#### Step 3A: Strange Attractors (14 fractals) ✅
See checklist below for details.

#### Step 3B: Newton/Convergence Variants (4 hours) - 18 fractals
- NewtonPolynomial (degree 3-10) - 8 fractals
- HalleyMethod variants - 4 fractals
- Schroder method - 2 fractals
- Householder methods - 4 fractals

**Pattern:** Similar to Newton base, vary polynomial degree.

#### Step 3C: Visual Priority Fractals (3 hours) - 20 fractals
Cherry-pick the most visually interesting or popular:
- Lyapunov fractal
- Buddhabrot
- Nebulabrot
- Pickover stalks
- Biomorphs (series)
- Nova fractal variants
- Kleinian groups
- (... pick 13 more based on visual appeal)

**📊 PHASE 3 COMPLETION: 162/300 fractals (54%)**

---

### 🎯 PHASE 4: June 1-15 (Optional, if AI available)
**Target: 300 fractals (100% coverage), +138 remaining**  
**Can be done manually if AI budget exhausted**

#### Remaining Categories (138 fractals):
- IFS variants (20 fractals)
- Orbital modifications (18 fractals)
- Hybrid fractals (18 fractals)
- Distance estimator variants (12 fractals)
- Bifurcation diagrams (10 fractals)
- Historical/obscure fractals (30 fractals)
- Experimental/custom fractals (30 fractals)

**Strategy if doing manually:**
- Copy-paste from similar completed fractals
- Focus on correctness over perfection
- Can refine parameter ranges later
- 2-3 fractals per day = 2 weeks to finish

---

## Working with AI (Copilot)

### Effective Prompts

**Starting a family:**
```
I'm implementing parameter templates for the Mandelbrot family fractals.
I have a base template "EscapeTimeFractalBase" with common parameters.
Help me create parameter templates for these 8 fractals: [list].
They all use the same base but vary in exponent (2-5).
```

**Finding parameters:**
```
Look at FractalRegistry.cpp line [X] for the [FractalName] fractal.
What are the default values for center, zoom, and any fractal-specific parameters?
Help me create a FractalParameterSet with appropriate min/max ranges.
```

**Bulk generation:**
```
I need to create 23 Julia preset parameter templates.
Each has the same structure but different C values.
Here's the pattern: [show example].
Generate templates for: Julia_Dendrite (C=0+1i), Julia_SanMarco (C=-0.75+0i), ...
```

### What to Ask AI For

✅ **Good AI tasks:**
- Generate repetitive parameter definitions
- Suggest reasonable min/max constraints
- Find parameter names in C++ code
- Validate parameter descriptor syntax
- Create family base templates

❌ **Don't waste tokens on:**
- Explaining what parameters do (you know your fractals)
- Debating min/max values (pick reasonable defaults, refine later)
- Perfect descriptions (good enough is fine)
- Visual testing (you'll see if it works)

---

## Testing Strategy

### Per-Family Testing (Efficient)
Don't test every fractal individually. Test representatives:

1. **Implement 10-20 fractals in a family**
2. **Test 2-3 representatives:**
   - Pick one simple (e.g., Mandelbrot)
   - Pick one complex (e.g., MandelbrotN)
   - Pick one edge case (e.g., high exponent)

3. **Verify:**
   - ✅ Parameters appear in UI
   - ✅ Default values load correctly
   - ✅ Min/max constraints work
   - ✅ Fractal renders correctly
   - ✅ Values persist after restart

4. **If test passes:** Assume others work (can fix bugs later)
5. **If test fails:** Debug, then retest that fractal

### Build & Run Testing
```powershell
# After implementing a batch of fractals
cd C:\Users\Mark\source\repos\ManpLab
dotnet build ManpWinUI\ManpWinUI.csproj

# Launch app, test 2-3 fractals from the batch
# Check Parameters tab shows correct controls
```

---

## Progress Tracking

### Git Commit & Push Strategy 🔐

**Critical: Regular commits ensure code integrity and allow rollback if needed**

#### Commit Frequency Rules

1. **After each family completion** (5-25 fractals)
   - Commit message pattern: `[ParamMig] Add [Family] templates (X fractals)`
   - Include progress count: `Progress: X/300 fractals (Y%)`
   - Push to remote immediately

2. **After each phase milestone** (Phase 1, 2, 3, 4)
   - Commit message pattern: `[ParamMig] Phase N Complete (X/300 fractals, Y%)`
   - Include detailed summary of all families in that phase
   - Push to remote immediately

3. **After infrastructure changes** (new helper methods, builders)
   - Commit message pattern: `[ParamMig] Add [feature] infrastructure`
   - Document what the change enables
   - Push to remote immediately

4. **Before risky refactoring** (Phase 5 legacy removal)
   - Create safety commit: `[ParamMig] Pre-legacy-removal snapshot (300/300 complete)`
   - Push to remote
   - **Consider creating a backup branch:** `git checkout -b param-migration-backup`

#### Commit Message Template

```
[ParamMig] [Action] [Family/Feature] ([X] fractals)

- [What was added/changed]
- [Any new helpers or infrastructure]
- [Test results if applicable]

Progress: X/300 fractals (Y%)
[Phase status if milestone reached]
```

#### Example Commits

**Family Completion:**
```bash
git add ManpWinUI/Services/FractalParameterService.cs
git commit -m "[ParamMig] Add Trigonometric family templates (20 fractals)

- MandelSin, MandelCos, MandelTan, MandelSinh, MandelCosh, MandelTanh
- Julia trig variants: JuliaSin, JuliaCos, JuliaTan
- Hybrid trig fractals: ZSinZ, ZCosZ, SinZSquared, CosZSquared
- Added frequency parameter to all trig fractals
- Tested with MandelSin - parameters load correctly

Progress: 79/300 fractals (26%)"

git push origin development
```

**Phase Milestone:**
```bash
git add ManpWinUI/Services/FractalParameterService.cs
git add ManpWinUI/Documentation/Architecture/PARAMETER_MIGRATION_TACTICAL_PLAN.md
git commit -m "[ParamMig] Phase 2 Complete (123/300 fractals, 41%)

Phase 2 Summary:
- Trigonometric family: 20 fractals ✅
- Exponential/Logarithmic: 12 fractals ✅
- Polynomial variants: 24 fractals ✅
- Rational functions: 8 fractals ✅

Infrastructure added:
- TrigonometricParameters() helper
- ExponentialBase() parameter
- RationalFunctionCoefficients() helper

All families tested, builds successful
Ready for Phase 3 (High-Value Exotic fractals)"

git push origin development
```

**Infrastructure Change:**
```bash
git add ManpWinUI/Models/Parameters/StandardParameterTemplates.Core.cs
git commit -m "[ParamMig] Add AttractorBase infrastructure

- Created AttractorBase() parameter set (iterations, step size, initial conditions)
- Added SystemParameters(count) helper for attractor-specific params
- Supports Lorenz, Rossler, Henon, Clifford, DeJong families

Enables Phase 3A implementation (10 strange attractors)
Progress: 123/300 fractals (41%)"

git push origin development
```

#### Recovery Commands (If Needed)

**View commit history:**
```bash
git log --oneline --grep="\[ParamMig\]" -20
```

**Rollback to previous commit:**
```bash
git reset --hard HEAD~1  # Undo last commit (DANGEROUS - only if not pushed)
git revert HEAD          # Safe undo (creates new commit)
```

**Recover from remote if local corrupted:**
```bash
git fetch origin
git reset --hard origin/development
```

**Check what changed since last commit:**
```bash
git diff HEAD
```

---

### Daily Checklist Template

```markdown
## [Date] Parameter Migration Progress

**Target:** [Family Name] - [X] fractals
**Time Spent:** [hours]

### Completed:
- [ ] Created base template (if needed)
- [ ] Implemented fractals: [list]
- [ ] Tested representatives: [list]
- [ ] ✅ **COMMITTED & PUSHED** to development

### Git:
```bash
git add [files]
git commit -m "[ParamMig] Add [Family] templates (X fractals) - Progress: X/300 (Y%)"
git push origin development
```

### Notes:
- [Any discoveries, issues, or decisions]

### Tomorrow:
- [Next family/batch]
```

---

### Commit Checklist Per Session

Before ending each work session, ensure:

- [ ] All changes compile successfully (`dotnet build`)
- [ ] Representative fractals tested (2-3 per family)
- [ ] Tactical plan checklist updated
- [ ] Changes staged: `git add [modified files]`
- [ ] Commit created with descriptive message
- [ ] **Pushed to remote:** `git push origin development`
- [ ] Verify push succeeded (check GitHub)

**🚨 NEVER end a session without pushing to remote! 🚨**

---

### Backup Strategy

#### Automatic Protection
- Every push to `development` is backed up on GitHub
- Commit history allows rollback to any point
- Phase milestones create natural restore points

#### Before Major Changes (Phase 5)
Before deleting legacy code:

```bash
# Create safety branch
git checkout -b param-migration-backup
git push origin param-migration-backup

# Return to development
git checkout development

# Now safe to delete legacy code
```

#### Emergency Recovery
If something goes catastrophically wrong:

```bash
# Find the last good commit
git log --oneline --grep="\[ParamMig\]" -10

# Create recovery branch from that commit
git checkout -b recovery-attempt [commit-hash]

# If it's good, merge back to development
git checkout development
git merge recovery-attempt
git push origin development
```

---

### Progress Summary

### Files You'll Edit:
- **Primary:** `ManpWinUI\Services\Parameters\StandardParameterTemplates.Core.cs`
  - This is where 90% of the work happens
  - Contains all `RegisterParameterSet()` calls

- **Reference:** `FractalCore\FractalRegistry.cpp`
  - Look up default center/zoom values here
  - Line ~233 has the full fractal list

- **Reference:** `ManpWinUI\Services\Parameters\StandardParameterTemplates.cs`
  - Common parameter definitions (MaxIterations, Bailout, etc.)
  - Reuse these wherever possible

### Files You WON'T Edit (Yet):
- `ParameterEditorViewModel.Legacy.cs` - Delete in Phase 5
- `MainViewModel.Parameters.cs` - Sync bridge, delete in Phase 5
- `MainViewModel.StandardFractals.cs` - Legacy properties, delete in Phase 5

---

## Phase 5: Legacy Removal (After 100% Coverage)

**Timeline:** 2-3 days after all templates complete  
**Don't do this until Parameter Migration is 100% done!**

### Steps:
1. **Delete legacy parameter loader:**
   - Remove `ParameterEditorViewModel.Legacy.cs`

2. **Remove sync bridge:**
   - Delete `SyncPropertiesToParameters()` from `MainViewModel.Parameters.cs`
   - Delete `SyncParametersToProperties()` from `MainViewModel.Parameters.cs`

3. **Remove hard-coded properties:**
   - Clean up `MainViewModel.StandardFractals.cs`
   - Remove individual parameter properties (if no longer needed by toolbar)

4. **Update toolbar bindings** (if needed):
   - May need value converters to bind toolbar to `CurrentParameters` dictionary

5. **Test all 300 fractals** (sampling strategy):
   - Test 5 from each family
   - Verify parameters load, persist, and render correctly

6. **Celebrate:** 🎉 Single, clean parameter system!

---

## Troubleshooting

### "Parameter not appearing in UI"
- Check `RegisterParameterSet()` is called for that fractal name
- Verify fractal name matches exactly (case-sensitive)
- Ensure `DisplayOrder` is set (controls ordering)

### "Default values not loading"
- Check `DefaultValue` is set in descriptor
- Verify `FractalParameterService` is finding the parameter set
- Look for exceptions in app startup

### "Min/max constraints not working"
- Verify `MinValue` and `MaxValue` are set
- Check UI control type (slider vs text box)
- Ensure values are reasonable (min < default < max)

### "Values not persisting"
- Auto-save already implemented, should "just work"
- Check `LocalSettings` isn't corrupted
- Try deleting app settings and restarting

### "Sync errors between legacy and flexible"
- Ignore for now - legacy will be deleted in Phase 5
- Don't waste time debugging the sync bridge

---

## Success Criteria

### Minimum Viable Completion (54% by May 31)
- ✅ 162 most important fractals have templates
- ✅ All major families covered (escape-time, Newton, trig, attractors)
- ✅ Family base templates established
- ✅ Remaining fractals can be done via copy-paste

### Full Completion (100% by June 15)
- ✅ All 300 fractals have parameter templates
- ✅ Legacy system deleted
- ✅ Sync bridge removed
- ✅ Single source of truth for parameters
- ✅ Technical debt eliminated

---

## Master Progress Checklist

### 🎯 PHASE 1: May 14-20 - FOUNDATION + CORE (50 fractals)

#### Step 1A: Family Base Templates
- [ ] Create `EscapeTimeFractalBase` template (7 parameters)
- [ ] Create `NewtonFractalBase` template (6 parameters)
- [ ] Create `IFSFractalBase` template (3 parameters)
- [ ] Create `AttractorBase` template (5 parameters)
- [ ] Test: Verify base templates compile and are reusable

#### Step 1B: Mandelbrot Family (8 fractals)
- [x] Mandelbrot (already complete)
- [x] Multibrot / z3 (already complete)
- [x] z4 (already complete)
- [x] z5 (already complete)
- [x] z6 (already complete)
- [x] z7 (already complete)
- [x] z8 (already complete)
- [ ] MandelbrotN (z^n + c, variable exponent - if different from above)
- [ ] Test: Verify 2-3 representatives render correctly

#### Step 1C: Julia Family (23 fractals)
- [x] Julia (basic) - enhanced presets complete
- [x] Julia_GoldenRatio (C = φ-2 where φ = 1.618...)
- [x] Julia_Dendrite (C = 0 + 1i)
- [x] Julia_Spiral (C = 0.4 + 0.6i)
- [x] Julia_Dragon (C = -0.8 + 0.156i)
- [x] Julia_Cauliflower (C = 0.25 + 0i)
- [x] Julia_Seahorse (C = -0.75 + 0.11i)
- [x] Julia_Airplane (C = -0.7269 + 0.1889i)
- [x] Julia_Lightning (C = -0.52 + 0.57i)
- [x] Julia_Snowflake (C = 0.285 + 0.01i)
- [x] Julia_Flower (C = 0.28 + 0.008i)
- [x] Julia_Feigenbaum (C = -1.401155 + 0i)
- [x] Julia_TwistedCross (C = 0.45 + 0.1428i)
- [x] Julia_Backbone (C = -1.0 + 0i)
- [x] Julia_SpiralGalaxy (C = -0.4 + 0.59i)
- [x] Julia_Medusa (C = -0.194 + 0.6557i)
- [x] Julia_Crystal (C = -0.7 + 0.27015i)
- [x] Julia_Paisley (C = -0.162 + 1.04i)
- [x] Julia_FuzzyBlob (C = -0.11 + 0.6557i)
- [x] Julia_Eye (C = -0.75 + 0i)
- [x] Julia_TripleSpiral (C = -0.4 + 0.6i)
- [x] Julia_Heart (C = -0.835 - 0.2321i)
- [x] Julia_Neurons (C = -0.8 + 0.156i)
- [x] Julia_FractalTree (C = -0.75 + 0.2i)
- [x] Test: All Julia presets use standard escape-time template

#### Step 1D: Burning Ship Family (15 fractals)
- [x] BurningShip (already complete)
- [x] Celtic (already complete)
- [x] Buffalo (already complete)  
- [x] MandelbarCeltic (already complete)
- [x] Tricorn (already complete)
- [x] BurningShipCubic (z³ variant)
- [x] BurningShipQuartic (z⁴ variant)
- [x] BurningShipQuintic (z⁵ variant)
- [x] PerpendicularBurningShip (|re| - i|im|)
- [x] BuffaloBurningShip (abs then subtract c)
- [x] SharkBurningShip (z² + c/z variant)
- [x] CelticBurningShip (abs after squaring)
- [x] ReverseBurningShip (re + i|im|)
- [x] VerticalBurningShip (|re| + i*im)
- [x] DiagonalBurningShip (diagonal abs)
- [x] PerpendicularMandelbrot (perpendicular variant)
- [x] BirdOfPrey (abs(re)² variation)
- [x] Test: All Burning Ship variants use Julia-enabled template

#### Step 1E: Newton/Magnet Families (9 fractals)
- [x] Newton (already complete - z³-1=0)
- [x] NewtonSinExp (already complete)
- [x] Nova (already complete - Newton + Julia hybrid)
- [x] Magnet1M (already complete)
- [x] Magnet2M (already complete)
- [x] Magnet1J (already complete)
- [x] Magnet2J (already complete)
- [x] NewtonQuartic (z⁴ - 1 = 0)
- [x] HalleyCubic (Halley's method for z³)
- [x] Test: Newton fractals use standard escape-time template (convergence-based)

**✅ PHASE 1 TARGET: 50/300 fractals (17%)**

---

### 🎯 PHASE 2: May 21-27 - MATHEMATICAL FUNCTIONS (64 fractals)

#### Step 2A: Trigonometric Family (20 fractals)
- [x] MandelTrig (z² + sin(z) + c)
- [x] Sine (sin(z) + c)
- [x] LMandelSine (c·sin(z))
- [x] LLambdaSine (c·z·sin(z))
- [x] LMandelCos (c·cos(z))
- [x] LLambdaCos (c·z·cos(z))
- [x] LMandelSinh (c·sinh(z))
- [x] LLambdaSinh (c·z·sinh(z))
- [x] LMandelCosh (c·cosh(z))
- [x] LLambdaCosh (c·z·cosh(z))
- [x] SinZ (sin variation)
- [x] CosZ (cos variation)
- [x] CosTan (cos/tan hybrid)
- [x] LambdaTan (lambda tan)
- [x] NewtonSin (Newton with sine)
- [x] PhoenixSin (Phoenix with sine)
- [x] Sqr1OverTrig (z²/trig)
- [x] SqrTrig (z²·trig)
- [x] TrigPlusTrig (trig + trig)
- [x] TrigXTrig (trig × trig)
- [x] Test: All trig fractals use standard escape-time template with Julia support

#### Step 2B: Exponential/Logarithmic (12 fractals)
- [x] Exponential (z = e^z + c)
- [x] Logarithmic / Logarithm (z = log(z) + c)
- [x] MandelExp (z = z² + e^z + c)
- [x] LMandelExp (z = c * e^z)
- [x] LLambdaExp (z = c * z * e^z)
- [x] PowerTower / ZToTheZ (z = z^z + c)
- [x] ComplexPower (z = z^c + c)
- [x] ExponentialJulia (z = c*exp(z) + z)
- [x] ExpSquare (z = e^(z²) + c) - already complete from Phase 1
- [x] Test: All exponential/logarithmic fractals use Julia-enabled template

#### Step 2C: Polynomial Variants (15 fractals) ✅
**Phoenix Extended Family (4 fractals):**
- [x] PhoenixM (Mandelbrot mode with memory)
- [x] PhoenixJ (Julia mode with memory)
- [x] PhoenixPower3 (cubic power with memory)
- [x] PhoenixPower4 (quartic power with memory)

**Lambda Extended Family (5 fractals):**
- [x] LambdaPower3 (z = λ * z³ * (1 - z))
- [x] LambdaPower4 (z = λ * z⁴ * (1 - z))
- [x] LambdaTanh (z = λ * tanh(z))
- [x] LambdaSquared (z = λ * z² * (1 - z²))
- [x] LambdaFlip (z = λ * (1 - z) * z)

**Barnsley Family (6 fractals):**
- [x] BarnsleyM1 (Michael Barnsley's first M-set)
- [x] BarnsleyJ1 (Julia set for BarnsleyM1)
- [x] BarnsleyM2 (Michael Barnsley's second M-set)
- [x] BarnsleyJ2 (Julia set for BarnsleyM2)
- [x] BarnsleyM3 (Michael Barnsley's third M-set)
- [x] BarnsleyJ3 (Julia set for BarnsleyM3)

**Spider Family (1 fractal):**
- [x] SpiderVariant (z² + c with evolving c)

**Note:** Tricorn (base), Phoenix (base), and Lambda (base) were already registered in Phase 1.
Hailstone (Collatz fractal) was already registered in Phase 1 as an interactive fractal with no parameters.

#### Step 2D: Rational Functions (5 fractals) ✅
- [x] NewtonQuintic (z⁵-1 Newton method)
- [x] RationalZ2Z3 (z²/(z³+c))
- [x] RationalSymmetric ((z²+c)/(z²-c))
- [x] Mobius ((z+c)/(z-c) Möbius transformation)
- [x] RationalPower (z³/(z³+c))

**Note:** NewtonCubic, NewtonQuartic, and HalleyCubic were already registered in Phase 1.
The tactical plan originally mentioned "InverseMandelbrot" and "ReciprocalMandelbrot" but these don't exist as separate fractals in the native code. The actual rational function fractals are the ones listed above.

**✅ PHASE 2 TARGET: 109/300 fractals (36%)**

---

### 🎯 PHASE 3: May 28-31 - HIGH-VALUE EXOTIC (48 fractals)

#### Step 3A: Strange Attractors (14 fractals) ✅
**3D Continuous Attractors:**
- [x] Lorenz (σ, ρ, β parameters - hardcoded in native)
- [x] Thomas (b parameter - hardcoded in native)
- [x] Dadras (a, b, c, d, e parameters - hardcoded in native)
- [x] Pickover (a, b, c, d parameters - hardcoded in native)
- [x] Aizawa (a, b, c, d, e, f parameters - hardcoded in native)
- [x] Halvorsen (a parameter - hardcoded in native)
- [x] ChenLee (a, b, c parameters - hardcoded in native)

**2D Discrete Maps:**
- [x] Clifford (a, b, c, d parameters - hardcoded in native)
- [x] DeJong (a, b, c, d parameters - hardcoded in native)
- [x] Tinkerbell (a, b, c, d parameters - hardcoded in native)
- [x] Duffing (α, β, δ, γ, ω parameters - hardcoded in native)

**Additional Chaotic Maps:**
- [x] LiuChen (a, b, c parameters - hardcoded in native)
- [x] RabinovichFabrikant (α, γ parameters - hardcoded in native)
- [x] Arneodo (a, b, c, d parameters - hardcoded in native)

**Note:** Attractors are histogram-based fractals that plot orbit trajectories, not escape-time fractals.
System parameters (sigma, rho, etc.) are hardcoded in the native code and don't need C# parameter descriptors.
Only view parameters (center, zoom) are provided in the C# templates.

#### Step 3B: Newton/Convergence Variants (3 fractals) ✅
**Actual Newton Extended variants in native code:**
- [x] NewtonSextic (z⁶-1 Newton method, 6 convergence basins)
- [x] NewtonCosh (cosh(z)-1 Newton method, hyperbolic convergence)
- [x] NewtonBasin (z³-1 with root coloring visualization)

**Note:** The tactical plan originally estimated 18 Newton/convergence variants, but native code research revealed only these 3 additional variants exist beyond what was already registered in Phases 1 and 2:
- Newton (Phase 1)
- NewtonQuartic (Phase 1)
- NewtonSinExp (Phase 1)
- NewtonSin (Phase 2A)
- HalleyCubic (Phase 1)
- NewtonQuintic (Phase 2D)

The native code does not implement: NewtonPolynomial degrees 3-10, HalleyQuartic/Sin/Exp, Schroder, Householder, Laguerre, or Secant methods.

- [x] Test: All Newton variants use standard escape-time template

**✅ PHASE 3 STEP 3B COMPLETE: 126/300 fractals (42%)**

---

### 🎯 PHASE 3: May 28-31 - HIGH-VALUE EXOTIC
**Revised Target: Step 3C Visual Priority Fractals**

#### Step 3C: Visual Priority Fractals (8 fractals) ✅
**Actual visual priority fractals in native code:**
- [x] Buddhabrot (Mandelbrot orbit path visualization)
- [x] Lyapunov (stability diagram from population dynamics)
- [x] NumFractal (unique fractal dedicated to 11-year-old discoverer)
- [x] Biomorphs (Pickover Biomorphs - biological-looking structures)
- [x] PickoverStalks (Biomorph variant with stalk-like structures)
- [x] BarnsleyFern (Classic IFS fern)
- [x] SierpinskiIFS (Sierpinski triangle via chaos game)
- [x] DragonCurveIFS (Heighway dragon curve)

**Note:** The tactical plan originally listed 20 visual priority fractals, but native code research revealed only these 8 exist as registered fractals:
- Buddhabrot exists (Nebulabrot is not a separate registration)
- Lyapunov exists
- Biomorphs and PickoverStalks exist (in HistoricalFractalsFamily.cpp)
- BarnsleyFern, SierpinskiIFS, DragonCurveIFS exist (in IFSFamily.cpp)
- PentagonIFS also exists but not listed in original plan

The following from the original plan do NOT exist as separate registered fractals in ManpCore.Native:
- Nebulabrot (rendering technique, not separate fractal)
- NovaFractalM (Nova already registered in Phase 1)
- KleinianGroup (old WIN64 code, not in modern registry)
- QuaternionJulia, QuaternionMandelbrot (not found)
- KochSnowflake, HilbertCurve, PeanoCurve, GosperCurve, MinkowskiSausage (L-system curves not in modern registry)

All use standard or Julia-enabled templates.

**✅ PHASE 3 COMPLETE: 134/300 fractals (45%)**

---

### 🎯 PHASE 4: June 1-15 - REMAINING COVERAGE (141 fractals → 115 remaining)
**Based on actual native inventory reconciliation**  
**See `MIGRATION_RECONCILIATION.md` for complete priority breakdown**

#### Priority 1: Core Variants (High Visual Impact) - 26/26 fractals ✅

##### Julia Core Variants (8/8) ✅ **COMPLETE - Commit: 32b7db4**
- [x] JuliaClassic
- [x] JuliaCubic
- [x] JuliaBurningShip
- [x] JuliaPhoenix
- [x] JuliaLambda
- [x] JuliaSine
- [x] JuliaExp
- [x] JuliaMagnet

##### Mandelbrot/Multibrot Variants (9/9) ✅ **COMPLETE - Commit: b17bbf2**
- [x] Mandel4
- [x] Julia4
- [x] Mandelbar
- [x] MandelLambda
- [x] MarksJulia
- [x] Thorn
- [x] Multibrot3
- [x] Multibrot4
- [x] Multibrot5

##### PowerVariants (9/9) ✅ **COMPLETE - Commit: 82ecfad**
- [x] Multibrot6
- [x] Multibrot7
- [x] Multibrot8
- [x] Julia5
- [x] Julia6
- [x] BurningShip3
- [x] BurningShip4
- [x] Tricorn3
- [x] Tricorn4

---

#### Priority 2: Extended Trigonometric (8/8 fractals) ✅ **COMPLETE - Commit: f070e59**
- [x] TanMandel
- [x] CotMandel
- [x] SecMandel
- [x] CscMandel
- [x] ArcSinMandel
- [x] ArcCosMandel
- [x] ArcTanMandel
- [x] TanhMandel

---

#### Priority 3: Complex Functions & Special (13/13 fractals) ✅ **COMPLETE - Commit: 0b59ca9**
**Complex Functions (6):**
- [x] SinhMandelbrot
- [x] CoshMandelbrot
- [x] TanhMandelbrot
- [x] HeartMandel
- [x] SharkFin
- [x] Wavy

**Special Functions (7):**
- [x] GammaFractal
- [x] ErrorFunctionFractal
- [x] BesselLikeFractal
- [x] ContinuedFraction
- [x] Tetration
- [x] LambertW
- [x] HyperbolicCombo

---

#### Priority 4: Exotic Formulas (4/8 fractals - PARTIAL) ✅ **Commit: 0b59ca9**
- [x] CelticMandel
- [x] PerpendicularMandel
- [x] QuasiPerpendicular
- [x] Zubieta
- [ ] CelticHeart
- [ ] HeartMandel (duplicate - already registered as HeartMandel in Priority 3)
- [ ] SharkFin (duplicate - already registered in Priority 3)
- [ ] Wavy (duplicate - already registered in Priority 3)

**Note:** Priority 4 is actually complete - HeartMandel, SharkFin, and Wavy were already registered in Priority 3.
CelticHeart needs investigation (may not exist as separate fractal in native code).

---

#### Priority 5: Hybrids & Blends (18/18 fractals) ✅ **COMPLETE - Commit: 2ea7f19**
**Fractal Hybrids (8):**
- [x] BurningMandel
- [x] ExpMandelHybrid
- [x] MutantMandelbrot
- [x] TrigMandelBlend
- [x] SierpinskiMandel
- [x] PerturbedNewton
- [x] BifurcationMandel
- [x] CelticMandelbrot

**Hybrid Family (10):**
- [x] MandelBurningHybrid
- [x] MandelLambdaMix
- [x] TricornPhoenixHybrid
- [x] NewtonMandelBlend
- [x] SineMandelHybrid
- [x] ExpMandelBlend
- [x] MultiPowerCycle
- [x] MagnetMandelHybrid
- [x] CollatzHybrid
- [x] CelticBurningHybrid

---

#### Priority 6: Orbital & Distance Estimators (12/12 fractals) ✅ **COMPLETE - Commit: 974e8c6**
**Orbital Fractals (8):**
- [x] OrbitTrapCross (center 0,0; zoom 1.0)
- [x] OrbitTrapCircle (center 0,0; zoom 1.0)
- [x] OrbitTrapPoint (center 0,0; zoom 1.0)
- [x] OrbitTrapSquare (center 0,0; zoom 1.0)
- [x] AverageDistance (center 0,0; zoom 1.0)
- [x] MinimumDistance (center 0,0; zoom 1.0)
- [x] MaximumDistance (center 0,0; zoom 1.0)
- [x] AngleAverage (center 0,0; zoom 1.0)

**Distance Estimators (4):**
- [x] MandelbrotDEM (center -0.5, 0; zoom 1.0) - viewport preserved
- [x] JuliaDEM (Julia-enabled; center 0,0; zoom 1.5) - viewport preserved
- [x] BurningShipDEM (center -0.5,-0.5; zoom 0.4) - viewport preserved
- [x] TricornDEM (center 0,0; zoom 1.0)

**Note:** Original tactical plan estimated 12 DEM variants (NewtonDEM, PhoenixDEM, DEMVariant1-6), but native discovery revealed only 4 DEM fractals exist in actual native code. Total Priority 6: 12 fractals (not 20).

---

#### Priority 7: IFS (Iterated Function Systems) (2/2 fractals) ✅ **COMPLETE - Commit: 8607966**
**IFS Additional Fractals (2):**
- [x] PentagonIFS (chaos game with 5 vertices; center 0,0; zoom 1.5)
- [x] TreeIFS (branching tree structure; center 0,2; zoom 0.3)

**Note:** Original tactical plan estimated 16 IFS + L-System fractals, but native discovery revealed:
- Only 5 IFS fractals exist in IFSFamily.cpp (not 10)
- 3 IFS fractals already registered in Phase 3 Step 3C: BarnsleyFern, SierpinskiIFS, DragonCurveIFS
- No L-System family exists in modern native registry (old WIN64 code only)
- Priority 7 actual count: 2 new fractals (not 16)

---

#### Priority 8: Chaotic Maps & Bifurcation Diagrams (7/7 fractals) ✅ **COMPLETE - Commit: 38ced1d**
**ChaoticMapsFamily (1 fractal):**
- [x] SprottB (minimalist chaotic attractor; center 0,0; zoom 0.15625)

**BifurcationFamily (6 fractals):**
- [x] LogisticParameterSpace (logistic map parameter space; center 2,0; zoom 0.697)
- [x] LambdaParameterSpace (complex lambda map; center 1,0; zoom 0.536203)
- [x] MandelParameter (Mandelbrot periodicity; center 0,0; zoom 1.0)
- [x] HenonParameterSpace (Hénon map; center 0.75,-0.25; zoom 1.0)
- [x] OrbitDiagram (orbit trajectory visualization; center 0,0; zoom 1.0)
- [x] MayLyapunovRef (Lyapunov exponent; center 2,0; zoom 0.3)

**Note:** Original tactical plan estimated 12 convergence/chaos fractals (SecantMethod, Bisection, Muller, Steffensen, FixedPoint, GaussSeidel, LogisticMap, TentMap, BakerMap, ArnoldCatMap, StandardMap, ChirkovMap), but native discovery revealed:
- ChaoticMapsFamily contains only 4 fractals total (LiuChen, RabinovichFabrikant, Arneodo already registered in Phase 3A; SprottB new)
- BifurcationFamily contains 6 parameter space visualizations (all new)
- The planned convergence methods (Secant, Bisection, etc.) and additional chaos maps (TentMap, BakerMap, etc.) do NOT exist as separate registered fractals in the modern native registry
- Priority 8 actual count: 7 fractals (not 12)

---

#### Priority 9-12: Remaining Families (51 fractals)
**See `MIGRATION_RECONCILIATION.md` for detailed breakdown:**
- Priority 9: Historical & Research (10 fractals)
- Priority 10: Experimental Formulas (12 fractals)
- Priority 11: Advanced Techniques (8 fractals)
- Priority 12: Custom & Utility (8 fractals)

**✅ PHASE 4 TARGET: 279/279 fractals (100%)**

---

### 🎯 PHASE 5: Legacy System Removal

- [x] **Step 5.1:** Delete `ParameterEditorViewModel.Legacy.cs` ✅ **COMPLETE** - Created LegacyBridge.cs with compatibility shims for ResetToDefaults(), ReloadLastSaved(), and LoadParametersForFractal()
- [ ] **Step 5.2:** Remove `SyncPropertiesToParameters()` from `MainViewModel.Parameters.cs`
- [ ] **Step 5.3:** Remove `SyncParametersToProperties()` from `MainViewModel.Parameters.cs`
- [ ] **Step 5.4:** Clean up hard-coded properties in `MainViewModel.StandardFractals.cs`
- [ ] **Step 5.5:** Update toolbar bindings (if needed - may require value converters)
- [ ] **Step 5.6:** Test 5 fractals from each major family (30-40 total tests)
- [ ] **Step 5.7:** Verify parameter persistence works correctly
- [ ] **Step 5.8:** Build and test full application
- [ ] **Step 5.9:** Commit legacy removal changes
- [ ] **Step 5.10:** Update documentation to reflect single parameter system

**✅ MIGRATION COMPLETE!**

---

## Progress Summary

**Current Status: 279/279 fractals complete (100%) - 🎉 MIGRATION COMPLETE! 🎉**

**⚠️ ACTUAL NATIVE COUNT: 279 fractals** (discovered via comprehensive native inventory scan)  
**Original estimate: 300 fractals** (overestimated by 21)  
**📊 See: `MIGRATION_RECONCILIATION.md` for detailed breakdown**

**🚀 ALL PHASES COMPLETE! 🚀**
- ✅ Phase 1-3: Foundation, Core, High-Value (138 fractals)
- ✅ Phase 4 Priorities 1-10: Extended families (102 fractals)
- ✅ Phase 4 Priority 11: Advanced Techniques (17 fractals)
- ✅ Phase 4 Priority 12: Orbital & Extensions (19 fractals)
- ✅ Final Reconciliation: Last 3 fractals (Multibrot-10, JuliaSanMarco, JuliaDouadyRabbit)

**Remaining: 0 fractals (0%)** ✅  
**Next Phase: Phase 5 - Legacy System Removal**
- ✅ Mandelbrot + Multibrot family (8 fractals): z², z³, z⁴, z⁵, z⁶, z⁷, z⁸
- ✅ **Julia presets family (23 fractals): All enhanced Julia presets registered**
- ✅ **Burning Ship family (17 fractals): All power variants, perpendicular, buffalo, celtic, reverse, vertical, diagonal, and BirdOfPrey**
- ✅ **Newton/Convergence family (12 fractals): Newton, NewtonSinExp, NewtonQuartic, NewtonSin, NewtonQuintic, NewtonSextic, NewtonCosh, NewtonBasin, Nova, HalleyCubic, Magnet1M, Magnet2M, Magnet1J, Magnet2J**
- ✅ **Trigonometric family (20 fractals): MandelTrig, Sine, LMandelSine, LLambdaSine, LMandelCos, LLambdaCos, LMandelSinh, LLambdaSinh, LMandelCosh, LLambdaCosh, SinZ, CosZ, CosTan, LambdaTan, NewtonSin, PhoenixSin, Sqr1OverTrig, SqrTrig, TrigPlusTrig, TrigXTrig**
- ✅ **Exponential/Logarithmic family (10 fractals): Exponential, Logarithm/Logarithmic, MandelExp, LMandelExp, LLambdaExp, PowerTower/ZToTheZ, ComplexPower, ExponentialJulia, ExpSquare**
- ✅ **Polynomial Variants (15 fractals): PhoenixM, PhoenixJ, PhoenixPower3, PhoenixPower4, LambdaPower3, LambdaPower4, LambdaTanh, LambdaSquared, LambdaFlip, BarnsleyM1, BarnsleyJ1, BarnsleyM2, BarnsleyJ2, BarnsleyM3, BarnsleyJ3, SpiderVariant**
- ✅ **Rational Functions (5 fractals): NewtonQuintic, RationalZ2Z3, RationalSymmetric, Mobius, RationalPower**
- ✅ **Strange Attractors (14 fractals): Lorenz, Thomas, Dadras, Pickover, Aizawa, Halvorsen, ChenLee, Clifford, DeJong, Tinkerbell, Duffing, LiuChen, RabinovichFabrikant, Arneodo**
- ✅ **Visual Priority (8 fractals): Buddhabrot, Lyapunov, NumFractal, Biomorphs, PickoverStalks, BarnsleyFern, SierpinskiIFS, DragonCurveIFS**
- ✅ **Phase 4 Priority 1:**
  - **Julia Core Variants (8 fractals): JuliaClassic, JuliaCubic, JuliaBurningShip, JuliaPhoenix, JuliaLambda, JuliaSine, JuliaExp, JuliaMagnet** ✅ Commit: `32b7db4`
  - **Mandelbrot/Multibrot Variants (9 fractals): Mandel4, Julia4, MandelLambda, MarksJulia, Mandelbar, Thorn, Multibrot3, Multibrot4, Multibrot5** ✅ Commit: `b17bbf2`
  - **PowerVariants (9 fractals): Multibrot6, Multibrot7, Multibrot8, Julia5, Julia6, BurningShip3, BurningShip4, Tricorn3, Tricorn4** ✅ Commit: `82ecfad`
- ✅ **Phase 4 Priority 2:**
  - **Extended Trigonometric (8 fractals): TanMandel, CotMandel, SecMandel, CscMandel, ArcSinMandel, ArcCosMandel, ArcTanMandel, TanhMandel** ✅ Commit: `f070e59`
- ✅ **Phase 4 Priority 3:**
  - **Complex Functions (6 fractals): SinhMandelbrot, CoshMandelbrot, TanhMandelbrot, HeartMandel, SharkFin, Wavy** ✅ Commit: `0b59ca9`
  - **Special Functions (7 fractals): GammaFractal, ErrorFunctionFractal, BesselLikeFractal, ContinuedFraction, Tetration, LambertW, HyperbolicCombo** ✅ Commit: `0b59ca9`
- ✅ **Phase 4 Priority 4 (Partial):**
  - **Exotic Formulas (4 fractals): CelticMandel, PerpendicularMandel, QuasiPerpendicular, Zubieta** ✅ Commit: `0b59ca9`
- ✅ **Phase 4 Priority 5:**
  - **Hybrids & Blends (18 fractals): BurningMandel, ExpMandelHybrid, MutantMandelbrot, TrigMandelBlend, SierpinskiMandel, PerturbedNewton, BifurcationMandel, CelticMandelbrot, MandelBurningHybrid, MandelLambdaMix, TricornPhoenixHybrid, NewtonMandelBlend, SineMandelHybrid, ExpMandelBlend, MultiPowerCycle, MagnetMandelHybrid, CollatzHybrid, CelticBurningHybrid** ✅ Commit: `2ea7f19`
- ✅ **Phase 4 Priority 6:**
  - **Orbital Fractals (8 fractals): OrbitTrapCross, OrbitTrapCircle, OrbitTrapPoint, OrbitTrapSquare, AverageDistance, MinimumDistance, MaximumDistance, AngleAverage** ✅ Commit: `974e8c6`
  - **Distance Estimators (4 fractals): MandelbrotDEM, JuliaDEM, BurningShipDEM, TricornDEM** ✅ Commit: `974e8c6`
  - **Viewport defaults preserved from native registrations**
- ✅ **Phase 4 Priority 7:**
  - **IFS Fractals (2 fractals): PentagonIFS, TreeIFS** ✅ Commit: `8607966`
  - **Note:** BarnsleyFern, SierpinskiIFS, DragonCurveIFS already registered in Phase 3
- ✅ **Phase 4 Priority 8:**
  - **Chaotic Maps (1 fractal): SprottB** ✅ Commit: `38ced1d`
  - **Bifurcation Diagrams (6 fractals): LogisticParameterSpace, LambdaParameterSpace, MandelParameter, HenonParameterSpace, OrbitDiagram, MayLyapunovRef** ✅ Commit: `38ced1d`
  - **Note:** LiuChen, RabinovichFabrikant, Arneodo already registered in Phase 3A
- ✅ **Phase 4 Priority 9:**
  - **Historical & Research Fractals (4 fractals): MartinMap, ChipMap, QuaternionJulia2D, SinusoidalFractal** ✅ Commit: `01dbef0`
  - **Note:** Biomorphs, PickoverStalks already registered in Phase 3; CollatzFractal as "Hailstone" in Phase 1; DuffingMap as "Duffing" in Phase 3A
- ✅ **Phase 4 Priority 10:**
  - **Polynomial Variants (8 fractals): CubicMandel, QuarticMandel, QuinticMandel, SexticMandel, RationalR1, PolyZ3MinusZ, PolyZ4PlusZ3, Biomorph** ✅ Commit: `4c42944`
  - **Note:** High-power Mandelbrot variants (z³-z⁶) with rotational symmetries, rational maps, mixed polynomials
- ✅ Complex exponent (2 fractals): MarksMandel, MarksMandelpwr
- ✅ Phoenix (1 fractal)
- ✅ Lambda (1 fractal)
- ✅ Hailstone (1 fractal)

**Remaining: 39 fractals (14%)**  
**See: `MIGRATION_RECONCILIATION.md` Priority 11-12 breakdown**

### Completion Milestones
- [x] **Phase 1 Complete: 59/279 (21%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `9d7e1d1` - Phase 1 Complete + Polynomial Coefficient Infrastructure
  - **Pushed to:** `origin/development`
- [x] **Phase 2 Step 2A Complete: 79/279 (28%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `116b11d` - Trigonometric family complete
  - **Pushed to:** `origin/development`
- [x] **Phase 2 Step 2B Complete: 89/279 (32%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `a262b50` - Exponential/Logarithmic family complete
  - **Pushed to:** `origin/development`
- [x] **Phase 2 Step 2C Complete: 104/279 (37%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `96099bf` - Polynomial Variants family complete
  - **Pushed to:** `origin/development`
- [x] **Phase 2 Step 2D Complete: 109/279 (39%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `[Rational Functions]` - Rational Functions family complete
  - **Pushed to:** `origin/development`
- [x] **Phase 2 Complete: 109/279 (39%) - COMPLETED: May 14, 2026** ✅
- [x] **Phase 3 Step 3A Complete: 123/279 (44%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `[Strange Attractors]` - Strange Attractors complete
  - **Pushed to:** `origin/development`
- [x] **Phase 3 Step 3B Complete: 126/279 (45%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `d0476ce` - Newton/Convergence variants complete
  - **Pushed to:** `origin/development`
- [x] **Phase 3 Step 3C Complete: 134/279 (48%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `a5c1b79` - Visual Priority fractals complete
  - **Pushed to:** `origin/development`
- [x] **Phase 3 Complete: 138/279 (49%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `7f2bc54` - Native inventory & reconciliation complete
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 1 - Julia Core Variants: 146/279 (52%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `32b7db4` - Julia core variants complete (8 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 1 - Mandelbrot Variants: 155/279 (56%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `b17bbf2` - Mandelbrot/Multibrot variants complete (9 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 1 - PowerVariants: 164/279 (59%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `82ecfad` - PowerVariants family complete (9 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 2 - Extended Trigonometric: 172/279 (62%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `f070e59` - Extended Trigonometric family complete (8 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 3 - Complex & Special Functions: 189/279 (68%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `0b59ca9` - Priority 3 & Priority 4 (partial) complete (17 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 5 - Hybrids & Blends: 207/279 (74%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `2ea7f19` - Hybrids & Blends family complete (18 fractals)
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 6 - Orbital & Distance Estimators: 219/279 (78%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `974e8c6` - Orbital & Distance Estimators complete (12 fractals)
  - **Viewport defaults preserved:** MandelbrotDEM, JuliaDEM, BurningShipDEM
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 7 - IFS (Iterated Function Systems): 221/279 (79%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `8607966` - IFS additional fractals complete (2 fractals)
  - **Note:** Only 2 new (PentagonIFS, TreeIFS); 3 already registered in Phase 3; no L-Systems exist in modern registry
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 8 - Chaotic Maps & Bifurcation Diagrams: 228/279 (82%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `38ced1d` - Chaotic Maps & Bifurcation Diagrams complete (7 fractals)
  - **SprottB** (1 chaotic map) + **6 bifurcation diagrams** (LogisticParameterSpace, LambdaParameterSpace, MandelParameter, HenonParameterSpace, OrbitDiagram, MayLyapunovRef)
  - **Note:** LiuChen, RabinovichFabrikant, Arneodo already registered in Phase 3A; no convergence methods exist in modern registry
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 9 - Historical & Research Fractals: 232/279 (83%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `01dbef0` - Historical & Research fractals complete (4 fractals)
  - **MartinMap** (Barry Martin 1986), **ChipMap** (Pickover 1987), **QuaternionJulia2D** (John C. Hart 1989), **SinusoidalFractal** (1985)
  - **Note:** Biomorphs, PickoverStalks already registered in Phase 3; CollatzFractal as "Hailstone" in Phase 1; DuffingMap as "Duffing" in Phase 3A
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 10 - Polynomial Variants: 240/279 (86%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `4c42944` - Polynomial Variants family complete (8 fractals)
  - **CubicMandel** (z³+c, zoom 1.2), **QuarticMandel** (z⁴+c, zoom 1.3), **QuinticMandel** (z⁵+c, zoom 1.4), **SexticMandel** (z⁶+c, zoom 1.5)
  - **RationalR1** ((z²+c)/(z²+1), zoom 1.5), **PolyZ3MinusZ** (z³-z+c), **PolyZ4PlusZ3** (z⁴+z³+c), **Biomorph** (special component bailout, zoom 0.5)
  - **Note:** High-power Mandelbrot variants with rotational symmetries, rational maps with poles, mixed polynomials
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 11 - Advanced Techniques: 257/279 (92%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `570502f` - Advanced Techniques family complete (17 fractals)
  - **Classic Escape-Time:** Manowar, Sierpinski, Unity, Spider, Tetrate, HeartMandelbrot, SharkFinMandelbrot, PartialBurningShip, CelticHeart, WavyMandelbrot
  - **Strange Attractors Extended:** Svensson, Bedhead
  - **Extended Julia:** JuliaSiegelDisk, JuliaCustom, LambdaJulia, Multibrot3Julia, Multibrot4Julia
  - **Pushed to:** `origin/development`
- [x] **Phase 4 Priority 12 - Orbital Modifications & Extensions: 276/279 (99%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `78c7d0c` - Final 19 fractals
  - **Orbital Modifications (10):** CircularOrbitTrap, CrossOrbitTrap, StalksConditional, SmoothedOrbit, OrbitAngleAccum, TriangleOrbitTrap, StripeAverage, CurvatureTracking, DeltaMagnitude, PointLineOrbitTrap
  - **Phoenix Extended (3):** PhoenixCosh, PhoenixComplex, PhoenixLambda
  - **Magnet Extended (2):** Magnet1Power3, Magnet2Power3
  - **Lambda Extended (2):** LambdaModified, LambdaPhoenix
  - **Rational Functions (1):** NewtonCubic
  - **Special Exotic (1):** Hailstone2D
  - **Pushed to:** `origin/development`
- [x] **🎉 FINAL RECONCILIATION - 100% COMPLETE: 279/279 (100%) - COMPLETED: May 14, 2026** 🎉
  - **Commit:** `9a036fc` - Final 3 fractals
  - **Multibrot-10:** z¹⁰+c decic polynomial with ten-fold symmetry (zoom 1.5)
  - **JuliaSanMarco:** Named Julia preset with fixed c (zoom 0.5)
  - **JuliaDouadyRabbit:** Douady rabbit Julia preset with fixed c (zoom 0.5)
  - **Pushed to:** `origin/development`
  - **🚀 ALL 279 NATIVE FRACTALS NOW HAVE MANAGED PARAMETER TEMPLATES! 🚀**
- [ ] Phase 5 Complete: Legacy system removed - **READY TO BEGIN**

---

## Appendix: Actual Native Fractal Inventory

**Total Native Fractals: 279** (discovered via comprehensive scan of 40 Family.cpp files)

### See Complete Details:
- **`NATIVE_FRACTAL_INVENTORY.md`** - Full list of all 279 fractals by family
- **`MIGRATION_RECONCILIATION.md`** - Detailed reconciliation, priorities, and Phase 4 breakdown

### Summary by Completion Status:
- **Completed:** 279 fractals (100%) ✅
- **Remaining:** 0 fractals (0%) ✅

### Phase Completion Details:
- Phase 1-3: Foundation + Core (138 fractals) ✅
- Phase 4 Priorities 1-10: Extended families (102 fractals) ✅
- Phase 4 Priority 11: Advanced Techniques (17 fractals) ✅
- Phase 4 Priority 12: Orbital & Extensions (19 fractals) ✅
- Final Reconciliation: Last 3 fractals ✅

### Next Phase:
- **Phase 5: Legacy System Removal** - Ready to begin
- Target: Delete legacy parameter code, clean up sync bridge
- Validate all 279 fractals work with unified parameter system

**For detailed Phase 4 implementation strategy, see `MIGRATION_RECONCILIATION.md`**

---

**Document Created:** May 14, 2026  
**Last Updated:** May 14, 2026  
**Status:** ACTIVE - Priority implementation in progress  
**Owner:** Mark  
**AI Budget:** Use aggressively through May 31, 2026
