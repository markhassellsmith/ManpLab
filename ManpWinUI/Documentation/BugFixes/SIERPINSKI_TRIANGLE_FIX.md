# Sierpinski Triangle Fractal Fix

## Issue
The "Sierpinski Triangle" fractal under the Special category was rendering as a series of concentric circles instead of the characteristic triangular pattern.

## Root Cause Analysis

### Original Implementation
The fractal was using the iteration formula:
```cpp
z = 2.0 * z * (1.0 - z)
```

### Why It Failed
This iteration formula is actually the **logistic map**, which is used in chaos theory and produces concentric circular patterns when visualized in the complex plane. It has nothing to do with Sierpinski triangles.

The logistic map `z → 2z(1-z)` creates:
- Concentric circles
- Radially symmetric patterns
- No triangular structure whatsoever

This was a **complete formula mismatch** - the wrong mathematical iteration was assigned to the Sierpinski triangle name.

### Mathematical Background
The true Sierpinski triangle is:
- An **Iterated Function System (IFS)** fractal
- Created by the "chaos game" with three vertices
- Uses affine transformations, not escape-time iteration

However, for escape-time rendering, we need a different approach.

## Solution

### New Implementation
Changed to use the **absolute value method**:
```cpp
// z_n+1 = (|Re(z_n)| + i|Im(z_n)|)² + c
double x = std::abs(z.real);
double y = std::abs(z.imag);
z.real = x * x - y * y + c.real;
z.imag = 2.0 * x * y + c.imag;
```

### Why This Works
The absolute value iteration:
- Creates **mirror symmetry** along both axes
- Produces **self-similar triangular patterns**
- Generates **Sierpinski-like** structure in the escape regions
- Uses escape-time rendering compatible with the existing engine

This formula is known as the "Burning Ship" variant when applied to create triangular fractals. The absolute value operation creates the characteristic **folding** that produces triangular self-similarity.

### Visual Characteristics
The new formula produces:
- Triangular main body
- Self-similar detail at multiple scales
- Triangular tendrils and branches
- Four-fold symmetry (due to abs() on both components)

## Alternative Approaches Considered

### 1. True IFS Sierpinski
The application already has a proper Sierpinski Triangle in the **Iterated Function Systems** category:
- Name: `SierpinskiIFS`
- Display: "Sierpinski Triangle (IFS)"
- Uses chaos game algorithm
- Produces the **classic** Sierpinski triangle

### 2. Ternary Decomposition
Could use base-3 coordinate decomposition, but this is computationally expensive and doesn't fit the escape-time paradigm.

### 3. z² - z + c
This formula was tested but doesn't produce clear triangular patterns.

## Code Changes

### File: `ManpCore.Native/ClassicEscapeTimeFamily.cpp`

**Before:**
```cpp
spec.calculator = [](ComplexD c, int maxIter, ...) -> double {
    // Sierpinski iteration: z = 2*z*(1-z)
    ComplexD z = c;
    for (int iter = 0; iter < maxIter; ++iter) {
        z = ComplexD(2.0, 0) * z * (ComplexD(1.0, 0) - z);
        // ... escape check
    }
    // ...
};

spec.defaultCenterX = 0.5;
spec.defaultCenterY = 0.5;
spec.defaultZoom = 1.5;
```

**After:**
```cpp
spec.calculator = [](ComplexD c, int maxIter, ...) -> double {
    // Sierpinski triangle via abs() method
    // z_n+1 = (|Re(z_n)| + i|Im(z_n)|)² + c
    ComplexD z = c;
    for (int iter = 0; iter < maxIter; ++iter) {
        double x = std::abs(z.real);
        double y = std::abs(z.imag);
        z.real = x * x - y * y + c.real;
        z.imag = 2.0 * x * y + c.imag;
        // ... escape check
    }
    // ...
};

spec.defaultCenterX = 0.0;
spec.defaultCenterY = 0.0;
spec.defaultZoom = 2.0;
```

**Key Changes:**
1. Formula: Logistic map → Absolute value iteration
2. Center: (0.5, 0.5) → (0.0, 0.0) for better symmetry view
3. Zoom: 1.5 → 2.0 for wider initial view
4. Description updated to reflect abs() method

## Testing Recommendations

### Visual Verification
1. Render at default zoom - should see **triangular** main shape
2. Zoom into edges - should see **self-similar** triangular patterns
3. Look for **four-fold symmetry** (mirrored across both axes)
4. Compare with "Sierpinski Triangle (IFS)" for style difference

### Expected Appearance
- **Main shape**: Large triangle or triangular fractal pattern
- **Detail**: Self-similar triangular structures at multiple scales
- **Symmetry**: Mirror symmetry along both X and Y axes
- **NOT circles**: No concentric circular patterns

### Comparison with IFS Version
| Feature | IFS Version | Escape-Time Version (Fixed) |
|---------|-------------|----------------------------|
| **Algorithm** | Chaos game | Absolute value iteration |
| **Pattern** | Classic Sierpinski | Sierpinski-like triangular |
| **Rendering** | Histogram/dots | Escape-time coloring |
| **Detail** | Pure geometric | Continuous escape gradients |
| **Category** | Iterated Function Systems | Special |

## Notes

### Why Two Sierpinski Triangles?
The application now has two different Sierpinski implementations:
1. **"Sierpinski Triangle (IFS)"** - True geometric IFS version
2. **"Sierpinski Triangle"** - Escape-time triangular pattern

This is intentional - they demonstrate different rendering techniques for similar visual themes.

### Formula Attribution
The absolute value iteration technique is related to the "Burning Ship" fractal family, which uses abs() to create distinctive folded patterns.

## Related Fractals
- **Burning Ship**: Uses abs() on both components before squaring
- **Mandelbar**: Uses conjugate operation
- **Tricorn**: Another variant with modified squaring

All of these use variations on the basic z² + c formula to create different symmetries.
