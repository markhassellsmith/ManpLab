using ManpWinUI.ViewModels;
using ManpWinUI.ViewModels.Properties;
using ManpWinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace ManpWinUI.Views
{
    /// <summary>
    /// Main fractal explorer page with MVVM architecture.
    /// Core initialization and setup.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }
        private ParameterEditorViewModel ParameterEditorViewModel { get; }
        private ColorEditorViewModel ColorEditorViewModel { get; }
        private RenderSettingsViewModel RenderSettingsViewModel { get; }
        private ViewModels.Browser.FractalBrowserViewModel BrowserViewModel { get; } // Task 2: Inject browser ViewModel
        private IFractalMetadataService MetadataService { get; } // Task 3: Cached metadata service

        private bool _isDragging;
        private bool _isPanning; // Track if we're panning (right-click) vs zooming (left-click)
        private Windows.Foundation.Point _dragStartPoint;
        private System.Threading.Timer? _zoomTimer;
        private Grid? _fractalGrid; // Reference to the main fractal display grid

        public MainPage()
        {
            this.InitializeComponent();

            // Get ViewModel from DI container
            ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
            DataContext = ViewModel;

            // Task 2: Get BrowserViewModel from DI container
            BrowserViewModel = App.Current.Services.GetRequiredService<ViewModels.Browser.FractalBrowserViewModel>();

            // Task 3: Get MetadataService from DI container
            MetadataService = App.Current.Services.GetRequiredService<IFractalMetadataService>();

            // Get settings service from DI container
            var settingsService = App.Current.Services.GetRequiredService<IAppSettingsService>();

            // Initialize ParameterEditor ViewModel (Week 6)
            // Week 6 Task 6: Pass settings service for parameter persistence
            ParameterEditorViewModel = new ParameterEditorViewModel(settingsService);
            ParameterEditor.DataContext = ParameterEditorViewModel;

            // Week 6 Task 5: Subscribe to parameter changes for automatic re-rendering
            ParameterEditorViewModel.ParameterChanged += OnParameterChanged;

            // Initialize ColorEditor ViewModel (Week 7 Task 2)
            // ColorEditorView creates its own ViewModel internally, so we need to reference it
            ColorEditorViewModel = ColorEditor.ViewModel;

            // Week 7 Task 2: Subscribe to palette changes to trigger re-render with new colors
            ColorEditorViewModel.PaletteChanged += OnPaletteChanged;
            ColorEditorViewModel.ColorSettingsChanged += OnColorSettingsChanged;

            // Initialize RenderSettings ViewModel (Week 7 Task 2)
            // Week 9 Task 1 Fix: Use the SAME instance injected into MainViewModel from DI
            RenderSettingsViewModel = App.Current.Services.GetRequiredService<RenderSettingsViewModel>();
            RenderSettingsView.DataContext = RenderSettingsViewModel;

            // Week 7 Task 2: Subscribe to render settings changes
            RenderSettingsViewModel.RenderModeChanged += OnRenderModeChanged;
            RenderSettingsViewModel.RenderSettingsChanged += OnRenderSettingsChanged;

            // Task 2: Set BrowserView's ViewModel and subscribe to fractal selection
            BrowserView.ViewModel = BrowserViewModel;
            BrowserView.DataContext = BrowserViewModel;
            BrowserViewModel.FractalSelected += OnFractalSelected;

            // Initialize ViewModel asynchronously
            _ = InitializeViewModelAsync();

            // Subscribe to property changes to update coordinate axes
            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.CenterX) ||
                    e.PropertyName == nameof(ViewModel.CenterY) ||
                    e.PropertyName == nameof(ViewModel.Zoom) ||
                    e.PropertyName == nameof(ViewModel.ImageWidth) ||
                    e.PropertyName == nameof(ViewModel.ImageHeight) ||
                    e.PropertyName == nameof(ViewModel.FractalImage))
                {
                    UpdateCoordinateAxes();

                    // Week 6 Task 5: Sync view parameters back to ParameterEditor (for zoom/pan operations)
                    if (e.PropertyName == nameof(ViewModel.CenterX))
                        ParameterEditorViewModel.UpdateParameterValue("Center X", ViewModel.CenterX.ToString("F6"));
                    if (e.PropertyName == nameof(ViewModel.CenterY))
                        ParameterEditorViewModel.UpdateParameterValue("Center Y", ViewModel.CenterY.ToString("F6"));
                    if (e.PropertyName == nameof(ViewModel.Zoom))
                        ParameterEditorViewModel.UpdateParameterValue("Zoom", ViewModel.Zoom.ToString("F2"));
                }

                // Update Hailstone labels when relevant properties change
                if (e.PropertyName == nameof(ViewModel.ShowHailstoneLabels) ||
                    e.PropertyName == nameof(ViewModel.CurrentHailstoneResult) ||
                    e.PropertyName == nameof(ViewModel.HailstoneScaleX))
                {
                    // Only update labels after rendering completes (not during viewport changes)
                    if (!ViewModel.IsRendering)
                    {
                        UpdateHailstoneLabels();
                    }
                }

                // Update Hailstone info panel when result changes
                if (e.PropertyName == nameof(ViewModel.CurrentHailstoneResult) ||
                    e.PropertyName == nameof(ViewModel.IsHailstoneMode))
                {
                    UpdateHailstoneInfo();
                }
            };
        }

        /// <summary>
        /// Handle fractal selection from browser.
        /// Week 5 Task 6: Load selected fractal with default view parameters.
        /// Week 6 Task 2: Load parameters in ParameterEditorViewModel.
        /// Task 3: Use cached metadata service instead of direct P/Invoke.
        /// Task 7: Use flexible parameter system when available.
        /// Week 10: Update Info tab with fractal metadata.
        /// </summary>
        private void OnFractalSelected(object? sender, ViewModels.Browser.FractalSelectedEventArgs e)
        {
            try
            {
                if (e?.Fractal == null)
                {
                    Debug.WriteLine("[MainPage] OnFractalSelected: Fractal is null");
                    return;
                }

                Debug.WriteLine($"[MainPage] Loading fractal: {e.Fractal.Name}");

                // Special handling for 2-D Hailstone trajectory visualization
                if (e.Fractal.Name == "Hailstone2D")
                {
                    Debug.WriteLine("[MainPage] Detected Hailstone2D - switching to Hailstone mode");

                    // Switch to Hailstone mode (IsHailstoneMode is computed from SelectedFractalType)
                    ViewModel.SelectedFractalType = "Hailstone";

                    // Set default Hailstone parameters
                    ViewModel.HailstoneStartX = 27;
                    ViewModel.HailstoneStartY = 0;
                    ViewModel.HailstoneMaxIterations = 1000;
                    ViewModel.ShowHailstoneAxes = true;
                    ViewModel.ShowHailstonePoints = true;
                    ViewModel.ShowHailstoneLabels = true;
                    ViewModel.UseFixedHailstoneViewport = false;

                    ViewModel.CurrentVisualizationName = "2-D Hailstone Trajectory";
                    ViewModel.StatusMessage = "Loading 2-D Hailstone Trajectory...";

                    // Update Info tab
                    ViewModel.UpdateSelectedFractalInfo(e.Fractal.Name);

                    // Render the Hailstone sequence
                    _ = ViewModel.RenderCommand.ExecuteAsync(null);
                    return;
                }

                // Task 3: Get fractal metadata from cache (no P/Invoke!)
                var metadata = MetadataService.GetFractalOrDefault(e.Fractal.Name);
                Debug.WriteLine($"[MainPage] Got metadata for '{metadata.Name}' - Center: ({metadata.DefaultCenterX}, {metadata.DefaultCenterY}), Zoom: {metadata.DefaultZoom}");

                // Update ViewModel with fractal selection and default view
                ViewModel.SelectedFractalType = metadata.Name;
                ViewModel.CenterX = metadata.DefaultCenterX;
                ViewModel.CenterY = metadata.DefaultCenterY;
                ViewModel.Zoom = metadata.DefaultZoom;
                ViewModel.SelectedIterationMode = "Standard"; // Reset to standard mode when switching fractals

                // Set the current visualization name from the browser
                ViewModel.CurrentVisualizationName = metadata.DisplayName;

                // Week 10: Update Info tab with selected fractal metadata
                ViewModel.UpdateSelectedFractalInfo(e.Fractal.Name);

                // ═════════════════════════════════════════════════════════════════════════
                // TASK 7: Load parameter editor from flexible parameter system
                // ═════════════════════════════════════════════════════════════════════════
                if (ViewModel.CurrentParameters != null)
                {
                    Debug.WriteLine($"[MainPage] Loading parameter editor from flexible system ({ViewModel.CurrentParameters.Parameters.Count} parameters)");
                    ParameterEditorViewModel.LoadFromParameterSet(ViewModel.CurrentParameters);

                    // Subscribe to parameter changes for MainViewModel sync
                    if (ViewModel.CurrentParameters != null)
                    {
                        ViewModel.CurrentParameters.ParameterChanged -= OnFlexibleParameterChanged;
                        ViewModel.CurrentParameters.ParameterChanged += OnFlexibleParameterChanged;
                    }
                }
                else
                {
                    Debug.WriteLine($"[MainPage] Using legacy parameter loading (CurrentParameters is null)");
                    // Fallback to old method
                    ParameterEditorViewModel.LoadParametersForFractal(e.Fractal.Name);
                }

                // Auto-render the selected fractal
                ViewModel.StatusMessage = $"Loading {metadata.DisplayName}...";
                _ = ViewModel.RenderMandelbrotCommand.ExecuteAsync(null);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[MainPage] ERROR in OnFractalSelected: {ex.Message}");
                Debug.WriteLine($"[MainPage] Stack trace: {ex.StackTrace}");
                ViewModel.StatusMessage = $"Error loading fractal: {ex.Message}";
            }
        }

        /// <summary>
        /// Handle parameter changes from the parameter editor.
        /// Week 6 Task 5: Sync parameter values to MainViewModel and trigger re-render.
        /// </summary>
        private void OnParameterChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[MainPage] Parameter changed, syncing to ViewModel and re-rendering");

            // Sync editable parameters from ParameterEditor to MainViewModel
            var centerX = ParameterEditorViewModel.GetParameterValue("Center X");
            if (centerX != null && double.TryParse(centerX, out var x))
                ViewModel.CenterX = x;

            var centerY = ParameterEditorViewModel.GetParameterValue("Center Y");
            if (centerY != null && double.TryParse(centerY, out var y))
                ViewModel.CenterY = y;

            var zoom = ParameterEditorViewModel.GetParameterValue("Zoom");
            if (zoom != null && double.TryParse(zoom, out var z))
                ViewModel.Zoom = z;

            var maxIterations = ParameterEditorViewModel.GetParameterValue("Max Iterations");
            if (maxIterations != null && int.TryParse(maxIterations, out var iter))
                ViewModel.MaxIterations = iter;

            // Trigger re-render with updated parameters
            _ = ViewModel.RenderMandelbrotCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Handle parameter changes from flexible parameter system (Task 7).
        /// Parameters change in MainViewModel → update ParameterEditor UI.
        /// </summary>
        private void OnFlexibleParameterChanged(object? sender, ManpWinUI.Models.Parameters.ParameterChangedEventArgs e)
        {
            Debug.WriteLine($"[MainPage] Flexible parameter '{e.ParameterKey}' changed: {e.OldValue} → {e.NewValue}");

            // Parameter system already updated MainViewModel properties via bidirectional sync
            // No need to manually sync - just refresh parameter editor display if needed

            // Future: Could implement real-time UI updates here if ParameterEditor needs refresh
        }

        /// <summary>
        /// Handle palette selection changes from ColorEditorViewModel.
        /// Week 7 Task 2: Update MainViewModel palette and re-render.
        /// </summary>
        private void OnPaletteChanged(object? sender, EventArgs e)
        {
            if (ColorEditorViewModel.SelectedPalette != null)
            {
                var paletteName = ColorEditorViewModel.SelectedPalette.Name;
                Debug.WriteLine($"[MainPage] Palette changed to: {paletteName}");

                // Update MainViewModel with selected palette
                ViewModel.SelectedPalette = paletteName;

                // Only auto-render if we have a rendered image already
                if (ViewModel.FractalImage != null)
                {
                    _ = ViewModel.RenderCommand.ExecuteAsync(null);
                }
            }
        }

        /// <summary>
        /// Handle color settings changes (cycle speed, offset) from ColorEditorViewModel.
        /// Week 7 Task 3: Apply color cycle/offset adjustments to render parameters.
        /// </summary>
        private void OnColorSettingsChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[MainPage] Color settings changed - Speed: {ColorEditorViewModel.ColorCycleSpeed}, Offset: {ColorEditorViewModel.ColorOffset}");

            // Week 7 Task 3: Update MainViewModel with new color settings
            ViewModel.ColorCycleSpeed = ColorEditorViewModel.ColorCycleSpeed;
            ViewModel.ColorOffset = ColorEditorViewModel.ColorOffset;

            // Re-render if we have an existing image
            if (ViewModel.FractalImage != null)
            {
                _ = ViewModel.RenderCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Handle render mode changes from RenderSettingsViewModel.
        /// Week 7 Task 3: Apply smooth coloring setting to render parameters.
        /// </summary>
        private void OnRenderModeChanged(object? sender, EventArgs e)
        {
            var mode = RenderSettingsViewModel.SelectedRenderMode;
            Debug.WriteLine($"[MainPage] Render mode changed to: {mode}");

            // Week 7 Task 3: Update smooth coloring based on render mode
            // Note: Full render mode support requires native engine enhancements
            ViewModel.UseSmoothColoring = (mode == ViewModels.Properties.RenderMode.SmoothColoring);

            if (ViewModel.FractalImage != null)
            {
                ViewModel.StatusMessage = $"Render mode: {mode} - re-rendering with {(ViewModel.UseSmoothColoring ? "smooth" : "standard")} coloring";
                _ = ViewModel.RenderCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Handle quality settings changes from RenderSettingsViewModel.
        /// Week 7 Task 3: Apply smooth coloring toggle to render parameters.
        /// </summary>
        private void OnRenderSettingsChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[MainPage] Render settings changed - AA: {RenderSettingsViewModel.AntialiasingLevel}, Smooth: {RenderSettingsViewModel.UseSmoothColoring}, DeepZoom: {RenderSettingsViewModel.UseDeepZoom}");

            // Week 7 Task 3: Sync smooth coloring toggle to ViewModel
            ViewModel.UseSmoothColoring = RenderSettingsViewModel.UseSmoothColoring;

            // Note: Antialiasing and DeepZoom require native engine support (future enhancement)
            if (RenderSettingsViewModel.AntialiasingLevel != ViewModels.Properties.AntialiasingLevel.None)
            {
                ViewModel.StatusMessage = "⚠️ Antialiasing requires native engine enhancement (coming soon)";
            }

            // Auto re-render if smooth coloring changes and we have an image
            if (ViewModel.FractalImage != null && sender == RenderSettingsViewModel)
            {
                _ = ViewModel.RenderCommand.ExecuteAsync(null);
            }
        }

        private async System.Threading.Tasks.Task InitializeViewModelAsync()
        {
            await ViewModel.InitializeAsync();
        }
    }
}
