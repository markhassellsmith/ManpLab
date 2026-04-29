using ManpWinUI.ViewModels;
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
            ViewModel.IsJuliaMode = false; // Reset Julia mode when switching fractals

            // Auto-render the selected fractal
            ViewModel.StatusMessage = $"Loading {fractalInfo.DisplayName}...";
            _ = ViewModel.RenderMandelbrotCommand.ExecuteAsync(null);
        }

        private async System.Threading.Tasks.Task InitializeViewModelAsync()
        {
            await ViewModel.InitializeAsync();
        }
    }
}
