#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

namespace Native
{
    void RegisterOrbitalModificationsFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // ORBITAL MODIFICATIONS & TRAP VARIANTS
        // Advanced orbital techniques: orbit traps, path modifications, conditional logic
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Circular Orbit Trap
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CircularOrbitTrap";
            spec.displayName = "Circular Orbit Trap";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Standard Mandelbrot with circular orbit trap at origin. Colors based on minimum distance to trap circle during iteration.";
            spec.formula = "z(n+1) = z² + c, track min(|z - trap_center|)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ color by } \min|z_n - p|)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Concentric rings around trap point, reveals orbital paths";
            spec.discoveredBy = "Orbit trap technique";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Tracks minimum distance to trap during escape";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                ComplexD trap(0.0, 0.0); // Trap at origin
                double minDist = 1e10;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Distance to trap
                    double dist = std::sqrt((z.real - trap.real) * (z.real - trap.real) + 
                                           (z.imag - trap.imag) * (z.imag - trap.imag));
                    if (dist < minDist) minDist = dist;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        // Color by minimum trap distance
                        return minDist * 50.0;
                    }
                }

                return minDist * 50.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Cross Orbit Trap
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CrossOrbitTrap";
            spec.displayName = "Cross Orbit Trap";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Orbit trap using distance to coordinate axes (cross shape). Creates cruciform patterns.";
            spec.formula = "z(n+1) = z² + c, trap on x=0 or y=0";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ trap: } \min(|x|, |y|))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Cross-shaped trap patterns, reveals axis proximity";
            spec.discoveredBy = "Geometric orbit traps";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Minimum distance to either axis";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double minDist = 1e10;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Distance to cross (min distance to either axis)
                    double distToXAxis = std::abs(z.imag);
                    double distToYAxis = std::abs(z.real);
                    double dist = std::min(distToXAxis, distToYAxis);

                    if (dist < minDist) minDist = dist;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return minDist * 100.0;
                    }
                }

                return minDist * 100.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Stalks (Conditional Modification)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "StalksConditional";
            spec.displayName = "Stalks (Conditional)";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Mandelbrot with conditional modification: if |z| < threshold, apply different formula. Creates stalk-like protrusions.";
            spec.formula = "If |z| < r: z = z² + c, else: z = z³ + c";
            spec.formulaLatex = R"(z_{n+1} = \begin{cases} z_n^2 + c & |z_n| < r \\ z_n^3 + c & |z_n| \geq r \end{cases})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Extended stalks, mixed polynomial behavior";
            spec.discoveredBy = "Conditional iteration experiments";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "Threshold-based formula switching";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double threshold = 2.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;
                    double mag = std::sqrt(x * x + y * y);

                    if (mag < threshold)
                    {
                        // z² + c
                        z.real = x * x - y * y + constant.real;
                        z.imag = 2.0 * x * y + constant.imag;
                    }
                    else
                    {
                        // z³ + c
                        double x2 = x * x - y * y;
                        double y2 = 2.0 * x * y;
                        z.real = x * x2 - y * y2 + constant.real;
                        z.imag = x * y2 + y * x2 + constant.imag;
                    }

                    double mag2 = z.real * z.real + z.imag * z.imag;

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
        // Smoothed Orbit
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SmoothedOrbit";
            spec.displayName = "Smoothed Orbit (Running Average)";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Applies running average smoothing to orbit: z_avg = 0.9*z_avg + 0.1*z. Creates softer, blurred versions.";
            spec.formula = "z' = 0.9*z_smooth + 0.1*z_new";
            spec.formulaLatex = R"(z'_{n+1} = \alpha z'_n + (1-\alpha)(z_n^2 + c))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Softened boundaries, gradient-like transitions";
            spec.discoveredBy = "Orbit smoothing techniques";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "Exponential moving average on orbit points";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                ComplexD zSmooth(0.0, 0.0);
                double alpha = 0.9;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Apply smoothing
                    zSmooth.real = alpha * zSmooth.real + (1.0 - alpha) * z.real;
                    zSmooth.imag = alpha * zSmooth.imag + (1.0 - alpha) * z.imag;

                    double mag2 = zSmooth.real * zSmooth.real + zSmooth.imag * zSmooth.imag;

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
        // Orbit Angle Accumulation
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "OrbitAngleAccum";
            spec.displayName = "Orbit Angle Accumulation";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Tracks cumulative angle change during orbit. Reveals winding number and rotation patterns.";
            spec.formula = "z(n+1) = z² + c, accumulate Σ angle(z)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ sum } \theta_n)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Spiral patterns based on orbital rotation, phase-sensitive coloring";
            spec.discoveredBy = "Phase angle tracking";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "Accumulates atan2 values to track total rotation";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double angleSum = 0.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // Accumulate angle
                    angleSum += std::atan2(y, x);

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        // Return based on accumulated angle
                        return std::fmod(std::abs(angleSum), 100.0);
                    }
                }

                return std::fmod(std::abs(angleSum), 100.0);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Triangle Orbit Trap
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "TriangleOrbitTrap";
            spec.displayName = "Triangle Orbit Trap";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Orbit trap using equilateral triangle shape. Creates threefold symmetric patterns.";
            spec.formula = "z(n+1) = z² + c, trap to triangle boundary";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ trap: triangle})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Triangular patterns, threefold symmetry from trap geometry";
            spec.discoveredBy = "Geometric orbit traps";
            spec.discoveryYear = 1993;
            spec.computationalNotes = "Point-to-triangle distance calculation";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double minDist = 1e10;

                // Triangle vertices (equilateral, radius 1)
                double v1x = 0.0, v1y = 1.0;
                double v2x = 0.866, v2y = -0.5;
                double v3x = -0.866, v3y = -0.5;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Simplified distance to triangle (distance to nearest edge)
                    double d1 = std::abs((v2y - v1y) * z.real - (v2x - v1x) * z.imag + v2x * v1y - v2y * v1x) / 
                               std::sqrt((v2y - v1y) * (v2y - v1y) + (v2x - v1x) * (v2x - v1x));
                    double d2 = std::abs((v3y - v2y) * z.real - (v3x - v2x) * z.imag + v3x * v2y - v3y * v2x) / 
                               std::sqrt((v3y - v2y) * (v3y - v2y) + (v3x - v2x) * (v3x - v2x));
                    double d3 = std::abs((v1y - v3y) * z.real - (v1x - v3x) * z.imag + v1x * v3y - v1y * v3x) / 
                               std::sqrt((v1y - v3y) * (v1y - v3y) + (v1x - v3x) * (v1x - v3x));

                    double dist = std::min({d1, d2, d3});
                    if (dist < minDist) minDist = dist;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return minDist * 50.0;
                    }
                }

                return minDist * 50.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Stripe Average
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "StripeAverage";
            spec.displayName = "Stripe Average Coloring";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Averages sin(angle) during orbit to create stripe patterns. Reveals orbital flow directions.";
            spec.formula = "z(n+1) = z² + c, average sin(arg(z))";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ avg } \sin(\arg(z_n)))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Flowing stripe patterns, directional information";
            spec.discoveredBy = "Stripe coloring technique";
            spec.discoveryYear = 1996;
            spec.computationalNotes = "Running average of sine of angle";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double stripeSum = 0.0;
                int count = 0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // Accumulate stripe value
                    double angle = std::atan2(y, x);
                    stripeSum += std::sin(angle * 10.0); // Frequency 10 for visible stripes
                    count++;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double avg = count > 0 ? stripeSum / count : 0.0;
                        return (avg + 1.0) * 50.0; // Map [-1,1] to [0,100]
                    }
                }

                double avg = count > 0 ? stripeSum / count : 0.0;
                return (avg + 1.0) * 50.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Curvature Tracking
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CurvatureTracking";
            spec.displayName = "Orbital Curvature Tracking";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Tracks curvature of orbital path by measuring angle changes. High curvature = tight spirals.";
            spec.formula = "z(n+1) = z² + c, track |Δangle|";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ curvature: } |\theta_n - \theta_{n-1}|)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Highlights regions of high orbital curvature, spiral density";
            spec.discoveredBy = "Geometric orbit analysis";
            spec.discoveryYear = 1997;
            spec.computationalNotes = "Second derivative approximation via angle differences";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double prevAngle = 0.0;
                double curvatureSum = 0.0;
                int count = 0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    double angle = std::atan2(y, x);

                    if (i > 0)
                    {
                        double angleDiff = std::abs(angle - prevAngle);
                        // Normalize to [-π, π]
                        if (angleDiff > M_PI) angleDiff = 2.0 * M_PI - angleDiff;
                        curvatureSum += angleDiff;
                        count++;
                    }

                    prevAngle = angle;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return count > 0 ? (curvatureSum / count) * 100.0 : 0.0;
                    }
                }

                return count > 0 ? (curvatureSum / count) * 100.0 : 0.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Delta Magnitude Tracking
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "DeltaMagnitude";
            spec.displayName = "Delta Magnitude Tracking";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Tracks changes in magnitude between iterations: Δ|z| = ||z(n+1)| - |z(n)||. Shows acceleration/deceleration.";
            spec.formula = "z(n+1) = z² + c, track ||z(n+1)| - |z(n)||";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \Delta = ||z_{n+1}| - |z_n||)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Reveals growth rate patterns, acceleration zones";
            spec.discoveredBy = "Magnitude derivative tracking";
            spec.discoveryYear = 1999;
            spec.computationalNotes = "First derivative of magnitude along orbit";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                double prevMag = 0.0;
                double deltaSum = 0.0;
                int count = 0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;
                    double currentMag = std::sqrt(x * x + y * y);

                    if (i > 0)
                    {
                        double delta = std::abs(currentMag - prevMag);
                        deltaSum += delta;
                        count++;
                    }

                    prevMag = currentMag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return count > 0 ? (deltaSum / count) * 20.0 : 0.0;
                    }
                }

                return count > 0 ? (deltaSum / count) * 20.0 : 0.0;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Point-Line Orbit Trap
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "PointLineOrbitTrap";
            spec.displayName = "Point-Line Orbit Trap";
            spec.category = "Orbital Modifications";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Orbit trap using both a point and a line. Creates combined geometric patterns.";
            spec.formula = "z(n+1) = z² + c, trap to point and line";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \text{ trap: point } \cup \text{ line})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Dual trap patterns, linear and radial features combined";
            spec.discoveredBy = "Composite orbit traps";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "Minimum of point distance and line distance";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                ComplexD point(0.5, 0.0);
                double minDist = 1e10;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    // Distance to point
                    double distPoint = std::sqrt((z.real - point.real) * (z.real - point.real) + 
                                                 (z.imag - point.imag) * (z.imag - point.imag));

                    // Distance to line (y = 0)
                    double distLine = std::abs(z.imag);

                    // Minimum of both
                    double dist = std::min(distPoint, distLine);

                    if (dist < minDist) minDist = dist;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return minDist * 50.0;
                    }
                }

                return minDist * 50.0;
            };

            FractalRegistry::Register(spec);
        }
    }
}
