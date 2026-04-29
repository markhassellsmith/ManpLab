#include "FractalRegistryWrapper.h"
#include "FractalRegistry.h"
#include <string>
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace ManpCore::Native;
using namespace msclr::interop;

//=============================================================================
// FractalRegistryWrapper Implementation
//=============================================================================

void FractalRegistryWrapper::Initialize()
{
    ::Native::FractalRegistry::InitializeBuiltins();
}

List<FractalInfo^>^ FractalRegistryWrapper::GetAllFractals()
{
    auto result = gcnew List<FractalInfo^>();

    // Get all registered fractal names from native registry
    std::vector<std::string> names = ::Native::FractalRegistry::GetRegisteredNames();

    for (const auto& name : names)
    {
        const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(name);
        if (spec != nullptr)
        {
            auto info = gcnew FractalInfo();
            info->Name = marshal_as<String^>(spec->name);
            info->DisplayName = marshal_as<String^>(spec->displayName);
            info->Category = marshal_as<String^>(spec->category);
            info->Description = marshal_as<String^>(spec->description);
            info->SupportsJulia = spec->supportsJulia;
            info->DefaultCenterX = spec->defaultCenterX;
            info->DefaultCenterY = spec->defaultCenterY;
            info->DefaultZoom = spec->defaultZoom;

            result->Add(info);
        }
    }

    return result;
}

List<String^>^ FractalRegistryWrapper::GetCategories()
{
    auto result = gcnew List<String^>();

    std::vector<std::string> categories = ::Native::FractalRegistry::GetCategories();

    for (const auto& category : categories)
    {
        result->Add(marshal_as<String^>(category));
    }

    return result;
}

List<FractalInfo^>^ FractalRegistryWrapper::GetFractalsByCategory(String^ category)
{
    auto result = gcnew List<FractalInfo^>();

    if (String::IsNullOrEmpty(category))
        return result;

    std::string nativeCategory = marshal_as<std::string>(category);
    std::vector<std::string> names = ::Native::FractalRegistry::GetFractalsByCategory(nativeCategory);

    for (const auto& name : names)
    {
        const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(name);
        if (spec != nullptr)
        {
            auto info = gcnew FractalInfo();
            info->Name = marshal_as<String^>(spec->name);
            info->DisplayName = marshal_as<String^>(spec->displayName);
            info->Category = marshal_as<String^>(spec->category);
            info->Description = marshal_as<String^>(spec->description);
            info->SupportsJulia = spec->supportsJulia;
            info->DefaultCenterX = spec->defaultCenterX;
            info->DefaultCenterY = spec->defaultCenterY;
            info->DefaultZoom = spec->defaultZoom;

            result->Add(info);
        }
    }

    return result;
}

FractalInfo^ FractalRegistryWrapper::GetFractalInfo(String^ name)
{
    if (String::IsNullOrEmpty(name))
        return nullptr;

    std::string nativeName = marshal_as<std::string>(name);
    const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(nativeName);

    if (spec == nullptr)
        return nullptr;

    auto info = gcnew FractalInfo();
    info->Name = marshal_as<String^>(spec->name);
    info->DisplayName = marshal_as<String^>(spec->displayName);
    info->Category = marshal_as<String^>(spec->category);
    info->Description = marshal_as<String^>(spec->description);
    info->SupportsJulia = spec->supportsJulia;
    info->DefaultCenterX = spec->defaultCenterX;
    info->DefaultCenterY = spec->defaultCenterY;
    info->DefaultZoom = spec->defaultZoom;

    return info;
}

bool FractalRegistryWrapper::IsRegistered(String^ name)
{
    if (String::IsNullOrEmpty(name))
        return false;

    std::string nativeName = marshal_as<std::string>(name);
    return ::Native::FractalRegistry::IsRegistered(nativeName);
}

int FractalRegistryWrapper::GetCount()
{
    return (int)::Native::FractalRegistry::GetCount();
}
