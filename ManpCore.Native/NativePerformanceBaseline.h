#pragma once

#include <vector>
#include <chrono>
#include <cmath>
#include "../ManpCore.Native/MandelbrotCalculator.h"

namespace Native {

    /// <summary>
    /// Pure C++ performance baseline (no C++/CLI overhead)
    /// Used to measure wrapper overhead
    /// </summary>
    class NativePerformanceBaseline
    {
    public:
        struct BenchmarkResult
        {
            double averageTimeMs;
            double minTimeMs;
            double maxTimeMs;
            double stdDevMs;
            int totalPixels;
            long long totalIterations;
        };

        /// <summary>
        /// Run pure C++ Mandelbrot calculation benchmark
        /// </summary>
        static BenchmarkResult RunMandelbrotBenchmark(
            int width,
            int height,
            int maxIterations,
            int runs,
            double centerX = -0.5,
            double centerY = 0.0,
            double viewWidth = 3.0,
            double viewHeight = 2.25)
        {
            std::vector<double> times;
            times.reserve(runs);

            MandelbrotParams params;
            params.width = width;
            params.height = height;
            params.maxIterations = maxIterations;
            params.centerX = centerX;
            params.centerY = centerY;
            params.viewWidth = viewWidth;
            params.viewHeight = viewHeight;
            params.isJulia = false;

            // Pre-allocate pixel buffer (reused across runs)
            std::vector<unsigned char> pixelData(width * height * 4);
            long long totalIterations = 0;

            // Benchmark runs
            for (int run = 0; run < runs; run++)
            {
                auto start = std::chrono::high_resolution_clock::now();

                totalIterations = 0;

                // Render fractal
                for (int py = 0; py < height; py++)
                {
                    for (int px = 0; px < width; px++)
                    {
                        // Map pixel to complex plane
                        ComplexD c = MandelbrotCalculator::PixelToComplex(px, py, params);

                        // Calculate smooth iteration count
                        double iteration = MandelbrotCalculator::CalculateSmoothIterations(
                            c, maxIterations, false, ComplexD());

                        totalIterations++;

                        // Get color from palette (using Classic palette)
                        ColorRGB color = ColorPalette::GetColor(iteration, maxIterations, PaletteType::Classic);

                        // Write to pixel buffer
                        int offset = (py * width + px) * 4;
                        pixelData[offset] = color.r;
                        pixelData[offset + 1] = color.g;
                        pixelData[offset + 2] = color.b;
                        pixelData[offset + 3] = 255; // Alpha
                    }
                }

                auto end = std::chrono::high_resolution_clock::now();
                std::chrono::duration<double, std::milli> elapsed = end - start;
                times.push_back(elapsed.count());
            }

            // Calculate statistics
            double sum = 0.0;
            double minTime = times[0];
            double maxTime = times[0];

            for (double time : times)
            {
                sum += time;
                if (time < minTime) minTime = time;
                if (time > maxTime) maxTime = time;
            }

            double avgTime = sum / runs;

            // Calculate standard deviation
            double sumSquares = 0.0;
            for (double time : times)
            {
                double diff = time - avgTime;
                sumSquares += diff * diff;
            }
            double stdDev = std::sqrt(sumSquares / runs);

            BenchmarkResult result;
            result.averageTimeMs = avgTime;
            result.minTimeMs = minTime;
            result.maxTimeMs = maxTime;
            result.stdDevMs = stdDev;
            result.totalPixels = width * height;
            result.totalIterations = totalIterations;

            return result;
        }
    };

} // namespace Native
