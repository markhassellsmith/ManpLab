# Sierpinski Triangle Fix - Summary

## Problem
The "Sierpinski Triangle" in the Special category rendered as **concentric circles**, not a triangle.

## Root Cause
Used wrong formula: `z = 2z(1-z)` (logistic map → creates circles)

## Solution
Changed to absolute value iteration: `z = (|Re(z)| + i|Im(z)|)² + c`

This creates **triangular self-similar patterns** via mirror symmetry.

## Changes

### ClassicEscapeTimeFamily.cpp
- **Old formula**: `z = 2.0 * z * (1.0 - z)` (logistic map)
- **New formula**: `z = abs(z.real) + i*abs(z.imag)` then square and add c
- **Center**: (0.5, 0.5) → (0.0, 0.0)
- **Zoom**: 1.5 → 2.0

## Result
- Produces triangular patterns with self-similar structure
- Four-fold symmetry (mirrored on both axes)
- Escape-time rendering with continuous coloring
- Distinct from the IFS Sierpinski Triangle (which uses chaos game)

## Note
The app has two Sierpinski implementations:
1. **"Sierpinski Triangle (IFS)"** - True geometric version (chaos game)
2. **"Sierpinski Triangle"** - Escape-time version (abs() iteration)

Both are valid but demonstrate different rendering techniques.
