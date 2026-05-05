#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterHistoricalFractalsFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // HISTORICAL & LESSER-KNOWN FRACTALS
        // Classic fractals from early fractal research and lesser-known discoveries
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Pickover Biomorphs
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Biomorphs";
            spec.displayName = "Pickover Biomorphs";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Created by Clifford Pickover in the 1980s. Uses modified escape condition that creates biological-looking structures resembling microorganisms.";
            spec.formula = "z(n+1) = z² + c, escape when |Re(z)| > threshold OR |Im(z)| > threshold";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ escape: } |x| > R \text{ or } |y| > R)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Organic, biological forms; resembles microorganisms under microscope";
            spec.discoveredBy = "Clifford A. Pickover";
            spec.discoveryYear = 1986;
            spec.computationalNotes = "Modified escape condition creates bilateral symmetry";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 10.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double threshold = 10.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Standard z² + c iteration
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Biomorph escape condition: either component exceeds threshold
                    if (std::abs(z.real) > threshold || std::abs(z.imag) > threshold)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Pickover Stalks (Biomorph variant)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "PickoverStalks";
            spec.displayName = "Pickover Stalks";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Variant of biomorphs using sine function. Creates stalk-like structures extending from main body.";
            spec.formula = "z(n+1) = sin(z) + z² + c";
            spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + z_n^2 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Stalk-like protrusions, plant-like structures, feathery details";
            spec.discoveredBy = "Clifford A. Pickover";
            spec.discoveryYear = 1988;
            spec.computationalNotes = "Combines trigonometric and polynomial terms";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // sin(z) for complex z: sin(x+iy) = sin(x)cosh(y) + i*cos(x)sinh(y)
                    double sinReal = std::sin(x) * std::cosh(y);
                    double sinImag = std::cos(x) * std::sinh(y);

                    // z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    // sin(z) + z² + c
                    z.real = sinReal + zSqReal + constant.real;
                    z.imag = sinImag + zSqImag + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Martin Map (Hopalong variant)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "MartinMap";
            spec.displayName = "Martin Map";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Created by Barry Martin. Uses unusual iteration with square roots creating organic, flowing patterns.";
            spec.formula = "x(n+1) = y - sign(x)*sqrt(|bx - c|), y(n+1) = a - x";
            spec.formulaLatex = R"(x_{n+1} = y_n - \text{sgn}(x_n)\sqrt{|bx_n - c|}, \quad y_{n+1} = a - x_n)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Flowing, organic curves; strange attractor-like patterns";
            spec.discoveredBy = "Barry Martin";
            spec.discoveryYear = 1986;
            spec.computationalNotes = "Parameters a, b, c control structure; uses real-valued iteration";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.02;
            spec.defaultBailout = 1000.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                // Parameters derived from c
                double a = c.real * 10.0;
                double b = c.imag * 10.0;
                double cp = 0.5;

                double x = 0.0;
                double y = 0.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = y - (x >= 0.0 ? 1.0 : -1.0) * std::sqrt(std::abs(b * x - cp));
                    double yNew = a - x;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 1000.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Chip Map (Clifford Pickover)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ChipMap";
            spec.displayName = "Chip Map";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Another Pickover discovery. Creates silicon chip-like patterns with rectangular structures.";
            spec.formula = "z(n+1) = (z² + c) mod 2π";
            spec.formulaLatex = R"(z_{n+1} = (z_n^2 + c) \bmod 2\pi)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Rectangular, circuit board-like patterns; digital appearance";
            spec.discoveredBy = "Clifford A. Pickover";
            spec.discoveryYear = 1987;
            spec.computationalNotes = "Modulo operation creates periodic boundaries";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                const double TWO_PI = 6.283185307179586;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z² + c
                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Apply modulo 2π to create periodic boundaries
                    z.real = std::fmod(z.real, TWO_PI);
                    z.imag = std::fmod(z.imag, TWO_PI);

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Quaternion Julia (2D Projection)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "QuaternionJulia2D";
            spec.displayName = "Quaternion Julia (2D slice)";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "2D slice of 4D quaternion Julia set. Uses quaternion multiplication projected to complex plane.";
            spec.formula = "q(n+1) = q² + c (quaternion), project to (x,y)";
            spec.formulaLatex = R"(q_{n+1} = q_n^2 + c, \quad q \in \mathbb{H})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "3D-like depth appearance, rounded bulbous forms";
            spec.discoveredBy = "John C. Hart";
            spec.discoveryYear = 1989;
            spec.computationalNotes = "Simplified quaternion multiplication for 2D visualization";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                // Quaternion as (w, x, y, z) - we'll use (0, c.real, c.imag, 0) for initial
                double w = 0.0, x = c.real, y = c.imag, z = 0.0;
                ComplexD constant = isJulia ? juliaC : c;
                double cw = 0.0, cx = constant.real, cy = constant.imag, cz = 0.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Quaternion multiplication: q² 
                    // (w,x,y,z)² = (w²-x²-y²-z², 2wx, 2wy, 2wz)
                    double w2 = w * w - x * x - y * y - z * z;
                    double x2 = 2.0 * w * x;
                    double y2 = 2.0 * w * y;
                    double z2 = 2.0 * w * z;

                    // Add c
                    w = w2 + cw;
                    x = x2 + cx;
                    y = y2 + cy;
                    z = z2 + cz;

                    // Quaternion magnitude
                    double mag2 = w * w + x * x + y * y + z * z;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Collatz Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CollatzFractal";
            spec.displayName = "Collatz Fractal";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Visualization of Collatz conjecture in complex plane. Uses complex extension: if |z| even: z/2, if odd: (3z+1)/2.";
            spec.formula = "z(n+1) = z/2 if |z| even-like, else (3z+1)/2";
            spec.formulaLatex = R"(z_{n+1} = \begin{cases} z_n/2 & \text{if } |z_n| \equiv 0 \pmod{2} \\ (3z_n+1)/2 & \text{otherwise} \end{cases})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Fractal tree structure, branching patterns related to famous conjecture";
            spec.discoveredBy = "Extensions of Lothar Collatz's work";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "Complex extension of Collatz map creates fractal basins";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    // Use magnitude to determine "evenness" in complex extension
                    if (std::fmod(mag * 10.0, 2.0) < 1.0)
                    {
                        // "Even" path: z/2
                        z.real /= 2.0;
                        z.imag /= 2.0;
                    }
                    else
                    {
                        // "Odd" path: (3z+1)/2
                        z.real = (3.0 * z.real + 1.0) / 2.0;
                        z.imag = (3.0 * z.imag) / 2.0;
                    }

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    // Check for convergence to 1 (in Collatz) or escape
                    if (mag2 < 0.01 || mag2 > 256.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Duffing Map
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "DuffingMap";
            spec.displayName = "Duffing Map";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Discrete version of Duffing oscillator. Shows chaotic behavior and strange attractors from forced oscillator dynamics.";
            spec.formula = "x(n+1) = y, y(n+1) = -bx + ay - y³";
            spec.formulaLatex = R"(x_{n+1} = y_n, \quad y_{n+1} = -bx_n + ay_n - y_n^3)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Curved attractor basins, chaotic regions, double-well potential structure";
            spec.discoveredBy = "Georg Duffing";
            spec.discoveryYear = 1918;
            spec.computationalNotes = "Models nonlinear oscillator with cubic restoring force";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                // Parameters from position
                double a = 2.75;
                double b = 0.2;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = y;
                    double yNew = -b * x + a * y - y * y * y;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Sinusoidal Fractal (Early sine-based)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SinusoidalFractal";
            spec.displayName = "Sinusoidal Fractal";
            spec.category = "Historical Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Early transcendental fractal using z(n+1) = c*sin(z). Creates wavy, periodic structures due to sine periodicity.";
            spec.formula = "z(n+1) = c * sin(z)";
            spec.formulaLatex = R"(z_{n+1} = c \cdot \sin(z_n))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Wavy bands, periodic vertical structure, smooth curves";
            spec.discoveredBy = "Early transcendental fractal research";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "Sine function creates natural periodicity in imaginary direction";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.2;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // sin(z) = sin(x+iy) = sin(x)cosh(y) + i*cos(x)sinh(y)
                    double sinReal = std::sin(z.real) * std::cosh(z.imag);
                    double sinImag = std::cos(z.real) * std::sinh(z.imag);

                    // c * sin(z)
                    z.real = constant.real * sinReal - constant.imag * sinImag;
                    z.imag = constant.real * sinImag + constant.imag * sinReal;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }
    }
}
