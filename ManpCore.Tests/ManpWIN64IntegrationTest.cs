using System;
using ManpCore.Native;

namespace ManpCore.Tests
{
    /// <summary>
    /// Proof-of-concept test for ManpWIN64 engine integration.
    /// Validates that C++/CLI wrapper can successfully link to and call ManpWIN64 native code.
    /// </summary>
    public static class ManpWIN64IntegrationTest
    {
        public static void Run()
        {
            Console.WriteLine("=== ManpWIN64 Integration POC Test ===\n");
            Console.WriteLine("This test validates that the C++/CLI wrapper can link to ManpWIN64");
            Console.WriteLine("native code and call its functions. Full fractal type integration");
            Console.WriteLine("will be completed in Phase 3 with UI testing framework.\n");

            try
            {
                using var engine = new FractalEngineWrapper();

                // Test 1: Simple complex number magnitude calculation
                Console.WriteLine("Test 1: Complex number magnitude (3.0, 4.0)");
                double magnitude1 = engine.TestManpWIN64Integration(3.0, 4.0);
                Console.WriteLine($"  Result: {magnitude1:F6}");
                Console.WriteLine($"  Expected: 5.000000 (sqrt(3² + 4²))");
                
                if (Math.Abs(magnitude1 - 5.0) < 0.000001)
                {
                    Console.WriteLine("  ✓ PASSED\n");
                }
                else
                {
                    Console.WriteLine($"  ✗ FAILED - Expected 5.0, got {magnitude1}\n");
                    return;
                }

                // Test 2: Another complex number
                Console.WriteLine("Test 2: Complex number magnitude (1.0, 0.0)");
                double magnitude2 = engine.TestManpWIN64Integration(1.0, 0.0);
                Console.WriteLine($"  Result: {magnitude2:F6}");
                Console.WriteLine($"  Expected: 1.000000");
                
                if (Math.Abs(magnitude2 - 1.0) < 0.000001)
                {
                    Console.WriteLine("  ✓ PASSED\n");
                }
                else
                {
                    Console.WriteLine($"  ✗ FAILED - Expected 1.0, got {magnitude2}\n");
                    return;
                }

                // Test 3: Zero magnitude
                Console.WriteLine("Test 3: Complex number magnitude (0.0, 0.0)");
                double magnitude3 = engine.TestManpWIN64Integration(0.0, 0.0);
                Console.WriteLine($"  Result: {magnitude3:F6}");
                Console.WriteLine($"  Expected: 0.000000");
                
                if (Math.Abs(magnitude3) < 0.000001)
                {
                    Console.WriteLine("  ✓ PASSED\n");
                }
                else
                {
                    Console.WriteLine($"  ✗ FAILED - Expected 0.0, got {magnitude3}\n");
                    return;
                }

                // Test 4: Larger complex number
                Console.WriteLine("Test 4: Complex number magnitude (5.0, 12.0)");
                double magnitude4 = engine.TestManpWIN64Integration(5.0, 12.0);
                Console.WriteLine($"  Result: {magnitude4:F6}");
                Console.WriteLine($"  Expected: 13.000000 (sqrt(5² + 12²))");
                
                if (Math.Abs(magnitude4 - 13.0) < 0.000001)
                {
                    Console.WriteLine("  ✓ PASSED\n");
                }
                else
                {
                    Console.WriteLine($"  ✗ FAILED - Expected 13.0, got {magnitude4}\n");
                    return;
                }

                Console.WriteLine("==============================================");
                Console.WriteLine("✓ ALL TESTS PASSED");
                Console.WriteLine("==============================================\n");
                Console.WriteLine("ManpWIN64 integration POC successful!");
                Console.WriteLine("The C++/CLI wrapper can successfully:");
                Console.WriteLine("  - Link to ManpWIN64 project source files");
                Console.WriteLine("  - Include ManpWIN64 headers (Complex.h)");
                Console.WriteLine("  - Call ManpWIN64 functions (Complex::CFabs)");
                Console.WriteLine("  - Marshal data between managed and native code\n");
                Console.WriteLine("Next steps (Phase 3):");
                Console.WriteLine("  - Integrate full fractal type system (240+ types)");
                Console.WriteLine("  - Connect to UI for interactive testing");
                Console.WriteLine("  - Implement palette import from ManpWIN64");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ TEST FAILED with exception:");
                Console.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"\n{ex.StackTrace}");
            }
        }
    }
}
