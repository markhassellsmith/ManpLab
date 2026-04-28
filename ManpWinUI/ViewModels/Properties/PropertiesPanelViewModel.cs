using CommunityToolkit.Mvvm.ComponentModel;

namespace ManpWinUI.ViewModels.Properties;

/// <summary>
/// ViewModel for the Properties panel (right panel).
/// Manages parameter editing, color palettes, fractal info, and bookmarks.
/// Week 4: Wrapper around MainViewModel properties.
/// Week 5+: Enhanced with per-fractal parameter metadata.
/// </summary>
public partial class PropertiesPanelViewModel : ObservableObject
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // TAB SELECTION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Index of the currently selected tab.
    /// 0 = Parameters, 1 = Colors, 2 = Info, 3 = Bookmarks
    /// </summary>
    [ObservableProperty]
    private int selectedTabIndex = 0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL INFO (STUB FOR WEEK 4)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Name of the currently selected fractal.
    /// Week 5: Populate from FractalRegistry metadata.
    /// </summary>
    [ObservableProperty]
    private string currentFractalName = "Mandelbrot";

    /// <summary>
    /// Description of the currently selected fractal.
    /// Week 5: Populate from FractalRegistry metadata.
    /// </summary>
    [ObservableProperty]
    private string currentFractalDescription =
        "Select a fractal from the browser to see its description.\n\n" +
        "Week 5 Enhancement: Descriptions will be loaded from FractalRegistry metadata.";

    /// <summary>
    /// Formula text for the current fractal.
    /// Week 5: Populate from FractalRegistry.
    /// </summary>
    [ObservableProperty]
    private string currentFractalFormula = "z → z² + c";

    // ═══════════════════════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    public PropertiesPanelViewModel()
    {
        // Week 4: Basic initialization
        // Week 5: Wire up to MainViewModel for fractal selection updates
    }

    /// <summary>
    /// Update the info tab when fractal selection changes.
    /// Week 5: Called by MainViewModel when fractal changes.
    /// </summary>
    public void UpdateFractalInfo(string name, string description, string formula)
    {
        CurrentFractalName = name;
        CurrentFractalDescription = description;
        CurrentFractalFormula = formula;
    }
}
