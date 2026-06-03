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
// Manages persistent storage and retrieval of fractal initial view conditions
//=============================================================================

class InitialConditionsService
{
public:
    // Get initial conditions for a fractal by name
    // Returns default values (0.0, 0.0, 1.0) if fractal not found
    static InitialConditions Get(const std::string& fractalName);

    // Set/update initial conditions for a fractal
    // Creates new entry if fractal doesn't exist, updates if it does
    // Persists changes to the data file
    static void Set(const std::string& fractalName, double centerX, double centerY, double zoom);

    // Set/update initial conditions using struct
    static void Set(const std::string& fractalName, const InitialConditions& conditions);

    // Check if initial conditions exist for a fractal
    static bool Has(const std::string& fractalName);

    // Get count of stored fractal initial conditions
    static size_t GetCount();

    // Get all fractal names that have initial conditions stored
    static std::vector<std::string> GetFractalNames();

    // Reload data from file (useful for external edits)
    static void Reload();

    // Get the data file path
    static std::string GetDataFilePath();

private:
    // Singleton storage
    static std::map<std::string, InitialConditions>& GetRegistry();
    static bool s_initialized;

    // Initialize from data file
    static void Initialize();

    // Save to data file
    static void Save();

    // Get the writable data directory path
    static std::string GetDataDirectory();
};

} // namespace Native
