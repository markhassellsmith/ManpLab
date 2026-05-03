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
        ComplexD z(0, 0);
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
    spec.defaultCenterX = -0.5;   // Lambda main body view
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;       // Standard Mandelbrot-like zoom to show full set
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
    spec.name = "MandelTrig";
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
    // SIERPINSKI (12) - Sierpinski triangle
    //=========================================================================
    spec.name = "Sierpinski";
    spec.displayName = "Sierpinski Triangle";
    spec.category = "Special";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Classic Sierpinski triangle fractal";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Sierpinski iteration: z = 2*z*(1-z)
        ComplexD z = c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(2.0, 0) * z * (ComplexD(1.0, 0) - z);
            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.5;
    spec.defaultCenterY = 0.5;
    spec.defaultZoom = 1.5;
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
    spec.name = "MandelLambda";
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
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // MARKSMANDEL (21) - Marks Mandelbrot variant
    //=========================================================================
    spec.name = "MarksMandel";
    spec.displayName = "Marks Mandelbrot";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mark's variation of the Mandelbrot set";

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
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
