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
        LoadStubData();
    }

    /// <summary>
    /// Load stub category data for Week 4.
    /// Week 5: Replace with FractalRegistry integration.
    /// </summary>
    private void LoadStubData()
    {
        // Classic Fractals (4)
        var classic = new FractalCategoryNode
        {
            Name = "Classic Fractals",
            Icon = "📁",
            IsExpanded = true
        };
        classic.Fractals.Add(new FractalNode { Name = "Mandelbrot", Category = "Classic Fractals" });
        classic.Fractals.Add(new FractalNode { Name = "BurningShip", Category = "Classic Fractals" });
        classic.Fractals.Add(new FractalNode { Name = "Tricorn", Category = "Classic Fractals" });
        classic.Fractals.Add(new FractalNode { Name = "Phoenix", Category = "Classic Fractals" });
        Categories.Add(classic);

        // Multibrot Family (3)
        var multibrot = new FractalCategoryNode
        {
            Name = "Multibrot Family",
            Icon = "📁"
        };
        multibrot.Fractals.Add(new FractalNode { Name = "Multibrot3", Category = "Multibrot Family" });
        multibrot.Fractals.Add(new FractalNode { Name = "Multibrot4", Category = "Multibrot Family" });
        multibrot.Fractals.Add(new FractalNode { Name = "Multibrot5", Category = "Multibrot Family" });
        Categories.Add(multibrot);

        // Newton Method (2)
        var newton = new FractalCategoryNode
        {
            Name = "Newton Method",
            Icon = "📁"
        };
        newton.Fractals.Add(new FractalNode { Name = "Newton", Category = "Newton Method" });
        newton.Fractals.Add(new FractalNode { Name = "Nova", Category = "Newton Method" });
        Categories.Add(newton);

        // Magnet Fractals (2)
        var magnet = new FractalCategoryNode
        {
            Name = "Magnet Fractals",
            Icon = "📁"
        };
        magnet.Fractals.Add(new FractalNode { Name = "Magnet1", Category = "Magnet Fractals" });
        magnet.Fractals.Add(new FractalNode { Name = "Magnet2", Category = "Magnet Fractals" });
        Categories.Add(magnet);

        // Julia Presets (3)
        var julia = new FractalCategoryNode
        {
            Name = "Julia Presets",
            Icon = "📁"
        };
        julia.Fractals.Add(new FractalNode { Name = "JuliaSanMarco", Category = "Julia Presets" });
        julia.Fractals.Add(new FractalNode { Name = "JuliaDouadyRabbit", Category = "Julia Presets" });
        julia.Fractals.Add(new FractalNode { Name = "JuliaSiegelDisk", Category = "Julia Presets" });
        Categories.Add(julia);
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
/// Week 5: Add metadata (description, thumbnail, etc.)
/// </summary>
public class FractalNode
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
}
