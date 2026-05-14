# Parameter Migration: Tactical Implementation Plan

## Executive Summary

**Project:** Complete the flexible parameter system for 300 fractals  
**Timeline:** May 14 - June 15, 2026 (3-4 weeks)  
**Priority:** HIGH - Use AI budget while available  
**Status:** 14/300 fractals complete (5%)  
**Goal:** 100% coverage, eliminate dual parameter system

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

### ✅ Working Examples (14 fractals)
Completed templates exist for:
- Mandelbrot
- Julia (basic)
- Burning Ship
- Newton
- Magnet
- ~9 other core fractals

**Location:** `ManpWinUI\Services\Parameters\StandardParameterTemplates.Core.cs`

### ❌ What's Missing (286 fractals)
Need to create parameter templates for 286 remaining fractals.

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

#### Step 2C: Polynomial Variants (5 hours) - 24 fractals
- Tricorn family (5 fractals)
- Phoenix family (4 fractals)
- Lambda family (3 fractals)
- Collatz variants (4 fractals)
- Barnsley fractals (3 fractals)
- Spider fractals (5 fractals)

**Each family has unique parameters** - research required.

#### Step 2D: Rational Functions (2 hours) - 8 fractals
- RationalZ3
- RationalZ4
- InverseMandelbrot
- InverseJulia
- ReciprocalMandelbrot
- ReciprocalJulia
- (... 2 more)

**Pattern:** Escape-time base + numerator/denominator exponents.

**📊 PHASE 2 COMPLETION: 114/300 fractals (38%)**

---

### 🎯 PHASE 3: May 28-31 (Week 3) - HIGH-VALUE EXOTIC
**Target: 162 fractals total (54% coverage), +48 this week**  
**Estimated Time: 10-12 hours** (shorter week)

#### Step 3A: Strange Attractors (3 hours) - 10 fractals
- Lorenz
- Rossler
- Henon
- Ikeda
- CliffordAttractor
- DeJongAttractor
- (... 4 more)

**Pattern:** Use `AttractorBase` + system-specific parameters:
```csharp
// Lorenz example
new FractalParameterDescriptor("sigma", ParameterType.Double)
{
    DisplayName = "Sigma (σ)",
    DefaultValue = 10.0,
    MinValue = 0.1,
    MaxValue = 50.0,
    Category = "System Parameters"
},
new FractalParameterDescriptor("rho", ParameterType.Double)
{
    DisplayName = "Rho (ρ)",
    DefaultValue = 28.0,
    MinValue = 0.1,
    MaxValue = 100.0,
    Category = "System Parameters"
},
new FractalParameterDescriptor("beta", ParameterType.Double)
{
    DisplayName = "Beta (β)",
    DefaultValue = 2.667,
    MinValue = 0.1,
    MaxValue = 10.0,
    Category = "System Parameters"
}
```

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
- [ ] MandelExp (z = e^z + c)
- [ ] MandelLog (z = log(z) + c)
- [ ] MandelExpZ (z = z·e^z + c)
- [ ] MandelLogZ (z = z·log(z) + c)
- [ ] JuliaExp
- [ ] JuliaLog
- [ ] ZPowerExp (z = z^e^z)
- [ ] ExpZSquared (z = e^(z²))
- [ ] LogZSquared (z = log(z²))
- [ ] ExpLog (z = e^log(z))
- [ ] LogExp (z = log(e^z))
- [ ] ExponentialHybrid
- [ ] Test: Verify exponential base parameter for 2-3 fractals

#### Step 2C: Polynomial Variants (24 fractals)
**Tricorn Family (5 fractals):**
- [ ] Tricorn (z = conj(z)² + c)
- [ ] TricornCubic
- [ ] TricornPower
- [ ] TricornJulia
- [ ] TricornMultibrot

**Phoenix Family (4 fractals):**
- [ ] Phoenix (z_n+1 = z_n² + c + p·z_n-1)
- [ ] PhoenixJulia
- [ ] PhoenixCubic
- [ ] PhoenixVariant

**Lambda Family (3 fractals):**
- [ ] Lambda (z = λ·z·(1-z))
- [ ] LambdaJulia
- [ ] LambdaVariant

**Collatz Variants (4 fractals):**
- [ ] Collatz
- [ ] CollatzJulia
- [ ] CollatzVariant1
- [ ] CollatzVariant2

**Barnsley Fractals (3 fractals):**
- [ ] Barnsley1 (z = (z-1)·c if Re(z) >= 0)
- [ ] Barnsley2
- [ ] Barnsley3

**Spider Fractals (5 fractals):**
- [ ] Spider (iterates both z and c)
- [ ] SpiderJulia
- [ ] SpiderVariant1
- [ ] SpiderVariant2
- [ ] SpiderVariant3

- [ ] Test: Verify unique parameters for 4-5 family representatives

#### Step 2D: Rational Functions (8 fractals)
- [ ] RationalZ3 (z = (z³ + c)/(z² + c))
- [ ] RationalZ4
- [ ] InverseMandelbrot (z = 1/(z² + c))
- [ ] InverseJulia
- [ ] ReciprocalMandelbrot (z = c/(z² + 1))
- [ ] ReciprocalJulia
- [ ] RationalHybrid1
- [ ] RationalHybrid2
- [ ] Test: Verify numerator/denominator parameters for 2 fractals

**✅ PHASE 2 TARGET: 114/300 fractals (38%)**

---

### 🎯 PHASE 3: May 28-31 - HIGH-VALUE EXOTIC (48 fractals)

#### Step 3A: Strange Attractors (10 fractals)
- [ ] Lorenz (σ, ρ, β parameters)
- [ ] Rossler (a, b, c parameters)
- [ ] Henon (a, b parameters)
- [ ] Ikeda (u parameter)
- [ ] CliffordAttractor (a, b, c, d parameters)
- [ ] DeJongAttractor (a, b, c, d parameters)
- [ ] DuffingAttractor
- [ ] ChaoticPendulum
- [ ] Aizawa
- [ ] Thomas
- [ ] Test: Verify system parameters appear for 3 attractors

#### Step 3B: Newton/Convergence Variants (18 fractals)
**Newton Polynomials (8 fractals):**
- [ ] NewtonPolynomial3 (degree 3: z³ + az² + bz + c)
- [ ] NewtonPolynomial4 (degree 4: z⁴ + az³ + bz² + cz + d)
- [ ] NewtonPolynomial5 (degree 5: z⁵ + az⁴ + bz³ + cz² + dz + e)
- [ ] NewtonPolynomial6 (degree 6)
- [ ] NewtonPolynomial7 (degree 7)
- [ ] NewtonPolynomial8 (degree 8)
- [ ] NewtonPolynomial9 (degree 9)
- [ ] NewtonPolynomial10 (degree 10)

**Implementation Pattern:**
Use the new `CreateNewtonPolynomial` helper to automatically generate all coefficient parameters:

```csharp
// In FractalParameterService.cs:
RegisterTemplate("NewtonPolynomial3", () =>
    StandardParameterTemplates.CreateNewtonPolynomial("NewtonPolynomial3", 3));

RegisterTemplate("NewtonPolynomial4", () =>
    StandardParameterTemplates.CreateNewtonPolynomial("NewtonPolynomial4", 4));

RegisterTemplate("NewtonPolynomial5", () =>
    StandardParameterTemplates.CreateNewtonPolynomial("NewtonPolynomial5", 5));

// etc. for degrees 6-10
```

This will automatically create parameters like:
- **Degree 3:** `poly_coeff_a` (z²), `poly_coeff_b` (z¹), `poly_coeff_c` (z⁰)
- **Degree 4:** `poly_coeff_a` (z³), `poly_coeff_b` (z²), `poly_coeff_c` (z¹), `poly_coeff_d` (z⁰)
- **Degree 5:** `poly_coeff_a` (z⁴), `poly_coeff_b` (z³), `poly_coeff_c` (z²), `poly_coeff_d` (z¹), `poly_coeff_e` (z⁰)
- etc.

Each polynomial includes **all lower-degree terms**, not just the leading coefficient.

**Halley Method Variants (4 fractals):**
- [ ] HalleyCubic (z³ + az² + bz + c)
- [ ] HalleyQuartic (z⁴ + az³ + bz² + cz + d)
- [ ] HalleySin
- [ ] HalleyExp

**Other Convergence Methods (6 fractals):**
- [ ] Schroder (z³ + az² + bz + c variant)
- [ ] SchroderQuartic
- [ ] Householder3 (third-order method)
- [ ] Householder4 (fourth-order method)
- [ ] LaguerreMethod
- [ ] SecantMethod

- [ ] Test: Verify polynomial coefficient parameters appear and can be edited

#### Step 3C: Visual Priority Fractals (20 fractals)
- [ ] Lyapunov (stability diagram)
- [ ] Buddhabrot (orbit visualization)
- [ ] Nebulabrot (multi-layer Buddhabrot)
- [ ] PickoverStalks
- [ ] Biomorph1
- [ ] Biomorph2
- [ ] Biomorph3
- [ ] NovaFractalM (modified)
- [ ] KleinianGroup
- [ ] QuaternionJulia
- [ ] QuaternionMandelbrot
- [ ] BarnsleyFern
- [ ] SierpinskiCarpet
- [ ] SierpinskiTriangle
- [ ] KochSnowflake
- [ ] DragonCurve
- [ ] HilbertCurve
- [ ] PeanoCurve
- [ ] GosperCurve
- [ ] MinkowskiSausage
- [ ] Test: Verify 4-5 visually distinctive fractals render

**✅ PHASE 3 TARGET: 162/300 fractals (54%)**

---

### 🎯 PHASE 4: June 1-15 - REMAINING COVERAGE (138 fractals)

#### IFS Variants (20 fractals)
- [ ] BarnsleyFernVariant1
- [ ] BarnsleyFernVariant2
- [ ] SierpinskiVariant1
- [ ] SierpinskiVariant2
- [ ] SierpinskiVariant3
- [ ] FractalTree1
- [ ] FractalTree2
- [ ] FractalTree3
- [ ] PythagorasTree
- [ ] CantorSet
- [ ] CantorDust
- [ ] MengerSponge
- [ ] ApolonianGasket
- [ ] IFSAttractor1
- [ ] IFSAttractor2
- [ ] IFSAttractor3
- [ ] IFSAttractor4
- [ ] IFSAttractor5
- [ ] IFSAttractor6
- [ ] IFSCustom1

#### Orbital Modifications (18 fractals)
- [ ] OrbitTrapCircle
- [ ] OrbitTrapSquare
- [ ] OrbitTrapLine
- [ ] OrbitTrapCross
- [ ] OrbitTrapPoint
- [ ] OrbitTrapStar
- [ ] OrbitTrapSpiral
- [ ] OrbitTrapGalaxies
- [ ] OrbitTrapRing
- [ ] OrbitDistance1
- [ ] OrbitDistance2
- [ ] OrbitDistance3
- [ ] OrbitAngle1
- [ ] OrbitAngle2
- [ ] OrbitSum
- [ ] OrbitProduct
- [ ] OrbitMin
- [ ] OrbitMax

#### Hybrid Fractals (18 fractals)
- [ ] MandelbrotNewton (hybrid)
- [ ] JuliaNewton
- [ ] BurningShipNewton
- [ ] MandelbrotPhoenix
- [ ] JuliaPhoenix
- [ ] NewtonSpider
- [ ] MagnetSpider
- [ ] SinCosHybrid
- [ ] ExpLogHybrid
- [ ] TrigExpHybrid
- [ ] PolynomialTrig
- [ ] RationalTrig
- [ ] HybridVariant1
- [ ] HybridVariant2
- [ ] HybridVariant3
- [ ] HybridVariant4
- [ ] HybridVariant5
- [ ] HybridVariant6

#### Distance Estimator Variants (12 fractals)
- [ ] MandelbrotDE
- [ ] JuliaDE
- [ ] BurningShipDE
- [ ] NewtonDE
- [ ] TricornDE
- [ ] PhoenixDE
- [ ] DistanceEstimator1
- [ ] DistanceEstimator2
- [ ] DistanceEstimator3
- [ ] DistanceEstimator4
- [ ] DistanceEstimator5
- [ ] DistanceEstimator6

#### Bifurcation Diagrams (10 fractals)
- [ ] LogisticMap
- [ ] LogisticMapVariant1
- [ ] LogisticMapVariant2
- [ ] BifurcationDiagram1
- [ ] BifurcationDiagram2
- [ ] BifurcationDiagram3
- [ ] FeigenbaumDiagram
- [ ] ChaosMap1
- [ ] ChaosMap2
- [ ] ChaosMap3

#### Historical/Obscure Fractals (30 fractals)
- [ ] FatouSet1
- [ ] FatouSet2
- [ ] JuliaVariant1 through JuliaVariant10 (10 fractals)
- [ ] MandelbrotVariant1 through MandelbrotVariant5 (5 fractals)
- [ ] HistoricalFractal1 through HistoricalFractal13 (13 fractals)

#### Experimental/Custom Fractals (30 fractals)
- [ ] Experimental1 through Experimental15 (15 fractals)
- [ ] Custom1 through Custom15 (15 fractals)

**✅ PHASE 4 TARGET: 300/300 fractals (100%)**

---

### 🎯 PHASE 5: Legacy System Removal

- [ ] **Step 5.1:** Delete `ParameterEditorViewModel.Legacy.cs`
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

**Current Status: 79/300 fractals complete (~26%) - PHASE 2 STEP 2A COMPLETE! 🎉**

**What's Already Complete:**
- ✅ Mandelbrot + Multibrot family (8 fractals): z², z³, z⁴, z⁵, z⁶, z⁷, z⁸
- ✅ **Julia presets family (23 fractals): All enhanced Julia presets registered**
- ✅ **Burning Ship family (17 fractals): All power variants, perpendicular, buffalo, celtic, reverse, vertical, diagonal, and BirdOfPrey**
- ✅ **Newton/Convergence family (9 fractals): Newton, NewtonSinExp, NewtonQuartic, Nova, HalleyCubic, Magnet1M, Magnet2M, Magnet1J, Magnet2J**
- ✅ **Trigonometric family (20 fractals): MandelTrig, Sine, LMandelSine, LLambdaSine, LMandelCos, LLambdaCos, LMandelSinh, LLambdaSinh, LMandelCosh, LLambdaCosh, SinZ, CosZ, CosTan, LambdaTan, NewtonSin, PhoenixSin, Sqr1OverTrig, SqrTrig, TrigPlusTrig, TrigXTrig**
- ✅ Complex exponent (2 fractals): MarksMandel, MarksMandelpwr
- ✅ Phoenix (1 fractal)
- ✅ Lambda (1 fractal)
- ✅ ExpSquare (1 fractal)
- ✅ Hailstone (1 fractal)

### Completion Milestones
- [x] **Phase 1 Complete: 59/300 (20%) - COMPLETED: May 14, 2026** ✅
  - **Commit:** `9d7e1d1` - Phase 1 Complete + Polynomial Coefficient Infrastructure
  - **Pushed to:** `origin/development`
- [ ] **Phase 2 Step 2A Complete: 79/300 (26%) - COMPLETED: May 14, 2026** ✅
- [ ] Phase 2 Complete: 123/300 (41%) - Target: May 27, 2026
- [ ] Phase 3 Complete: 171/300 (57%) - Target: May 31, 2026
- [ ] Phase 4 Complete: 300/300 (100%) - Target: June 15, 2026
- [ ] Phase 5 Complete: Legacy system removed - Target: June 18, 2026

---

## Appendix: Fractal Family List

### Core Fractals (50)
- Mandelbrot family: 8
- Julia family: 23
- Burning Ship family: 10
- Newton/Magnet: 9

### Mathematical Functions (64)
- Trigonometric: 20
- Exponential/Logarithmic: 12
- Polynomial variants: 24
- Rational functions: 8

### Exotic Fractals (186)
- Strange attractors: 10
- IFS: 20
- Orbital modifications: 18
- Hybrid fractals: 18
- Distance estimators: 12
- Bifurcation diagrams: 10
- Newton polynomials: 18
- Chaotic maps: 16
- Historical fractals: 30
- Experimental: 34

**Total: 300 fractals**

---

**Document Created:** May 14, 2026  
**Last Updated:** May 14, 2026  
**Status:** ACTIVE - Priority implementation in progress  
**Owner:** Mark  
**AI Budget:** Use aggressively through May 31, 2026
