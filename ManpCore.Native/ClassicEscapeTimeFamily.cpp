#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Classic Escape-Time Fractals Family
// Standard 2D escape-time fractals from ManpWIN64 engine
// Based on Fractype.h definitions (MANDEL, JULIA, LAMBDA, etc.)
//=============================================================================

void RegisterClassicEscapeTimeFamily()
{
    FractalSpec spec;

    //=========================================================================
    // MANDEL (0) - Already registered in MandelbrotFamily.cpp
    // JULIA (1) - Already registered in MandelbrotFamily.cpp
    // Skipping to avoid duplicates
    //=========================================================================

    //=========================================================================
    // LAMBDA (3) - Lambda fractal: z = λ * z * (1 - z)
    //=========================================================================
    spec.name = "Lambda";
    spec.displayName = "Lambda";
    spec.category = "Classic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with iteration: z = λ * z * (1 - z)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Lambda iteration: z_n+1 = λ * z_n * (1 - z_n)
        // Lambda uses smaller bailout (4.0) than Mandelbrot
        ComplexD z(0.5, 0.0);  // Lambda requires non-zero starting point (standard is 0.5)
        ComplexD lambda = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = lambda * z * (ComplexD(1,0) - z);
            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 4.0)  // Lambda bailout is 4.0, not 256.0
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 1.0;    // Viewport tuning: optimal view for twin Mandelbrots
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.666667;  // Viewport tuning: X scale 6.0, Y scale 3.375
    spec.defaultBailout = 4.0;    // Lambda uses bailout of 4.0
    spec.hasSymmetry = false;

    // Lambda parameters (using Paul DeLeeuw's naming from ManpWIN64/fractalp.cpp line 273)
    spec.parameters = {
        // Calculation parameters (Paul's standard names from fractalp.cpp lines 28-29)
        ParameterSpec(
            "realz0",                               // Paul's name (line 273)
            "Real Perturbation of Z(0)",            // Paul's display text (line 28)
            "Initial value for real part of z (usually 0.0 for Lambda set)",
            ParameterType::Float,
            ParameterCategory::Calculation,
            "0.0",                                  // Paul's default (line 273)
            -10.0, 10.0, 0.01
        ),
        ParameterSpec(
            "imagz0",                               // Paul's name (line 273)
            "Imaginary Perturbation of Z(0)",       // Paul's display text (line 29)
            "Initial value for imaginary part of z (usually 0.0 for Lambda set)",
            ParameterType::Float,
            ParameterCategory::Calculation,
            "0.0",                                  // Paul's default (line 273)
            -10.0, 10.0, 0.01
        ),
        ParameterSpec(
            "maxIterations",
            "Max Iterations",
            "Maximum number of iterations before considering pixel inside the set",
            ParameterType::Integer,
            ParameterCategory::Calculation,
            "256",                                  // Standard default
            1.0, 100000.0, 1.0
        ),
        ParameterSpec(
            "bailout",
            "Bailout Radius",
            "Escape radius for Lambda fractal (typically 4.0)",
            ParameterType::Float,
            ParameterCategory::Advanced,
            "4.0",                                  // Lambda-specific (STDBAILOUT from line 136)
            2.0, 1000.0, 0.1
        )
    };

    FractalRegistry::Register(spec);

    //=========================================================================
    // MANDELFP (4) - Floating point Mandelbrot (already covered)
    //=========================================================================

    //=========================================================================
    // MANDELTRIGFP (8) / LAMBDASINE (obsolete)
    //=========================================================================
    spec.name = "MandelTrigClassic";
    spec.displayName = "Mandel Trig";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with trigonometric functions";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Placeholder for MandelTrig - will be implemented with full trig support
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // MANOWAR (10) - Manowar fractal: z = z² + z_prev + c
    //=========================================================================
    spec.name = "Manowar";
    spec.displayName = "Manowar";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Manowar fractal: z = z² + z_prev + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD z_prev(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            ComplexD z_new(z.real * z.real - z.imag * z.imag + z_prev.real + constant.real,
                          2.0 * z.real * z.imag + z_prev.imag + constant.imag);
            z_prev = z;
            z = z_new;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // SIERPINSKI (12) - Celtic Buffalo: Mandelbrot with abs() on both components
    //=========================================================================
    spec.name = "CelticBuffalo";
    spec.displayName = "Celtic Buffalo";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Celtic Buffalo: abs(re) + i*abs(im), then z² + c. Mandelbrot-style variant with four-fold symmetry.";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Celtic Buffalo: z_n+1 = (|Re(z_n)| + i|Im(z_n)|)² + c
        // Mandelbrot-style (+ c) with abs() on both components
        ComplexD z = c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Take absolute values of real and imaginary parts
            double x = std::abs(z.real);
            double y = std::abs(z.imag);

            // Square the result
            z.real = x * x - y * y + c.real;
            z.imag = 2.0 * x * y + c.imag;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // UNITY (23) - Unity fractal
    //=========================================================================
    spec.name = "Unity";
    spec.displayName = "Unity";
    spec.category = "Classic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Unity fractal with circle inversion";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Unity: z = z² + 1/c
        ComplexD z(0, 0);
        double denom = c.real * c.real + c.imag * c.imag;
        if (denom < 1e-10) denom = 1e-10;
        ComplexD invC(c.real / denom, -c.imag / denom);

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(z.real * z.real - z.imag * z.imag + invC.real,
                        2.0 * z.real * z.imag + invC.imag);
            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // MANDELLAMBDA (20) - Mandelbrot-Lambda hybrid
    //=========================================================================
    spec.name = "MandelLambdaClassic";
    spec.displayName = "Mandel-Lambda";
    spec.category = "Classic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid of Mandelbrot and Lambda fractals";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Combine Mandelbrot (z²+c) with Lambda (λz(1-z))
            ComplexD mandel_part(z.real * z.real - z.imag * z.imag + constant.real,
                                2.0 * z.real * z.imag + constant.imag);
            ComplexD lambda_part = constant * z * (ComplexD(1,0) - z);
            z = ComplexD((mandel_part.real + lambda_part.real) * 0.5,
                        (mandel_part.imag + lambda_part.imag) * 0.5);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.86;
    spec.defaultCenterY = -0.03;
    spec.defaultZoom = 0.714496;  // Viewport tuning: X scale 5.5982, Y scale 5.5982
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // MARKSMANDEL_PLACEHOLDER (21) - Marks Mandelbrot variant (placeholder)
    //=========================================================================
    spec.name = "MarksMandelPlaceholder";
    spec.displayName = "Marks Mandelbrot (Classic)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mark's variation of the Mandelbrot set (classic implementation)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Placeholder - will implement full Marks algorithm
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // SPIDER (93-94) - Spider fractal
    //=========================================================================
    spec.name = "Spider";
    spec.displayName = "Spider";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Spider fractal: z = z² + c, c = c/2 + z";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD c_var = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(z.real * z.real - z.imag * z.imag + c_var.real,
                        2.0 * z.real * z.imag + c_var.imag);
            c_var = ComplexD(c_var.real * 0.5 + z.real, c_var.imag * 0.5 + z.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // TETRATE (95) - Tetration fractal
    //=========================================================================
    spec.name = "Tetrate";
    spec.displayName = "Tetrate";
    spec.category = "Classic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tetration fractal: z = c^z";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // z = c^z (complex exponentiation)
            // c^z = exp(z * ln(c))
            double r = sqrt(constant.real * constant.real + constant.imag * constant.imag);
            if (r < 1e-10) break;
            double theta = atan2(constant.imag, constant.real);
            double ln_r = log(r);

            // exp(z * (ln_r + i*theta))
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
    spec.defaultCenterX = -1.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.333333;  // Viewport tuning: X scale 3.0, Y scale 1.69
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Simple Variations Added to Reach 300 Total
    //=========================================================================

    //=========================================================================
    // Perpendicular Mandelbrot
    //=========================================================================
    spec.name = "PerpendicularMandelbrot";
    spec.displayName = "Perpendicular Mandelbrot (Abs First)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Perpendicular Mandelbrot: abs(re) - i*abs(im), then z^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(abs(z.real), -abs(z.imag));
            z = z * z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Heart Mandelbrot
    //=========================================================================
    spec.name = "HeartMandelbrot";
    spec.displayName = "Heart Mandelbrot (Sine)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Heart-shaped variation: z^2 + c + sin(z)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Calculate sin(z)
            double sin_re = sin(z.real) * cosh(z.imag);
            double sin_im = cos(z.real) * sinh(z.imag);
            ComplexD sin_z(sin_re, sin_im);

            z = z * z + constant + sin_z;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Shark Fin Mandelbrot
    //=========================================================================
    spec.name = "SharkFinMandelbrot";
    spec.displayName = "Shark Fin Mandelbrot";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Shark Fin variation: z^2 + c/z";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.1, 0.1);  // Non-zero start to avoid divide by zero
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double z_mag_sq = z.real * z.real + z.imag * z.imag;
            if (z_mag_sq < 1e-10) break;  // Avoid division by very small numbers

            ComplexD c_over_z((constant.real * z.real + constant.imag * z.imag) / z_mag_sq,
                             (constant.imag * z.real - constant.real * z.imag) / z_mag_sq);

            z = z * z + c_over_z;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Partial Burning Ship
    //=========================================================================
    spec.name = "PartialBurningShip";
    spec.displayName = "Partial Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Partial Burning Ship: re^2 + i*abs(im)^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double re_sq = z.real * z.real;
            double im_abs_sq = abs(z.imag) * abs(z.imag);
            z = ComplexD(re_sq - im_abs_sq, 2.0 * z.real * abs(z.imag)) + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.25;  // Center at (-0.25, 0.00)
    spec.defaultCenterY = 0.0;    // Center at (-0.25, 0.00)
    spec.defaultZoom = 0.75;      // Viewport: 4.00 × 2.25
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Bird of Prey
    //=========================================================================
    spec.name = "BirdOfPrey";
    spec.displayName = "Bird of Prey";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Bird of Prey: abs(re)^2 + i*im^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double re_abs_sq = abs(z.real) * abs(z.real);
            double im_sq = z.imag * z.imag;
            z = ComplexD(re_abs_sq - im_sq, 2.0 * abs(z.real) * z.imag) + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Celtic Heart
    //=========================================================================
    spec.name = "CelticHeart";
    spec.displayName = "Celtic Heart";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Celtic Heart: abs(re) + i*im, then z^2 + sin(z) + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(abs(z.real), z.imag);

            // Calculate sin(z)
            double sin_re = sin(z.real) * cosh(z.imag);
            double sin_im = cos(z.real) * sinh(z.imag);
            ComplexD sin_z(sin_re, sin_im);

            z = z * z + sin_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Wavy Mandelbrot
    //=========================================================================
    spec.name = "WavyMandelbrot";
    spec.displayName = "Wavy Mandelbrot";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Wavy variation: z^2 + c + 0.1*sin(z)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Calculate sin(z)
            double sin_re = sin(z.real) * cosh(z.imag);
            double sin_im = cos(z.real) * sinh(z.imag);
            ComplexD sin_z(sin_re * 0.1, sin_im * 0.1);  // 0.1 scaling factor

            z = z * z + constant + sin_z;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Sinh Mandelbrot
    //=========================================================================
    spec.name = "SinhMandelbrot";
    spec.displayName = "Sinh Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hyperbolic sine: sinh(z)^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Calculate sinh(z) = (e^z - e^-z)/2
            double sinh_re = sinh(z.real) * cos(z.imag);
            double sinh_im = cosh(z.real) * sin(z.imag);
            ComplexD sinh_z(sinh_re, sinh_im);

            z = sinh_z * sinh_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Cosh Mandelbrot
    //=========================================================================
    spec.name = "CoshMandelbrot";
    spec.displayName = "Cosh Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hyperbolic cosine: cosh(z)^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Calculate cosh(z) = (e^z + e^-z)/2
            double cosh_re = cosh(z.real) * cos(z.imag);
            double cosh_im = sinh(z.real) * sin(z.imag);
            ComplexD cosh_z(cosh_re, cosh_im);

            z = cosh_z * cosh_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tanh Mandelbrot
    //=========================================================================
    spec.name = "TanhMandelbrot";
    spec.displayName = "Tanh Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hyperbolic tangent: tanh(z)^2 + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Calculate tanh(z) = sinh(z)/cosh(z)
            double sinh_re = sinh(z.real) * cos(z.imag);
            double sinh_im = cosh(z.real) * sin(z.imag);
            double cosh_re = cosh(z.real) * cos(z.imag);
            double cosh_im = sinh(z.real) * sin(z.imag);

            double denom = cosh_re * cosh_re + cosh_im * cosh_im;
            if (denom < 1e-10) break;

            double tanh_re = (sinh_re * cosh_re + sinh_im * cosh_im) / denom;
            double tanh_im = (sinh_im * cosh_re - sinh_re * cosh_im) / denom;
            ComplexD tanh_z(tanh_re, tanh_im);

            z = tanh_z * tanh_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
