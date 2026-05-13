#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Burning Ship Family
// Formula: z = (|Re(z)| + i|Im(z)|)² + c
//=============================================================================

void RegisterBurningShipFamily()
{
    FractalSpec spec;

    spec.name = "BurningShip";
    spec.displayName = "Burning Ship";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship fractal. Takes absolute values before squaring: z = (|Re(z)| + i|Im(z)|)² + c";

    // Use existing calculation function
    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateBurningShip(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;

    // Burning Ship has different default view (ship appears in bottom-left quadrant)
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = -0.5;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;  // No symmetry due to absolute values

    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Burning Ship Power Variations
    //=========================================================================

    // Burning Ship Cubic (power 3)
    spec.name = "BurningShipCubic";
    spec.displayName = "Burning Ship Cubic";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship with cubic power: z = (|Re(z)| + i|Im(z)|)³ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double abs_re = abs(z.real);
            double abs_im = abs(z.imag);
            ComplexD abs_z(abs_re, abs_im);

            // z^3
            z = abs_z * abs_z * abs_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;  // Center at origin
    spec.defaultCenterY = 0.0;  // Center at origin
    spec.defaultZoom = 0.6;     // Viewport: 5.0000 × 2.8125
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Burning Ship Quartic (power 4)
    spec.name = "BurningShipQuartic";
    spec.displayName = "Burning Ship Quartic";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship with quartic power: z = (|Re(z)| + i|Im(z)|)⁴ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double abs_re = abs(z.real);
            double abs_im = abs(z.imag);
            ComplexD abs_z(abs_re, abs_im);
            ComplexD z2 = abs_z * abs_z;

            z = z2 * z2 + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;  // Center at origin
    spec.defaultCenterY = 0.0;  // Center at origin
    spec.defaultZoom = 0.6;     // Viewport: 5.0000 × 2.8125
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Burning Ship Quintic (power 5)
    spec.name = "BurningShipQuintic";
    spec.displayName = "Burning Ship Quintic";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship with quintic power: z = (|Re(z)| + i|Im(z)|)⁵ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double abs_re = abs(z.real);
            double abs_im = abs(z.imag);
            ComplexD abs_z(abs_re, abs_im);
            ComplexD z2 = abs_z * abs_z;
            ComplexD z4 = z2 * z2;

            z = z4 * abs_z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;  // Center at origin
    spec.defaultCenterY = 0.0;  // Center at origin
    spec.defaultZoom = 0.6435;  // Viewport: 4.662204 × 2.662490
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Perpendicular Burning Ship
    spec.name = "PerpendicularBurningShip";
    spec.displayName = "Perpendicular Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Perpendicular variant: z = (|Re(z)| - i|Im(z)|)² + c";

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
    spec.defaultCenterX = -0.4;  // Center at (-0.4, 0.5)
    spec.defaultCenterY = 0.5;   // Center at (-0.4, 0.5)
    spec.defaultZoom = 0.6821;   // Viewport: 4.397398 × 2.473536
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Buffalo Burning Ship
    spec.name = "BuffaloBurningShip";
    spec.displayName = "Buffalo Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Buffalo variant with subtraction: z = (|Re(z)| + i|Im(z)|)² - c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(abs(z.real), abs(z.imag));
            z = z * z - constant;  // Note: minus c

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.3;  // Center at (0.3, 0.6)
    spec.defaultCenterY = 0.6;  // Center at (0.3, 0.6)
    spec.defaultZoom = 0.8;     // Viewport: 3.750000 × 2.109375
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Shark Burning Ship
    spec.name = "SharkBurningShip";
    spec.displayName = "Shark Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Shark variant: z = (|Re(z)| + i|Im(z)|)² + c/z";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.1, 0.1);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double z_mag_sq = z.real * z.real + z.imag * z.imag;
            if (z_mag_sq < 1e-10) break;

            ComplexD c_over_z((constant.real * z.real + constant.imag * z.imag) / z_mag_sq,
                             (constant.imag * z.real - constant.real * z.imag) / z_mag_sq);

            z = ComplexD(abs(z.real), abs(z.imag));
            z = z * z + c_over_z;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;   // Center at origin
    spec.defaultCenterY = 0.0;   // Center at origin
    spec.defaultZoom = 5.8203;   // Viewport: 0.515444 × 0.29937
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Celtic Burning Ship
    spec.name = "CelticBurningShip";
    spec.displayName = "Celtic Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Celtic variant: z = (|Re(z²)| + i*Im(z²)) + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = z * z;
            z = ComplexD(abs(z.real), z.imag) + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.75;  // Center at (-0.75, 0.00)
    spec.defaultCenterY = 0.0;    // Center at (-0.75, 0.00)
    spec.defaultZoom = 0.4854;    // Viewport: 6.180277 × 3.476406
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Reverse Burning Ship
    spec.name = "ReverseBurningShip";
    spec.displayName = "Reverse Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Reverse variant: z = (Re(z) + i|Im(z)|)² + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(z.real, abs(z.imag));
            z = z * z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;   // Center at origin
    spec.defaultCenterY = 0.0;   // Center at origin
    spec.defaultZoom = 0.6709;   // Viewport: 4.471218 × 2.515060
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Vertical Burning Ship
    spec.name = "VerticalBurningShip";
    spec.displayName = "Vertical Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Vertical variant: z = (|Re(z)| + i*Im(z))² + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            z = ComplexD(abs(z.real), z.imag);
            z = z * z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;   // Center at origin
    spec.defaultCenterY = 0.0;   // Center at origin
    spec.defaultZoom = 1.2015;   // Viewport: 2.497043 × 1.404587
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    // Diagonal Burning Ship
    spec.name = "DiagonalBurningShip";
    spec.displayName = "Diagonal Burning Ship";
    spec.category = "Burning Ship Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Diagonal variant: z = (|Re(z) + Im(z)| + i|Re(z) - Im(z)|)² + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            double sum = abs(z.real + z.imag);
            double diff = abs(z.real - z.imag);
            z = ComplexD(sum, diff);
            z = z * z + constant;

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;   // Center at (0.00, -0.20)
    spec.defaultCenterY = -0.2;  // Center at (0.00, -0.20)
    spec.defaultZoom = 1.2059;   // Viewport: 2.487729 × 1.399347
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
