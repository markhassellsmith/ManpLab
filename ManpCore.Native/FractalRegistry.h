#pragma once

#include <string>
#include <map>
#include <vector>
#include <functional>
#include <memory>

namespace Native {

// Forward declarations
struct ComplexD;

//=============================================================================
// Parameter Types
//=============================================================================

enum class ParameterType
{
    Integer,
    Float,
    Boolean,
    Choice  // Dropdown/enum
};

struct ParameterSpec
{
    std::string name;
    std::string displayName;
    ParameterType type;
    double defaultValue;
    double minValue;
    double maxValue;
    std::vector<std::string> choiceValues;  // For Choice type
    std::string description;

    ParameterSpec() 
        : type(ParameterType::Float), defaultValue(0.0), minValue(0.0), maxValue(1.0) {}

    ParameterSpec(std::string n, ParameterType t, double defVal, double minVal, double maxVal, std::string desc)
        : name(n), displayName(n), type(t), defaultValue(defVal), minValue(minVal), maxValue(maxVal), description(desc) {}

    ParameterSpec(std::string n, std::vector<std::string> choices, std::string desc)
        : name(n), displayName(n), type(ParameterType::Choice), defaultValue(0.0), 
          minValue(0.0), maxValue((double)(choices.size() - 1)), choiceValues(choices), description(desc) {}
};

//=============================================================================
// Fractal Categories
//=============================================================================

enum class FractalCategory
{
    EscapeTime2D,           // Standard 2D escape-time fractals
    Sequence2D,             // Hailstone, bifurcation, etc.
    AttractorBased3D,       // Lorenz, Rössler, etc.
    Special                 // Perturbation, Buddhabrot, etc.
};

//=============================================================================
// Calculator Function Signature
//=============================================================================

// Parameters passed to all calculators
// Use std::map for extensibility - each fractal can read what it needs
typedef std::map<std::string, double> ParamMap;

// Standard 2D escape-time calculator signature
// Returns smooth iteration count (or maxIter if in set)
typedef std::function<double(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params)> 
    FractalCalculator;

//=============================================================================
// Fractal Specification
//=============================================================================

struct FractalSpec
{
    std::string name;               // Unique identifier (e.g., "BurningShip")
    std::string displayName;        // UI-friendly name (e.g., "Burning Ship")
    std::string category;           // Group name (e.g., "Mandelbrot Variants")
    FractalCategory type;           // Category enum

    FractalCalculator calculator;   // Calculation function

    std::vector<ParameterSpec> parameters;  // Custom parameters

    bool supportsJulia;             // Can it render Julia sets?

    // Default view settings
    double defaultCenterX;
    double defaultCenterY;
    double defaultZoom;

    // Rendering hints
    double defaultBailout;          // Escape radius (usually 256.0)
    bool hasSymmetry;               // Can use symmetry optimization?

    std::string description;        // Help text

    // Constructor with defaults
    FractalSpec() 
        : type(FractalCategory::EscapeTime2D),
          supportsJulia(false),
          defaultCenterX(0.0),
          defaultCenterY(0.0),
          defaultZoom(1.0),
          defaultBailout(256.0),
          hasSymmetry(false) {}
};

//=============================================================================
// Fractal Registry
//=============================================================================

class FractalRegistry
{
public:
    // Register a fractal type
    static void Register(const FractalSpec& spec);

    // Get full specification by name
    static const FractalSpec* GetSpec(const std::string& name);

    // Get calculator function only (lightweight)
    static FractalCalculator GetCalculator(const std::string& name);

    // Get all registered fractal names
    static std::vector<std::string> GetRegisteredNames();

    // Get fractals by category
    static std::vector<std::string> GetFractalsByCategory(const std::string& category);

    // Get all category names
    static std::vector<std::string> GetCategories();

    // Get count of registered fractals
    static size_t GetCount();

    // Check if a fractal is registered
    static bool IsRegistered(const std::string& name);

    // Initialize all built-in fractals (called once at startup)
    static void InitializeBuiltins();

private:
    // Singleton storage
    static std::map<std::string, FractalSpec>& GetRegistry();
    static bool s_initialized;
};

} // namespace Native
