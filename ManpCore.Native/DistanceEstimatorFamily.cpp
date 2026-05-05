#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Distance Estimator (DEM) Family
// Fractals with distance estimation for smooth boundary visualization
//=============================================================================

void RegisterDistanceEstimatorFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Mandelbrot with Distance Estimator
    //=========================================================================
    spec.name = "MandelbrotDEM";
    spec.displayName = "Mandelbrot (Distance Estimator)";
    spec.category = "Distance Estimator";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with precise boundary distance estimation";
    spec.formula = "z = z² + c, with derivative tracking for distance";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; z'_{n+1} = 2z_n \cdot z'_n + 1)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD dz(1.0, 0.0);  // Derivative
        const double bailout = 256.0;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
            {
                // Distance estimator formula
                double mag = std::sqrt(magSq);
                double dmag = std::sqrt(dz.real * dz.real + dz.imag * dz.imag);
                double distance = 2.0 * mag * std::log(mag) / dmag;

                // Convert distance to smooth iteration value
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout)) + distance * 10.0;
            }

            // dz = 2*z*dz + 1
            ComplexD temp = dz;
            dz.real = 2.0 * (z.real * temp.real - z.imag * temp.imag) + 1.0;
            dz.imag = 2.0 * (z.real * temp.imag + z.imag * temp.real);

            // z = z^2 + c
            temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia with Distance Estimator
    //=========================================================================
    spec.name = "JuliaDEM";
    spec.displayName = "Julia (Distance Estimator)";
    spec.category = "Distance Estimator";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with precise boundary distance estimation";
    spec.formula = "z = z² + c, with derivative tracking";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; z'_{n+1} = 2z_n \cdot z'_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param = isJulia ? juliaC : ComplexD(-0.4, 0.6);
        ComplexD dz(1.0, 0.0);
        const double bailout = 256.0;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
            {
                double mag = std::sqrt(magSq);
                double dmag = std::sqrt(dz.real * dz.real + dz.imag * dz.imag);
                double distance = 2.0 * mag * std::log(mag) / dmag;

                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout)) + distance * 10.0;
            }

            // dz = 2*z*dz
            ComplexD temp = dz;
            dz.real = 2.0 * (z.real * temp.real - z.imag * temp.imag);
            dz.imag = 2.0 * (z.real * temp.imag + z.imag * temp.real);

            // z = z^2 + param
            temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + param.real;
            z.imag = 2.0 * temp.real * temp.imag + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Burning Ship with Distance Estimator
    //=========================================================================
    spec.name = "BurningShipDEM";
    spec.displayName = "Burning Ship (Distance Estimator)";
    spec.category = "Distance Estimator";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship fractal with distance estimation for smooth edges";
    spec.formula = "z = (|Re(z)| + i|Im(z)|)² + c";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n)| + i|Im(z_n)|)^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD dz(1.0, 0.0);
        const double bailout = 256.0;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
            {
                double mag = std::sqrt(magSq);
                double dmag = std::sqrt(dz.real * dz.real + dz.imag * dz.imag);
                double distance = 2.0 * mag * std::log(mag) / dmag;

                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout)) + distance * 10.0;
            }

            // Derivative (approximate)
            double sign_real = z.real >= 0.0 ? 1.0 : -1.0;
            double sign_imag = z.imag >= 0.0 ? 1.0 : -1.0;

            ComplexD temp = dz;
            dz.real = 2.0 * (std::abs(z.real) * temp.real * sign_real - std::abs(z.imag) * temp.imag * sign_imag) + 1.0;
            dz.imag = 2.0 * (std::abs(z.real) * temp.imag * sign_real + std::abs(z.imag) * temp.real * sign_imag);

            // z iteration
            double abs_real = std::abs(z.real);
            double abs_imag = std::abs(z.imag);
            temp = z;
            z.real = abs_real * abs_real - abs_imag * abs_imag + c.real;
            z.imag = 2.0 * abs_real * abs_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = -0.5;
    spec.defaultZoom = 0.4;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tricorn with Distance Estimator
    //=========================================================================
    spec.name = "TricornDEM";
    spec.displayName = "Tricorn (Distance Estimator)";
    spec.category = "Distance Estimator";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tricorn (Mandelbar) with distance estimation";
    spec.formula = "z = conj(z)² + c";
    spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD dz(1.0, 0.0);
        const double bailout = 256.0;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
            {
                double mag = std::sqrt(magSq);
                double dmag = std::sqrt(dz.real * dz.real + dz.imag * dz.imag);
                double distance = 2.0 * mag * std::log(mag) / dmag;

                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout)) + distance * 10.0;
            }

            // dz = 2*conj(z)*conj(dz) + 1
            ComplexD temp = dz;
            dz.real = 2.0 * (z.real * temp.real + z.imag * temp.imag) + 1.0;
            dz.imag = 2.0 * (z.real * temp.imag - z.imag * temp.real);

            // z = conj(z)^2 + c
            temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = -2.0 * temp.real * temp.imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);
}

} // namespace Native
