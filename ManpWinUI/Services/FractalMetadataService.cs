using ManpWinUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Implementation of fractal metadata service.
/// Caches all fractal definitions from native registry at startup.
/// Provides fast, thread-safe access without P/Invoke overhead.
/// </summary>
public class FractalMetadataService : IFractalMetadataService
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // STATE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cache of fractal metadata by name.
    /// Populated once during InitializeAsync().
    /// </summary>
    private readonly Dictionary<string, FractalDescriptor> _cache = new();

    /// <summary>
    /// Cache of fractals grouped by category.
    /// Populated once during InitializeAsync().
    /// </summary>
    private readonly Dictionary<string, List<FractalDescriptor>> _categoryCache = new();

    /// <summary>
    /// Lock for thread-safe initialization.
    /// </summary>
    private readonly SemaphoreSlim _initLock = new(1, 1);

    /// <summary>
    /// Initialization state flag.
    /// </summary>
    private bool _initialized = false;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════════════════════════════════

    public bool IsInitialized => _initialized;

    public int Count => _cache.Count;

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_initialized)
            {
                Debug.WriteLine("[FractalMetadataService] Already initialized, skipping");
                return;
            }

            Debug.WriteLine("[FractalMetadataService] Initializing metadata cache...");
            var startTime = Stopwatch.StartNew();

            // Load all fractals from native registry on background thread
            await Task.Run(() =>
            {
                try
                {
                    // Get all registered fractals from native registry
                    var nativeFractals = ManpCore.Native.FractalRegistryWrapper.GetAllFractals();

                    Debug.WriteLine($"[FractalMetadataService] Loading {nativeFractals.Count} fractals from native registry");

                    // Load metadata for each fractal
                    foreach (var nativeInfo in nativeFractals)
                    {
                        try
                        {
                            if (nativeInfo == null)
                            {
                                Debug.WriteLine($"[FractalMetadataService] Warning: Null fractal info in registry");
                                continue;
                            }

                            var descriptor = new FractalDescriptor
                            {
                                Name = nativeInfo.Name,
                                DisplayName = nativeInfo.DisplayName,
                                Category = nativeInfo.Category,
                                Description = nativeInfo.Description,
                                SupportsJulia = nativeInfo.SupportsJulia,
                                DefaultCenterX = nativeInfo.DefaultCenterX,
                                DefaultCenterY = nativeInfo.DefaultCenterY,
                                DefaultZoom = nativeInfo.DefaultZoom,
                                DefaultMaxIterations = 512, // TODO: Get from native spec when available
                                DefaultBailout = 256.0,     // TODO: Get from native spec when available
                                Tags = new List<string>(),  // TODO: Parse from native spec when available
                                IsExperimental = false,     // TODO: Flag from native spec when available
                                RenderComplexity = "medium" // TODO: Estimate from native spec when available
                            };

                            // Add to main cache
                            _cache[descriptor.Name] = descriptor;

                            // Add to category cache
                            if (!_categoryCache.ContainsKey(descriptor.Category))
                            {
                                _categoryCache[descriptor.Category] = new List<FractalDescriptor>();
                            }
                            _categoryCache[descriptor.Category].Add(descriptor);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[FractalMetadataService] Error loading fractal: {ex.Message}");
                        }
                    }

                    // Sort fractals within each category by display name
                    foreach (var category in _categoryCache.Values)
                    {
                        category.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[FractalMetadataService] Critical error during initialization: {ex.Message}");
                    throw;
                }
            });

            _initialized = true;
            startTime.Stop();

            Debug.WriteLine($"[FractalMetadataService] Initialized with {_cache.Count} fractals in {_categoryCache.Count} categories ({startTime.ElapsedMilliseconds}ms)");

            // Log categories
            foreach (var category in GetCategories())
            {
                var count = _categoryCache[category].Count;
                Debug.WriteLine($"[FractalMetadataService]   - {category}: {count} fractals");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalMetadataService] Initialization failed: {ex.Message}");
            Debug.WriteLine($"[FractalMetadataService] Stack trace: {ex.StackTrace}");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public FractalDescriptor? GetFractal(string name)
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        if (string.IsNullOrWhiteSpace(name))
            return null;

        return _cache.TryGetValue(name, out var descriptor) ? descriptor : null;
    }

    public FractalDescriptor GetFractalOrDefault(string name)
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        if (string.IsNullOrWhiteSpace(name))
            return FractalDescriptor.CreateGenericFallback("Unknown");

        if (_cache.TryGetValue(name, out var descriptor))
            return descriptor;

        Debug.WriteLine($"[FractalMetadataService] Fractal '{name}' not found, returning generic fallback");
        return FractalDescriptor.CreateGenericFallback(name);
    }

    public IReadOnlyList<FractalDescriptor> GetFractalsByCategory(string category)
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        if (string.IsNullOrWhiteSpace(category))
            return Array.Empty<FractalDescriptor>();

        if (_categoryCache.TryGetValue(category, out var fractals))
        {
            // Return defensive copy to prevent external modification
            return fractals.ToList();
        }

        return Array.Empty<FractalDescriptor>();
    }

    public IReadOnlyList<string> GetCategories()
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        // Return sorted list of category names
        return _categoryCache.Keys.OrderBy(c => c).ToList();
    }

    public IReadOnlyList<FractalDescriptor> GetAllFractals()
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        // Return defensive copy sorted by category then display name
        return _cache.Values
            .OrderBy(f => f.Category)
            .ThenBy(f => f.DisplayName)
            .ToList();
    }

    public IReadOnlyList<FractalDescriptor> SearchFractals(string query)
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        if (string.IsNullOrWhiteSpace(query))
            return GetAllFractals();

        var lowerQuery = query.ToLowerInvariant();

        var results = _cache.Values.Where(f =>
            f.Name.ToLowerInvariant().Contains(lowerQuery) ||
            f.DisplayName.ToLowerInvariant().Contains(lowerQuery) ||
            (f.Description != null && f.Description.ToLowerInvariant().Contains(lowerQuery)) ||
            f.Tags.Any(tag => tag.ToLowerInvariant().Contains(lowerQuery))
        ).OrderBy(f => f.DisplayName).ToList();

        Debug.WriteLine($"[FractalMetadataService] Search '{query}' returned {results.Count} results");

        return results;
    }

    public bool Exists(string name)
    {
        if (!_initialized)
            throw new InvalidOperationException("FractalMetadataService is not initialized. Call InitializeAsync() first.");

        return !string.IsNullOrWhiteSpace(name) && _cache.ContainsKey(name);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // DIAGNOSTICS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Dumps the entire metadata cache to debug output.
    /// Useful for troubleshooting initialization issues.
    /// </summary>
    public void DumpToDebug()
    {
        if (!_initialized)
        {
            Debug.WriteLine("[FractalMetadataService] Service not initialized");
            return;
        }

        Debug.WriteLine("╔═══════════════════════════════════════════════════════════════════════════");
        Debug.WriteLine($"║ FractalMetadataService Cache ({_cache.Count} fractals)");
        Debug.WriteLine("╠═══════════════════════════════════════════════════════════════════════════");

        foreach (var category in GetCategories())
        {
            var fractals = _categoryCache[category];
            Debug.WriteLine($"║ [{category}] ({fractals.Count} fractals)");

            foreach (var fractal in fractals)
            {
                Debug.WriteLine($"║   - {fractal.DisplayName}");
                Debug.WriteLine($"║     Name: {fractal.Name}");
                Debug.WriteLine($"║     Default View: ({fractal.DefaultCenterX:F6}, {fractal.DefaultCenterY:F6}) @ {fractal.DefaultZoom:F2}x");
                Debug.WriteLine($"║     Julia Support: {(fractal.SupportsJulia ? "Yes" : "No")}");
                if (!string.IsNullOrWhiteSpace(fractal.Description))
                    Debug.WriteLine($"║     Description: {fractal.Description}");
            }
            Debug.WriteLine("║");
        }

        Debug.WriteLine("╚═══════════════════════════════════════════════════════════════════════════");
    }
}
