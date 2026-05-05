#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <set>
#include <stdexcept>

namespace Native {

// Static initialization
bool FractalRegistry::s_initialized = false;

// Singleton registry storage
std::map<std::string, FractalSpec>& FractalRegistry::GetRegistry()
{
    static std::map<std::string, FractalSpec> registry;
    return registry;
}

//=============================================================================
// Registration
//=============================================================================

void FractalRegistry::Register(const FractalSpec& spec)
{
    if (spec.name.empty())
        throw std::invalid_argument("Fractal name cannot be empty");

    if (!spec.calculator)
        throw std::invalid_argument("Fractal calculator function is required");

    auto& registry = GetRegistry();
    registry[spec.name] = spec;
}

//=============================================================================
// Lookup
//=============================================================================

const FractalSpec* FractalRegistry::GetSpec(const std::string& name)
{
    auto& registry = GetRegistry();
    auto it = registry.find(name);

    if (it != registry.end())
        return &it->second;

    return nullptr;
}

FractalCalculator FractalRegistry::GetCalculator(const std::string& name)
{
    const FractalSpec* spec = GetSpec(name);

    if (spec)
        return spec->calculator;

    return nullptr;
}

double FractalRegistry::Calculate(
    const std::string& fractalName,
    ComplexD c,
    int maxIter,
    bool isJulia,
    ComplexD juliaC,
    const ParamMap& params)
{
    const FractalSpec* spec = GetSpec(fractalName);

    if (spec && spec->calculator)
    {
        // Call the calculator entirely in native code - no boundary crossing
        return spec->calculator(c, maxIter, isJulia, juliaC, params);
    }

    // Fallback: return max iterations (point is in the set)
    return static_cast<double>(maxIter);
}

bool FractalRegistry::IsRegistered(const std::string& name)
{
    return GetSpec(name) != nullptr;
}

//=============================================================================
// Enumeration
//=============================================================================

std::vector<std::string> FractalRegistry::GetRegisteredNames()
{
    std::vector<std::string> names;
    auto& registry = GetRegistry();

    for (const auto& pair : registry)
    {
        names.push_back(pair.first);
    }

    return names;
}

std::vector<std::string> FractalRegistry::GetFractalsByCategory(const std::string& category)
{
    std::vector<std::string> names;
    auto& registry = GetRegistry();

    for (const auto& pair : registry)
    {
        if (pair.second.category == category)
        {
            names.push_back(pair.first);
        }
    }

    return names;
}

std::vector<std::string> FractalRegistry::GetCategories()
{
    std::vector<std::string> categories;
    auto& registry = GetRegistry();

    // Collect unique categories
    std::set<std::string> uniqueCategories;
    for (const auto& pair : registry)
    {
        uniqueCategories.insert(pair.second.category);
    }

    categories.assign(uniqueCategories.begin(), uniqueCategories.end());
    return categories;
}

size_t FractalRegistry::GetCount()
{
    return GetRegistry().size();
}

//=============================================================================
// Built-in Initialization
// Forward declarations from family files
//=============================================================================

// These will be implemented in separate family files
extern void RegisterMandelbrotFamily();
extern void RegisterBurningShipFamily();
extern void RegisterTricornFamily();
extern void RegisterPhoenixFamily();
extern void RegisterMultibrotFamily();
extern void RegisterNewtonFamily();
extern void RegisterMagnetFamily();
extern void RegisterClassicEscapeTimeFamily();  // Lambda, Manowar, Sierpinski, etc.
extern void RegisterBarnsleyFamily();           // Barnsley M1/J1/M2/J2/M3/J3
extern void RegisterSpecialExoticFamily();      // Hailstone, Buddhabrot, Lyapunov, etc.
extern void RegisterAttractors3DFamily();       // Lorenz, Rossler, Henon, Chua, etc.
extern void RegisterTrigonometricFamily();      // Sine, Cosine, Sinh, Cosh variants
extern void RegisterExponentialFamily();        // Exponential, Logarithm, Power variants
extern void RegisterExtendedJuliaFamily();      // Additional Julia set variations
extern void RegisterPowerVariantsFamily();      // Higher-power Mandelbrot/Julia/BurningShip/Tricorn
extern void RegisterPhoenixExtendedFamily();    // Phoenix variations with different powers and functions
extern void RegisterNewtonExtendedFamily();     // Newton method for various polynomials and functions
extern void RegisterMagnetExtendedFamily();     // Magnet Julia modes and power variants
extern void RegisterLambdaExtendedFamily();     // Lambda power variants and function combinations
extern void RegisterHybridFamily();             // Hybrid fractals combining multiple formulas
extern void RegisterMandelVariantsFamily();     // Mandelbrot variants: Mandel4, MandelLambda, Thorn, Spider, etc.
extern void RegisterComplexFunctionsFamily();   // Complex function combinations: SqrTrig, TrigSqr, TrigPlusTrig, etc.
extern void RegisterBifurcationFamily();        // Bifurcation diagrams and parameter space
extern void RegisterIFSFamily();                // Iterated Function Systems: Barnsley fern, Sierpinski, Dragon curve, etc.
extern void RegisterDistanceEstimatorFamily();  // Distance estimator variants for smooth boundaries
extern void RegisterExoticFormulasFamily();     // Exotic formulas: Celtic, Buffalo, Heart, Zubieta, etc.
extern void RegisterOrbitalFractalsFamily();    // Orbit trap and modification techniques
extern void RegisterPolynomialVariantsFamily(); // Polynomial variants: Cubic, Quartic, Rational, etc.
extern void RegisterTrigonometricExtendedFamily(); // Extended trig functions: tan, cot, sec, csc, arcsin, arccos, arctan, tanh
extern void RegisterJuliaVariantsFamily();      // Julia set variations with different formulas
extern void RegisterStrangeAttractorsExtendedFamily(); // Extended strange attractors: Clifford, De Jong, Tinkerbell, etc.
extern void RegisterPolynomialFamily();         // Multibrot 3-10, Tricorn, Buffalo = 8
extern void RegisterExponentialLogarithmicFamily(); // Exponential, Logarithmic, ExpSquare, PowerTower, ComplexPower, ExponentialJulia = 6
extern void RegisterRationalFunctionFamily();   // Newton z³-1, z⁴-1, z⁵-1, Halley, Möbius, Rational maps = 8
extern void RegisterHistoricalFractalsFamily(); // Biomorphs, Pickover Stalks, Martin, Chip, Quaternion2D, Collatz, Duffing, Sinusoidal = 8
extern void RegisterSpecialFunctionFamily();    // Gamma, Error, Bessel-like, Continued Fraction, Tetration, Lambert W, Hyperbolic = 7
extern void RegisterChaoticMapsFamily();        // Clifford, De Jong, Tinkerbell, Bedhead, Svensson, SymmetricIcon, Gingerbreadman, Sprott = 8
extern void RegisterFractalHybridsFamily();     // Burning-Mandel, Exp-Mandel, Mutant, Trig-Blend, Sierpinski-Mandel, Perturbed Newton, Bifurcation-Mandel, Celtic = 8

void FractalRegistry::InitializeBuiltins()
{
    if (s_initialized)
        return;

    // Register all built-in fractal families
    RegisterMandelbrotFamily();         // Mandelbrot + 3 Julia presets = 4
    RegisterBurningShipFamily();        // Burning Ship = 1
    RegisterTricornFamily();            // Tricorn = 1
    RegisterPhoenixFamily();            // Phoenix = 1
    RegisterMultibrotFamily();          // Multibrot 3,4,5 = 3
    RegisterNewtonFamily();             // Newton, Nova = 2
    RegisterMagnetFamily();             // Magnet I, II = 2
    RegisterClassicEscapeTimeFamily();  // Lambda, Manowar, Sierpinski, Unity, Spider, Tetrate, etc. = 8
    RegisterBarnsleyFamily();           // Barnsley M1/J1/M2/J2/M3/J3 = 6
    RegisterSpecialExoticFamily();      // Hailstone, NumFractal, Buddhabrot, Lyapunov, Popcorn, Mandelbar, Thorn, Tetration = 8
    RegisterAttractors3DFamily();       // Lorenz, Rossler, Henon, Pickover, Gingerbread, Chua, Ikeda, Hopalong = 8
    RegisterTrigonometricFamily();      // Sine, Cosine, Sinh, Cosh variants = 12
    RegisterExponentialFamily();        // Exponential, Logarithm, Power variants = 6
    RegisterExtendedJuliaFamily();      // Additional Julia set variations = 8
    RegisterPowerVariantsFamily();      // Higher-power Mandelbrot/Julia/BurningShip/Tricorn = 9
    RegisterPhoenixExtendedFamily();    // Phoenix variations with different powers and functions = 8
    RegisterNewtonExtendedFamily();     // Newton method for various polynomials and functions = 6
    RegisterMagnetExtendedFamily();     // Magnet Julia modes and power variants = 4
    RegisterLambdaExtendedFamily();     // Lambda power variants and function combinations = 8
    RegisterHybridFamily();             // Hybrid fractals combining multiple formulas = 10
    RegisterMandelVariantsFamily();     // Mandelbrot variants: Mandel4, MandelLambda, Thorn, Spider, etc. = 8
    RegisterComplexFunctionsFamily();   // Complex function combinations: SqrTrig, TrigSqr, etc. = 8
    RegisterBifurcationFamily();        // Bifurcation diagrams and parameter space = 6
    RegisterIFSFamily();                // Iterated Function Systems = 5
    RegisterDistanceEstimatorFamily();  // Distance estimator variants = 4
    RegisterExoticFormulasFamily();     // Exotic formulas: Celtic, Buffalo, Heart, etc. = 8
    RegisterOrbitalFractalsFamily();    // Orbit trap and modification = 8
    RegisterPolynomialVariantsFamily(); // Polynomial variants: Cubic, Quartic, Rational, etc. = 8
    RegisterTrigonometricExtendedFamily(); // Extended trig functions = 8
    RegisterJuliaVariantsFamily();      // Julia set variations = 8
    RegisterStrangeAttractorsExtendedFamily(); // Extended strange attractors = 6
    RegisterPolynomialFamily();         // Multibrot 3-10, Tricorn, Buffalo = 8
    RegisterExponentialLogarithmicFamily(); // Exponential/logarithmic variants = 6
    RegisterRationalFunctionFamily();   // Newton, Halley, Rational maps = 8
    RegisterHistoricalFractalsFamily(); // Historical fractals = 8
    RegisterSpecialFunctionFamily();    // Special functions = 7
    RegisterChaoticMapsFamily();        // Chaotic maps = 8
    RegisterFractalHybridsFamily();     // Fractal hybrids = 8
    // Total: 245 fractals

    s_initialized = true;
}

} // namespace Native
