#include "InitialConditionsService.h"
#include <fstream>
#include <sstream>
#include <algorithm>
#include <filesystem>
#include <windows.h>

namespace Native {

bool InitialConditionsService::s_initialized = false;

//=============================================================================
// Private Helper - Get Singleton Registry
//=============================================================================

std::map<std::string, InitialConditions>& InitialConditionsService::GetRegistry()
{
    static std::map<std::string, InitialConditions> registry;
    return registry;
}

//=============================================================================
// Get Resource File Path (Read-Only Application Data)
//=============================================================================

std::string InitialConditionsService::GetResourceFilePath()
{
    // Check relative to executable (deployed scenario)
    wchar_t exePath[MAX_PATH];
    GetModuleFileNameW(NULL, exePath, MAX_PATH);
    std::filesystem::path exeDir = std::filesystem::path(exePath).parent_path();
    std::filesystem::path deployedResourcePath = exeDir / "Resources" / "InitialConditions.txt";
    if (std::filesystem::exists(deployedResourcePath))
    {
        return deployedResourcePath.string();
    }

    // Check in the native project's Resources folder (development scenario)
    std::filesystem::path devResourcePath = "ManpCore.Native\\Resources\\InitialConditions.txt";
    if (std::filesystem::exists(devResourcePath))
    {
        return devResourcePath.string();
    }

    return "";
}

//=============================================================================
// Initialize - Load from Resource File (Read-Only)
//=============================================================================

void InitialConditionsService::Initialize()
{
    if (s_initialized)
        return;

    auto& registry = GetRegistry();
    registry.clear();

    // Load from read-only resource file
    std::string resourcePath = GetResourceFilePath();
    if (resourcePath.empty())
    {
        // Resource file not found - registry will remain empty
        s_initialized = true;
        return;
    }

    std::ifstream file(resourcePath);
    if (!file.is_open())
    {
        // Couldn't open file - registry will remain empty
        s_initialized = true;
        return;
    }

    std::string line;
    while (std::getline(file, line))
    {
        // Skip empty lines and comments
        if (line.empty() || line[0] == '#')
            continue;

        // Format: FractalName|CenterX|CenterY|Zoom
        std::stringstream ss(line);
        std::string name, centerXStr, centerYStr, zoomStr;

        if (std::getline(ss, name, '|') &&
            std::getline(ss, centerXStr, '|') &&
            std::getline(ss, centerYStr, '|') &&
            std::getline(ss, zoomStr))
        {
            try
            {
                InitialConditions conditions;
                conditions.centerX = std::stod(centerXStr);
                conditions.centerY = std::stod(centerYStr);
                conditions.zoom = std::stod(zoomStr);
                registry[name] = conditions;
            }
            catch (...)
            {
                // Skip malformed lines
                continue;
            }
        }
    }

    file.close();
    s_initialized = true;
}

//=============================================================================
// Get - Retrieve Initial Conditions (Read-Only)
//=============================================================================

InitialConditions InitialConditionsService::Get(const std::string& fractalName)
{
    if (!s_initialized)
        Initialize();

    auto& registry = GetRegistry();
    auto it = registry.find(fractalName);

    if (it != registry.end())
        return it->second;

    // Return default if not found
    return InitialConditions();
}

//=============================================================================
// Has - Check if Fractal Has Stored Conditions
//=============================================================================

bool InitialConditionsService::Has(const std::string& fractalName)
{
    if (!s_initialized)
        Initialize();

    auto& registry = GetRegistry();
    return registry.find(fractalName) != registry.end();
}

//=============================================================================
// GetCount - Number of Stored Fractals
//=============================================================================

size_t InitialConditionsService::GetCount()
{
    if (!s_initialized)
        Initialize();

    return GetRegistry().size();
}

//=============================================================================
// GetFractalNames - List All Stored Fractal Names
//=============================================================================

std::vector<std::string> InitialConditionsService::GetFractalNames()
{
    if (!s_initialized)
        Initialize();

    std::vector<std::string> names;
    auto& registry = GetRegistry();

    for (const auto& entry : registry)
    {
        names.push_back(entry.first);
    }

    return names;
}

//=============================================================================
// Reload - Refresh from File
//=============================================================================

void InitialConditionsService::Reload()
{
    s_initialized = false;
    Initialize();
}

} // namespace Native
