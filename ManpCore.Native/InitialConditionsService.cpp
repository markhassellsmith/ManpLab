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
// Private Helper - Get Data Directory
//=============================================================================

std::string InitialConditionsService::GetDataDirectory()
{
    // Try to use the executable's directory first for deployed scenarios
    wchar_t exePath[MAX_PATH];
    GetModuleFileNameW(NULL, exePath, MAX_PATH);
    std::filesystem::path exeDir = std::filesystem::path(exePath).parent_path();
    std::filesystem::path dataDir = exeDir / "FractalData";

    // Ensure directory exists
    if (!std::filesystem::exists(dataDir))
    {
        std::filesystem::create_directories(dataDir);
    }

    return dataDir.string();
}

//=============================================================================
// Get Source Resource Path (for initial data file)
//=============================================================================

std::string GetResourceFilePath()
{
    // Check in the native project's Resources folder (development scenario)
    std::filesystem::path devResourcePath = "ManpCore.Native\\Resources\\InitialConditions.txt";
    if (std::filesystem::exists(devResourcePath))
    {
        return devResourcePath.string();
    }

    // Check relative to executable (deployed scenario)
    wchar_t exePath[MAX_PATH];
    GetModuleFileNameW(NULL, exePath, MAX_PATH);
    std::filesystem::path exeDir = std::filesystem::path(exePath).parent_path();
    std::filesystem::path deployedResourcePath = exeDir / "Resources" / "InitialConditions.txt";
    if (std::filesystem::exists(deployedResourcePath))
    {
        return deployedResourcePath.string();
    }

    return "";
}

//=============================================================================
// Get Data File Path
//=============================================================================

std::string InitialConditionsService::GetDataFilePath()
{
    return GetDataDirectory() + "\\InitialConditions.txt";
}

//=============================================================================
// Initialize - Load from File
//=============================================================================

void InitialConditionsService::Initialize()
{
    if (s_initialized)
        return;

    auto& registry = GetRegistry();
    registry.clear();

    std::string filePath = GetDataFilePath();
    std::ifstream file(filePath);

    // If the writable file doesn't exist, try to copy from Resources
    if (!file.is_open())
    {
        std::string resourcePath = GetResourceFilePath();
        if (!resourcePath.empty())
        {
            try
            {
                std::filesystem::copy_file(resourcePath, filePath, 
                    std::filesystem::copy_options::overwrite_existing);
                file.open(filePath);
            }
            catch (...)
            {
                // Couldn't copy - will create new file on first save
            }
        }

        if (!file.is_open())
        {
            // File doesn't exist yet - that's okay, it will be created on first save
            s_initialized = true;
            return;
        }
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
// Save - Write to File
//=============================================================================

void InitialConditionsService::Save()
{
    std::string filePath = GetDataFilePath();
    std::ofstream file(filePath);

    if (!file.is_open())
    {
        // TODO: Consider logging error
        return;
    }

    // Write header
    file << "# Fractal Initial Conditions Data File\n";
    file << "# Format: FractalName|CenterX|CenterY|Zoom\n";
    file << "# Auto-generated - edits will be preserved but may be reformatted\n";
    file << "#\n";

    auto& registry = GetRegistry();
    for (const auto& entry : registry)
    {
        file << entry.first << "|"
             << entry.second.centerX << "|"
             << entry.second.centerY << "|"
             << entry.second.zoom << "\n";
    }

    file.close();
}

//=============================================================================
// Get - Retrieve Initial Conditions
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
// Set - Store/Update Initial Conditions
//=============================================================================

void InitialConditionsService::Set(const std::string& fractalName, double centerX, double centerY, double zoom)
{
    if (!s_initialized)
        Initialize();

    InitialConditions conditions(centerX, centerY, zoom);
    auto& registry = GetRegistry();
    registry[fractalName] = conditions;

    // Persist to file
    Save();
}

void InitialConditionsService::Set(const std::string& fractalName, const InitialConditions& conditions)
{
    Set(fractalName, conditions.centerX, conditions.centerY, conditions.zoom);
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
