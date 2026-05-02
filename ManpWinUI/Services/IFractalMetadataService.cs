using ManpWinUI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Service for managing cached fractal metadata.
/// Provides thread-safe access to fractal definitions without P/Invoke overhead.
/// Initialized once at app startup by loading all fractals from native registry.
/// </summary>
public interface IFractalMetadataService
{
    /// <summary>
    /// Initialize the service by loading all fractal metadata from native registry.
    /// Must be called once at app startup before any other methods.
    /// Thread-safe - multiple calls are safe (subsequent calls are no-ops).
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Gets cached metadata for a specific fractal by name.
    /// Returns null if fractal doesn't exist in registry.
    /// Thread-safe - can be called from any thread after initialization.
    /// </summary>
    /// <param name="name">Fractal name (case-sensitive, matches native registry key)</param>
    /// <returns>Fractal descriptor, or null if not found</returns>
    FractalDescriptor? GetFractal(string name);

    /// <summary>
    /// Gets cached metadata for a specific fractal, with fallback to generic defaults.
    /// Never returns null - if fractal doesn't exist, returns a generic descriptor.
    /// Use this in UI code where you need guaranteed non-null result.
    /// </summary>
    /// <param name="name">Fractal name</param>
    /// <returns>Fractal descriptor (either real or generic fallback)</returns>
    FractalDescriptor GetFractalOrDefault(string name);

    /// <summary>
    /// Gets all fractals in a specific category.
    /// Returns empty list if category doesn't exist.
    /// Thread-safe - returns a defensive copy.
    /// </summary>
    /// <param name="category">Category name (e.g., "Classic Escape-Time")</param>
    /// <returns>Read-only list of fractals in that category</returns>
    IReadOnlyList<FractalDescriptor> GetFractalsByCategory(string category);

    /// <summary>
    /// Gets all category names in the registry.
    /// Useful for populating browser tree or dropdown menus.
    /// Thread-safe - returns a defensive copy.
    /// </summary>
    /// <returns>Read-only list of category names</returns>
    IReadOnlyList<string> GetCategories();

    /// <summary>
    /// Gets all fractals in the registry.
    /// Thread-safe - returns a defensive copy.
    /// </summary>
    /// <returns>Read-only list of all fractal descriptors</returns>
    IReadOnlyList<FractalDescriptor> GetAllFractals();

    /// <summary>
    /// Searches fractals by name, display name, description, or tags.
    /// Case-insensitive partial match.
    /// Example: query="man" returns Mandelbrot, Mandelbar, etc.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>Read-only list of matching fractals</returns>
    IReadOnlyList<FractalDescriptor> SearchFractals(string query);

    /// <summary>
    /// Checks if a fractal exists in the registry.
    /// Faster than GetFractal() if you only need existence check.
    /// </summary>
    /// <param name="name">Fractal name</param>
    /// <returns>True if fractal exists, false otherwise</returns>
    bool Exists(string name);

    /// <summary>
    /// Gets the total number of fractals in the registry.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Checks if the service has been initialized.
    /// </summary>
    bool IsInitialized { get; }
}
