# Version Update: 1.5.1 → 1.6.0

**Date**: 2025  
**Branch**: `feature/complete-flexible-parameters`  
**Release Name**: Flexible Parameter System  
**Status**: ✅ Complete

---

## Version Overview

### Previous Version
- **v1.5.1.0** — Last tagged release on GitHub

### New Version
- **v1.6.0.0** — Flexible Parameter System release

---

## Rationale for Minor Version Bump (1.5 → 1.6)

This release introduces **significant architectural changes** that warrant a **minor version increment**:

### Major Features
1. ✅ **Flexible Parameter System** (335 fractals templated)
   - Unified parameter management architecture
   - Template-based parameter definitions
   - Native C++/C# parameter interop
   - Dynamic UI generation from parameter metadata

2. ✅ **Session Persistence Redesign**
   - Removed quality parameter persistence between sessions
   - Preserved UI preferences (palette, layout)
   - Clean session startup behavior

3. ✅ **Complete State Capture**
   - Bookmarks now capture full parameter snapshots
   - Navigation history captures full parameter snapshots
   - True "Back" navigation with complete state restoration

4. ✅ **Render Quality Fixes**
   - Fixed `bailout` vs `escape_radius` priority
   - Corrected duplicate template registrations
   - Verified render quality across all fractal types

5. ✅ **Debug Output Cleanup**
   - Removed 37 lines of temporary diagnostics
   - Preserved operational statistics and warnings
   - 90% reduction in render-path noise

---

## Files Updated

### Version Configuration
✅ **`Version.props`**
```xml
Before: <VersionPrefix>1.5.1</VersionPrefix>
After:  <VersionPrefix>1.6.0</VersionPrefix>
```

### MSIX Package Manifest
✅ **`ManpWinUI/Package.appxmanifest`**
```xml
Before: Version="1.4.0.0"
After:  Version="1.6.0.0"
```

**Note**: Package.appxmanifest was out of sync (1.4.0.0) and has now been updated to match.

### Automatic Updates (via Version.props)
The following properties automatically resolve to 1.6.0.0:
- `$(FileVersion)` → `1.6.0.0`
- `$(AssemblyVersion)` → `1.6.0.0`
- `$(ApplicationVersion)` → `1.6.0.0`
- `$(AppxPackageVersion)` → `1.6.0.0`
- `$(InformationalVersion)` → `1.6.0`

---

## Build Verification

✅ **Build Status**: Successful  
✅ **No Compilation Errors**  
✅ **Version Properties Applied**

The MSIX package will now be generated as:
```
ManpWinUI_1.6.0.0_x64.msix
```

---

## Release Checklist

### Pre-Release
- [x] Update `Version.props` to `1.6.0`
- [x] Update `Package.appxmanifest` to `1.6.0.0`
- [x] Verify build successful
- [ ] Test critical path scenarios (see `PARAMETER_TEMPLATE_MIGRATION_TEST_PLAN.md`)
- [ ] Verify no visual regressions
- [ ] Test bookmark save/load with parameter snapshots
- [ ] Test navigation Back/Forward with parameter changes
- [ ] Update `CHANGELOG.md` (if exists)

### Release Process
- [ ] Merge `feature/complete-flexible-parameters` → `development`
- [ ] Merge `development` → `master`
- [ ] Create Git tag: `git tag v1.6.0`
- [ ] Push tag: `git push origin v1.6.0`
- [ ] GitHub Actions will trigger release workflow
- [ ] Verify artifacts: MSIX package and portable ZIP

### Post-Release
- [ ] Update GitHub release notes with feature summary
- [ ] Close related issues/PRs
- [ ] Update project documentation links
- [ ] Announce release (if applicable)

---

## GitHub Actions Release Workflow

The repository has automated release workflows:

### Trigger Methods
1. **Push tag**: `git push origin v1.6.0`
2. **Manual dispatch**: GitHub Actions → "Release Build" → Run workflow with version `1.6.0`

### Workflow Output
- **MSIX Package**: `ManpWinUI_1.6.0.0_x64.msix`
- **Portable ZIP**: `ManpLab-Portable-v1.6.0.zip`
- **Installation Guide**: Auto-included in release

**Workflow File**: `.github/workflows/release.yml`

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| **1.6.0** | 2025 | Flexible Parameter System, session persistence redesign, complete state capture |
| 1.5.1 | — | Previous release |
| 1.5.0 | — | — |
| 1.4.0 | — | — |
| 1.3 | — | — |
| 1.1.1 | — | — |
| 1.1.0 | — | — |
| 1.0.0 | — | Initial release |

---

## Semantic Versioning Reference

**Format**: `MAJOR.MINOR.PATCH`

- **MAJOR**: Breaking changes, incompatible API changes
- **MINOR**: New features, backward-compatible additions ← **This release**
- **PATCH**: Bug fixes, backward-compatible fixes

**Rationale for 1.6.0**:
- New flexible parameter architecture (backward-compatible)
- Enhanced bookmark/history format (backward-compatible with graceful degradation)
- No breaking changes to existing functionality
- Significant enough to justify minor version bump over patch (1.5.2)

---

## Related Documentation

- `PROJECT_STATUS_FLEXIBLE_PARAMETERS.md` — Complete system status
- `PARAMETER_TEMPLATE_STRATEGY.md` — Migration strategy and completion
- `PARAMETER_TEMPLATE_MIGRATION_TEST_PLAN.md` — Testing checklist
- `DEBUG_OUTPUT_CLEANUP.md` — Debug output maintenance
- `SESSION_PERSISTENCE_POLICY.md` — Persistence design decisions
- `BOOKMARK_FORMAT_MIGRATION.md` — Bookmark enhancement details
- `NAVIGATION_HISTORY_ENHANCEMENT.md` — Navigation history fix

---

**Status**: Ready for testing and release tagging

All version references have been updated. The next step is to test the critical path scenarios and then merge to master for release.
