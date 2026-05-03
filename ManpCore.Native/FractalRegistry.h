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

// Parameter type enumeration (mirrors C# ParameterType)
enum class ParameterType
{
    Integer,
    Float,
    Boolean,
    Choice,      // Dropdown/enum
    Complex      // Complex number (real + imaginary pair)
};

// Parameter category enumeration (mirrors C# ParameterCategory)
enum class ParameterCategory
{
    General,        // General purpose
    Calculation,    // Algorithm parameters (iterations, bailout)
    View,           // View parameters (center, zoom)
    Color,          // Color/palette parameters
    Advanced        // Advanced/expert parameters
};

// Parameter specification with full metadata
struct ParameterSpec
{
    std::string name;           // Internal key (e.g., "realz0", "maxIterations")
    std::string displayName;    // UI label (e.g., "Real Perturbation of Z(0)")
    std::string description;    // Help text

    ParameterType type;         // Data type
    ParameterCategory category; // Grouping category

    // Default value (stored as string for flexibility)
    std::string defaultValue;   // "0.0", "256", "true", etc.

    // Range constraints (for numeric types)
    double minValue;
    double maxValue;
    double step;                // Step size for UI controls

    // Choice options (for Choice type)
    std::vector<std::string> choiceValues;

    // Visibility/editability
    bool isAdvanced;            // false = visible by default, true = advanced
    bool isReadOnly;            // false = user can edit, true = readonly

    // Display formatting
    std::string formatString;   // e.g., "F6" for 6 decimal places
    std::string unit;           // e.g., "x" for zoom, "°" for angles
    int displayOrder;           // UI ordering hint

    // Default constructor
    ParameterSpec() 
        : type(ParameterType::Float), 
          category(ParameterCategory::General),
          minValue(0.0), 
          maxValue(1.0), 
          step(0.01),
          isAdvanced(false),
          isReadOnly(false),
          displayOrder(0) {}

    // Simple constructor for numeric parameters
    ParameterSpec(
        std::string n, 
        std::string display,
        std::string desc,
        ParameterType t, 
        ParameterCategory cat,
        std::string defVal, 
        double minVal, 
        double maxVal,
        double stepVal = 0.01)
        : name(n), 
          displayName(display),
          description(desc),
          type(t), 
          category(cat),
          defaultValue(defVal), 
          minValue(minVal), 
          maxValue(maxVal),
          step(stepVal),
          isAdvanced(false),
          isReadOnly(false),
          displayOrder(0) {}

    // Choice parameter constructor
    ParameterSpec(
        std::string n, 
        std::string display,
        std::vector<std::string> choices, 
        std::string desc)
        : name(n), 
          displayName(display), 
          description(desc),
          type(ParameterType::Choice), 
          category(ParameterCategory::General),
          defaultValue("0"),
          minValue(0.0), 
          maxValue((double)(choices.size() - 1)), 
          step(1.0),
          choiceValues(choices),
          isAdvanced(false),
          isReadOnly(false),
          displayOrder(0) {}
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

    // Calculate a single point (pure native, no std::function crossing boundary)
    // Use this from C++/CLI instead of GetCalculator() to avoid marshaling issues
    static double Calculate(
        const std::string& fractalName,
        ComplexD c,
        int maxIter,
        bool isJulia,
        ComplexD juliaC,
        const ParamMap& params);

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
