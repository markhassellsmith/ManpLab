using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ManpWinUI.ViewModels.Browser;

/// <summary>
/// ViewModel for the Fractal Browser panel.
/// Manages fractal categories, search, and selection.
/// Week 4: Stub implementation with hardcoded categories.
/// Week 5: Populate from FractalRegistry dynamically.
/// </summary>
public partial class FractalBrowserViewModel : ObservableObject
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // EVENTS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Event raised when a fractal is selected in the browser.
    /// Week 5 Task 6: MainViewModel subscribes to this to handle fractal loading.
    /// </summary>
    public event EventHandler<FractalSelectedEventArgs>? FractalSelected;
    // ═══════════════════════════════════════════════════════════════════════════════
    // SEARCH & FILTER
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Search query text for filtering fractals.
    /// Week 5: Wire up actual filtering logic.
    /// </summary>
    [ObservableProperty]
    private string searchQuery = string.Empty;

    partial void OnSearchQueryChanged(string value)
    {
        // Week 5: Implement filtering
        // For now, just log the search
        System.Diagnostics.Debug.WriteLine($"[FractalBrowserViewModel] Search query: {value}");
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL CATEGORIES (STUB DATA)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Collection of fractal categories for TreeView.
    /// Week 4: Hardcoded stub data.
    /// Week 5: Populate from FractalRegistry.GetCategories().
    /// </summary>
    public ObservableCollection<FractalCategoryNode> Categories { get; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // SELECTION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Currently selected fractal node in the browser.
    /// Week 5: Trigger render when fractal selected.
    /// </summary>
    [ObservableProperty]
    private FractalNode? selectedFractal;

    partial void OnSelectedFractalChanged(FractalNode? value)
    {
        if (value != null)
        {
            System.Diagnostics.Debug.WriteLine($"[FractalBrowserViewModel] Selected: {value.Name}");
            // Week 5: Fire FractalSelected event to MainViewModel
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    public FractalBrowserViewModel()
    {
        Categories = new ObservableCollection<FractalCategoryNode>();
        LoadFromRegistry();
    }

    /// <summary>
    /// Load categories and fractals from the native FractalRegistry.
    /// Week 5: Replaces stub data with real registry integration.
    /// </summary>
    private void LoadFromRegistry()
    {
        try
        {
            var categories = ManpCore.Native.FractalRegistryWrapper.GetCategories();

            foreach (var categoryName in categories)
            {
                var fractals = ManpCore.Native.FractalRegistryWrapper.GetFractalsByCategory(categoryName);

                if (fractals.Count == 0)
                    continue;

                var categoryNode = new FractalCategoryNode
                {
                    Name = categoryName,
                    Icon = "📁",
                    IsExpanded = categoryName == "Classic Fractals" // Expand Classic by default
                };

                foreach (var fractalInfo in fractals)
                {
                    categoryNode.Fractals.Add(new FractalNode
                    {
                        Name = fractalInfo.Name,
                        DisplayName = fractalInfo.DisplayName,
                        Category = fractalInfo.Category,
                        Description = fractalInfo.Description
                    });
                }

                Categories.Add(categoryNode);
            }

            System.Diagnostics.Debug.WriteLine(
                $"[FractalBrowserViewModel] Loaded {Categories.Count} categories from registry");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[FractalBrowserViewModel] Error loading registry: {ex.Message}");
            // Fall back to empty state rather than showing stale stub data
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // COMMANDS (WEEK 5)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Clear the search query.
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        SearchQuery = string.Empty;
    }

    /// <summary>
    /// Handle fractal selection from the browser.
    /// Week 5 Task 6: Raise event to notify MainViewModel.
    /// </summary>
    [RelayCommand]
    private void SelectFractal(FractalNode fractal)
    {
        if (fractal == null)
            return;

        SelectedFractal = fractal;

        System.Diagnostics.Debug.WriteLine($"[FractalBrowserViewModel] Fractal selected: {fractal.Name}");

        // Raise event with fractal info
        FractalSelected?.Invoke(this, new FractalSelectedEventArgs(fractal));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// EVENT ARGS
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Event arguments for fractal selection.
/// </summary>
public class FractalSelectedEventArgs : EventArgs
{
    public FractalNode Fractal { get; }

    public FractalSelectedEventArgs(FractalNode fractal)
    {
        Fractal = fractal;
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// DATA MODELS
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Represents a category node in the fractal browser TreeView.
/// </summary>
public class FractalCategoryNode
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "📁";
    public bool IsExpanded { get; set; } = false;
    public ObservableCollection<FractalNode> Fractals { get; } = new();
}

/// <summary>
/// Represents a fractal node in the browser.
/// Week 5: Populated from FractalRegistry with metadata.
/// </summary>
public class FractalNode
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
}
