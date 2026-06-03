#pragma once

#include <string>
#include <map>
#include <vector>

namespace Native {

//=============================================================================
// Initial Conditions Structure
//=============================================================================

struct InitialConditions
{
    double centerX;
    double centerY;
    double zoom;

    InitialConditions() 
        : centerX(0.0), centerY(0.0), zoom(1.0) {}

    InitialConditions(double cx, double cy, double z) 
        : centerX(cx), centerY(cy), zoom(z) {}
};

//=============================================================================
// InitialConditions Service
// READ-ONLY service for fractal default viewport positions
// Loads from Resources\InitialConditions.txt (shipped with application)
// This is immutable application data, NOT user preferences
//=============================================================================

class InitialConditionsService
{
public:
    // Get initial conditions for a fractal by name
    // Returns default values (0.0, 0.0, 1.0) if fractal not found
    static InitialConditions Get(const std::string& fractalName);

    // Check if initial conditions exist for a fractal
    static bool Has(const std::string& fractalName);

    // Get count of stored fractal initial conditions
    static size_t GetCount();

    // Get all fractal names that have initial conditions stored
    static std::vector<std::string> GetFractalNames();

    // Reload data from file (useful for external edits during development)
    static void Reload();

    // Get the resource file path (for debugging/verification)
    static std::string GetResourceFilePath();

private:
    // Singleton storage
    static std::map<std::string, InitialConditions>& GetRegistry();
    static bool s_initialized;

    // Initialize from resource file (read-only)
    static void Initialize();
};

} // namespace Native
