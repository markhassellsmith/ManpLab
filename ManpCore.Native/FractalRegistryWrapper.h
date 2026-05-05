#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace ManpCore {
namespace Native {

    /// <summary>
    /// Represents metadata for a single fractal type.
    /// Exposes C++ FractalSpec data to managed C#/WinUI code.
    /// </summary>
    public ref class FractalInfo
    {
    public:
        /// <summary>Unique identifier (e.g., "Mandelbrot", "BurningShip")</summary>
        property String^ Name;

        /// <summary>Display name for UI (e.g., "Mandelbrot Set", "Burning Ship")</summary>
        property String^ DisplayName;

        /// <summary>Category name (e.g., "Classic Fractals", "Julia Sets")</summary>
        property String^ Category;

        /// <summary>Description text for help/tooltips</summary>
        property String^ Description;

        /// <summary>Mathematical formula (e.g., "z_{n+1} = z_n^2 + c")</summary>
        property String^ Formula;

        /// <summary>LaTeX formula for rendering (optional)</summary>
        property String^ FormulaLatex;

        /// <summary>Mathematical background and explanation</summary>
        property String^ Derivation;

        /// <summary>Visual characteristics description</summary>
        property String^ VisualCharacteristics;

        /// <summary>Who discovered/invented this fractal</summary>
        property String^ DiscoveredBy;

        /// <summary>Year discovered/published (0 = unknown)</summary>
        property int DiscoveryYear;

        /// <summary>Performance and precision notes</summary>
        property String^ ComputationalNotes;

        /// <summary>Interesting viewpoints to explore</summary>
        property List<String^>^ SuggestedViewpoints;

        /// <summary>Related or similar fractals</summary>
        property List<String^>^ RelatedFractals;

        /// <summary>References (papers, links, etc.)</summary>
        property List<String^>^ References;

        /// <summary>Supports Julia mode rendering</summary>
        property bool SupportsJulia;

        /// <summary>Default center X coordinate</summary>
        property double DefaultCenterX;

        /// <summary>Default center Y coordinate</summary>
        property double DefaultCenterY;

        /// <summary>Default zoom level</summary>
        property double DefaultZoom;
    };

    /// <summary>
    /// Parameter type enumeration (mirrors native ParameterType)
    /// </summary>
    public enum class ManagedParameterType
    {
        Integer = 0,
        Float = 1,
        Boolean = 2,
        Choice = 3,
        Complex = 4
    };

    /// <summary>
    /// Parameter category enumeration (mirrors native ParameterCategory)
    /// </summary>
    public enum class ManagedParameterCategory
    {
        General = 0,
        Calculation = 1,
        View = 2,
        Color = 3,
        Advanced = 4
    };

    /// <summary>
    /// Represents metadata for a single parameter.
    /// Exposes C++ ParameterSpec data to managed C#/WinUI code.
    /// </summary>
    public ref class ParameterInfo
    {
    public:
        /// <summary>Internal parameter key (e.g., "realz0", "maxIterations")</summary>
        property String^ Name;

        /// <summary>Display name for UI (e.g., "Real Perturbation of Z(0)")</summary>
        property String^ DisplayName;

        /// <summary>Description text for help/tooltips</summary>
        property String^ Description;

        /// <summary>Parameter type (Integer, Float, Boolean, Choice, Complex)</summary>
        property ManagedParameterType Type;

        /// <summary>Parameter category (General, Calculation, View, Color, Advanced)</summary>
        property ManagedParameterCategory Category;

        /// <summary>Default value as string (e.g., "0.0", "256", "true")</summary>
        property String^ DefaultValue;

        /// <summary>Minimum value (for numeric types)</summary>
        property double MinValue;

        /// <summary>Maximum value (for numeric types)</summary>
        property double MaxValue;

        /// <summary>Step size for UI controls (for numeric types)</summary>
        property double Step;

        /// <summary>Choice values (for Choice type)</summary>
        property List<String^>^ ChoiceValues;

        /// <summary>Is this an advanced parameter?</summary>
        property bool IsAdvanced;

        /// <summary>Is this parameter read-only?</summary>
        property bool IsReadOnly;

        /// <summary>Format string for display (e.g., "F6")</summary>
        property String^ FormatString;

        /// <summary>Unit label (e.g., "x", "°")</summary>
        property String^ Unit;

        /// <summary>Display order hint</summary>
        property int DisplayOrder;
    };

    /// <summary>
    /// Managed wrapper for native C++ FractalRegistry.
    /// Provides access to all registered fractals for the WinUI browser.
    /// </summary>
    public ref class FractalRegistryWrapper
    {
    public:
        /// <summary>
        /// Initialize all built-in fractals.
        /// Call once at application startup before accessing registry.
        /// </summary>
        static void Initialize();

        /// <summary>
        /// Get metadata for all registered fractals.
        /// </summary>
        /// <returns>List of FractalInfo objects</returns>
        static List<FractalInfo^>^ GetAllFractals();

        /// <summary>
        /// Get all unique category names.
        /// </summary>
        /// <returns>List of category names (e.g., "Classic Fractals", "Julia Sets")</returns>
        static List<String^>^ GetCategories();

        /// <summary>
        /// Get all fractals in a specific category.
        /// </summary>
        /// <param name="category">Category name to filter by</param>
        /// <returns>List of FractalInfo objects in that category</returns>
        static List<FractalInfo^>^ GetFractalsByCategory(String^ category);

        /// <summary>
        /// Get metadata for a specific fractal by name.
        /// </summary>
        /// <param name="name">Fractal name (e.g., "Mandelbrot")</param>
        /// <returns>FractalInfo or nullptr if not found</returns>
        static FractalInfo^ GetFractalInfo(String^ name);

        /// <summary>
        /// Get parameter metadata for a specific fractal.
        /// </summary>
        /// <param name="fractalName">Fractal name (e.g., "Lambda")</param>
        /// <returns>List of ParameterInfo objects, or empty list if fractal not found or has no parameters</returns>
        static List<ParameterInfo^>^ GetParameters(String^ fractalName);

        /// <summary>
        /// Check if a fractal is registered.
        /// </summary>
        /// <param name="name">Fractal name to check</param>
        /// <returns>True if registered, false otherwise</returns>
        static bool IsRegistered(String^ name);

        /// <summary>
        /// Get count of registered fractals.
        /// </summary>
        /// <returns>Number of registered fractals</returns>
        static int GetCount();
    };

}} // namespace ManpCore::Native
