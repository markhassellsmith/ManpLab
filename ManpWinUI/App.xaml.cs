using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ManpWinUI.Services;
using ManpWinUI.ViewModels;
using ManpCore.Services.Algorithms;
using ManpCore.Services.Color;
using Serilog;
using System;
using System.IO;

namespace ManpWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? window;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the current App instance as a strongly-typed object.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the service provider for dependency injection.
        /// </summary>
        public IServiceProvider Services => _serviceProvider;

        /// <summary>
        /// Gets the main application window.
        /// </summary>
        public Window? MainWindow => window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // Configure Serilog
            ConfigureLogging();

            // Initialize native fractal registry (Week 5: Phase 2)
            InitializeFractalRegistry();

            // Configure dependency injection
            _serviceProvider = ConfigureServices();
        }

        /// <summary>
        /// Initialize the native FractalRegistry with all built-in fractals.
        /// Week 5: Required for browser integration.
        /// </summary>
        private void InitializeFractalRegistry()
        {
            try
            {
                ManpCore.Native.FractalRegistryWrapper.Initialize();
                int count = ManpCore.Native.FractalRegistryWrapper.GetCount();
                Log.Information("FractalRegistry initialized with {Count} fractals", count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize FractalRegistry");
            }
        }

        /// <summary>
        /// Configures Serilog for application logging.
        /// </summary>
        private void ConfigureLogging()
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ManpWinUI",
                "logs",
                "app.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("ManpWinUI application starting");
        }

        /// <summary>
        /// Configures dependency injection services.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            // Shared Core Services (platform-agnostic)
            services.AddSingleton<IColorPalette, HsvColorPalette>();
            services.AddSingleton<IHailstoneCalculator, HailstoneCalculator>();

            // WinUI-Specific Services
            services.AddSingleton<IFractalRenderService, FractalRenderService>();
            services.AddSingleton<IHailstoneService, HailstoneService>();
            services.AddSingleton<IHailstoneRenderService, HailstoneRenderServiceWin2D>(); // Default: Win2D backend
            services.AddSingleton<IImageExportService, ImageExportService>();
            services.AddSingleton<IHailstoneExportService, HailstoneExportService>();
            services.AddSingleton<IBookmarkService, BookmarkService>();
            services.AddSingleton<INavigationHistoryService, NavigationHistoryService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            window ??= new Window();

            if (window.Content is not Frame rootFrame)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                window.Content = rootFrame;
            }

            _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);

            // Subscribe to window close event to save navigation history
            window.Closed += OnWindowClosed;

            window.Activate();

            Log.Information("ManpWinUI window activated");
        }

        /// <summary>
        /// Invoked when the application window is closing.
        /// Saves navigation history to persistent storage.
        /// </summary>
        private async void OnWindowClosed(object sender, WindowEventArgs args)
        {
            try
            {
                var historyService = _serviceProvider.GetService<INavigationHistoryService>();
                if (historyService != null)
                {
                    await historyService.SaveHistoryAsync();
                    Log.Information("Navigation history saved on app close");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving navigation history on close");
            }

            Log.Information("ManpWinUI application closing");
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Log.Error("Navigation failed to {PageType}", e.SourcePageType.FullName);
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}