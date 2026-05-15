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
using Microsoft.UI.Xaml;

namespace ManpWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? window;
        private readonly IServiceProvider _serviceProvider;
        private ResourceDictionary? _customThemeDict; // Track loaded custom theme

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

            // Add global exception handler to catch InvalidCastException with full stack trace
            this.UnhandledException += (sender, e) =>
            {
                var exceptionInfo = new System.Text.StringBuilder();
                exceptionInfo.AppendLine("═══════════════════════════════════════════════════════════");
                exceptionInfo.AppendLine($"UNHANDLED EXCEPTION: {e.Exception.GetType().Name}");
                exceptionInfo.AppendLine($"Message: {e.Exception.Message}");
                exceptionInfo.AppendLine($"Stack Trace:\n{e.Exception.StackTrace}");
                exceptionInfo.AppendLine("═══════════════════════════════════════════════════════════");

                var logMessage = exceptionInfo.ToString();
                System.Diagnostics.Debug.WriteLine(logMessage);

                // Also log to Serilog if available
                try
                {
                    Log.Error(e.Exception, "Unhandled exception caught in App.UnhandledException handler");
                }
                catch { /* Serilog might not be initialized yet */ }

                // Mark as handled to prevent crash
                e.Handled = true;
            };

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
            services.AddSingleton<IFractalParameterService, FractalParameterService>(); // Task 1: Flexible parameter system
            services.AddSingleton<IFractalMetadataService, FractalMetadataService>(); // Task 3: Metadata caching

            // Animation Services (Phase 1)
            services.AddSingleton<Services.Animation.FrameInterpolator>();
            services.AddSingleton<Services.Animation.AnimationRenderer>();
            services.AddSingleton<Services.Animation.AnimationService>();
            services.AddSingleton<Services.Animation.Export.IAnimationExporter, Services.Animation.Export.Mp4Exporter>();

            // ViewModels
            services.AddSingleton<MainViewModel>(); // Must be singleton to preserve app state
            services.AddSingleton<ViewModels.Browser.FractalBrowserViewModel>(); // Task 2: Fix DI pattern
            services.AddSingleton<ViewModels.Properties.RenderSettingsViewModel>(); // Week 9 Task 2: Deep zoom toggle
            services.AddSingleton<ViewModels.Properties.ColorEditorViewModel>(); // Color palette with persistence
            services.AddSingleton<AnimationViewModel>(); // Phase 1: Animation control - Singleton to preserve state across tab switches

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

            // Get settings service for theme configuration
            var settingsService = _serviceProvider.GetRequiredService<IAppSettingsService>();

            if (window.Content is not Frame rootFrame)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Apply saved theme BEFORE setting window content to avoid double-initialization
                var themeName = settingsService.GetTheme();

                // Handle custom themes
                if (themeName == "Ocean Blue")
                {
                    LoadCustomTheme("ms-appx:///Themes/OceanBlue.xaml");
                    rootFrame.RequestedTheme = ElementTheme.Light;
                }
                else
                {
                    rootFrame.RequestedTheme = ThemeNameToElementTheme(themeName);
                }

                window.Content = rootFrame;
            }

            _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);

            // Set window title
            window.Title = "ManpLab - Fractal Explorer";

            // Set title bar icon (WinUI 3 requires explicit API call)
            SetTitleBarIcon(window);

            // Subscribe to window close event to save navigation history
            window.Closed += OnWindowClosed;

            window.Activate();

            Log.Information("ManpWinUI window activated with theme: {Theme}", settingsService.GetTheme());

            // Initialize services (Tasks 1 & 3)
            _ = InitializeServicesAsync();
        }

        /// <summary>
        /// Initialize services that require async loading after app startup.
        /// Tasks 1 & 3: Parameter system and metadata caching.
        /// </summary>
        private async System.Threading.Tasks.Task InitializeServicesAsync()
        {
            try
            {
                // Task 3: Initialize metadata cache first (required for parameter service)
                var metadataService = _serviceProvider.GetService<IFractalMetadataService>();
                if (metadataService != null)
                {
                    await metadataService.InitializeAsync();
                    Log.Information("FractalMetadataService initialized with {Count} fractals", metadataService.Count);
                }

                // Task 1: Initialize parameter service
                var paramService = _serviceProvider.GetService<IFractalParameterService>();
                if (paramService != null)
                {
                    await paramService.InitializeAsync();
                    Log.Information("FractalParameterService initialized");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize services");
            }
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

        // ═══════════════════════════════════════════════════════════════════════════════
        // THEME SUPPORT
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Applies the saved theme preference to the application window.
        /// Supports custom themes (Ocean Blue) via dynamic resource dictionary loading.
        /// Preserves all application state including rendered fractals, panel visibility, and settings.
        /// Theme changes are purely visual and do not trigger navigation or state reset.
        /// </summary>
        public void ApplyTheme()
        {
            if (window == null)
                return;

            try
            {
                var settingsService = _serviceProvider.GetRequiredService<IAppSettingsService>();
                var themeName = settingsService.GetTheme();

                if (window.Content is Frame rootFrame)
                {
                    // Store current RequestedTheme to detect changes
                    var previousTheme = rootFrame.RequestedTheme;

                    // Handle custom themes (Ocean Blue)
                    if (themeName == "Ocean Blue")
                    {
                        // Unload any previous custom theme first
                        UnloadCustomTheme();

                        // Load custom theme resources
                        LoadCustomTheme("ms-appx:///Themes/OceanBlue.xaml");

                        // Use Light as base for custom themes
                        rootFrame.RequestedTheme = ElementTheme.Light;
                    }
                    else
                    {
                        // Unload any custom theme first
                        UnloadCustomTheme();

                        // Apply built-in theme
                        var elementTheme = ThemeNameToElementTheme(themeName);
                        rootFrame.RequestedTheme = elementTheme;
                    }

                    // Force theme refresh without destroying state
                    // Temporarily toggle to trigger re-evaluation, then set back
                    if (rootFrame.RequestedTheme == previousTheme)
                    {
                        // If theme enum didn't change (e.g., Light->OceanBlue both use Light),
                        // toggle to force refresh
                        var temp = rootFrame.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
                        rootFrame.RequestedTheme = temp;
                    }
                    rootFrame.RequestedTheme = themeName == "Ocean Blue" ? ElementTheme.Light : ThemeNameToElementTheme(themeName);

                    Log.Information("Applied theme: {Theme} (State preserved, no navigation)", themeName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme");
            }
        }

        /// <summary>
        /// Loads a custom theme dictionary from the specified URI.
        /// </summary>
        private void LoadCustomTheme(string uri)
        {
            try
            {
                // Unload previous custom theme if any
                UnloadCustomTheme();

                // Create and load new custom theme
                _customThemeDict = new ResourceDictionary { Source = new Uri(uri) };

                // Add at the END so it takes precedence (WinUI evaluates merged dictionaries in reverse order)
                Application.Current.Resources.MergedDictionaries.Add(_customThemeDict);

                Log.Information("Loaded custom theme from {Uri}", uri);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load custom theme from {Uri}", uri);
                _customThemeDict = null;
            }
        }

        /// <summary>
        /// Unloads the currently loaded custom theme dictionary.
        /// </summary>
        private void UnloadCustomTheme()
        {
            if (_customThemeDict != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_customThemeDict);
                _customThemeDict = null;
                Log.Information("Unloaded custom theme");
            }
        }

        /// <summary>
        /// Converts a theme name string to an ElementTheme enum value.
        /// For custom themes like Ocean Blue, we use Default and rely on custom ResourceDictionary.
        /// </summary>
        private static ElementTheme ThemeNameToElementTheme(string themeName)
        {
            return themeName switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                "Ocean Blue" => ElementTheme.Default, // Uses custom ResourceDictionary
                "System" => ElementTheme.Default,
                _ => ElementTheme.Default
            };
        }

        /// <summary>
        /// Sets the title bar icon for the window.
        /// WinUI 3 requires explicit AppWindow API calls to display title bar icons.
        /// </summary>
        private static void SetTitleBarIcon(Window window)
        {
            try
            {
                // Get the AppWindow for the Window
                var appWindow = GetAppWindowForWindow(window);
                if (appWindow != null)
                {
                    // Set the icon using the unplated 32x32 variant
                    var iconPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                        "Assets", "ManpLab-Square44x44Logo.targetsize-32_altform-unplated.png");

                    if (File.Exists(iconPath))
                    {
                        appWindow.SetIcon(iconPath);
                        Log.Information("Title bar icon set successfully: {IconPath}", iconPath);
                    }
                    else
                    {
                        Log.Warning("Title bar icon not found at {IconPath}", iconPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to set title bar icon");
            }
        }

        /// <summary>
        /// Gets the AppWindow for a WinUI 3 Window.
        /// </summary>
        private static Microsoft.UI.Windowing.AppWindow? GetAppWindowForWindow(Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        }
    }
}
