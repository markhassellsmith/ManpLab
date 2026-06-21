#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {
    //=============================================================================
    // Special & Exotic Fractals Family
    // Unique and unusual fractals from ManpWIN64
    // Includes: Hailstone, Buddhabrot, Lyapunov, Cellular, etc.
    //=============================================================================

    void RegisterSpecialExoticFamily()
    {
        FractalSpec spec;
        InitialConditions ic;  // Declare ONCE at the top

        //=========================================================================
        // HAILSTONE (245) - 2D Hailstone sequence with cycle detection
        //=========================================================================
        spec.name = "Hailstone";
        spec.displayName = "Hailstone Sequence";
        spec.category = "Special";
        spec.type = FractalCategory::Sequence2D;
        spec.description = "2D visualization of Collatz (3n+1) sequence with cycle detection";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // Hailstone (Collatz) sequence: n → n/2 (even) or 3n+1 (odd)
            // Use both real and imaginary parts to create 2D variation
            // Map c.real to starting value n, and c.imag to a variation parameter

            long long n = static_cast<long long>(fabs(c.real * 100.0) + fabs(c.imag * 10.0));
            if (n < 1) n = 1;

            int steps = 0;
            long long maxValue = n;  // Track maximum value reached
            long long totalPath = n; // Sum of path values for variation

            while (n != 1 && steps < maxIter) {
                if (n % 2 == 0) {
                    n = n / 2;
                }
                else {
                    n = 3 * n + 1;
                }
                if (n > maxValue) maxValue = n;
                totalPath += n;
                steps++;
                if (n > 1000000000LL) break;  // Prevent overflow
            }

            // Color by combination of steps and path characteristics
            double result = static_cast<double>(steps);
            // Add variation based on maximum height reached
            result += std::log(static_cast<double>(maxValue + 1)) * 5.0;
            // Add subtle variation based on total path length
            result += std::log(static_cast<double>(totalPath)) * 0.1;

            return result;
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = 0.39;
        //spec.defaultCenterY = -0.44;
        //spec.defaultZoom = 0.026667;  // Viewport tuning: X scale 150.0
        ic = InitialConditionsService::Get("Hailstone");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // HAILSTONE2D - 2D Trajectory Visualization with Axes and Labels
        //=========================================================================
        spec.name = "Hailstone2D";
        spec.displayName = "2-D Hailstone Trajectory";
        spec.category = "Special";
        spec.type = FractalCategory::Sequence2D;
        spec.description = "Interactive 2D visualization of Collatz sequence trajectory with coordinate axes, grid, point labels, and path rendering on black background";

        // This is a marker entry - actual rendering uses HailstoneRenderService
        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // This calculator won't be used - HailstoneRenderService handles the rendering
            // But we need a valid calculator for registry compliance
            return 0.0;
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = 27.0;   // Classic starting point
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 1.0;
        ic = InitialConditionsService::Get("Hailstone2D");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 1000.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // NUMFRACTAL (244) - Fractal dedicated to an 11-year-old discoverer
        //=========================================================================
        spec.name = "NumFractal";
        spec.displayName = "NumFractal";
        spec.category = "Special";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Unique fractal dedicated to an 11-year-old discoverer";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // Placeholder implementation - unique iteration formula
            ComplexD z(0, 0);
            ComplexD constant = isJulia ? juliaC : c;

            for (int iter = 0; iter < maxIter; ++iter) {
                // z = z³ + c (cubic variant)
                double x2 = z.real * z.real;
                double y2 = z.imag * z.imag;
                double x3 = x2 * z.real - 3.0 * z.real * y2;
                double y3 = 3.0 * x2 * z.imag - y2 * z.imag;

                z = ComplexD(x3 + constant.real, y3 + constant.imag);

                double modulus = z.real * z.real + z.imag * z.imag;
                if (modulus > 256.0)
                    return iter + 1.0 - log(log(modulus)) / log(2.0);
            }
            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = true;
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 0.821355;  // Viewport tuning: X scale 4.87
        ic = InitialConditionsService::Get("NumFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // BUDDHABROT (229) - Buddhabrot rendering technique
        //=========================================================================
        spec.name = "Buddhabrot";
        spec.displayName = "Buddhabrot (Classic)";
        spec.category = "Special";
        spec.type = FractalCategory::BuddhabrotBased;  // Requires path accumulation rendering
        spec.description = "Monte Carlo path accumulation fractal producing subtle nebula-like imagery.\n\n"
                          "ALGORITHM: Samples millions of random starting points across the complex plane, "
                          "tests which points escape to infinity, then tracks and accumulates their complete "
                          "Mandelbrot orbit paths into a 3-channel histogram. All orbit points accumulate "
                          "into all RGB channels with different multipliers (R=0.09, G=0.11, B=0.18) producing "
                          "subtle color variations based on density.\n\n"
                          "PERFORMANCE WARNING: COMPUTATIONALLY EXPENSIVE\n"
                          "- 1280×720 resolution: 5-15 seconds (9.2M sample points)\n"
                          "- 1920×1080 resolution: 15-30 seconds (20.7M sample points)\n"
                          "- 3840×2160 (4K) resolution: 90-180 seconds (82.9M sample points)\n"
                          "Render time scales with: (width × height × 100) sample points × maxIterations.\n\n"
                          "VISUAL CHARACTERISTICS:\n"
                          "- Grayscale-dominant with subtle blue/green/red tinting\n"
                          "- Dark void in center (Mandelbrot set silhouette)\n"
                          "- Bright diffuse glow around boundary\n"
                          "- Organic, asymmetric structure\n"
                          "- Elegant, understated aesthetic\n\n"
                          "BEST COORDINATES: Center (-0.33, 0.03), Zoom 1.066667, Iterations 1000-5000";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // NOTE: This calculator is a placeholder for registry compliance.
            // True Buddhabrot rendering happens via RenderBuddhabrotFractal() in FractalEngineWrapper.
            // The per-pixel calculator model cannot produce authentic Buddhabrot imagery.
            ComplexD z(0.0, 0.0);
            double orbitSum = 0.0;
            int escapeIter = maxIter;

            for (int i = 0; i < maxIter; ++i)
            {
                double zr2 = z.real * z.real;
                double zi2 = z.imag * z.imag;
                double modulus = zr2 + zi2;

                if (modulus > 256.0) {
                    escapeIter = i;
                    break;
                }

                orbitSum += std::sqrt(modulus);

                z.imag = 2.0 * z.real * z.imag + c.imag;
                z.real = zr2 - zi2 + c.real;
            }

            if (escapeIter == maxIter) {
                return orbitSum * 0.5;
            }

            return maxIter - escapeIter + orbitSum * 0.1;
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("Buddhabrot");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;  // Buddhabrot rendering breaks symmetry
        // TODO: Add custom parameters when parameter system is integrated
        // Parameters needed: brightness (0.1-5.0, default 1.0), threshold (100-50000, default 1000)
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // NEBULABROT (230) - Dramatic RGB-separated Buddhabrot variant
        //=========================================================================
        spec.name = "Nebulabrot";
        spec.displayName = "Nebulabrot (Dramatic RGB)";
        spec.category = "Special";
        spec.type = FractalCategory::BuddhabrotBased;  // Uses same path accumulation as Buddhabrot
        spec.description = "DRAMATIC RGB-SEPARATED variant of Buddhabrot producing vivid, poster-worthy nebula imagery.\n\n"
                          "ALGORITHM: THREE SEPARATE render passes with different iteration thresholds:\n"
                          "- BLUE channel: Fast escapers (100-500 iterations) → bright core edges\n"
                          "- GREEN channel: Medium escapers (1000-5000 iterations) → spiraling tendrils\n"
                          "- RED channel: Slow escapers (5000-50000 iterations) → diffuse outer halo\n"
                          "Each channel is independently normalized for maximum contrast, producing vivid primaries "
                          "and secondary colors (cyan, magenta, yellow) in overlap regions.\n\n"
                          "PERFORMANCE WARNING: 3× MORE EXPENSIVE THAN BUDDHABROT\n"
                          "- 1280×720 resolution: 15-45 seconds (3 passes × 9.2M samples)\n"
                          "- 1920×1080 resolution: 45-90 seconds (3 passes × 20.7M samples)\n"
                          "- 3840×2160 (4K) resolution: 4-9 minutes (3 passes × 82.9M samples)\n"
                          "Render time = 3× Buddhabrot because three separate threshold ranges must be computed.\n\n"
                          "VISUAL CHARACTERISTICS:\n"
                          "- HIGHLY SATURATED vivid colors (electric blue, emerald green, deep red)\n"
                          "- Cyan highlights where core meets tendrils\n"
                          "- Yellow/magenta transitions in mid-regions\n"
                          "- Dramatic contrast and depth\n"
                          "- Iconic, astronomical nebula appearance\n"
                          "- Poster-worthy aesthetic impact\n\n"
                          "BEST COORDINATES: Center (-0.33, 0.03), Zoom 1.066667, Iterations 10000+\n"
                          "NOTE: Requires higher iteration counts (10000-50000) for full red channel detail.";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // Placeholder calculator (identical to Buddhabrot).
            // True Nebulabrot rendering happens via RenderBuddhabrotFractal() with mode flag.
            ComplexD z(0.0, 0.0);
            double orbitSum = 0.0;
            int escapeIter = maxIter;

            for (int i = 0; i < maxIter; ++i)
            {
                double zr2 = z.real * z.real;
                double zi2 = z.imag * z.imag;
                double modulus = zr2 + zi2;

                if (modulus > 256.0) {
                    escapeIter = i;
                    break;
                }

                orbitSum += std::sqrt(modulus);

                z.imag = 2.0 * z.real * z.imag + c.imag;
                z.real = zr2 - zi2 + c.real;
            }

            if (escapeIter == maxIter) {
                return orbitSum * 0.5;
            }

            return maxIter - escapeIter + orbitSum * 0.1;
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("Buddhabrot");  // Same default view as Buddhabrot
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;
        // TODO: Add custom parameters for threshold ranges:
        // blueMin (50-1000, default 100), blueMax (100-2000, default 500)
        // greenMin (500-5000, default 1000), greenMax (1000-10000, default 5000)
        // redMin (2000-50000, default 5000), redMax (5000-100000, default 50000)
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // LYAPUNOV (123) - Lyapunov fractal based on population dynamics
        //=========================================================================
        spec.name = "Lyapunov";
        spec.displayName = "Lyapunov";
        spec.category = "Special";
        spec.type = FractalCategory::Sequence2D;
        spec.description = "Lyapunov exponent fractal from population dynamics";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            // Lyapunov: iterate x = r*x*(1-x) with alternating r values
            double a = fabs(c.real);
            double b = fabs(c.imag);
            if (a < 0.1) a = 0.1;
            if (b < 0.1) b = 0.1;
            if (a > 4.0) a = 4.0;
            if (b > 4.0) b = 4.0;

            double x = 0.5;
            double sum = 0.0;

            for (int iter = 0; iter < maxIter; ++iter) {
                double r = (iter % 2 == 0) ? a : b;
                x = r * x * (1.0 - x);
                if (x > 0.0 && x < 1.0) {
                    sum += log(fabs(r * (1.0 - 2.0 * x)));
                }
            }

            double lyapunov = sum / maxIter;
            return lyapunov * 50.0 + 128.0;  // Scale for coloring
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = -0.01;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 0.205128;  // Viewport tuning: X scale 19.5
        ic = InitialConditionsService::Get("Lyapunov");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // MANDELBAR (231) - Mandelbar (Tricorn without conjugate in z²)
        //=========================================================================
        spec.name = "MandelbarExotic";
        spec.displayName = "Mandelbar";
        spec.category = "Mandelbrot Variants";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Mandelbar fractal: z = conj(z)² + c";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0, 0);
            ComplexD constant = isJulia ? juliaC : c;

            for (int iter = 0; iter < maxIter; ++iter) {
                // Mandelbar: conjugate before squaring
                z = ComplexD(z.real, -z.imag);
                z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                    2.0 * z.real * z.imag + constant.imag);

                double modulus = z.real * z.real + z.imag * z.imag;
                if (modulus > 256.0)
                    return iter + 1.0 - log(log(modulus)) / log(2.0);
            }
            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = true;
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 0.505051;  // Viewport tuning: X scale 7.92
        ic = InitialConditionsService::Get("MandelbarExotic");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = true;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // THORN (227) - Thorn fractal (classic variant)
        //=========================================================================
        spec.name = "ThornClassic";
        spec.displayName = "Thorn (Classic)";
        spec.category = "Mandelbrot Variants";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Thorn fractal: z = z²/c + c";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.1, 0.0);  // Non-zero starting point for better structure
            ComplexD constant = isJulia ? juliaC : c;
            double c_mag2 = constant.real * constant.real + constant.imag * constant.imag;
            if (c_mag2 < 1e-10) return static_cast<double>(maxIter);

            for (int iter = 0; iter < maxIter; ++iter) {
                // z = z²/c + c
                double z_sq_real = z.real * z.real - z.imag * z.imag;
                double z_sq_imag = 2.0 * z.real * z.imag;

                // Complex division: z²/c
                double div_real = (z_sq_real * constant.real + z_sq_imag * constant.imag) / c_mag2;
                double div_imag = (z_sq_imag * constant.real - z_sq_real * constant.imag) / c_mag2;

                z.real = div_real + constant.real;
                z.imag = div_imag + constant.imag;

                double modulus = z.real * z.real + z.imag * z.imag;
                if (modulus > 100.0)  // Lower bailout for better structure visibility
                    return iter + 1.0 - log(log(modulus)) / log(2.0);
            }
            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = true;
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 4.566210;  // Viewport tuning: X scale 0.876
        ic = InitialConditionsService::Get("ThornClassic");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);

        //=========================================================================
        // TETRATION (236) - Infinite tower: z^z^z^...
        //=========================================================================
        spec.name = "TetrationClassic";
        spec.displayName = "Tetration (Classic)";
        spec.category = "Special";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Infinite power tower: z^z^z^z...";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(1, 0);  // Start with z = 1
            ComplexD constant = isJulia ? juliaC : c;

            for (int iter = 0; iter < maxIter; ++iter) {
                // z = c^z (tetration approximation)
                double r = sqrt(constant.real * constant.real + constant.imag * constant.imag);
                if (r < 1e-10) break;
                double theta = atan2(constant.imag, constant.real);
                double ln_r = log(r);

                double re_exp = z.real * ln_r - z.imag * theta;
                double im_exp = z.real * theta + z.imag * ln_r;
                double exp_re = exp(re_exp);

                z = ComplexD(exp_re * cos(im_exp), exp_re * sin(im_exp));

                double modulus = z.real * z.real + z.imag * z.imag;
                if (modulus > 256.0)
                    return iter + 1.0 - log(log(modulus)) / log(2.0);
            }
            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = true;
        //spec.defaultCenterX = -0.22;
        //spec.defaultCenterY = 0.05;
        //spec.defaultZoom = 0.32;  // Viewport tuning: X scale 12.5
        ic = InitialConditionsService::Get("TetrationClassic");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;
        spec.parameters = {};

        FractalRegistry::Register(spec);
    }
} // namespace Native
