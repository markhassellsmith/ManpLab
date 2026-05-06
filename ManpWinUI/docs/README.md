# ManpWinUI Documentation Index

**Last Updated**: January 2025  
**Purpose**: Guide to essential project documentation

---

## 🚀 **Start Here**

If you're new to the project or resuming work, start with these:

1. **`RESUME_HERE.md`** (in ManpWinUI root) - Current session state and next steps
2. **`PROJECT_PLAN.md`** - Master project plan and roadmap
3. **`ROADMAP_STATUS_UPDATE.md`** - Current feature implementation status

---

## 📋 **Documentation Categories**

### 🎯 **Planning & Progress**

| Document | Purpose |
|----------|---------|
| **PROJECT_PLAN.md** | Master project plan with phases and milestones |
| **ROADMAP_STATUS_UPDATE.md** | Current status of all planned features |
| **FEATURE_VERIFICATION_SUMMARY.md** | Feature completion verification (Animation, Navigation, Status Bar, Fractals) |
| **PROGRESS_SUMMARY.md** | Overall project progress summary |
| **KNOWN_ISSUES.md** | Active bugs and technical debt tracking |

---

### 🏗️ **Architecture & Technical Design**

| Document | Purpose |
|----------|---------|
| **ARCHITECTURE_NATIVE_ENGINE.md** | C++/C# interop architecture and native integration |
| **RENDERING_ABSTRACTION_SUMMARY.md** | Rendering pipeline architecture (in Services folder) |
| **DEVELOPER_INFRASTRUCTURE_SUMMARY.md** | Developer tooling, build system, and workflows |
| **FRACTAL_DEVELOPER_INFRASTRUCTURE.md** | Fractal-specific development infrastructure |

---

### 🎨 **Feature Documentation**

#### Animation System
- **ANIMATION_FEATURE_PLAN.md** - Complete animation roadmap and Phase 1 implementation details
  - MainViewModel integration pattern
  - Singleton/Transient lifetime management
  - Tab-switch state persistence
  - Settings interdependencies (zoom speed presets)
  - Smart default filenames

#### Fractals
- **FRACTAL_REGISTRY_PROGRESS.md** - Implementation status (115/276 fractals, 41.7%)
- **README_FRACTAL_DEVELOPMENT.md** - Guide to adding new fractals
- **Hailstone.md** - Hailstone sequence fractal documentation
- **FRACTAL_METADATA_SUMMARY.md** - Metadata system for fractal properties
- **METADATA_POPULATION_MAINTENANCE.md** - Metadata maintenance guide

#### Deep Zoom
- **DEEP_ZOOM_INTEGRATION_PLAN.md** - Perturbation theory integration plan (12-17 days)
- **DEEP_ZOOM_THRESHOLD_FIX.md** - Deep zoom threshold fix documentation

#### Other Features
- **BatchRenderer-Guide.md** - Batch rendering system documentation
- **FRACTAL_KNOWLEDGE_BASE_PLAN.md** - Knowledge base system planning

---

### 🐛 **Bug Fixes & Technical Solutions**

| Document | Purpose |
|----------|---------|
| **BLA_IMAGE_DIMENSION_FIX.md** | Bilinear approximation image dimension fix |
| **DEEP_ZOOM_THRESHOLD_FIX.md** | Deep zoom auto-enable threshold fix |
| **NEXT_ACTION_LINKER_FIX.md** | Linker configuration fix documentation |
| **KNOWN_ISSUES.md** | Current known issues and workarounds |

---

### 🔧 **Integration & Export**

| Document | Purpose |
|----------|---------|
| **FILE_EXPORT_TESTING.md** | File export testing procedures (PNG, JPEG, SVG) |
| **MPEG_INTEGRATION.md** | MPEG video integration notes (legacy encoder) |

---

### 📐 **Developer Standards**

**Root `docs` folder** (for repository-wide standards):

| Document | Purpose |
|----------|---------|
| **PARAMETER_NAMING_CONVENTIONS.md** | Parameter naming standards for fractals |
| **FRACTAL_VISUAL_VALIDATION.md** | Visual validation procedures for fractal correctness |
| **DEPLOYMENT_FIX.md** | Deployment troubleshooting guide |
| **MSIX_DEPLOYMENT.md** | MSIX packaging and deployment guide |
| **HAILSTONE.md** | Root Hailstone documentation (repository-level) |

---

### 🛠️ **Diagnostic Scripts**

| Script | Purpose |
|--------|---------|
| **CleanRebuild-DeepZoom.ps1** | Clean rebuild for deep zoom changes |
| **DeepZoom-Diagnostic.ps1** | Deep zoom diagnostic and troubleshooting |

---

## 🎯 **Quick Reference by Task**

### "I want to add a new fractal"
1. Read **README_FRACTAL_DEVELOPMENT.md**
2. Check **FRACTAL_REGISTRY_PROGRESS.md** for what exists
3. Follow **PARAMETER_NAMING_CONVENTIONS.md**
4. Test with **FRACTAL_VISUAL_VALIDATION.md**

### "I want to understand the animation system"
1. Read **ANIMATION_FEATURE_PLAN.md** → Phase 1 Implementation Details
2. Review **FEATURE_VERIFICATION_SUMMARY.md** → Animation section

### "I want to implement deep zoom"
1. Read **DEEP_ZOOM_INTEGRATION_PLAN.md**
2. Check **ARCHITECTURE_NATIVE_ENGINE.md** for C++ integration
3. Use **CleanRebuild-DeepZoom.ps1** for builds

### "I want to fix a build/deployment issue"
1. Check **KNOWN_ISSUES.md**
2. Review **DEPLOYMENT_FIX.md** and **MSIX_DEPLOYMENT.md**
3. Check specific fix documents (BLA, linker, threshold)

### "I want to know what's next"
1. Read **RESUME_HERE.md** (in ManpWinUI root)
2. Check **ROADMAP_STATUS_UPDATE.md**
3. Review **PROJECT_PLAN.md**

---

## 📊 **Documentation Statistics**

- **Total Documents**: 27 markdown files (after cleanup)
- **Primary Planning**: 5 docs
- **Architecture**: 4 docs
- **Features**: 11 docs
- **Bug Fixes**: 4 docs
- **Standards**: 5 docs
- **Scripts**: 2 PowerShell files

---

## 🎉 **Recent Cleanup**

**Date**: January 2025  
**Action**: Removed 33 obsolete/redundant documents (55% reduction)

**What was removed**:
- Obsolete phase/week progress reports (14 files)
- Redundant animation fix docs → consolidated into ANIMATION_FEATURE_PLAN.md (4 files)
- Duplicate deep zoom documents (3 files)
- Superseded fractal expansion summaries (6 files)
- Root docs folder duplicates (4 files)
- Temporary test/commit files (3 files)

See **FINAL_CLEANUP_SUMMARY.md** for details.

---

## 📝 **Maintenance Guidelines**

### When to Create New Documentation
- ✅ New major feature (create `{FEATURE}_PLAN.md`)
- ✅ Important architectural decision (add to ARCHITECTURE or create new)
- ✅ Complex bug fix with lessons learned (create fix doc)
- ✅ New developer tooling (update DEVELOPER_INFRASTRUCTURE)

### When to Update Existing Documentation
- ✅ Feature completion (update ROADMAP_STATUS_UPDATE.md, FEATURE_VERIFICATION)
- ✅ New fractals added (update FRACTAL_REGISTRY_PROGRESS.md)
- ✅ Known issue resolved (remove from KNOWN_ISSUES.md)
- ✅ Session resume (update RESUME_HERE.md)

### When to Delete Documentation
- ❌ Phase/week progress reports after consolidation
- ❌ Temporary commit message files
- ❌ Test result summaries (keep only in version control history)
- ❌ Duplicate documents (keep one authoritative version)
- ❌ Superseded plans (mark as obsolete in doc, then delete after major milestone)

---

## 🔗 **External Resources**

- **GitHub Repository**: https://github.com/markhassellsmith/ManpLab
- **Project Wiki**: (TBD)
- **Issue Tracker**: GitHub Issues

---

**For questions or documentation updates, see RESUME_HERE.md or contact the maintainer.**
