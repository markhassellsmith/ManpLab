using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Services;
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
    private readonly IAppSettingsService? _settingsService;

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
    /// Week 5 Task 7: Filters categories and fractals by name/description.
    /// </summary>
    [ObservableProperty]
    private string searchQuery = string.Empty;

    /// <summary>
    /// Master list of all categories (unfiltered).
    /// Used to restore full list when search is cleared.
    /// </summary>
    private List<FractalCategoryNode> _allCategories = new();

    partial void OnSearchQueryChanged(string value)
    {
        FilterCategories(value);
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

    public FractalBrowserViewModel(IAppSettingsService? settingsService = null)
    {
        _settingsService = settingsService;
        Categories = new ObservableCollection<FractalCategoryNode>();
        LoadFromRegistry();
        RestoreSelection();
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

                _allCategories.Add(categoryNode);
            }

            // Populate Categories with all items initially
            foreach (var category in _allCategories)
            {
                Categories.Add(category);
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

    /// <summary>
    /// Filter categories and fractals based on search query.
    /// Week 5 Task 7: Searches by name, display name, and description.
    /// </summary>
    private void FilterCategories(string query)
    {
        Categories.Clear();

        // If no search query, show all categories
        if (string.IsNullOrWhiteSpace(query))
        {
            foreach (var category in _allCategories)
            {
                Categories.Add(category);
            }
            return;
        }

        // Case-insensitive search
        var searchLower = query.ToLower();

        // Filter each category
        foreach (var category in _allCategories)
        {
            // Check if category name matches
            var categoryMatches = category.Name.ToLower().Contains(searchLower);

            // Create filtered category with matching fractals
            var filteredCategory = new FractalCategoryNode
            {
                Name = category.Name,
                Icon = category.Icon,
                IsExpanded = true // Expand categories with search results
            };

            // Add fractals that match the search
            foreach (var fractal in category.Fractals)
            {
                var nameMatches = fractal.Name.ToLower().Contains(searchLower);
                var displayNameMatches = fractal.DisplayName.ToLower().Contains(searchLower);
                var descriptionMatches = !string.IsNullOrEmpty(fractal.Description) &&
                                        fractal.Description.ToLower().Contains(searchLower);

                if (categoryMatches || nameMatches || displayNameMatches || descriptionMatches)
                {
                    filteredCategory.Fractals.Add(fractal);
                }
            }

            // Only add category if it has matching fractals or its name matches
            if (filteredCategory.Fractals.Count > 0 || categoryMatches)
            {
                Categories.Add(filteredCategory);
            }
        }

        System.Diagnostics.Debug.WriteLine(
            $"[FractalBrowserViewModel] Filtered to {Categories.Count} categories for query: '{query}'");
    }

    /// <summary>
    /// Restore the previously selected fractal from settings.
    /// Week 5 Task 8: Persist selection across app restarts.
    /// </summary>
    private void RestoreSelection()
    {
        if (_settingsService == null)
            return;

        var savedFractalName = _settingsService.GetSelectedFractal();
        if (string.IsNullOrEmpty(savedFractalName))
            return;

        // Find the fractal in the loaded categories
        foreach (var category in _allCategories)
        {
            var fractal = category.Fractals.FirstOrDefault(f => f.Name == savedFractalName);
            if (fractal != null)
            {
                // Expand the category containing the saved fractal
                var displayedCategory = Categories.FirstOrDefault(c => c.Name == category.Name);
                if (displayedCategory != null)
                {
                    displayedCategory.IsExpanded = true;
                }

                // Trigger selection and render
                SelectFractal(fractal);

                System.Diagnostics.Debug.WriteLine(
                    $"[FractalBrowserViewModel] Restored selection: {savedFractalName}");
                break;
            }
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
    /// Week 5 Task 8: Save selection to settings for persistence.
    /// </summary>
    [RelayCommand]
    private void SelectFractal(FractalNode fractal)
    {
        if (fractal == null)
            return;

        SelectedFractal = fractal;

        System.Diagnostics.Debug.WriteLine($"[FractalBrowserViewModel] Fractal selected: {fractal.Name}");

        // Save selection to settings (Week 5 Task 8)
        _settingsService?.SetSelectedFractal(fractal.Name);

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
