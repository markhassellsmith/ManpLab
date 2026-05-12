#pragma once

#include <cmath>
#include "ColorPalette.h"

// Native C++ Mandelbrot calculator
// Simple implementation for Phase 2 interop testing
// Will be replaced with full ManpWIN64 engine integration later

namespace Native {

    // Simple complex number structure for C++ calculations
    struct ComplexD
    {
        union {
            struct { double x, y; };      // For compatibility with existing Complex class
            struct { double real, imag; }; // For new code readability
        };

        ComplexD() : x(0.0), y(0.0) {}
        ComplexD(double r, double i) : x(r), y(i) {}

        // Basic arithmetic operators
        ComplexD operator+(const ComplexD& other) const {
            return ComplexD(x + other.x, y + other.y);
        }

        ComplexD operator-(const ComplexD& other) const {
            return ComplexD(x - other.x, y - other.y);
        }

        ComplexD operator*(const ComplexD& other) const {
            return ComplexD(x * other.x - y * other.y, x * other.y + y * other.x);
        }

        ComplexD operator/(const ComplexD& other) const {
            double denom = other.x * other.x + other.y * other.y;
            return ComplexD((x * other.x + y * other.y) / denom,
                          (y * other.x - x * other.y) / denom);
        }

        ComplexD operator*(double scalar) const {
            return ComplexD(x * scalar, y * scalar);
        }

        ComplexD operator/(double scalar) const {
            return ComplexD(x / scalar, y / scalar);
        }

        friend ComplexD operator*(double scalar, const ComplexD& c) {
            return ComplexD(c.x * scalar, c.y * scalar);
        }
    };

    /// <summary>
    /// Parameters for Mandelbrot calculation
    /// </summary>
    struct MandelbrotParams
    {
        int width;
        int height;
        int maxIterations;
        double centerX;
        double centerY;
        double viewWidth;
        double viewHeight;
        bool isJulia;
        double juliaCX;
        double juliaCY;
    };

    /// <summary>
    /// Simple Mandelbrot calculator
    /// Based on classic algorithm: z = z^2 + c
    /// </summary>
    class MandelbrotCalculator
    {
    public:
        /// <summary>
        /// Calculate Mandelbrot/Julia set iteration count for a point
        /// </summary>
        /// <param name="c">Complex point to test</param>
        /// <param name="maxIter">Maximum iterations</param>
        /// <param name="isJulia">If true, use Julia set mode with fixed c</param>
        /// <param name="juliaC">Fixed c value for Julia set</param>
        /// <returns>Iteration count (maxIter if in set)</returns>
        static int CalculateIterations(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                // Julia set: z starts at pixel location, c is fixed
                z = c;
                constant = juliaC;
            }
            else
            {
                // Mandelbrot: z starts at origin, c is pixel location
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            int iteration = 0;
            double bailout = 4.0;  // Standard bailout radius^2

            // Main iteration loop: z = z^2 + c
            while (iteration < maxIter)
            {
                // Calculate z^2
                double x2 = z.x * z.x;
                double y2 = z.y * z.y;

                // Bailout test: |z|^2 > 4
                if (x2 + y2 > bailout)
                    break;

                // z = z^2 + c
                // real part: z.x^2 - z.y^2 + c.x
                // imag part: 2*z.x*z.y + c.y
                double zx_new = x2 - y2 + constant.x;
                double zy_new = 2.0 * z.x * z.y + constant.y;

                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return iteration;
        }

        /// <summary>
        /// Calculate smooth iteration count for better coloring
        /// Uses continuous potential method
        /// </summary>
        static double CalculateSmoothIterations(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            int iteration = 0;
            double bailout = 256.0;  // Higher bailout for smooth coloring

            while (iteration < maxIter)
            {
                double x2 = z.x * z.x;
                double y2 = z.y * z.y;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    // Smooth iteration using log formula
                    // n + 1 - log(log|z|) / log(2)
                    double log_zn = log(magnitude2) / 2.0;  // log(|z|)
                    double nu = log(log_zn / log(2.0)) / log(2.0);
                    return iteration + 1.0 - nu;
                }

                double zx_new = x2 - y2 + constant.x;
                double zy_new = 2.0 * z.x * z.y + constant.y;

                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return (double)maxIter;  // Point is in the set
        }

        /// <summary>
        /// Calculate Burning Ship fractal: z = (|Re(z)| + i|Im(z)|)^2 + c
        /// </summary>
        static double CalculateBurningShip(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            int iteration = 0;
            double bailout = 256.0;

            while (iteration < maxIter)
            {
                // Burning Ship: take absolute values before squaring
                double zx_abs = fabs(z.x);
                double zy_abs = fabs(z.y);

                double x2 = zx_abs * zx_abs;
                double y2 = zy_abs * zy_abs;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    double log_zn = log(magnitude2) / 2.0;
                    double nu = log(log_zn / log(2.0)) / log(2.0);
                    return iteration + 1.0 - nu;
                }

                double zx_new = x2 - y2 + constant.x;
                double zy_new = 2.0 * zx_abs * zy_abs + constant.y;

                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return (double)maxIter;
        }

        /// <summary>
        /// Calculate Tricorn (Mandelbar) fractal: z = conj(z)^2 + c
        /// </summary>
        static double CalculateTricorn(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            int iteration = 0;
            double bailout = 256.0;

            while (iteration < maxIter)
            {
                double x2 = z.x * z.x;
                double y2 = z.y * z.y;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    double log_zn = log(magnitude2) / 2.0;
                    double nu = log(log_zn / log(2.0)) / log(2.0);
                    return iteration + 1.0 - nu;
                }

                // Tricorn: conjugate before squaring (negate imaginary part)
                double zx_new = x2 - y2 + constant.x;
                double zy_new = -2.0 * z.x * z.y + constant.y;

                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return (double)maxIter;
        }

        /// <summary>
        /// Calculate Phoenix fractal: z = z^2 + Re(c) + Im(c)*p
        /// where p is the previous z value
        /// </summary>
        static double CalculatePhoenix(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            ComplexD p(0.0, 0.0);  // Previous z value
            int iteration = 0;
            double bailout = 256.0;

            while (iteration < maxIter)
            {
                double x2 = z.x * z.x;
                double y2 = z.y * z.y;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    double log_zn = log(magnitude2) / 2.0;
                    double nu = log(log_zn / log(2.0)) / log(2.0);
                    return iteration + 1.0 - nu;
                }

                // Phoenix: z_new = z^2 + real(c) + imag(c)*previous_z
                double zx_new = x2 - y2 + constant.x + constant.y * p.x;
                double zy_new = 2.0 * z.x * z.y + constant.y * p.y;

                p = z;  // Save current z as previous
                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return (double)maxIter;
        }

        /// <summary>
        /// Map pixel coordinates to complex plane
        /// </summary>
        static ComplexD PixelToComplex(int px, int py, const MandelbrotParams& params)
        {
            // Map pixel (px, py) to complex coordinates
            // X: left to right in both screen and fractal coords
            double x = params.centerX - (params.viewWidth / 2.0) + (px * params.viewWidth / params.width);

            // Y: screen coords go top-to-bottom, fractal coords go bottom-to-top
            // py=0 (top) should map to centerY + viewHeight/2 (top of fractal)
            // py=height (bottom) should map to centerY - viewHeight/2 (bottom of fractal)
            double y = params.centerY + (params.viewHeight / 2.0) - (py * params.viewHeight / params.height);

            return ComplexD(x, y);
        }

        /// <summary>
        /// Color iteration using selected palette
        /// </summary>
        static ColorRGB IterationToColor(double iteration, int maxIter, PaletteType palette, int colorOffset = 0)
        {
            return ColorPalette::GetColor(iteration, maxIter, palette, colorOffset);
        }

        /// <summary>
        /// Simple grayscale coloring based on iteration count (legacy)
        /// </summary>
        static void IterationToGrayscale(double iteration, int maxIter, unsigned char& r, unsigned char& g, unsigned char& b)
        {
            ColorRGB color = ColorPalette::GetColor(iteration, maxIter, PaletteType::Grayscale);
            r = color.r;
            g = color.g;
            b = color.b;
        }

        /// <summary>
        /// Calculate distance estimation for edge highlighting.
        /// Returns a value representing distance to the set boundary.
        /// </summary>
        static double CalculateDistanceEstimation(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            ComplexD dz(1.0, 0.0);  // Derivative starts at 1
            int iteration = 0;
            double bailout = 256.0;

            while (iteration < maxIter)
            {
                double x2 = z.x * z.x;
                double y2 = z.y * z.y;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    // Distance estimation formula: |z|*log|z| / |dz|
                    double magnitude = sqrt(magnitude2);
                    double dzMagnitude = sqrt(dz.x * dz.x + dz.y * dz.y);

                    if (dzMagnitude > 0.0)
                    {
                        double distance = (magnitude * log(magnitude)) / dzMagnitude;
                        // Return normalized value for coloring (0-1 range mapped to 0-maxIter)
                        return (1.0 - exp(-distance * 10.0)) * maxIter;
                    }
                    else
                    {
                        return iteration;
                    }
                }

                // Update derivative: dz = 2*z*dz + 1 (for Mandelbrot)
                double dz_x = 2.0 * (z.x * dz.x - z.y * dz.y);
                double dz_y = 2.0 * (z.x * dz.y + z.y * dz.x);
                dz.x = dz_x;
                dz.y = dz_y;

                // Update z: z = z^2 + c
                double zx_new = x2 - y2 + constant.x;
                double zy_new = 2.0 * z.x * z.y + constant.y;
                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            return 0.0;  // Inside the set = black
        }

        /// <summary>
        /// Calculate orbit trap coloring based on minimum distance to trap point/shape.
        /// Colors based on how close the orbit gets to the origin (circular trap).
        /// </summary>
        static double CalculateOrbitTrap(ComplexD c, int maxIter, bool isJulia = false, ComplexD juliaC = ComplexD())
        {
            ComplexD z;
            ComplexD constant;

            if (isJulia)
            {
                z = c;
                constant = juliaC;
            }
            else
            {
                z = ComplexD(0.0, 0.0);
                constant = c;
            }

            double minDistance = 1e10;  // Track minimum distance to trap
            int iteration = 0;
            double bailout = 256.0;

            while (iteration < maxIter)
            {
                // Calculate distance to origin (circular trap at 0,0)
                double distance = sqrt(z.x * z.x + z.y * z.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }

                double x2 = z.x * z.x;
                double y2 = z.y * z.y;
                double magnitude2 = x2 + y2;

                if (magnitude2 > bailout)
                {
                    // Color based on minimum distance to trap
                    // Normalize to iteration range for palette mapping
                    return (1.0 - exp(-minDistance * 5.0)) * maxIter;
                }

                // Update z: z = z^2 + c
                double zx_new = x2 - y2 + constant.x;
                double zy_new = 2.0 * z.x * z.y + constant.y;
                z.x = zx_new;
                z.y = zy_new;

                iteration++;
            }

            // Inside the set - color by minimum distance
            return (1.0 - exp(-minDistance * 5.0)) * maxIter;
        }
    };

} // namespace Native

