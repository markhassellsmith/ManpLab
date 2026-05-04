# Fractal Knowledge Base Implementation Plan

## Overview
This document outlines the strategy for adding rich metadata to all 276+ fractals in ManpLab, making the application both educational and computational.

## Phase 1: Schema Extension ✅ COMPLETE

### Native Layer (C++)
Extended `FractalSpec` in `ManpCore.Native/FractalRegistry.h` with:
- `formula` - Plain text mathematical formula
- `formulaLatex` - LaTeX version for rendering
- `derivation` - Mathematical background/explanation
- `visualCharacteristics` - Visual description
- `discoveredBy` - Historical attribution
- `discoveryYear` - When discovered/published
- `computationalNotes` - Performance/precision considerations
- `suggestedViewpoints` - Interesting coordinates (vector of strings)
- `relatedFractals` - Related fractals (vector of strings)
- `references` - Papers, links, etc. (vector of strings)

### Managed Layer (C++/CLI)
Extended `FractalInfo` in `ManpCore.Native/FractalRegistryWrapper.h` with corresponding managed properties:
- All string fields as `String^`
- All vector fields as `List<String^>^`
- Added `PopulateFractalInfo()` helper for clean marshalling

### Build Status
✅ Native and managed layers compile successfully
✅ Extended metadata flows from C++ to C# seamlessly

---

## Phase 2: JSON Knowledge Base 🚧 IN PROGRESS

### Files Created
1. **`ManpWinUI/Assets/FractalKnowledge/schema.json`**
   - JSON Schema definition for validation
   - Documents all metadata fields and types

2. **`ManpWinUI/Assets/FractalKnowledge/fractals_sample.json`**
   - Sample with 3 fully documented fractals:
     - Mandelbrot Set
     - Burning Ship
     - Julia - San Marco
   - Use as template for quality/format

3. **`ManpWinUI/Assets/FractalKnowledge/fractals_template.json`**
   - All 276 fractals extracted from `Fractype.h`
   - Auto-categorized by name patterns
   - Contains TODO placeholders for metadata

4. **`ManpWinUI/Scripts/Extract-FractalMetadata.ps1`**
   - PowerShell script to regenerate template
   - Extracts from legacy codebase
   - Auto-categorizes by naming convention

### Extraction Results
- **276 fractal types** found
- Categories discovered:
  - Classic Fractals: 15
  - Julia Sets: 4
  - Mandelbrot Variants: 2
  - Lambda Fractals: 7
  - Newton Fractals: 8
  - Barnsley Fractals: 12
  - Trigonometric Fractals: 20
  - 3D Attractors: 8
  - Magnet Fractals: 6
  - Phoenix Fractals: 8
  - IFS: 1
  - Other: 180

---

## Phase 3: Bulk Population Strategy

### Option A: AI-Assisted Batch Generation (Recommended)
Use AI (Claude, GPT-4) to populate metadata in batches:

1. **Extract formula from legacy code**
   - Search for calculation functions in `ManpWIN64/*.cpp`
   - Extract iteration logic

2. **Generate descriptions with AI**
   ```
   Prompt: "For the fractal '{name}' with formula '{formula}', 
           provide: description, visual characteristics, 
           historical context, and computational notes."
   ```

3. **Manual review and refinement**
   - Verify mathematical accuracy
   - Add specific viewpoints from experience
   - Cross-reference with literature

### Option B: Community-Sourced
- Create GitHub wiki with fractal list
- Invite contributions from fractal enthusiasts
- Review and merge via pull requests

### Option C: Hybrid Approach
1. AI generates initial metadata for all 276
2. Mark reviews and refines top 50 most important
3. Community contributes to the rest over time

### Prioritization
**Tier 1: Essential (20 fractals)** - Fully document first
- Mandelbrot, Julia sets, Burning Ship
- Newton, Phoenix, Magnet families
- Classic 3D attractors

**Tier 2: Popular (50 fractals)** - Document next
- All Mandelbrot/Julia variants
- Trigonometric families
- Barnsley ferns

**Tier 3: Comprehensive (206 fractals)** - Document over time
- Exotic/experimental types
- Historical/academic interest
- Variations and hybrids

---

## Phase 4: Registry Integration

### Implement JSON Loader
Create `FractalKnowledgeLoader` service:

```cpp
// Native layer
class FractalKnowledgeLoader
{
public:
    static void LoadFromJson(const std::string& jsonPath);
    static void MergeIntoRegistry();  // Populate extended fields
};
```

```csharp
// Managed layer
public class FractalKnowledgeService
{
    public static void LoadKnowledge(string jsonPath);
    public static void EnrichRegistry();  // Call native loader
}
```

### Load at Startup
In `App.xaml.cs`:
```csharp
private void InitializeFractalRegistry()
{
    // Initialize native registry (existing)
    ManpCore.Native.FractalRegistryWrapper.Initialize();

    // Load extended metadata from JSON (new)
    var jsonPath = Path.Combine(AppContext.BaseDirectory, 
        "Assets", "FractalKnowledge", "fractals.json");
    FractalKnowledgeService.LoadKnowledge(jsonPath);

    Logger.Info($"Loaded {count} fractals with enriched metadata");
}
```

---

## Phase 5: UI Integration

### 5.1: Browser Enhancements
Add "Info" button to each fractal in browser:
```xaml
<Button Command="{Binding ShowFractalInfoCommand}"
        CommandParameter="{Binding Name}"
        Content="ℹ️" />
```

### 5.2: Fractal Info Panel
Create new `FractalInfoView.xaml`:
- **Header**: Display name, category, discovery info
- **Formula Section**: Render LaTeX if available, else plain text
- **Description**: Full derivation and visual characteristics
- **Exploration Section**: Suggested viewpoints as clickable buttons
- **Related Fractals**: Links to similar fractals
- **References**: External links (Wikipedia, papers)

### 5.3: In-Render Overlay
Add optional overlay during rendering:
- Formula displayed in corner
- Quick facts tooltip
- "Learn More" button

---

## Phase 6: Advanced Features (Future)

### 6.1: LaTeX Rendering
- Integrate `MathJax` or `KaTeX` via WebView2
- Render beautiful mathematical formulas

### 6.2: Interactive Formula Editor
- Allow users to modify formulas
- Custom fractal creation

### 6.3: Educational Mode
- Guided tours of fractals
- Step-by-step iteration visualization
- Mathematical explanations

### 6.4: Export/Share
- Export fractal with metadata as PDF
- Share discoveries with community

---

## Implementation Timeline

### Week 1: Foundation ✅
- [x] Extend native schema
- [x] Extend managed wrapper
- [x] Build verification
- [x] Create JSON schema
- [x] Extract all fractal types

### Week 2: Population (Current)
- [ ] Document Tier 1 (20 fractals)
- [ ] Create JSON loader service
- [ ] Integrate with registry startup

### Week 3: UI Integration
- [ ] Add Info button to browser
- [ ] Create FractalInfoView panel
- [ ] Wire up navigation

### Week 4: Polish & Testing
- [ ] LaTeX rendering (optional)
- [ ] User testing
- [ ] Documentation
- [ ] Document Tier 2 (50 fractals)

---

## File Organization

```
ManpWinUI/
├── Assets/
│   └── FractalKnowledge/
│       ├── schema.json                 # JSON Schema
│       ├── fractals_sample.json        # Examples (3 fractals)
│       ├── fractals_template.json      # All 276 (TODO placeholders)
│       ├── fractals_tier1.json         # Top 20 (fully documented) [TODO]
│       ├── fractals_tier2.json         # Next 50 [TODO]
│       └── fractals_complete.json      # All 276 merged [TODO]
├── Scripts/
│   ├── Extract-FractalMetadata.ps1     # Extraction tool
│   ├── Merge-FractalJson.ps1           # Merge tier files [TODO]
│   └── Validate-FractalJson.ps1        # JSON Schema validator [TODO]
└── Services/
    └── FractalKnowledgeService.cs      # Loader service [TODO]
```

---

## Next Steps

1. **Review `fractals_template.json`** - Inspect the 276 extracted fractals
2. **Choose population strategy** - AI-assisted, community, or hybrid?
3. **Start Tier 1 documentation** - Pick 20 most important fractals
4. **Create loader service** - Build infrastructure to load JSON at startup
5. **Test with sample data** - Verify the pipeline works end-to-end

---

## Success Metrics

- ✅ All 276 fractals have at least description + formula
- ✅ Top 50 fractals have full documentation
- ✅ Educational panel accessible from browser
- ✅ LaTeX formulas render beautifully
- ✅ Users report learning while exploring

---

## Questions for Mark

1. **Population strategy**: Should we use AI to bulk-generate initial metadata, then refine manually?
2. **Prioritization**: Which 20 fractals should be in Tier 1?
3. **UI placement**: Info panel as flyout, sidebar, or separate window?
4. **LaTeX rendering**: Worth the complexity, or stick to plain text formulas?
5. **Timeline**: Is 4 weeks realistic, or should we phase this longer-term?
