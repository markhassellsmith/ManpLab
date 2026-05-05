#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Orbital Fractals Family
// Fractals with orbit modifications and trapping techniques
//=============================================================================

void RegisterOrbitalFractalsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Orbit Trap - Cross
    //=========================================================================
    spec.name = "OrbitTrapCross";
    spec.displayName = "Orbit Trap (Cross)";
    spec.category = "Orbit Trap";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with cross-shaped orbit trap";
    spec.formula = "z = z² + c, trap: min(|Re(z)|, |Im(z)|)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{trap} = \min(|Re(z)|, |Im(z)|))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return minDist * maxIter;

            // Cross trap: distance to axes
            double dist = std::min(std::abs(z.real), std::abs(z.imag));
            if (dist < minDist)
                minDist = dist;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return minDist * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Orbit Trap - Circle
    //=========================================================================
    spec.name = "OrbitTrapCircle";
    spec.displayName = "Orbit Trap (Circle)";
    spec.category = "Orbit Trap";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with circular orbit trap";
    spec.formula = "z = z² + c, trap: ||z| - r|";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{trap} = ||z| - r|)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        const double trapRadius = 0.5;
        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return minDist * maxIter;

            // Circle trap: distance from circle of radius trapRadius
            double mag = std::sqrt(magSq);
            double dist = std::abs(mag - trapRadius);
            if (dist < minDist)
                minDist = dist;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return minDist * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Orbit Trap - Point
    //=========================================================================
    spec.name = "OrbitTrapPoint";
    spec.displayName = "Orbit Trap (Point)";
    spec.category = "Orbit Trap";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with point orbit trap";
    spec.formula = "z = z² + c, trap: |z - p|";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{trap} = |z - p|)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        ComplexD trapPoint(0.0, 0.0);
        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return minDist * maxIter;

            // Point trap: distance to specific point
            double dx = z.real - trapPoint.real;
            double dy = z.imag - trapPoint.imag;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return minDist * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Orbit Trap - Square
    //=========================================================================
    spec.name = "OrbitTrapSquare";
    spec.displayName = "Orbit Trap (Square)";
    spec.category = "Orbit Trap";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with square orbit trap";
    spec.formula = "z = z² + c, trap: max(|Re(z)|, |Im(z)|)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{trap} = \max(|Re(z)|, |Im(z)|))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        const double trapSize = 0.5;
        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return minDist * maxIter;

            // Square trap: distance from square boundary
            double dist = std::max(std::abs(z.real), std::abs(z.imag));
            dist = std::abs(dist - trapSize);
            if (dist < minDist)
                minDist = dist;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return minDist * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Average Distance
    //=========================================================================
    spec.name = "AverageDistance";
    spec.displayName = "Average Distance";
    spec.category = "Orbit Modification";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot colored by average orbit distance";
    spec.formula = "z = z² + c, color by average |z|";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{avg}(\sum |z_n|))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        double sumDist = 0.0;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                break;

            sumDist += std::sqrt(magSq);

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        if (i == 0) return 0.0;
        return (sumDist / i) * maxIter * 0.1;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Minimum Distance
    //=========================================================================
    spec.name = "MinimumDistance";
    spec.displayName = "Minimum Distance";
    spec.category = "Orbit Modification";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot colored by minimum orbit distance from origin";
    spec.formula = "z = z² + c, color by min(|z|)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \min(|z_n|))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return minDist * maxIter;

            double mag = std::sqrt(magSq);
            if (mag < minDist)
                minDist = mag;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return minDist * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Maximum Distance
    //=========================================================================
    spec.name = "MaximumDistance";
    spec.displayName = "Maximum Distance";
    spec.category = "Orbit Modification";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot colored by maximum orbit distance before escape";
    spec.formula = "z = z² + c, color by max(|z|)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \max(|z_n|))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        double maxDist = 0.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return maxDist * maxIter * 0.1;

            double mag = std::sqrt(magSq);
            if (mag > maxDist)
                maxDist = mag;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return maxDist * maxIter * 0.1;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Angle Average
    //=========================================================================
    spec.name = "AngleAverage";
    spec.displayName = "Angle Average";
    spec.category = "Orbit Modification";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot colored by average orbit angle";
    spec.formula = "z = z² + c, color by average arg(z)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; \text{avg}(\arg(z_n)))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;
        double sumAngle = 0.0;
        const double pi = 3.14159265358979323846;

        int i;
        for (i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                break;

            if (magSq > 1e-10)
                sumAngle += std::atan2(z.imag, z.real);

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        if (i == 0) return 0.0;
        double avgAngle = sumAngle / i;
        return ((avgAngle + pi) / (2.0 * pi)) * maxIter;  // Normalize to [0, maxIter]
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
