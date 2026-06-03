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
// Get Resource File Path (CSV Format)
//=============================================================================

std::string InitialConditionsService::GetResourceFilePath()
{
    // Check relative to executable (deployed scenario)
    wchar_t exePath[MAX_PATH];
    GetModuleFileNameW(NULL, exePath, MAX_PATH);
    std::filesystem::path exeDir = std::filesystem::path(exePath).parent_path();
    std::filesystem::path deployedResourcePath = exeDir / "Resources" / "InitialConditions.csv";
    if (std::filesystem::exists(deployedResourcePath))
    {
        return deployedResourcePath.string();
    }

    // Check in the native project's Resources folder (development scenario)
    std::filesystem::path devResourcePath = "ManpCore.Native\\Resources\\InitialConditions.csv";
    if (std::filesystem::exists(devResourcePath))
    {
        return devResourcePath.string();
    }

    return "";
}

//=============================================================================
// Initialize - Load from CSV Resource File
//=============================================================================

void InitialConditionsService::Initialize()
{
    if (s_initialized)
        return;

    auto& registry = GetRegistry();
    registry.clear();

    // Load from CSV resource file
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
    bool isFirstLine = true;

    while (std::getline(file, line))
    {
        // Skip header row
        if (isFirstLine)
        {
            isFirstLine = false;
            continue;
        }

        // Skip empty lines and comments
        if (line.empty() || line[0] == '#')
            continue;

        // Format: FractalName,CenterX,CenterY,Zoom
        std::stringstream ss(line);
        std::string name, centerXStr, centerYStr, zoomStr;

        if (std::getline(ss, name, ',') &&
            std::getline(ss, centerXStr, ',') &&
            std::getline(ss, centerYStr, ',') &&
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

//=============================================================================
// Set - Update Initial Conditions for a Fractal
//=============================================================================

void InitialConditionsService::Set(const std::string& fractalName, const InitialConditions& conditions)
{
    if (!s_initialized)
        Initialize();

    auto& registry = GetRegistry();
    registry[fractalName] = conditions;
}

//=============================================================================
// Save - Persist Registry to CSV File
//=============================================================================

bool InitialConditionsService::Save()
{
    if (!s_initialized)
        Initialize();

    std::string resourcePath = GetResourceFilePath();
    if (resourcePath.empty())
    {
        return false; // No file path available
    }

    // Open file for writing with UTF-8 BOM
    std::ofstream file(resourcePath, std::ios::binary);
    if (!file.is_open())
    {
        return false;
    }

    // Write UTF-8 BOM
    const unsigned char bom[] = { 0xEF, 0xBB, 0xBF };
    file.write(reinterpret_cast<const char*>(bom), sizeof(bom));

    // Write header
    file << "FractalName,CenterX,CenterY,Zoom\n";

    // Write all entries
    auto& registry = GetRegistry();
    for (const auto& entry : registry)
    {
        file << entry.first << ","
             << entry.second.centerX << ","
             << entry.second.centerY << ","
             << entry.second.zoom << "\n";
    }

    file.close();
    return true;
}

} // namespace Native
