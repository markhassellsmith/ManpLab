using System;
using System.Diagnostics;
using System.Linq;
using ManpCore.Native;

namespace ManpCore.Tests
{
    /// <summary>
    /// Performance benchmark to measure C++/CLI wrapper overhead
    /// Target: <5% overhead compared to pure C++ implementation
    /// </summary>
    class PerformanceBenchmark
    {
        private const int WARMUP_RUNS = 2;
        private const int BENCHMARK_RUNS = 5;

        public static void Run()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         ManpCore.Native Performance Benchmark                    ║");
            Console.WriteLine("║         C++/CLI Wrapper Overhead Analysis                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝\n");

            Console.WriteLine($"Configuration:");
            Console.WriteLine($"  - Warmup runs: {WARMUP_RUNS}");
            Console.WriteLine($"  - Benchmark runs: {BENCHMARK_RUNS}");
            Console.WriteLine($"  - Target overhead: <5%\n");

            // Test configurations with varying complexity
            var configurations = new[]
            {
                new BenchmarkConfig
                {
                    Name = "Small Image (400x300)",
                    Width = 400,
                    Height = 300,
                    MaxIterations = 256
                },
                new BenchmarkConfig
                {
                    Name = "Standard Image (800x600)",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 256
                },
                new BenchmarkConfig
                {
                    Name = "High Iteration (800x600)",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 1024
                },
                new BenchmarkConfig
                {
                    Name = "Large Image (1920x1080)",
                    Width = 1920,
                    Height = 1080,
                    MaxIterations = 256
                },
                new BenchmarkConfig
                {
                    Name = "Deep Zoom (800x600)",
                    Width = 800,
                    Height = 600,
                    MaxIterations = 512,
                    CenterX = -0.7463,
                    CenterY = 0.1102,
                    ViewWidth = 0.005,
                    ViewHeight = 0.00375
                }
            };

            var results = new BenchmarkResult[configurations.Length];

            for (int i = 0; i < configurations.Length; i++)
            {
                results[i] = RunBenchmark(configurations[i]);
                Console.WriteLine(); // Spacing between tests
            }

            // Summary report
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     BENCHMARK SUMMARY                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝\n");

            Console.WriteLine($"{"Configuration",-30} {"Pixels",-12} {"Avg Time",-12} {"Throughput",-15}");
            Console.WriteLine(new string('─', 75));

            foreach (var result in results)
            {
                int totalPixels = result.Config.Width * result.Config.Height;
                double pixelsPerMs = totalPixels / result.AverageTime;

                Console.WriteLine($"{result.Config.Name,-30} {totalPixels,-12:N0} {result.AverageTime,-12:F2} ms {pixelsPerMs,-15:F0} px/ms");
            }

            // Calculate overall statistics
            double totalAvgTime = results.Average(r => r.AverageTime);
            double totalStdDev = CalculateStdDev(results.Select(r => r.AverageTime).ToArray());

            Console.WriteLine($"\nOverall Statistics:");
            Console.WriteLine($"  - Average render time: {totalAvgTime:F2} ms");
            Console.WriteLine($"  - Standard deviation: {totalStdDev:F2} ms");
            Console.WriteLine($"  - Coefficient of variation: {(totalStdDev / totalAvgTime * 100):F2}%");

            // Performance validation
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   PERFORMANCE VALIDATION                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝\n");

            // Estimate pure C++ baseline (theoretical - we'll measure this separately)
            // For now, assume wrapper overhead should be minimal
            Console.WriteLine("Overhead Analysis:");
            Console.WriteLine("  Note: C++/CLI wrapper overhead includes:");
            Console.WriteLine("    - Managed/native boundary crossing");
            Console.WriteLine("    - Array marshalling for pixel data");
            Console.WriteLine("    - Event marshalling for progress updates");
            Console.WriteLine("    - Parameter struct copying");
            Console.WriteLine();

            // Memory allocation analysis
            var largestTest = results.OrderByDescending(r => r.Config.Width * r.Config.Height).First();
            long pixelDataSize = largestTest.Config.Width * largestTest.Config.Height * 4; // RGBA
            double memoryMB = pixelDataSize / (1024.0 * 1024.0);

            Console.WriteLine($"Memory Footprint:");
            Console.WriteLine($"  - Largest test: {largestTest.Config.Width}x{largestTest.Config.Height}");
            Console.WriteLine($"  - Pixel data: {memoryMB:F2} MB");
            Console.WriteLine($"  - Estimated total: ~{memoryMB * 2:F2} MB (double buffering)");
            Console.WriteLine();

            // Consistency check
            bool allConsistent = results.All(r => r.CoefficientOfVariation < 10.0);
            Console.WriteLine($"Consistency Check:");
            Console.WriteLine($"  - All runs within 10% variation: {(allConsistent ? "✓ PASS" : "✗ FAIL")}");

            if (!allConsistent)
            {
                Console.WriteLine($"  ⚠ Warning: High variation detected in some tests");
                foreach (var result in results.Where(r => r.CoefficientOfVariation >= 10.0))
                {
                    Console.WriteLine($"    - {result.Config.Name}: {result.CoefficientOfVariation:F2}% CV");
                }
            }

            Console.WriteLine();

            // Overhead comparison test
            RunOverheadComparison();
        }

        private static void RunOverheadComparison()
        {
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               C++/CLI WRAPPER OVERHEAD ANALYSIS                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝\n");

            Console.WriteLine("Comparing pure C++ baseline vs C++/CLI wrapper performance...\n");

            var testConfigs = new[]
            {
                new { Width = 800, Height = 600, MaxIterations = 256, Name = "Standard (800x600, 256 iter)" },
                new { Width = 800, Height = 600, MaxIterations = 512, Name = "High Iterations (800x600, 512 iter)" },
                new { Width = 1920, Height = 1080, MaxIterations = 256, Name = "Large Image (1920x1080, 256 iter)" }
            };

            Console.WriteLine($"{"Test",-40} {"Native C++",-15} {"C++/CLI",-15} {"Overhead",-12}");
            Console.WriteLine(new string('─', 85));

            using var engine = new FractalEngineWrapper();

            foreach (var config in testConfigs)
            {
                // Run native baseline
                double nativeTime = engine.RunNativeBaselineBenchmark(
                    config.Width, config.Height, config.MaxIterations, BENCHMARK_RUNS);

                // Run C++/CLI wrapper
                var parameters = new FractalParameters
                {
                    Width = config.Width,
                    Height = config.Height,
                    MaxIterations = config.MaxIterations,
                    CenterX = -0.5,
                    CenterY = 0.0,
                    ViewWidth = 3.0,
                    ViewHeight = 2.25,
                    IsJuliaSet = false,
                    Palette = ColorPalette.Classic
                };

                // Warmup
                for (int i = 0; i < WARMUP_RUNS; i++)
                {
                    engine.Calculate(parameters);
                }

                // Benchmark
                var times = new double[BENCHMARK_RUNS];
                var stopwatch = new Stopwatch();
                for (int i = 0; i < BENCHMARK_RUNS; i++)
                {
                    stopwatch.Restart();
                    engine.Calculate(parameters);
                    stopwatch.Stop();
                    times[i] = stopwatch.Elapsed.TotalMilliseconds;
                }

                double wrapperTime = times.Average();
                double overhead = ((wrapperTime - nativeTime) / nativeTime) * 100.0;

                string overheadStr = $"{overhead:F2}%";
                if (overhead < 5.0)
                    overheadStr += " ✓";
                else
                    overheadStr += " ⚠";

                Console.WriteLine($"{config.Name,-40} {nativeTime,-15:F2} {wrapperTime,-15:F2} {overheadStr,-12}");
            }

            Console.WriteLine();
            Console.WriteLine("Target: < 5% overhead");
            Console.WriteLine("✓ = Within target, ⚠ = Exceeds target\n");
        }

        private static BenchmarkResult RunBenchmark(BenchmarkConfig config)
        {
            Console.WriteLine($"┌─ {config.Name} " + new string('─', 60 - config.Name.Length));
            Console.WriteLine($"│  Dimensions: {config.Width}x{config.Height}, Max Iterations: {config.MaxIterations}");

            using var engine = new FractalEngineWrapper();

            var parameters = new FractalParameters
            {
                FractalType = "Mandelbrot",
                Width = config.Width,
                Height = config.Height,
                MaxIterations = config.MaxIterations,
                CenterX = config.CenterX,
                CenterY = config.CenterY,
                ViewWidth = config.ViewWidth,
                ViewHeight = config.ViewHeight,
                IsJuliaSet = false,
                Palette = ColorPalette.Classic
            };

            // Warmup runs (JIT compilation, cache warming)
            Console.Write($"│  Warmup: ");
            for (int i = 0; i < WARMUP_RUNS; i++)
            {
                engine.Calculate(parameters);
                Console.Write(".");
            }
            Console.WriteLine(" done");

            // Benchmark runs
            Console.Write($"│  Benchmark: ");
            var times = new double[BENCHMARK_RUNS];
            var stopwatch = new Stopwatch();

            for (int i = 0; i < BENCHMARK_RUNS; i++)
            {
                stopwatch.Restart();
                var result = engine.Calculate(parameters);
                stopwatch.Stop();

                times[i] = stopwatch.Elapsed.TotalMilliseconds;
                Console.Write(".");
            }
            Console.WriteLine(" done");

            // Calculate statistics
            double avgTime = times.Average();
            double minTime = times.Min();
            double maxTime = times.Max();
            double stdDev = CalculateStdDev(times);
            double coefficientOfVariation = (stdDev / avgTime) * 100.0;

            int totalPixels = config.Width * config.Height;
            long totalIterations = (long)totalPixels * config.MaxIterations;
            double iterationsPerMs = totalIterations / avgTime;

            Console.WriteLine($"│");
            Console.WriteLine($"│  Results:");
            Console.WriteLine($"│    Average time:    {avgTime:F2} ms");
            Console.WriteLine($"│    Min time:        {minTime:F2} ms");
            Console.WriteLine($"│    Max time:        {maxTime:F2} ms");
            Console.WriteLine($"│    Std deviation:   {stdDev:F2} ms");
            Console.WriteLine($"│    Variance:        {coefficientOfVariation:F2}%");
            Console.WriteLine($"│    Throughput:      {iterationsPerMs:F0} iterations/ms");
            Console.WriteLine($"│    Throughput:      {(totalPixels / avgTime):F0} pixels/ms");
            Console.WriteLine($"└" + new string('─', 70));

            return new BenchmarkResult
            {
                Config = config,
                Times = times,
                AverageTime = avgTime,
                MinTime = minTime,
                MaxTime = maxTime,
                StdDev = stdDev,
                CoefficientOfVariation = coefficientOfVariation
            };
        }

        private static double CalculateStdDev(double[] values)
        {
            double avg = values.Average();
            double sumOfSquares = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumOfSquares / values.Length);
        }

        private class BenchmarkConfig
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int MaxIterations { get; set; }
            public double CenterX { get; set; } = -0.5;
            public double CenterY { get; set; } = 0.0;
            public double ViewWidth { get; set; } = 3.0;
            public double ViewHeight { get; set; } = 2.25;
        }

        private class BenchmarkResult
        {
            public BenchmarkConfig Config { get; set; }
            public double[] Times { get; set; }
            public double AverageTime { get; set; }
            public double MinTime { get; set; }
            public double MaxTime { get; set; }
            public double StdDev { get; set; }
            public double CoefficientOfVariation { get; set; }
        }
    }
}
