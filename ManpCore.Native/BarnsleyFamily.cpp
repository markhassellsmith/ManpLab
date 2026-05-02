#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Barnsley Family
// Michael Barnsley's fractals (IFS-based and escape-time variants)
// Based on Fractype.h: BARNSLEYM1/J1/M2/J2/M3/J3 (13-16, 28-29, 76-81)
//=============================================================================

void RegisterBarnsleyFamily()
{
    FractalSpec spec;

    //=========================================================================
    // BARNSLEYM1 (13) - Barnsley's first M-set
    //=========================================================================
    spec.name = "BarnsleyM1";
    spec.displayName = "Barnsley M1";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Michael Barnsley's first Mandelbrot-like set";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Barnsley M1: if x >= 0: z = (x-1) + iy, else: z = (x+1) + iy
            // Then z = z² + c
            if (z.real >= 0) {
                z = ComplexD(z.real - 1.0, z.imag);
            } else {
                z = ComplexD(z.real + 1.0, z.imag);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

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
    // BARNSLEYJ1 (14) - Barnsley J1 Julia set
    //=========================================================================
    spec.name = "BarnsleyJ1";
    spec.displayName = "Barnsley J1";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for Barnsley M1";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant(0.6, 1.1);  // Classic Barnsley J1 parameter

        for (int iter = 0; iter < maxIter; ++iter) {
            if (z.real >= 0) {
                z = ComplexD(z.real - 1.0, z.imag);
            } else {
                z = ComplexD(z.real + 1.0, z.imag);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // BARNSLEYM2 (15) - Barnsley's second M-set
    //=========================================================================
    spec.name = "BarnsleyM2";
    spec.displayName = "Barnsley M2";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Michael Barnsley's second Mandelbrot-like set";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Barnsley M2: if xy < 0: z = (x-1) + i(y-1), else: z = (x+1) + i(y+1)
            if (z.real * z.imag < 0) {
                z = ComplexD(z.real - 1.0, z.imag - 1.0);
            } else {
                z = ComplexD(z.real + 1.0, z.imag + 1.0);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

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
    // BARNSLEYJ2 (16) - Barnsley J2 Julia set
    //=========================================================================
    spec.name = "BarnsleyJ2";
    spec.displayName = "Barnsley J2";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for Barnsley M2";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant(0.6, 1.1);

        for (int iter = 0; iter < maxIter; ++iter) {
            if (z.real * z.imag < 0) {
                z = ComplexD(z.real - 1.0, z.imag - 1.0);
            } else {
                z = ComplexD(z.real + 1.0, z.imag + 1.0);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // BARNSLEYM3 (28) - Barnsley's third M-set
    //=========================================================================
    spec.name = "BarnsleyM3";
    spec.displayName = "Barnsley M3";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Michael Barnsley's third Mandelbrot-like set";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Barnsley M3 uses a different branch condition
            double x2 = z.real * z.real;
            double y2 = z.imag * z.imag;

            if (x2 + y2 < (z.real + 1.0) * (z.real + 1.0) + z.imag * z.imag) {
                z = ComplexD(z.real - 1.0, z.imag);
            } else {
                z = ComplexD(z.real + 1.0, z.imag);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

            double modulus = x2 + y2;
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
    // BARNSLEYJ3 (29) - Barnsley J3 Julia set
    //=========================================================================
    spec.name = "BarnsleyJ3";
    spec.displayName = "Barnsley J3";
    spec.category = "Barnsley";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for Barnsley M3";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant(0.6, 1.1);

        for (int iter = 0; iter < maxIter; ++iter) {
            double x2 = z.real * z.real;
            double y2 = z.imag * z.imag;

            if (x2 + y2 < (z.real + 1.0) * (z.real + 1.0) + z.imag * z.imag) {
                z = ComplexD(z.real - 1.0, z.imag);
            } else {
                z = ComplexD(z.real + 1.0, z.imag);
            }

            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

            double modulus = x2 + y2;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
