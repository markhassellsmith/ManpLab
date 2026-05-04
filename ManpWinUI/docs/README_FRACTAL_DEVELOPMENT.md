# ManpLab Fractal Development - Documentation Index

## 📖 Overview

This index provides quick access to all fractal development documentation. Choose the guide that matches your needs and experience level.

---

## 🚀 Getting Started (Choose Your Path)

### ⚡ I Want to Add a Fractal Right Now (5 Minutes)
→ **Start Here**: [`FRACTAL_QUICK_REFERENCE.md`](../../ManpCore.Native/FRACTAL_QUICK_REFERENCE.md)
- Minimal code template
- One-page reference
- Quick commands
- Printable format

### 📚 I Want to Learn the System (30 Minutes)
→ **Start Here**: [`ADDING_FRACTALS.md`](../../ManpCore.Native/ADDING_FRACTALS.md)
- Complete step-by-step guide
- Architecture explanation
- 20+ code examples
- Troubleshooting section

### 🏗️ I Want to Understand the Infrastructure (1 Hour)
→ **Start Here**: [`FRACTAL_DEVELOPER_INFRASTRUCTURE.md`](FRACTAL_DEVELOPER_INFRASTRUCTURE.md)
- System architecture
- Component descriptions
- Advanced features
- Best practices

### 🎓 I Want to Add Rich Metadata (Ongoing)
→ **Start Here**: [`FRACTAL_KNOWLEDGE_BASE_PLAN.md`](FRACTAL_KNOWLEDGE_BASE_PLAN.md)
- 6-phase implementation plan
- JSON schema reference
- Population strategies
- Educational content guide

---

## 📁 Complete File Reference

### Core Documentation

| File | Purpose | Length | Audience |
|------|---------|--------|----------|
| **[FRACTAL_QUICK_REFERENCE.md](../../ManpCore.Native/FRACTAL_QUICK_REFERENCE.md)** | Quick reference card | 1 page | All developers |
| **[ADDING_FRACTALS.md](../../ManpCore.Native/ADDING_FRACTALS.md)** | Detailed development guide | 6,000 words | New contributors |
| **[FRACTAL_DEVELOPER_INFRASTRUCTURE.md](FRACTAL_DEVELOPER_INFRASTRUCTURE.md)** | Infrastructure overview | 4,000 words | Maintainers |
| **[DEVELOPER_INFRASTRUCTURE_SUMMARY.md](DEVELOPER_INFRASTRUCTURE_SUMMARY.md)** | Implementation summary | Summary | Project leads |

### Templates and Tools

| File | Purpose | Type |
|------|---------|------|
| **[FractalTemplate.cpp.template](../../ManpCore.Native/FractalTemplate.cpp.template)** | C++ implementation skeleton | Template |
| **[New-FractalFamily.ps1](../Scripts/New-FractalFamily.ps1)** | Automated generator script | PowerShell |
| **[Extract-FractalMetadata.ps1](../Scripts/Extract-FractalMetadata.ps1)** | Legacy extraction tool | PowerShell |

### Metadata System

| File | Purpose | Type |
|------|---------|------|
| **[schema.json](../Assets/FractalKnowledge/schema.json)** | JSON Schema definition | Schema |
| **[fractals_sample.json](../Assets/FractalKnowledge/fractals_sample.json)** | Example metadata (3 fractals) | JSON |
| **[fractals_tier1.json](../Assets/FractalKnowledge/fractals_tier1.json)** | **✅ 20 gold-tier fractals** | JSON |
| **[fractals_template.json](../Assets/FractalKnowledge/fractals_template.json)** | All 276 fractals extracted | JSON |

### Planning and History

| File | Purpose | Type |
|------|---------|------|
| **[FRACTAL_KNOWLEDGE_BASE_PLAN.md](FRACTAL_KNOWLEDGE_BASE_PLAN.md)** | Metadata implementation plan | Planning |
| **[METADATA_POPULATION_MAINTENANCE.md](METADATA_POPULATION_MAINTENANCE.md)** | **✅ Population & maintenance guide** | Guide |
| **[FRACTAL_METADATA_SUMMARY.md](FRACTAL_METADATA_SUMMARY.md)** | Metadata session summary | Summary |
| **[FRACTAL_TYPE_EXPANSION_TASK.md](FRACTAL_TYPE_EXPANSION_TASK.md)** | Original task definition | Planning |
| **[FRACTAL_TYPE_EXPANSION_SUCCESS.md](FRACTAL_TYPE_EXPANSION_SUCCESS.md)** | Success summary | Summary |

---

## 🎯 Use Cases

### Use Case 1: First-Time Contributor Adds a Fractal

**Path**:
1. Read `FRACTAL_QUICK_REFERENCE.md` (5 min)
2. Run generator script
3. Implement formula using template
4. Reference `ADDING_FRACTALS.md` as needed
5. Build and test

**Time**: ~15 minutes

---

### Use Case 2: Experienced Developer Adds Complex Fractal

**Path**:
1. Run generator with `-Interactive` mode
2. Study relevant examples in `ADDING_FRACTALS.md`
3. Implement formula + custom parameters
4. Add rich metadata to JSON
5. Test thoroughly

**Time**: ~30 minutes

---

### Use Case 3: Maintainer Extends Infrastructure

**Path**:
1. Review `FRACTAL_DEVELOPER_INFRASTRUCTURE.md`
2. Understand current architecture
3. Modify template or generator
4. Update documentation
5. Test with sample fractal
6. Update summary docs

**Time**: 1-2 hours

---

### Use Case 4: Community Member Contributes Metadata

**Path**:
1. Review `FRACTAL_KNOWLEDGE_BASE_PLAN.md`
2. Study `fractals_sample.json` examples
3. Fill in metadata for chosen fractals
4. Validate against `schema.json`
5. Submit pull request

**Time**: 10-15 minutes per fractal

---

## 📊 Documentation Map

```
Documentation Structure:

┌─────────────────────────────────────────────────────┐
│  Quick Reference (Print This!)                      │
│  • 1-page cheat sheet                               │
│  • Minimal code examples                            │
│  • Common commands                                  │
└───────────────────┬─────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────┐
│  Detailed Guide                                     │
│  • Step-by-step instructions                        │
│  • Complete field reference                         │
│  • Algorithm patterns                               │
│  • Troubleshooting                                  │
└───────────────────┬─────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────┐
│  Infrastructure Overview                            │
│  • System architecture                              │
│  • Component descriptions                           │
│  • Advanced features                                │
│  • Best practices                                   │
└───────────────────┬─────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────┐
│  Reference Implementations                          │
│  • MandelbrotFamily.cpp                             │
│  • NewtonFamily.cpp                                 │
│  • BurningShipFamily.cpp                            │
│  • (Study these for patterns)                       │
└─────────────────────────────────────────────────────┘
```

---

## 🛠️ Tool Reference

### Generator Script

```powershell
# Basic usage
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "MyFractal" -Category "My Category"

# Interactive mode
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "MyFractal" -Interactive

# Get help
Get-Help .\ManpWinUI\Scripts\New-FractalFamily.ps1 -Detailed
```

### Metadata Extraction

```powershell
# Extract all fractals from legacy code
.\ManpWinUI\Scripts\Extract-FractalMetadata.ps1
```

### JSON Validation

```powershell
# Install validator (one-time)
npm install -g ajv-cli

# Validate metadata
ajv validate -s Assets/FractalKnowledge/schema.json -d MyFractal_metadata.json
```

---

## 📚 Learning Path

### Level 1: Beginner (Day 1)
- [ ] Read `FRACTAL_QUICK_REFERENCE.md`
- [ ] Run generator script
- [ ] Modify template with simple formula
- [ ] Build and test
- [ ] Success: First fractal renders!

### Level 2: Intermediate (Week 1)
- [ ] Read `ADDING_FRACTALS.md` thoroughly
- [ ] Implement 3-5 fractals of increasing complexity
- [ ] Add custom parameters
- [ ] Implement Julia mode support
- [ ] Add metadata to JSON
- [ ] Success: Can add any escape-time fractal!

### Level 3: Advanced (Month 1)
- [ ] Study `FRACTAL_DEVELOPER_INFRASTRUCTURE.md`
- [ ] Implement complex multi-variant families
- [ ] Optimize performance
- [ ] Contribute to infrastructure
- [ ] Success: Expert contributor!

---

## 🎓 Code Examples by Algorithm Type

### Escape-Time Fractals
- **Mandelbrot**: `MandelbrotFamily.cpp` lines 18-42
- **Burning Ship**: `BurningShipFamily.cpp` lines 15-60
- **Tricorn**: `TricornFamily.cpp` lines 12-45

### Newton Method
- **Cubic**: `NewtonFamily.cpp` lines 18-55
- **Quartic**: `NewtonFamily.cpp` lines 60-95
- **Sin(z)**: `NewtonFamily.cpp` lines 100-140

### Trigonometric
- **MandelTrig**: `ClassicEscapeTimeFamily.cpp` lines 200-250
- **SinZ**: `ClassicEscapeTimeFamily.cpp` lines 255-300

### Phoenix Family
- **PhoenixM**: `PhoenixFamily.cpp` lines 15-60
- **PhoenixJ**: `PhoenixFamily.cpp` lines 65-110

### Multibrot
- **Multibrot3**: `MultibrotFamily.cpp` lines 18-65
- **Multibrot4**: `MultibrotFamily.cpp` lines 70-115

---

## 🔍 Quick Links

### Templates
- [C++ Template](../../ManpCore.Native/FractalTemplate.cpp.template)
- [JSON Template](../Assets/FractalKnowledge/fractals_template.json)

### Documentation
- [Quick Reference](../../ManpCore.Native/FRACTAL_QUICK_REFERENCE.md) - Start here!
- [Detailed Guide](../../ManpCore.Native/ADDING_FRACTALS.md)
- [Infrastructure](FRACTAL_DEVELOPER_INFRASTRUCTURE.md)

### Tools
- [Generator Script](../Scripts/New-FractalFamily.ps1)
- [Extractor Script](../Scripts/Extract-FractalMetadata.ps1)

### Metadata
- [JSON Schema](../Assets/FractalKnowledge/schema.json)
- [Sample Data](../Assets/FractalKnowledge/fractals_sample.json)
- [Knowledge Plan](FRACTAL_KNOWLEDGE_BASE_PLAN.md)

### Source Code
- [FractalRegistry.h](../../ManpCore.Native/FractalRegistry.h)
- [FractalRegistry.cpp](../../ManpCore.Native/FractalRegistry.cpp)
- [Example: Mandelbrot](../../ManpCore.Native/MandelbrotFamily.cpp)
- [Example: Newton](../../ManpCore.Native/NewtonFamily.cpp)

---

## 🆘 Need Help?

### I'm stuck on...

**...understanding the architecture**
→ Read: `FRACTAL_DEVELOPER_INFRASTRUCTURE.md`

**...implementing my iteration formula**
→ Read: `ADDING_FRACTALS.md` → "Calculator Function Guide" → "Algorithm Patterns"

**...adding custom parameters**
→ Read: `ADDING_FRACTALS.md` → "Custom Parameters"

**...getting Julia mode to work**
→ Read: `FRACTAL_QUICK_REFERENCE.md` → "Julia Mode Support"

**...build configuration**
→ Use generator script instead of manual setup

**...metadata format**
→ Study: `fractals_sample.json` for examples

### I want to...

**...add my first fractal quickly**
→ Start: `FRACTAL_QUICK_REFERENCE.md`

**...learn the system properly**
→ Start: `ADDING_FRACTALS.md`

**...contribute to infrastructure**
→ Start: `FRACTAL_DEVELOPER_INFRASTRUCTURE.md`

**...add educational content**
→ Start: `FRACTAL_KNOWLEDGE_BASE_PLAN.md`

---

## 📈 Statistics

- **7 documentation files** (15,000+ words)
- **2 PowerShell scripts** (500+ lines)
- **1 C++ template** (180 lines)
- **3 JSON assets** (schema + examples + 276 fractals)
- **20+ code examples** throughout docs
- **5 algorithm patterns** documented

**Result**: Complete end-to-end infrastructure for fractal development!

---

## ✅ Quick Verification

Before adding a fractal, make sure you have:
- [ ] Read at least the Quick Reference
- [ ] Generator script is accessible
- [ ] Solution builds successfully
- [ ] Existing fractals render correctly

After adding a fractal, verify:
- [ ] Appears in browser under correct category
- [ ] Renders without errors
- [ ] Julia mode works (if enabled)
- [ ] Metadata displays correctly
- [ ] Build successful with no warnings

---

## 🎉 Ready to Start!

**Choose your starting point above and begin adding fractals to ManpLab!**

**Questions?** Open an issue with tag `[Fractal Development]`

**Contributions?** Follow the guide and submit a PR!

**Happy fractal development! 🌀✨**

---

**Last Updated**: 2026-05-04  
**Version**: 1.0  
**Maintainer**: ManpLab Development Team
