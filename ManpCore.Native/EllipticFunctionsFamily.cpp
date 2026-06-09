#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>
#include <complex>

namespace Native
{
    //=============================================================================
    // Helper: Weierstrass ζ (Zeta) Function
    // The logarithmic derivative of the Weierstrass sigma function
    // ζ(z) = 1/z + Σ[(1/(z-ω) + 1/ω + z/ω²)]
    //=============================================================================
    static ComplexD WeierstrassZeta(ComplexD z, double g2, double g3)
    {
        const double eps = 1e-10;
        double magSq = z.real * z.real + z.imag * z.imag;

        if (magSq < eps)
        {
            // Near pole at origin: ζ(z) ≈ 1/z
            double invMag = 1.0 / magSq;
            return ComplexD(z.real * invMag, -z.imag * invMag);
        }

        // Series approximation: ζ(z) ≈ 1/z + (g₂/60)z³ + (g₃/140)z⁵
        double z2_real = z.real * z.real - z.imag * z.imag;
        double z2_imag = 2.0 * z.real * z.imag;

        // 1/z
        double invz_real = z.real / magSq;
        double invz_imag = -z.imag / magSq;

        // z³ = z·z²
        double z3_real = z.real * z2_real - z.imag * z2_imag;
        double z3_imag = z.real * z2_imag + z.imag * z2_real;

        // z⁵ = z²·z³
        double z5_real = z2_real * z3_real - z2_imag * z3_imag;
        double z5_imag = z2_real * z3_imag + z2_imag * z3_real;

        double result_real = invz_real + (g2 / 60.0) * z3_real + (g3 / 140.0) * z5_real;
        double result_imag = invz_imag + (g2 / 60.0) * z3_imag + (g3 / 140.0) * z5_imag;

        return ComplexD(result_real, result_imag);
    }

    //=============================================================================
    // Helper: Weierstrass σ (Sigma) Function
    // An entire function related to the Weierstrass ℘ function
    // σ(z) = z·∏[1 - z/ω]·exp[z/ω + z²/(2ω²)]
    //=============================================================================
    static ComplexD WeierstrassSigma(ComplexD z, double g2, double g3)
    {
        // Series approximation: σ(z) ≈ z - (g₂/240)z⁵ - (g₃/840)z⁷
        double z2_real = z.real * z.real - z.imag * z.imag;
        double z2_imag = 2.0 * z.real * z.imag;

        // z³ = z·z²
        double z3_real = z.real * z2_real - z.imag * z2_imag;
        double z3_imag = z.real * z2_imag + z.imag * z2_real;

        // z⁵ = z²·z³
        double z5_real = z2_real * z3_real - z2_imag * z3_imag;
        double z5_imag = z2_real * z3_imag + z2_imag * z3_real;

        // z⁷ = z²·z⁵
        double z7_real = z2_real * z5_real - z2_imag * z5_imag;
        double z7_imag = z2_real * z5_imag + z2_imag * z5_real;

        double result_real = z.real - (g2 / 240.0) * z5_real - (g3 / 840.0) * z7_real;
        double result_imag = z.imag - (g2 / 240.0) * z5_imag - (g3 / 840.0) * z7_imag;

        return ComplexD(result_real, result_imag);
    }

    //=============================================================================
    // Register Elliptic Functions Family
    //=============================================================================
    void RegisterEllipticFunctionsFamily()
    {
        FractalSpec spec;
        InitialConditions ic;

        //=========================================================================
        // Weierstrass ℘-function (Elliptic Function)
        //=========================================================================
        spec.name = "WeierstrassP";
        spec.displayName = "Weierstrass ℘-function";
        spec.category = "Elliptic Functions";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses the Weierstrass elliptic ℘-function: z = ℘(z) + c. The ℘-function is doubly periodic with poles and satisfies (℘')² = 4℘³ - g₂℘ - g₃.";
        spec.formula = "z = ℘(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \wp(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.0);  // Better starting point
            const double bailout = 1000.0;  // Increased bailout

            // Weierstrass invariants - try more extreme values for better escape behavior
            const double g2 = 4.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Simplified approach: ℘(z) ≈ 1/z² for small z (near pole)
                // Use the fundamental property that ℘ has a double pole at origin

                if (magSq < 1e-10) {
                    break;  // Too close to pole
                }

                // ℘(z) ≈ 1/z² + (g₂/20)z² + ...
                // Compute 1/z²
                double z2_real = z.real * z.real - z.imag * z.imag;
                double z2_imag = 2.0 * z.real * z.imag;
                double z2_mag = z2_real * z2_real + z2_imag * z2_imag;

                if (z2_mag < 1e-10) break;

                // 1/z² = z²* / |z²|²
                double invz2_real = z2_real / z2_mag;
                double invz2_imag = -z2_imag / z2_mag;

                // Add correction: (g₂/20)z²
                double correction_scale = g2 / 20.0;
                invz2_real += correction_scale * z2_real;
                invz2_imag += correction_scale * z2_imag;

                z.real = invz2_real + c.real;
                z.imag = invz2_imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("WeierstrassP");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 1000.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Jacobi Elliptic sn Function
        //=========================================================================
        spec.name = "JacobiSN";
        spec.displayName = "Jacobi Elliptic sn";
        spec.category = "Elliptic Functions";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Jacobi elliptic sine amplitude: z = sn(z, k) + c. Doubly periodic function that generalizes sine, with parameter k (elliptic modulus).";
        spec.formula = "z = sn(z, k) + c";
        spec.formulaLatex = R"(z_{n+1} = \text{sn}(z_n, k) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 100.0;
            const double k = 0.7;  // Elliptic modulus (0 < k < 1)

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Jacobi sn approximation using series expansion
                // For small z: sn(z,k) ≈ z - (1+k²)z³/6 + (1+14k²+k⁴)z⁵/120
                // For general case, use relation to sine for fractal visualization
                // sn(z,k) reduces to sin(z) when k=0

                double u = z.real;
                double v = z.imag;

                // Simplified sn using modified sine with elliptic modulus effect
                // sn(u+iv, k) ≈ sin(u)·cosh(k'v) / sqrt(1-k²sin²u·sinh²(k'v)) + i·term
                // For visualization, use approximation: sn ≈ sin(z) / (1 + k²·z²/6)

                double sinU = std::sin(u);
                double cosU = std::cos(u);
                double sinhV = std::sinh(v);
                double coshV = std::cosh(v);

                // Real part: sin(u)·cosh(v)
                double snReal = sinU * coshV;
                // Imaginary part: cos(u)·sinh(v)
                double snImag = cosU * sinhV;

                // Apply elliptic modulus correction (simplified)
                double k2 = k * k;
                double correction = 1.0 / (1.0 + k2 * magSq / 6.0);

                z.real = snReal * correction + c.real;
                z.imag = snImag * correction + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("JacobiSN");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Weierstrass ζ (Zeta) Function
        //=========================================================================
        spec.name = "WeierstrassZeta";
        spec.displayName = "Weierstrass ζ-function";
        spec.category = "Elliptic Functions";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses the Weierstrass zeta function: z = ζ(z) + c. The ζ-function is the logarithmic derivative of the sigma function and is quasi-periodic.";
        spec.formula = "z = ζ(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \zeta(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.3, 0.3);
            const double bailout = 100.0;
            const double g2 = 4.0;
            const double g3 = 0.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                if (magSq < 1e-10)
                    break;

                ComplexD zeta = WeierstrassZeta(z, g2, g3);
                z.real = zeta.real + c.real;
                z.imag = zeta.imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("WeierstrassZeta");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Weierstrass σ (Sigma) Function
        //=========================================================================
        spec.name = "WeierstrassSigma";
        spec.displayName = "Weierstrass σ-function";
        spec.category = "Elliptic Functions";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses the Weierstrass sigma function: z = σ(z) + c. The σ-function is an entire function whose logarithmic derivative is the zeta function.";
        spec.formula = "z = σ(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \sigma(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 100.0;
            const double g2 = 4.0;
            const double g3 = 0.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                ComplexD sigma = WeierstrassSigma(z, g2, g3);
                z.real = sigma.real + c.real;
                z.imag = sigma.imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("WeierstrassSigma");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);
    }
}
