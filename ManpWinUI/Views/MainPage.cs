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

        private async System.Threading.Tasks.Task InitializeViewModelAsync()
        {
            await ViewModel.InitializeAsync();
        }
    }
}
