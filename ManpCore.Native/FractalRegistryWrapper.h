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
