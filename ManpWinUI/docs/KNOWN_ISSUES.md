# Known Issues & Technical Debt

**Purpose**: Track bugs, code quality issues, and technical debt discovered during feature development.  
**Status**: Issues will be systematically addressed during scheduled quality review phases (see PROJECT_PLAN.md).

---

## 🐛 Bugs & Issues Discovered

### Week 9 - Deep Zoom Feature Development

#### Issue #1: Temporary Deep Zoom Implementation (HIGH PRIORITY - PLANNED FOR REPLACEMENT)
**Discovered**: Week 9 Task 1 implementation review  
**Files**: 
  - `ManpWinUI/Services/FractalRenderService.cs`
  - `ManpCore.Native/FractalEngineWrapper.cpp`
  - `ManpCore.Native/BigDoubleMarshaller.h`
**Severity**: High - Technical debt blocking production-quality deep zoom  
**Status**: ⚠️ **TEMPORARY SOLUTION IN PLACE** - Full replacement planned

**Description**:
The current deep zoom implementation uses a simple BigDouble coordinate conversion approach:
- Converts double coordinates → BigDouble (25 decimal places) → native MPFR
- Recalculates every pixel independently with high-precision arithmetic
- No optimization, caching, or advanced algorithms

**Limitations**:
1. **Poor performance beyond 10^20 zoom**: 2-5x slowdown grows exponentially at extreme zooms
2. **Fixed precision**: Cannot dynamically adjust precision based on zoom level
3. **No reference orbit caching**: Wastes computation recalculating similar orbits
4. **Missing perturbation theory**: Does not use delta-based optimization
5. **No BLA acceleration**: Cannot skip iterations using bilinear approximation
6. **No glitch detection**: May produce artifacts at extreme zooms
7. **Maximum zoom limited**: ~10^20 practical limit vs. 10^100+ with perturbation

**Root Cause**:
This was implemented as a **deliberate temporary compromise** to:
- Get basic deep zoom working quickly (1 day vs. 2+ weeks)
- Test UI/UX for deep zoom features
- Provide a working baseline for comparison
- Allow continued development while planning proper integration

**Proper Solution** (Phase 3.5 - 12-17 days):
Integrate Paul de Leeuw's production-grade perturbation theory engine from ManpWIN64:

1. **Perturbation Theory**:
   - Calculate ONE high-precision reference orbit (expensive, done once)
   - Use perturbation formula for all other pixels: ΔZ_n ≈ 2·Z_n·ΔZ_(n-1) + ΔC
   - Result: 10-100x faster at extreme zooms

2. **BLA (Bilinear Approximation)**:
   - Skip 50-90% of iterations using series approximation
   - Precomputed coefficients: ΔZ_(n+k) ≈ A·ΔZ_n + B·ΔC
   - Additional 2-5x speedup

3. **Reference Orbit Caching**:
   - Reuse orbit when panning (center change < threshold)
   - Massive speedup for exploration

4. **Glitch Detection & Correction**:
   - Detect when perturbation fails (pixel escapes before reference)
   - Fallback to high-precision calculation for glitched pixels

5. **Dynamic Precision**:
   - Auto-adjust precision based on zoom level
   - Balance accuracy vs. performance

**Implementation Plan**:
See `DEEP_ZOOM_INTEGRATION_PLAN.md` for detailed 5-phase roadmap:
- Phase A: Architecture Analysis (2-3 days)
- Phase B: Minimal Perturbation Bridge (3-4 days)
- Phase C: Full Feature Integration (4-5 days)
- Phase D: Testing & Optimization (2-3 days)
- Phase E: Documentation & Release (1-2 days)

**Timeline**: Start immediately after documentation cleanup (current session)

**Code Locations to Replace**:
```csharp
// FractalRenderService.cs lines ~180-220 (BigDouble conversion)
if (useDeepZoom && parameters.BigCenterX != null && ...)
{
    // TODO: Replace with perturbation theory implementation
    // See DEEP_ZOOM_INTEGRATION_PLAN.md Phase B
    nativeParams.BigCenterX = parameters.BigCenterX.ToDouble(); // TEMPORARY
    ...
}
```

```cpp
// FractalEngineWrapper.cpp lines ~230-280 (deep zoom path)
if (useDeepZoom)
{
    Debug::WriteLine("Native Calculate: Deep Zoom Mode - Using MPFR BigDouble precision");
    // TODO: Replace with ReferenceZoomPoint() and perturbation calculation
    // See DEEP_ZOOM_INTEGRATION_PLAN.md Phase B
    Debug::WriteLine("WARNING: Deep zoom BigDouble rendering path not yet implemented - falling back to double");
}
```

**Expected Outcome**:
- ✅ Zoom to 10^100+ magnification
- ✅ 10-100x faster than current implementation at extreme zooms
- ✅ Smooth exploration with reference orbit caching
- ✅ Production-quality rendering with glitch correction
- ✅ Auto-enable at appropriate zoom thresholds

**Impact**: 
- Blocks production release of deep zoom feature
- Current implementation adequate for moderate zooms (10^15 - 10^20)
- User expectation: Deep zoom should support "Manp-level" extreme zooms (10^100+)

**Recommended Action**: 
Execute Phase 3.5 integration plan starting next session (after doc cleanup commit)

---

#### Issue #2: Source-Generated ViewModel Properties (MainViewModel.g.cs) (LOW PRIORITY)
**Discovered**: During deep zoom status bar implementation  
**File**: `..\..\..\AppData\Local\Temp\.vsdbgsrc\...\ManpWinUI.ViewModels.MainViewModel.g.cs`  
**Severity**: Low - Code quality / maintainability  

**Description**:
The auto-generated MainViewModel partial class contains numerous property implementations and partial method stubs that suggest potential issues:

1. **Excessive property change notifications**: Many properties trigger multiple dependent property notifications (e.g., `ImageWidth` triggers 7 different property change notifications including `TotalPixels`, `IsHDResolution`, `Is4KResolution`, etc.)

2. **Inconsistent backing field patterns**: Mix of `field`, `_fieldName`, and named backing fields (e.g., `isBrowserPanelVisible`, `browserPanelWidth`)

3. **Unused partial methods**: Large number of generated `OnXyzChanging/Changed` partial method stubs that are never implemented, adding code bloat

4. **Performance concerns**: Deep property change cascades could cause UI update storms if properties change frequently

**Impact**: 
- Potential performance degradation during rapid property updates
- Code maintenance complexity
- Increased memory footprint from unused generated code

**Recommended Action** (for quality review phase - Phase 4):
- Review property dependency chains and optimize notification patterns
- Consider implementing value coalescing for frequently-changing properties
- Evaluate whether all generated partial methods are necessary (could use attributes to suppress unused ones)
- Consider refactoring to use computed properties where appropriate instead of cascading notifications

**Notes**:
- This is likely a pattern issue across multiple ViewModels, not just MainViewModel
- Related to CommunityToolkit.Mvvm source generator behavior (version 8.4.0.0)

---

#### Issue #3: Render Button Remains Disabled After Render Completion (RESOLVED ✅)
**Discovered**: Week 9 - User observation during testing  
**File**: Likely in render command execution flow (`MainViewModel.Commands.cs`)  
**Severity**: Medium - Blocks user workflow  
**Status**: ✅ **FIXED** in Week 9 Task 1 Bug Fix

**Description**:
After rendering completes, the "Render" button sometimes remained disabled, requiring the user to modify a parameter to re-enable it.

**Root Cause**:
Two separate instances of `RenderSettingsViewModel`:
1. One registered in DI container (used by MainViewModel)
2. One created locally in MainPage.cs (bound to UI)

When UI updated the local instance, MainViewModel still read from the DI instance, creating state mismatch.

**Solution Implemented**:
Changed `MainPage.cs` line 68 from:
```csharp
RenderSettingsViewModel = new RenderSettingsViewModel(); // ❌ WRONG
```
to:
```csharp
RenderSettingsViewModel = viewModel.RenderSettingsViewModel; // ✅ CORRECT - Use DI singleton
```

**Verification**:
- ✅ Render button re-enables immediately after render completion
- ✅ Deep zoom toggle affects rendering correctly
- ✅ No more state mismatches

**Documentation**: See `Week9-Task1-BugFix.md` for detailed analysis

---

## 📝 Technical Debt Inventory

### High Priority (Blocks Production Features)

1. **Deep Zoom Perturbation Integration** (Issue #1)
   - Timeline: 12-17 days
   - Plan: `DEEP_ZOOM_INTEGRATION_PLAN.md`
   - Blocks: Production-quality deep zoom (10^100+ zoom)

### Medium Priority (Quality Issues)

(None currently identified)

### Low Priority (Code Quality / Maintainability)

1. **ViewModel Property Notification Optimization** (Issue #2)
   - Timeline: 1-2 days during Phase 4 quality review
   - Impact: Minor performance overhead
   - Recommendation: Profile before optimizing

2. **Code Documentation**
   - Add XML comments to all public APIs
   - Document complex algorithms (BLA, series approximation after integration)
   - Create architecture diagrams for native/managed boundary

3. **Unit Test Coverage**
   - Create unit tests for FractalRenderService
   - Test BigDouble marshaling edge cases
   - Test perturbation orbit caching logic (after integration)

---

## 🔄 Resolution Process

### For High Priority Issues:
1. Create detailed implementation plan (see DEEP_ZOOM_INTEGRATION_PLAN.md)
2. Create feature branch (`feature/perturbation-integration`)
3. Implement in phases with testing at each stage
4. Document changes and update architecture docs
5. Merge to development with comprehensive testing

### For Medium/Low Priority Issues:
1. Log in this document with severity and impact
2. Schedule during Phase 4 (Quality Review)
3. Create GitHub issues if appropriate
4. Address in batch during dedicated quality sprints

---

## 📊 Issue Tracking Statistics

### Total Issues: 3
- ❌ Open High Priority: 1 (Deep Zoom Integration)
- ✅ Resolved: 2 (Render Button State, DI Container)

### Average Resolution Time:
- Critical bugs: < 1 day
- Feature gaps: 1-3 weeks (with planning)
- Code quality: Scheduled for Phase 4

---

**Last Updated**: January 2025 (Documentation Cleanup Session)  
**Next Review**: After Phase 3.5 (Perturbation Integration) completion
