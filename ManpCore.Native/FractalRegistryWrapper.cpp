#include "FractalRegistryWrapper.h"
#include "FractalRegistry.h"
#include <string>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace ManpCore::Native;

// Helper function to convert managed string to std::string without msclr/marshal
static std::string ManagedToStdString(String^ str)
{
    if (String::IsNullOrEmpty(str))
        return std::string();

    array<unsigned char>^ bytes = System::Text::Encoding::UTF8->GetBytes(str);
    pin_ptr<unsigned char> pinnedBytes = &bytes[0];
    return std::string(reinterpret_cast<char*>(pinnedBytes), bytes->Length);
}

// Helper function to convert std::string to managed String
static String^ StdStringToManaged(const std::string& str)
{
    if (str.empty())
        return String::Empty;

    array<unsigned char>^ bytes = gcnew array<unsigned char>((int)str.size());
    Marshal::Copy(IntPtr((void*)str.data()), bytes, 0, (int)str.size());
    return System::Text::Encoding::UTF8->GetString(bytes);
}

// Helper to convert std::vector<std::string> to List<String^>
static List<String^>^ StdVectorToManagedList(const std::vector<std::string>& vec)
{
    auto result = gcnew List<String^>();
    for (const auto& str : vec)
    {
        result->Add(StdStringToManaged(str));
    }
    return result;
}

// Helper to populate FractalInfo from FractalSpec
static void PopulateFractalInfo(FractalInfo^ info, const ::Native::FractalSpec* spec)
{
    info->Name = StdStringToManaged(spec->name);
    info->DisplayName = StdStringToManaged(spec->displayName);
    info->Category = StdStringToManaged(spec->category);
    info->Description = StdStringToManaged(spec->description);
    info->Formula = StdStringToManaged(spec->formula);
    info->FormulaLatex = StdStringToManaged(spec->formulaLatex);
    info->Derivation = StdStringToManaged(spec->derivation);
    info->VisualCharacteristics = StdStringToManaged(spec->visualCharacteristics);
    info->DiscoveredBy = StdStringToManaged(spec->discoveredBy);
    info->DiscoveryYear = spec->discoveryYear;
    info->ComputationalNotes = StdStringToManaged(spec->computationalNotes);
    info->SuggestedViewpoints = StdVectorToManagedList(spec->suggestedViewpoints);
    info->RelatedFractals = StdVectorToManagedList(spec->relatedFractals);
    info->References = StdVectorToManagedList(spec->references);
    info->SupportsJulia = spec->supportsJulia;
    info->DefaultCenterX = spec->defaultCenterX;
    info->DefaultCenterY = spec->defaultCenterY;
    info->DefaultZoom = spec->defaultZoom;
}


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
            info->Name = StdStringToManaged(spec->name);
            info->DisplayName = StdStringToManaged(spec->displayName);
            info->Category = StdStringToManaged(spec->category);
            info->Description = StdStringToManaged(spec->description);
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
        result->Add(StdStringToManaged(category));
    }

    return result;
}

List<FractalInfo^>^ FractalRegistryWrapper::GetFractalsByCategory(String^ category)
{
    auto result = gcnew List<FractalInfo^>();

    if (String::IsNullOrEmpty(category))
        return result;

    std::string nativeCategory = ManagedToStdString(category);
    std::vector<std::string> names = ::Native::FractalRegistry::GetFractalsByCategory(nativeCategory);

    for (const auto& name : names)
    {
        const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(name);
        if (spec != nullptr)
        {
            auto info = gcnew FractalInfo();
            PopulateFractalInfo(info, spec);
            result->Add(info);
        }
    }

    return result;
}

FractalInfo^ FractalRegistryWrapper::GetFractalInfo(String^ name)
{
    if (String::IsNullOrEmpty(name))
        return nullptr;

    std::string nativeName = ManagedToStdString(name);
    const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(nativeName);

    if (spec == nullptr)
        return nullptr;

    auto info = gcnew FractalInfo();
    PopulateFractalInfo(info, spec);
    return info;
}

List<ParameterInfo^>^ FractalRegistryWrapper::GetParameters(String^ fractalName)
{
    auto result = gcnew List<ParameterInfo^>();

    if (String::IsNullOrEmpty(fractalName))
        return result;

    std::string nativeName = ManagedToStdString(fractalName);
    const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(nativeName);

    if (spec == nullptr)
        return result;

    // Convert each ParameterSpec to ParameterInfo
    for (const auto& param : spec->parameters)
    {
        auto info = gcnew ParameterInfo();
        info->Name = StdStringToManaged(param.name);
        info->DisplayName = StdStringToManaged(param.displayName);
        info->Description = StdStringToManaged(param.description);

        // Map native enums to managed enums
        info->Type = static_cast<ManagedParameterType>(param.type);
        info->Category = static_cast<ManagedParameterCategory>(param.category);

        info->DefaultValue = StdStringToManaged(param.defaultValue);
        info->MinValue = param.minValue;
        info->MaxValue = param.maxValue;
        info->Step = param.step;

        // Convert choice values
        info->ChoiceValues = gcnew List<String^>();
        for (const auto& choice : param.choiceValues)
        {
            info->ChoiceValues->Add(StdStringToManaged(choice));
        }

        info->IsAdvanced = param.isAdvanced;
        info->IsReadOnly = param.isReadOnly;
        info->FormatString = StdStringToManaged(param.formatString);
        info->Unit = StdStringToManaged(param.unit);
        info->DisplayOrder = param.displayOrder;

        result->Add(info);
    }

    return result;
}

bool FractalRegistryWrapper::IsRegistered(String^ name)
{
    if (String::IsNullOrEmpty(name))
        return false;

    std::string nativeName = ManagedToStdString(name);
    return ::Native::FractalRegistry::IsRegistered(nativeName);
}

int FractalRegistryWrapper::GetCount()
{
    return (int)::Native::FractalRegistry::GetCount();
}
