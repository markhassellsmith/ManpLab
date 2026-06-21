#include "FractalRegistry.h"

namespace Native {

/// <summary>
/// Register L-System (Lindenmayer System) fractals - turtle graphics rendered in managed layer.
/// These fractals use string rewriting rules (grammar) and turtle graphics interpretation.
/// Native layer only provides metadata - rendering happens in C# LSystemRenderService.
/// </summary>
void RegisterLSystemFamily()
{
    FractalSpec spec;

    // ═════════════════════════════════════════════════════════════════════════
    // L-SYSTEM FRACTALS
    // ═════════════════════════════════════════════════════════════════════════
    // Category: LSystem - special rendering in managed layer
    // These fractals do not use per-pixel escape-time calculation
    // Instead, they expand grammar rules and interpret turtle graphics commands
    //
    // Rendering path:
    //   1. Native layer returns empty result with category = LSystem
    //   2. C# LSystemRenderService expands grammar (F → F+F--F+F, etc.)
    //   3. Turtle interprets commands: F=forward, +=left, -=right, [=push, ]=pop
    //   4. Lines are rasterized using Bresenham algorithm in C#
    //
    // Each fractal has:
    //   - Axiom: starting string (e.g., "F++F++F" for Koch Snowflake)
    //   - Rules: production rules for string rewriting
    //   - Turn angle: rotation angle for '+' and '-' commands
    //   - Default generations: typical depth for good visual quality
    // ═════════════════════════════════════════════════════════════════════════

    // ═════════════════════════════════════════════════════════════════════════
    // KOCH SNOWFLAKE: F → F+F--F+F, angle=60°, axiom=F++F++F
    // ═════════════════════════════════════════════════════════════════════════
    // Helge von Koch's 1904 continuous but nowhere-differentiable curve
    // One of the first fractal curves ever described mathematically
    spec = FractalSpec();
    spec.name = "KochSnowflake";
    spec.displayName = "Koch Snowflake";
    spec.description = "Helge von Koch's 1904 continuous but nowhere-differentiable curve. Classic 60° triangular fractal.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;  // L-Systems are rendered in managed layer
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // DRAGON CURVE: F → F+G, G → F-G, angle=90°, axiom=F
    // ═════════════════════════════════════════════════════════════════════════
    // Heighway dragon discovered in 1967 by NASA physicists
    // Self-similar space-filling curve that tiles the plane
    spec = FractalSpec();
    spec.name = "DragonCurve";
    spec.displayName = "Dragon Curve";
    spec.description = "Heighway dragon discovered in 1967. Self-similar space-filling curve with intricate folding patterns.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // SIERPINSKI TRIANGLE: F → G-F-G, G → F+G+F, angle=60°, axiom=F
    // ═════════════════════════════════════════════════════════════════════════
    // Sierpinski arrowhead curve, L-System variant of 1915 triangle fractal
    // Creates recursive triangular subdivision with infinite detail
    spec = FractalSpec();
    spec.name = "SierpinskiTriangle";
    spec.displayName = "Sierpinski Triangle";
    spec.description = "Sierpinski arrowhead curve, L-System variant of Sierpinski's 1915 triangle fractal.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // HILBERT CURVE: X → -YF+XFX+FY-, Y → +XF-YFY-FX+, angle=90°, axiom=X
    // ═════════════════════════════════════════════════════════════════════════
    // David Hilbert's 1891 continuous space-filling curve
    // Every point in 2D space is arbitrarily close to the curve
    spec = FractalSpec();
    spec.name = "HilbertCurve";
    spec.displayName = "Hilbert Curve";
    spec.description = "David Hilbert's 1891 space-filling curve. Continuous curve that fills a square completely.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // FRACTAL PLANT: F → FF, X → F-[[X]+X]+F[+FX]-X, angle=25°, axiom=X
    // ═════════════════════════════════════════════════════════════════════════
    // Aristid Lindenmayer's original 1968 plant model
    // Demonstrates biological branching patterns using stack-based branching
    spec = FractalSpec();
    spec.name = "FractalPlant";
    spec.displayName = "Fractal Plant";
    spec.description = "Aristid Lindenmayer's original 1968 plant model. Demonstrates biological branching patterns.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // KOCH CURVE: F → F+F-F-F+F, angle=90°, axiom=F
    // ═════════════════════════════════════════════════════════════════════════
    // Quadratic Koch variant with 90° angles
    // Single-edge variant (not closed snowflake)
    spec = FractalSpec();
    spec.name = "KochCurve";
    spec.displayName = "Koch Curve";
    spec.description = "Quadratic Koch variant with 90° angles. Single-edge variant creating square-based fractal.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);

    // ═════════════════════════════════════════════════════════════════════════
    // PEANO CURVE: X → XFYFX+F+YFXFY-F-XFYFX, Y → YFXFY-F-XFYFX+F+YFXFY, angle=90°, axiom=X
    // ═════════════════════════════════════════════════════════════════════════
    // Giuseppe Peano's 1890 curve - first space-filling curve ever discovered
    // Fills a square completely with a single continuous line
    spec = FractalSpec();
    spec.name = "PeanoCurve";
    spec.displayName = "Peano Curve";
    spec.description = "Giuseppe Peano's 1890 curve, the first space-filling curve ever discovered. Fills a square completely.";
    spec.category = "L-Systems";
    spec.type = FractalCategory::LSystem;
    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.calculator = nullptr;
    FractalRegistry::Register(spec);
}

} // namespace Native
