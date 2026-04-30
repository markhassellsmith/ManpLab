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

            // Get settings service from DI container
            var settingsService = App.Current.Services.GetRequiredService<IAppSettingsService>();

            // Initialize ParameterEditor ViewModel (Week 6)
            // Week 6 Task 6: Pass settings service for parameter persistence
            ParameterEditorViewModel = new ParameterEditorViewModel(settingsService);
            ParameterEditor.DataContext = ParameterEditorViewModel;

            // Week 6 Task 5: Subscribe to parameter changes for automatic re-rendering
            ParameterEditorViewModel.ParameterChanged += OnParameterChanged;

            // Initialize ColorEditor ViewModel (Week 7 Task 2)
            ColorEditorViewModel = new ColorEditorViewModel();
            ColorEditor.DataContext = ColorEditorViewModel;

            // Week 7 Task 2: Subscribe to palette changes to trigger re-render with new colors
            ColorEditorViewModel.PaletteChanged += OnPaletteChanged;
            ColorEditorViewModel.ColorSettingsChanged += OnColorSettingsChanged;

            // Initialize RenderSettings ViewModel (Week 7 Task 2)
            RenderSettingsViewModel = new RenderSettingsViewModel();
            RenderSettingsView.DataContext = RenderSettingsViewModel;

            // Week 7 Task 2: Subscribe to render settings changes
            RenderSettingsViewModel.RenderModeChanged += OnRenderModeChanged;
            RenderSettingsViewModel.RenderSettingsChanged += OnRenderSettingsChanged;

            // Subscribe to browser fractal selection (Week 5 Task 6)
            BrowserView.ViewModel.FractalSelected += OnFractalSelected;

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
        /// </summary>
        private void OnFractalSelected(object? sender, ViewModels.Browser.FractalSelectedEventArgs e)
        {
            if (e?.Fractal == null)
                return;

            Debug.WriteLine($"[MainPage] Loading fractal: {e.Fractal.Name}");

            // Get fractal metadata from registry
            var fractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(e.Fractal.Name);
            if (fractalInfo == null)
            {
                ViewModel.StatusMessage = $"Error: Fractal '{e.Fractal.Name}' not found in registry";
                return;
            }

            // Update ViewModel with fractal selection and default view
            ViewModel.SelectedFractalType = e.Fractal.Name;
            ViewModel.CenterX = fractalInfo.DefaultCenterX;
            ViewModel.CenterY = fractalInfo.DefaultCenterY;
            ViewModel.Zoom = fractalInfo.DefaultZoom;
            ViewModel.SelectedIterationMode = "Standard"; // Reset to standard mode when switching fractals

            // Week 6 Task 2: Load parameters in ParameterEditor
            ParameterEditorViewModel.LoadParametersForFractal(e.Fractal.Name);

            // Auto-render the selected fractal
            ViewModel.StatusMessage = $"Loading {fractalInfo.DisplayName}...";
            _ = ViewModel.RenderMandelbrotCommand.ExecuteAsync(null);
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
        /// Week 7 Task 2: Real-time color adjustments.
        /// </summary>
        private void OnColorSettingsChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[MainPage] Color settings changed - Speed: {ColorEditorViewModel.ColorCycleSpeed}, Offset: {ColorEditorViewModel.ColorOffset}");

            // TODO Week 7 Task 3: Apply color cycle/offset adjustments
            // For now, just re-render with the adjusted palette
            if (ViewModel.FractalImage != null)
            {
                _ = ViewModel.RenderCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Handle render mode changes from RenderSettingsViewModel.
        /// Week 7 Task 2: Switch between escape-time, smooth coloring, distance estimation, orbit trap.
        /// </summary>
        private void OnRenderModeChanged(object? sender, EventArgs e)
        {
            var mode = RenderSettingsViewModel.SelectedRenderMode;
            Debug.WriteLine($"[MainPage] Render mode changed to: {mode}");

            // TODO Week 7 Task 3: Pass render mode to native engine
            // For now, just log the change
            if (ViewModel.FractalImage != null)
            {
                ViewModel.StatusMessage = $"Render mode: {mode} (re-render to apply)";
            }
        }

        /// <summary>
        /// Handle quality settings changes from RenderSettingsViewModel.
        /// Week 7 Task 2: Antialiasing, smooth coloring, deep zoom toggles.
        /// </summary>
        private void OnRenderSettingsChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[MainPage] Render settings changed - AA: {RenderSettingsViewModel.AntialiasingLevel}, Smooth: {RenderSettingsViewModel.UseSmoothColoring}, DeepZoom: {RenderSettingsViewModel.UseDeepZoom}");

            // TODO Week 7 Task 3: Pass quality flags to native engine
            // For now, just log the change
        }

        private async System.Threading.Tasks.Task InitializeViewModelAsync()
        {
            await ViewModel.InitializeAsync();
        }
    }
}
