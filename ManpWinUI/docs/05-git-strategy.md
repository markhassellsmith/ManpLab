# Git Versioning & Commit Strategy

## Branch Strategy

### Main Branches

```
main (or master)
├── Production-ready code (C++ ManpWIN64)
└── Stable releases only (tagged: v1.0.0, v1.1.0, etc.)

development ⭐ INTEGRATION/TESTING BRANCH
├── All completed work merges here FIRST
├── C++ bugfixes go here
├── Test integration before releasing to main
└── Base for ALL feature branches

feature/add-winui-interface (your current branch)
├── Long-running feature branch for WinUI development
├── Based on development
├── Regularly merges FROM development (to get C++ fixes)
└── Merges TO development when phases complete
```

**🎯 Key Workflow:**
```
1. C++ bugfix → commit to development
2. development → merge into feature/add-winui-interface (you get the fix)
3. Work on WinUI → commit to feature/add-winui-interface
4. Phase complete → merge feature/add-winui-interface → development (integration test)
5. All phases done → merge development → main (production release)
```

### Feature Branch Naming Convention

```
feature/add-winui-interface          # Main WinUI branch (YOUR BRANCH)
├── feature/phase1-planning          # Optional sub-branches per phase
├── feature/phase2-cpp-interop
├── feature/phase3-mvvm-foundation
├── feature/phase4-core-ui
├── feature/phase5-advanced-features
├── feature/phase6-file-operations
├── feature/phase7-polish
├── feature/phase8-testing
└── feature/phase9-deployment

hotfix/*                             # Critical C++ fixes during development
docs/*                               # Documentation updates
```

### 🚀 Setting Up `development` Branch

**First-Time Setup:**
```powershell
# 1. Make sure main is up to date
git checkout main
git pull origin main

# 2. Create development from main
git checkout -b development
git push origin development

# 3. Rebase your feature branch on development
git checkout feature/add-winui-interface
git rebase development

# 4. Set development as default base for future branches
git config branch.feature/add-winui-interface.merge refs/heads/development
```

**If `development` Already Exists:**
```powershell
# Just rebase your feature branch
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git rebase development
```

---

## Commit Strategy by Phase

### Phase 1: Planning & Analysis (10-15 commits)

**Branch:** `feature/add-winui-interface` (already created) or `feature/phase1-planning`

```
Commit examples:
✅ docs: add DESIGN_PLAN.md with project analysis
✅ docs: document C++ components to keep vs redesign
✅ docs: add interop strategy and architecture decisions
✅ docs: create file format specification
✅ docs: complete Phase 1 checklist

Commit pattern: docs: <what was documented>
```

**Merge point:** After Phase 1 complete → merge to `development` with tag `v0.1.0-planning`

```powershell
# After Phase 1 complete
git checkout development
git merge feature/add-winui-interface
git tag v0.1.0-planning
git push origin development --tags

# Continue working on feature branch
git checkout feature/add-winui-interface
```

---

### Phase 2: C++ Core Preparation (20-30 commits)

**Branch:** `feature/phase2-cpp-interop`

```
Commit examples:
feat(cpp): extract calculation engine interface
feat(cpp): create ManpCore.Native C++/CLI project
feat(interop): define FractalParameters data structure
feat(interop): implement Complex type marshalling
feat(interop): add BigDouble wrapper with tests
test(interop): benchmark C++/C# boundary performance
docs(api): document C++ interop API

Commit pattern: 
- feat(cpp): <C++ changes>
- feat(interop): <interop layer changes>
- test(interop): <tests>
```

**Merge point:** After Phase 2 complete → merge to `feature/add-winui-interface` with tag `v0.2.0-interop`

```powershell
# If using sub-branch for Phase 2
git checkout feature/add-winui-interface
git merge feature/phase2-cpp-interop
git tag v0.2.0-interop
git push origin feature/add-winui-interface --tags
```

---

### Phase 3: WinUI Foundation (15-25 commits)

**Branch:** `feature/phase3-mvvm-foundation`

```
Commit examples:
feat(winui): add CommunityToolkit.Mvvm package
feat(mvvm): create base ViewModelBase class
feat(di): configure dependency injection container
feat(ui): create MainWindow basic layout
feat(config): implement JSON settings service
feat(logging): add Serilog with file output
test(mvvm): add ViewModel unit tests

Commit pattern:
- feat(winui): <WinUI project setup>
- feat(mvvm): <MVVM infrastructure>
- feat(di): <dependency injection>
- feat(ui): <basic UI>
```

**Merge point:** After Phase 3 complete → merge to `feature/add-winui-interface` with tag `v0.3.0-foundation`

```powershell
git checkout feature/add-winui-interface
git merge feature/phase3-mvvm-foundation
git tag v0.3.0-foundation
git push origin feature/add-winui-interface --tags

# Then merge to development for integration testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

### Phase 4: Core UI Development (30-50 commits)

**Branch:** `feature/phase4-core-ui`

```
Commit examples:
feat(ui): add NavigationView with fractal categories
feat(ui): implement fractal display canvas
feat(ui): create parameter panel with NumberBox controls
feat(viewmodel): add FractalParametersViewModel
feat(ui): add real-time Julia preview
feat(ui): implement color palette editor
feat(ui): add rendering progress indicator
style(ui): apply Fluent Design theme
test(ui): add UI automation tests for navigation

Commit pattern:
- feat(ui): <UI component>
- feat(viewmodel): <ViewModel>
- style(ui): <styling>
```

**Merge point:** After Phase 4 complete → merge to `feature/add-winui-interface` with tag `v0.4.0-core-ui`

**Milestone:** 🎉 **First usable prototype** - can render basic fractals

```powershell
# Phase 4 complete - major milestone!
git checkout feature/add-winui-interface
git merge feature/phase4-core-ui
git tag v0.4.0-core-ui
git push origin feature/add-winui-interface --tags

# Merge to development for integration testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

### Phase 5: Advanced Features (25-40 commits)

**Branch:** `feature/phase5-advanced-features`

```
Commit examples:
feat(animation): add animation timeline control
feat(ui): implement zoom box selection with touch
feat(3d): integrate 3D visualization
feat(perturbation): add deep zoom UI controls
feat(bla): add BLA approximation settings panel
feat(slope): implement slope shading options

Commit pattern:
- feat(animation): <animation features>
- feat(perturbation): <deep zoom>
- feat(<feature>): <specific feature>
```

**Merge point:** After Phase 5 complete → merge to `feature/add-winui-interface` with tag `v0.5.0-advanced`

---

### Phase 6: File Operations (15-25 commits)

**Branch:** `feature/phase6-file-operations`

```
Commit examples:
feat(file): add WinUI file picker integration
feat(file): implement .MAP file import/export
feat(file): add .PAR (Fractint) compatibility
feat(file): implement .KFR (Kalles) reader
feat(export): add PNG export with metadata
feat(export): replace MPEG with H.264 encoder
feat(ui): add drag-and-drop file support
feat(ui): implement recent files menu

Commit pattern:
- feat(file): <file I/O>
- feat(export): <export functionality>
```

**Merge point:** After Phase 6 complete → merge to `feature/add-winui-interface` with tag `v0.6.0-files`

**Milestone:** 🎉 **Feature complete** - all major features implemented

```powershell
# Feature complete!
git checkout feature/add-winui-interface
git merge feature/phase6-file-operations
git tag v0.6.0-files
git push origin feature/add-winui-interface --tags

# Merge to development
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

### Phase 7: Polish & Features (20-35 commits)

**Branch:** `feature/phase7-polish`

```
Commit examples:
feat(ux): add keyboard shortcuts (Ctrl+R, Ctrl+Z, etc)
feat(ui): implement context menus
feat(undo): add undo/redo system
feat(presets): create fractal gallery
feat(share): integrate Windows Share contract
perf(ui): optimize touch/pen input
feat(theme): add dark/light theme toggle
a11y(ui): add screen reader support
i18n: prepare for localization

Commit pattern:
- feat(ux): <UX improvements>
- perf(ui): <performance>
- a11y(ui): <accessibility>
- i18n: <internationalization>
```

**Merge point:** After Phase 7 complete → merge to `feature/add-winui-interface` with tag `v0.7.0-polish`

---

### Phase 8: Testing & Optimization (20-30 commits)

**Branch:** `feature/phase8-testing`

```
Commit examples:
test(viewmodel): add comprehensive ViewModel tests
test(interop): add C++/C# integration tests
test(ui): expand UI automation coverage
perf: profile and optimize rendering pipeline
fix(memory): resolve memory leak in bitmap handling
test(stress): add deep zoom stress tests
test(thread): validate multi-threading safety
test(file): add cross-version file format tests

Commit pattern:
- test(<component>): <tests added>
- perf: <optimization>
- fix(<issue>): <bug fix>
```

**Merge point:** After Phase 8 complete → merge to `feature/add-winui-interface` with tag `v0.8.0-tested`

**Milestone:** 🎉 **Release Candidate** - ready for beta testing

```powershell
# Release candidate!
git checkout feature/add-winui-interface
git merge feature/phase8-testing
git tag v0.8.0-tested
git push origin feature/add-winui-interface --tags

# Final merge to development for pre-release testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

### Phase 9: Documentation & Deployment (10-20 commits)

**Branch:** `feature/phase9-deployment`

```
Commit examples:
docs: add user manual
docs: create developer interop guide
docs: write formula language reference
docs: add getting started tutorial
docs(api): complete XML documentation
build: add MSIX packaging configuration
build: configure release pipeline
ci: add GitHub Actions workflow
release: prepare v1.0.0

Commit pattern:
- docs: <documentation>
- build: <build/deployment>
- ci: <CI/CD>
- release: <release prep>
```

**Merge point:** After Phase 9 complete → merge to `development` then to `main` with tag `v1.0.0`

**Milestone:** 🎉 **Version 1.0 Release** - Production ready!

---

## Semantic Versioning Scheme

### During Development

- `v0.1.0-planning` - Phase 1 complete
- `v0.2.0-interop` - Phase 2 complete
- `v0.3.0-foundation` - Phase 3 complete
- `v0.4.0-core-ui` - Phase 4 complete (first prototype)
- `v0.5.0-advanced` - Phase 5 complete
- `v0.6.0-files` - Phase 6 complete (feature complete)
- `v0.7.0-polish` - Phase 7 complete
- `v0.8.0-tested` - Phase 8 complete (RC)
- `v0.9.0-beta1`, `v0.9.1-beta2`, etc. - Beta releases
- `v1.0.0` - First production release

### After Release

- `v1.0.x` - Patch releases (bug fixes)
- `v1.1.x` - Minor releases (new features, backward compatible)
- `v2.0.x` - Major releases (breaking changes)

---

## Commit Message Format (Conventional Commits)

### Format Template

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Formatting, missing semicolons, etc.
- `refactor`: Code restructuring
- `perf`: Performance improvement
- `test`: Adding tests
- `build`: Build system changes
- `ci`: CI/CD changes
- `chore`: Maintenance tasks
- `revert`: Revert a previous commit

### Scopes

- `cpp`: C++ code changes
- `interop`: C++/CLI wrapper
- `winui`: WinUI project
- `ui`: User interface
- `mvvm`: ViewModels/MVVM
- `di`: Dependency injection
- `file`: File I/O
- `export`: Export functionality
- `animation`: Animation system
- `perturbation`: Deep zoom
- `api`: API changes

### Examples

```
feat(interop): add BigComplex marshalling support

Implements two-way marshalling between C++ BigComplex
and managed Complex types with proper memory management.

Closes #42

---

fix(ui): resolve rendering flicker on zoom

The fractal display was flickering during zoom operations
due to incorrect bitmap buffer handling.

Fixes #108

---

perf(cpp): optimize perturbation iteration loop

Reduced memory allocations in hot path by 40%.
Benchmark shows 15% performance improvement.

---

docs: complete Phase 1 planning documentation

Added comprehensive analysis of C++ components,
architecture decisions, and implementation roadmap.
```

---

## Merge Strategy

### Option 1: Squash Merge (Recommended for small phases)

- Clean linear history
- One commit per phase in main branch
- Good for: Phases 1-3, 6, 9

### Option 2: Merge Commit (Recommended for large phases)

- Preserve detailed commit history
- Shows all work done
- Good for: Phases 4, 5, 7, 8

### Option 3: Rebase and Merge

- Linear history with detailed commits
- Requires careful rebasing
- Good for: Individual features within phases

---

## Handling Dual Codebase (ManpWIN64 + ManpWinUI)

### Directory Structure in Git

```
ManpLab/
├── ManpWIN64/              # Legacy C++ (keep maintained)
├── parser/                 # Shared C++ library
├── qdlib/                  # Shared C++ library
├── zlib/                   # Shared C++ library
├── MPEG/                   # Shared C++ library
├── ManpCore.Native/        # NEW: C++/CLI wrapper (Phase 2)
├── ManpWinUI/              # NEW: WinUI app (Phase 3+)
├── .github/workflows/      # CI/CD
├── docs/                   # Documentation
└── README.md
```

### Commit Guidelines for Shared C++ Code

```bash
# If you modify shared C++ libraries:
git commit -m "feat(cpp/parser): add new formula function

This change affects both ManpWIN64 and ManpWinUI.
Tested in both codebases.

Related: #feature/add-winui-interface"
```

### Syncing ManpWIN64 Improvements

```bash
# Apply C++ fixes to both codebases
git checkout development
git pull origin development

# Make fix to ManpWIN64
git commit -m "fix(cpp): resolve parser memory leak"
git push origin development

# Ensure it's available for WinUI branch
git checkout feature/add-winui-interface
git merge development  # Pull C++ fix into your feature branch
git push origin feature/add-winui-interface
```

**Why This Matters:**
- C++ fixes go to `development` (affects both old and new code)
- You merge `development` into your feature branch to get those fixes
- Keeps your WinUI work up-to-date with C++ improvements

---

## Recommended Daily Workflow

### Daily Work Cycle

```bash
# Start of work session
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface

# Get any new C++ fixes from development
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git merge development  # Integrate C++ fixes

# Create sub-branch for specific work (optional)
git checkout -b feature/phase4-parameter-panel

# Make changes, commit frequently
git add .
git commit -m "feat(ui): add parameter panel NumberBox controls"

# More work...
git commit -m "feat(viewmodel): bind parameters to FractalParametersViewModel"

# End of work session - push to remote
git push origin feature/phase4-parameter-panel

# When sub-feature complete, merge back
git checkout feature/add-winui-interface
git merge feature/phase4-parameter-panel
git push origin feature/add-winui-interface
```

### Daily Sync Checklist

- ✅ Pull latest feature/add-winui-interface
- ✅ Check development for new C++ fixes
- ✅ Merge development if needed
- ✅ Work on feature
- ✅ Commit frequently (every 15-30 min)
- ✅ Push at end of session

---

## Milestone Checklist

### Before Each Merge to `development`

- [ ] All phase tasks checked off in DESIGN_PLAN.md
- [ ] Code builds without errors
- [ ] Tests pass (if tests exist for that phase)
- [ ] Documentation updated
- [ ] README.md reflects current state
- [ ] Tag created with version number
- [ ] PR created with phase summary

### Before v1.0.0 Release

- [ ] All 9 phases complete
- [ ] Feature parity with ManpWIN64 achieved
- [ ] User documentation complete
- [ ] MSIX package created and tested
- [ ] Beta testing complete
- [ ] Known issues documented
- [ ] Migration guide written (C++ → WinUI)

---

## Summary

**Key Points:**
1. Use `development` as integration branch
2. Keep `main` for production releases only
3. Use feature branches for all work
4. Commit frequently with conventional commit messages
5. Tag each phase milestone
6. Sync with `development` regularly to get C++ fixes
7. Squash commits for clean history on main phases

**Remember:** Good commit hygiene makes collaboration easier, enables easy rollback, and provides clear project history. 🚀
