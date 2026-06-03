#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {
    //=============================================================================
    // Trigonometric Extended Family
    // Additional trigonometric function-based fractals
    //=============================================================================

    void RegisterTrigonometricExtendedFamily()
    {
        FractalSpec spec;
        InitialConditions ic;  // Declare ONCE at the top

        //=========================================================================
        // Chebyshev Polynomial (First Kind)
        //=========================================================================
        spec.name = "ChebyshevPolynomial";
        spec.displayName = "Chebyshev Polynomial";
        spec.category = "Classical Polynomials";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Chebyshev polynomials of the first kind Tₙ(z): z = Tₙ(z) + c. Computed via recurrence: T₀=1, T₁=z, Tₙ₊₁=2zTₙ - Tₙ₋₁. Related to cos(n·arccos(z)) and used in approximation theory. Named after Pafnuty Chebyshev.";
        spec.formula = "z = Tₙ(z) + c";
        spec.formulaLatex = R"(z_{n+1} = T_n(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 1000.0;  // Increased bailout for better escape detection
            const int n = 4;  // Degree 4 for better balance of escape/bounded regions

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute Tₙ(z) using recurrence relation
                // T₀(z) = 1, T₁(z) = z
                // Tₖ₊₁(z) = 2z·Tₖ(z) - Tₖ₋₁(z)

                ComplexD T_prev(1.0, 0.0);  // T₀ = 1
                ComplexD T_curr = z;         // T₁ = z

                if (n == 0) {
                    z.real = 1.0 + c.real;
                    z.imag = c.imag;
                }
                else if (n == 1) {
                    z.real += c.real;
                    z.imag += c.imag;
                }
                else {
                    // Compute Tₙ using recurrence
                    for (int k = 1; k < n; ++k) {
                        // T_next = 2z * T_curr - T_prev
                        ComplexD zT_curr(
                            2.0 * (z.real * T_curr.real - z.imag * T_curr.imag),
                            2.0 * (z.real * T_curr.imag + z.imag * T_curr.real)
                        );

                        ComplexD T_next(
                            zT_curr.real - T_prev.real,
                            zT_curr.imag - T_prev.imag
                        );

                        T_prev = T_curr;
                        T_curr = T_next;
                    }

                    z.real = T_curr.real + c.real;
                    z.imag = T_curr.imag + c.imag;
                }
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = -0.8737499999999998;  // Viewport tuning: from registry
        //spec.defaultCenterY = -0.0089062499999999975;  // Viewport tuning: from registry
        //spec.defaultZoom = 1.2;  // Viewport tuning: X Scale Width from registry
        ic = InitialConditionsService::Get("ChebyshevPolynomial");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 1000.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Cotangent Fractal
        //=========================================================================
        spec.name = "CotMandel";
        spec.displayName = "Cotangent Mandelbrot";
        spec.category = "Trigonometric";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Mandelbrot with cotangent function: z = cot(z) + c";
        spec.formula = "z = cot(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \cot(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.1, 0.1);
            const double bailout = 256.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // cot(z) = cos(z)/sin(z)
                double sin_real = std::sin(z.real) * std::cosh(z.imag);
                double sin_imag = std::cos(z.real) * std::sinh(z.imag);

                double cos_real = std::cos(z.real) * std::cosh(z.imag);
                double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

                double sin_magSq = sin_real * sin_real + sin_imag * sin_imag;
                if (sin_magSq < 1e-10) break;

                double cot_real = (cos_real * sin_real + cos_imag * sin_imag) / sin_magSq;
                double cot_imag = (cos_imag * sin_real - cos_real * sin_imag) / sin_magSq;

                z.real = cot_real + c.real;
                z.imag = cot_imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = -3.18;  // Viewport tuning: from registry
        //spec.defaultCenterY = -0.20;  // Viewport tuning: from registry
        //spec.defaultZoom = 10.202898551;  // Viewport tuning: X Scale Width from registry
        ic = InitialConditionsService::Get("CotMandel");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Secant Fractal
        //=========================================================================
        spec.name = "SecMandel";
        spec.displayName = "Secant Mandelbrot";
        spec.category = "Trigonometric";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Mandelbrot with secant function: z = sec(z) + c";
        spec.formula = "z = sec(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \sec(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 256.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // sec(z) = 1/cos(z)
                double cos_real = std::cos(z.real) * std::cosh(z.imag);
                double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

                double cos_magSq = cos_real * cos_real + cos_imag * cos_imag;
                if (cos_magSq < 1e-10) break;

                double sec_real = cos_real / cos_magSq;
                double sec_imag = -cos_imag / cos_magSq;

                z.real = sec_real + c.real;
                z.imag = sec_imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = 1.54;  // Viewport tuning: from registry
        //spec.defaultCenterY = 0.02;  // Viewport tuning: from registry
        //spec.defaultZoom = 64.0;  // Viewport tuning: X Scale Width from registry
        ic = InitialConditionsService::Get("SecMandel");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Cosecant Fractal
        //=========================================================================
        spec.name = "CscMandel";
        spec.displayName = "Cosecant Mandelbrot";
        spec.category = "Trigonometric";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Mandelbrot with cosecant function: z = csc(z) + c";
        spec.formula = "z = csc(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \csc(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.1, 0.1);
            const double bailout = 256.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // csc(z) = 1/sin(z)
                double sin_real = std::sin(z.real) * std::cosh(z.imag);
                double sin_imag = std::cos(z.real) * std::sinh(z.imag);

                double sin_magSq = sin_real * sin_real + sin_imag * sin_imag;
                if (sin_magSq < 1e-10) break;

                double csc_real = sin_real / sin_magSq;
                double csc_imag = -sin_imag / sin_magSq;

                z.real = csc_real + c.real;
                z.imag = csc_imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 1.5;
        ic = InitialConditionsService::Get("CscMandel");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

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
        //spec.defaultCenterX = -7.084664462935784;  // Viewport tuning: from registry
        //spec.defaultCenterY = 0.1481917598835060;  // Viewport tuning: from registry
        //spec.defaultZoom = 0.19299539049631545;  // Viewport tuning: X Scale Width from registry
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
        //spec.defaultCenterX = 0.0043229257991153069;  // Viewport tuning: from registry
        //spec.defaultCenterY = 0.043608552897599878;  // Viewport tuning: from registry
        //spec.defaultZoom = 0.15261363593213784;  // Viewport tuning: X Scale Width from registry
        ic = InitialConditionsService::Get("JacobiSN");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Legendre Polynomial
        //=========================================================================
        spec.name = "LegendrePolynomial";
        spec.displayName = "Legendre Polynomial";
        spec.category = "Classical Polynomials";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Legendre polynomials Pₙ(z): z = Pₙ(z) + c. Computed via recurrence: P₀=1, P₁=z, Pₙ₊₁=(2n+1)zPₙ - nPₙ₋₁/(n+1). Studied by Legendre and Laplace.";
        spec.formula = "z = Pₙ(z) + c";
        spec.formulaLatex = R"(z_{n+1} = P_n(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 100.0;
            const int n = 5;  // Polynomial degree

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute Pₙ(z) using recurrence relation
                // P₀(z) = 1, P₁(z) = z
                // Pₖ₊₁(z) = ((2k+1)·z·Pₖ(z) - k·Pₖ₋₁(z)) / (k+1)

                ComplexD P_prev(1.0, 0.0);  // P₀ = 1
                ComplexD P_curr = z;         // P₁ = z

                if (n == 0) {
                    z.real = 1.0 + c.real;
                    z.imag = c.imag;
                }
                else if (n == 1) {
                    z.real += c.real;
                    z.imag += c.imag;
                }
                else {
                    // Compute Pₙ using recurrence
                    for (int k = 1; k < n; ++k) {
                        double coeff1 = (2.0 * k + 1.0) / (k + 1.0);
                        double coeff2 = static_cast<double>(k) / (k + 1.0);

                        // P_next = coeff1 * z * P_curr - coeff2 * P_prev
                        ComplexD zP_curr(
                            coeff1 * (z.real * P_curr.real - z.imag * P_curr.imag),
                            coeff1 * (z.real * P_curr.imag + z.imag * P_curr.real)
                        );

                        ComplexD P_next(
                            zP_curr.real - coeff2 * P_prev.real,
                            zP_curr.imag - coeff2 * P_prev.imag
                        );

                        P_prev = P_curr;
                        P_curr = P_next;
                    }

                    z.real = P_curr.real + c.real;
                    z.imag = P_curr.imag + c.imag;
                }
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 1.5;
        ic = InitialConditionsService::Get("LegendrePolynomial");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Hyperbolic Secant
        //=========================================================================
        spec.name = "SechMandel";
        spec.displayName = "Sech Mandelbrot";
        spec.category = "Trigonometric";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Mandelbrot with hyperbolic secant: z = sech(z) + c";
        spec.formula = "z = sech(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \text{sech}(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.0, 0.0);
            const double bailout = 256.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // sech(z) = 1/cosh(z)
                double cosh_real = std::cosh(z.real) * std::cos(z.imag);
                double cosh_imag = std::sinh(z.real) * std::sin(z.imag);

                double cosh_magSq = cosh_real * cosh_real + cosh_imag * cosh_imag;
                if (cosh_magSq < 1e-10) break;

                double sech_real = cosh_real / cosh_magSq;
                double sech_imag = -cosh_imag / cosh_magSq;

                z.real = sech_real + c.real;
                z.imag = sech_imag + c.imag;
            }

            return static_cast<double>(maxIter);
            };

        spec.supportsJulia = false;
        //spec.defaultCenterX = -0.77812500000000007;  // Viewport tuning: from registry
        //spec.defaultCenterY = 0.03515625;  // Viewport tuning: from registry
        //spec.defaultZoom = 12.0;  // Viewport tuning: X Scale Width from registry
        ic = InitialConditionsService::Get("SechMandel");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);
    }
} // namespace Native
