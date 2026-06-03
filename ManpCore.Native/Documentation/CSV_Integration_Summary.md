# CSV to Data Store Integration Summary

## Date
$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

## Objective
Integrate user-provided initial conditions from `FractalRegistry.csv` into the service-backed data store (`InitialConditions.txt`), ensuring CSV values are authoritative where provided.

## Current Status

### Data Store
- **Total entries**: 294 (matches code registrations)
- **CSV-sourced values**: 142 fractals
- **Default fallback values**: 152 fractals

### CSV Mapping
- **Total CSV rows**: 296
- **CSV rows with internal names mapped**: 191
- **CSV rows with complete initial conditions**: 142
- **CSV rows unmapped**: 103 (35%)
- **Match rate**: 65%

### File Locations
- CSV with internal names: `ManpWinUI\Documentation\Architecture\FractalRegistry_WithInternalNames.csv`
- Data store: `ManpCore.Native\Resources\InitialConditions.txt`
- Original CSV: `ManpWinUI\Documentation\Architecture\FractalRegistry.csv`

## What Was Accomplished

### 1. Internal Name Column Added to CSV
Created `FractalRegistry_WithInternalNames.csv` with a new "Internal Name" column that maps:
- CSV descriptive names (e.g., "Lorenz Attractor")
- To code internal names (e.g., "Lorenz")

**Matching strategies used:**
- Exact display name match
- Case-insensitive matching
- Pattern matching for "Julia - X" -> "JuliaX"
- Parenthetical pattern handling "X (Y)" -> "XY", "X", "Y"
- Normalization (remove spaces, special characters)

**Results:**
- 191 of 296 CSV entries successfully matched to internal names
- 103 entries remain unmatched (marked as 'UNMATCHED')

### 2. Data Store Synchronized with CSV
The `InitialConditions.txt` file now contains:

**142 CSV-authoritative entries** where:
- CSV provided: descriptive name, Center X, Center Y, X Scale Width
- Zoom calculated as: `zoom = 3.0 / xScaleWidth`
- These values reflect your explicit tuning decisions

**152 default-fallback entries** where:
- CSV did not provide values OR internal name was not mapped
- Default values used: CenterX=0.00, CenterY=0.00, Zoom=1.0
- These need review/tuning

## Unmatched CSV Entries (Sample)

The following CSV entries could not be automatically matched to internal names:

1. Aizawa Attractor
2. Chen-Lee Attractor
3. Dadras Attractor
4. Halvorsen Attractor
5. Pickover Attractor
6. Thomas Attractor
7. Mandelbrot Parameter Space
8. May-Lyapunov Reference
9. Burning Ship (Power 3)
10. Burning Ship (Power 4)
... and 93 more

**Likely reasons:**
- CSV descriptive name doesn't exactly match code's `displayName`
- CSV name is a variation or alias
- CSV contains fractals not yet implemented in code
- Code contains fractals not yet documented in CSV

## Scripts Created

### 1. `AddInternalNamesToCSV.ps1`
- Initial CSV enhancer
- Created the "Internal Name" column

### 2. `SmartMatchInternalNames.ps1`
- Pattern-based matching
- Handles common naming conventions

### 3. `BuildNameMappingFromCode.ps1`
- Extracts `spec.name` and `spec.displayName` from code
- Creates authoritative code-to-display mapping

### 4. `ComprehensiveNameMatcher.ps1`
- Multi-strategy matching algorithm
- Achieved 65% match rate

### 5. `SyncInitialConditionsFromCSV.ps1`
- Syncs CSV values into data store
- Preserves CSV order (UI panel organization)

### 6. `BuildCompleteDataStore.ps1` ⭐
- **Final comprehensive script**
- Ensures all 294 registered fractals have entries
- Priority: CSV > Git store > Defaults

### 7. `GenerateMappingReviewReport.ps1`
- Creates human-readable comparison report
- Lists unmatched entries and available code names

## Data Quality Assessment

### CSV Values (142 fractals) ✅
These reflect your explicit tuning and are now authoritative in the data store:
- Arneodo (0.00, 0.00, zoom=2.075)
- Barnsley Fern (0.00, 5.75, zoom=0.153)
- Bifurcation-Mandelbrot (1.00, 0.00, zoom=0.451)
- ... and 139 more

### Default Values (152 fractals) ⚠️
These fractals currently use placeholder defaults and may need tuning:
- All values set to (0.00, 0.00, zoom=1.0)
- Include important fractals like:
  - Mandelbrot Set
  - Unity
  - Lorenz Attractor
  - Many Julia presets
  - Many burning ship variants

## Recommendations

### Short Term
1. ✅ **DONE**: All 294 fractals now have entries in data store
2. ✅ **DONE**: 142 CSV-provided values are now authoritative
3. ⚠️ **TODO**: Review and populate the 152 fractals using defaults

### Medium Term
1. **Manual Review**: For the 103 unmatched CSV entries:
   - Determine correct internal names by code inspection
   - Update `FractalRegistry_WithInternalNames.csv`
   - Re-run `BuildCompleteDataStore.ps1`

2. **CSV Completion**: For fractals currently using defaults:
   - Add explicit values to CSV
   - Re-sync data store

### Long Term
1. **CSV as Source of Truth**:
   - Maintain CSV with all 294 fractals
   - Include internal name column
   - Use as authoritative source for initial conditions

2. **Automated Validation**:
   - Script to verify all code registrations have CSV entries
   - Script to detect CSV entries not in code
   - CI/CD integration

## Files Modified

- `ManpCore.Native\Resources\InitialConditions.txt` - Updated with 294 entries
- `ManpWinUI\Documentation\Architecture\FractalRegistry_WithInternalNames.csv` - Created

## Files Created (Scripts)

All in `ManpCore.Native\Scripts\`:
- `AddInternalNamesToCSV.ps1`
- `SmartMatchInternalNames.ps1`
- `BuildNameMappingFromCode.ps1`
- `ComprehensiveNameMatcher.ps1`
- `SyncInitialConditionsFromCSV.ps1`
- `BuildCompleteDataStore.ps1` ⭐ (recommended for future use)
- `GenerateMappingReviewReport.ps1`
- `CreateManualMappingTemplate.ps1`
- `SuggestInternalNames.ps1`

## Next Steps

1. **Verify Build**: Confirm application builds and runs with new data store
2. **Test Rendering**: Spot-check that CSV-specified fractals render at correct initial view
3. **Review Defaults**: Identify which of the 152 default-valued fractals need tuning
4. **Complete CSV**: Add internal names for unmapped entries
5. **Re-sync**: Run `BuildCompleteDataStore.ps1` after CSV updates

## Conclusion

✅ **Success**: The data store now contains all 294 registered fractals  
✅ **Success**: Your 142 explicit CSV values are now authoritative  
⚠️ **Action Required**: 152 fractals use placeholder defaults and need proper values  
📝 **Reference**: This document + scripts provide full traceability and reproducibility

---
Generated by CSV-to-DataStore integration workflow
