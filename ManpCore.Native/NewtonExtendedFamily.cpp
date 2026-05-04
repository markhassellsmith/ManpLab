#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>
#include <complex>

#ifndef M_PI
#define M_PI 3.141592653589793238462643383279
#endif

namespace Native {

//=============================================================================
// Newton Extended Family
// Newton-Raphson method for various polynomials and transcendental functions
// Formula: z_new = z - f(z)/f'(z)
//=============================================================================

// Helper function for complex power
static ComplexD ComplexPower(ComplexD z, int n)
{
    if (n == 2) {
        return ComplexD(z.real*z.real - z.imag*z.imag, 2.0*z.real*z.imag);
    }

    // Use polar form: z^n = r^n * (cos(nθ) + i*sin(nθ))
    double r = std::sqrt(z.real*z.real + z.imag*z.imag);
    if (r < 1e-10) return ComplexD(0.0, 0.0);

    double theta = std::atan2(z.imag, z.real);
    double r_n = std::pow(r, n);
    double n_theta = n * theta;

    return ComplexD(r_n * std::cos(n_theta), r_n * std::sin(n_theta));
}

void RegisterNewtonExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Newton Quartic (z⁴ - 1 = 0)
    //=========================================================================
    spec.name = "NewtonQuartic";
    spec.displayName = "Newton Quartic (z⁴-1)";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for z⁴ - 1 = 0, showing 4 convergence basins";
    spec.formula = "z = z - (z⁴ - 1)/(4z³)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^4 - 1}{4z_n^3})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.0001;
        const double bailout = 100.0;

        // Four roots: 1, -1, i, -i
        const ComplexD roots[4] = {
            ComplexD(1.0, 0.0),
            ComplexD(-1.0, 0.0),
            ComplexD(0.0, 1.0),
            ComplexD(0.0, -1.0)
        };

        for (int i = 0; i < maxIter; ++i)
        {
            // Check convergence to any root
            for (int r = 0; r < 4; ++r)
            {
                double dx = z.real - roots[r].real;
                double dy = z.imag - roots[r].imag;
                double dist = std::sqrt(dx*dx + dy*dy);

                if (dist < tolerance)
                {
                    // Smooth iteration based on distance
                    return i + 1.0 - std::log(dist / tolerance) / std::log(2.0);
                }
            }

            // Newton iteration: z = z - (z⁴ - 1)/(4z³)
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;
            ComplexD z4 = z2 * z2;

            // f(z) = z⁴ - 1
            ComplexD numerator = z4 - ComplexD(1.0, 0.0);

            // f'(z) = 4z³
            ComplexD denominator = z3 * 4.0;

            // Avoid division by zero
            double denom_mag2 = denominator.real*denominator.real + denominator.imag*denominator.imag;
            if (denom_mag2 < 1e-10) break;

            // z = z - numerator/denominator
            ComplexD ratio = numerator / denominator;
            z = z - ratio;

            // Bailout
            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton Quintic (z⁵ - 1 = 0)
    //=========================================================================
    spec.name = "NewtonQuintic";
    spec.displayName = "Newton Quintic (z⁵-1)";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for z⁵ - 1 = 0, showing 5 convergence basins";
    spec.formula = "z = z - (z⁵ - 1)/(5z⁴)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^5 - 1}{5z_n^4})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.0001;
        const double bailout = 100.0;

        // Five roots of unity
        ComplexD roots[5];
        for (int r = 0; r < 5; ++r)
        {
            double angle = 2.0 * M_PI * r / 5.0;
            roots[r] = ComplexD(std::cos(angle), std::sin(angle));
        }

        for (int i = 0; i < maxIter; ++i)
        {
            // Check convergence
            for (int r = 0; r < 5; ++r)
            {
                double dx = z.real - roots[r].real;
                double dy = z.imag - roots[r].imag;
                double dist = std::sqrt(dx*dx + dy*dy);

                if (dist < tolerance)
                    return i + 1.0 - std::log(dist / tolerance) / std::log(2.0);
            }

            // Newton: z = z - (z⁵ - 1)/(5z⁴)
            ComplexD z4 = ComplexPower(z, 4);
            ComplexD z5 = ComplexPower(z, 5);

            ComplexD numerator = z5 - ComplexD(1.0, 0.0);
            ComplexD denominator = z4 * 5.0;

            double denom_mag2 = denominator.real*denominator.real + denominator.imag*denominator.imag;
            if (denom_mag2 < 1e-10) break;

            ComplexD ratio = numerator / denominator;
            z = z - ratio;

            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton Sextic (z⁶ - 1 = 0)
    //=========================================================================
    spec.name = "NewtonSextic";
    spec.displayName = "Newton Sextic (z⁶-1)";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for z⁶ - 1 = 0, showing 6 convergence basins";
    spec.formula = "z = z - (z⁶ - 1)/(6z⁵)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^6 - 1}{6z_n^5})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.0001;
        const double bailout = 100.0;

        // Six roots of unity
        ComplexD roots[6];
        for (int r = 0; r < 6; ++r)
        {
            double angle = 2.0 * M_PI * r / 6.0;
            roots[r] = ComplexD(std::cos(angle), std::sin(angle));
        }

        for (int i = 0; i < maxIter; ++i)
        {
            // Check convergence
            for (int r = 0; r < 6; ++r)
            {
                double dx = z.real - roots[r].real;
                double dy = z.imag - roots[r].imag;
                double dist = std::sqrt(dx*dx + dy*dy);

                if (dist < tolerance)
                    return i + 1.0 - std::log(dist / tolerance) / std::log(2.0);
            }

            // Newton: z = z - (z⁶ - 1)/(6z⁵)
            ComplexD z5 = ComplexPower(z, 5);
            ComplexD z6 = ComplexPower(z, 6);

            ComplexD numerator = z6 - ComplexD(1.0, 0.0);
            ComplexD denominator = z5 * 6.0;

            double denom_mag2 = denominator.real*denominator.real + denominator.imag*denominator.imag;
            if (denom_mag2 < 1e-10) break;

            ComplexD ratio = numerator / denominator;
            z = z - ratio;

            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton Sine (sin(z) = 0)
    //=========================================================================
    spec.name = "NewtonSin";
    spec.displayName = "Newton Sine";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for sin(z) = 0, converging to integer multiples of π";
    spec.formula = "z = z - sin(z)/cos(z)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{\sin(z_n)}{\cos(z_n)})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.01;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Check if near a root (multiples of π on real axis)
            // sin(z) ≈ 0 when real part is near nπ and imag is near 0
            double near_root_real = std::abs(std::sin(z.real)) * std::cosh(z.imag);
            double near_root_imag = std::abs(std::cos(z.real)) * std::sinh(z.imag);

            if (near_root_real < tolerance && near_root_imag < tolerance)
                return i + 1.0 - std::log(near_root_real + near_root_imag + 1e-10) / std::log(2.0);

            // Complex sin(z) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double sin_z_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_z_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_z_real, sin_z_imag);

            // Complex cos(z) = cos(a)cosh(b) - i*sin(a)sinh(b)
            double cos_z_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_z_imag = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(cos_z_real, cos_z_imag);

            // Avoid division by zero
            double cos_mag2 = cos_z.real*cos_z.real + cos_z.imag*cos_z.imag;
            if (cos_mag2 < 1e-10) break;

            // Newton: z = z - sin(z)/cos(z)
            ComplexD ratio = sin_z / cos_z;
            z = z - ratio;

            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton Cosh (cosh(z) = 1)
    //=========================================================================
    spec.name = "NewtonCosh";
    spec.displayName = "Newton Cosh";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for cosh(z) - 1 = 0";
    spec.formula = "z = z - (cosh(z) - 1)/sinh(z)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{\cosh(z_n) - 1}{\sinh(z_n)})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.001;
        const double bailout = 100.0;

        // Primary roots are 0 and ±2πi, ±4πi, etc.
        for (int i = 0; i < maxIter; ++i)
        {
            // Check for convergence to zero (main root)
            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag < tolerance)
                return i + 1.0 - std::log(mag / tolerance) / std::log(2.0);

            // Complex cosh(z) = cosh(a)cos(b) + i*sinh(a)sin(b)
            double cosh_z_real = std::cosh(z.real) * std::cos(z.imag);
            double cosh_z_imag = std::sinh(z.real) * std::sin(z.imag);
            ComplexD cosh_z(cosh_z_real, cosh_z_imag);

            // f(z) = cosh(z) - 1
            ComplexD f_z = cosh_z - ComplexD(1.0, 0.0);

            // Complex sinh(z) = sinh(a)cos(b) + i*cosh(a)sin(b)
            double sinh_z_real = std::sinh(z.real) * std::cos(z.imag);
            double sinh_z_imag = std::cosh(z.real) * std::sin(z.imag);
            ComplexD sinh_z(sinh_z_real, sinh_z_imag);

            // Avoid division by zero
            double sinh_mag2 = sinh_z.real*sinh_z.real + sinh_z.imag*sinh_z.imag;
            if (sinh_mag2 < 1e-10) break;

            // Newton: z = z - f(z)/f'(z) = z - (cosh(z)-1)/sinh(z)
            ComplexD ratio = f_z / sinh_z;
            z = z - ratio;

            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton Basin (z³ - 1 with root coloring)
    //=========================================================================
    spec.name = "NewtonBasin";
    spec.displayName = "Newton Basin (z³-1)";
    spec.category = "Newton Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Classic Newton basins for z³ - 1 = 0, colored by which root is reached";
    spec.formula = "z = z - (z³ - 1)/(3z²)";
    spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^3 - 1}{3z_n^2})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        const double tolerance = 0.0001;
        const double bailout = 100.0;

        // Three cube roots of unity
        const ComplexD roots[3] = {
            ComplexD(1.0, 0.0),                      // Root 0: 1
            ComplexD(-0.5, 0.866025403784439),        // Root 1: e^(2πi/3)
            ComplexD(-0.5, -0.866025403784439)        // Root 2: e^(4πi/3)
        };

        for (int i = 0; i < maxIter; ++i)
        {
            // Check which root we're converging to
            for (int r = 0; r < 3; ++r)
            {
                double dx = z.real - roots[r].real;
                double dy = z.imag - roots[r].imag;
                double dist = std::sqrt(dx*dx + dy*dy);

                if (dist < tolerance)
                {
                    // Return root index encoded with iteration count
                    // This allows coloring by both root reached and speed of convergence
                    return (r * 100.0) + i;
                }
            }

            // Newton: z = z - (z³ - 1)/(3z²)
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;

            ComplexD numerator = z3 - ComplexD(1.0, 0.0);
            ComplexD denominator = z2 * 3.0;

            double denom_mag2 = denominator.real*denominator.real + denominator.imag*denominator.imag;
            if (denom_mag2 < 1e-10) break;

            ComplexD ratio = numerator / denominator;
            z = z - ratio;

            double mag = std::sqrt(z.real*z.real + z.imag*z.imag);
            if (mag > bailout) return static_cast<double>(maxIter);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);
}

} // namespace Native
