using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Models.Animation;
using ManpWinUI.Models.Parameters;
using ManpWinUI.Services.Animation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.ViewModels;

/// <summary>
/// ViewModel for animation control panel.
/// Manages animation settings, rendering, and export.
/// </summary>
public partial class AnimationViewModel : ObservableObject
{
    private readonly AnimationService _animationService;
    private readonly ILogger<AnimationViewModel> _logger;
    private MainViewModel? _mainViewModel; // Access to current fractal view (can be updated)
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isUpdatingZoomValues; // Prevent circular updates between zoom properties

    // Collections for ComboBox bindings
    public IEnumerable<AnimationType> AvailableAnimationTypes { get; } = Enum.GetValues<AnimationType>();
    public IEnumerable<EasingFunction> AvailableEasingFunctions { get; } = Enum.GetValues<EasingFunction>();
    public IEnumerable<ExportFormat> AvailableExportFormats { get; } = Enum.GetValues<ExportFormat>();
    public IEnumerable<ZoomSpeedPreset> AvailableZoomSpeedPresets { get; } = Enum.GetValues<ZoomSpeedPreset>();

    public AnimationViewModel(
        AnimationService animationService,
        ILogger<AnimationViewModel> logger)
    {
        _animationService = animationService;
        _logger = logger;

        // Load last used directory from app settings, or use Documents folder as default
        var lastDirectory = LoadLastUsedDirectory();
        var defaultFileName = $"ManpWinUI_Animation_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
        outputPath = System.IO.Path.Combine(lastDirectory, defaultFileName);

        _logger.LogDebug("AnimationViewModel initialized with output path: {OutputPath}", outputPath);
    }

    /// <summary>
    /// Sets or updates the MainViewModel reference.
    /// Called by the view when MainViewModel is available.
    /// </summary>
    public void SetMainViewModel(MainViewModel? mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _logger.LogDebug("MainViewModel reference {Status}", 
            mainViewModel != null ? "set" : "cleared");
    }

    /// <summary>
    /// Load the last used directory from app settings, or return Documents folder.
    /// </summary>
    private string LoadLastUsedDirectory()
    {
        try
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue("AnimationLastDirectory", out var value) && value is string lastDir)
            {
                // Verify directory still exists
                if (System.IO.Directory.Exists(lastDir))
                {
                    _logger.LogDebug("Loaded last used directory: {Directory}", lastDir);
                    return lastDir;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load last used directory from settings");
        }

        // Default to Documents folder
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    /// <summary>
    /// Save the directory from the current output path for next time.
    /// </summary>
    private void SaveLastUsedDirectory(string filePath)
    {
        try
        {
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && System.IO.Directory.Exists(directory))
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                settings.Values["AnimationLastDirectory"] = directory;
                _logger.LogDebug("Saved last used directory: {Directory}", directory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save last used directory to settings");
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ANIMATION TYPE SELECTION
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private AnimationType selectedAnimationType = AnimationType.Zoom;

    // ═══════════════════════════════════════════════════════════════════════════════
    // GENERAL SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private int frameCount = 90; // 3 seconds at 30fps (common short animation)

    [ObservableProperty]
    private int frameRate = 30; // Standard 30fps for smooth playback

    [ObservableProperty]
    private double durationSeconds = 3.0; // 3 seconds - good for preview/testing

    [ObservableProperty]
    private EasingFunction selectedEasing = EasingFunction.EaseInOutQuad; // Smooth acceleration/deceleration

    [ObservableProperty]
    private ExportFormat selectedExportFormat = ExportFormat.MP4; // Most universal format

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputPathStatus))]
    private string outputPath = ""; // Will be set by Browse button or default path

    /// <summary>
    /// Status message about the output path (shows if file exists and will be overwritten).
    /// </summary>
    public string OutputPathStatus
    {
        get
        {
            if (string.IsNullOrWhiteSpace(OutputPath))
                return "No output file selected";

            if (System.IO.File.Exists(OutputPath))
                return "⚠️ File exists and will be overwritten";

            return "✓ Ready to create new file";
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ZOOM ANIMATION SETTINGS (most common animation type)
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ZoomRatio), nameof(ZoomRatioDisplay))]
    private double startZoom = 1.0; // Default Mandelbrot view

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ZoomRatio), nameof(ZoomRatioDisplay))]
    private double endZoom = 1000.0; // Deep zoom for dramatic effect

    [ObservableProperty]
    private ZoomSpeedPreset selectedZoomSpeed = ZoomSpeedPreset.Medium; // Default to medium speed

    /// <summary>
    /// Computed zoom ratio (EndZoom / StartZoom).
    /// </summary>
    public double ZoomRatio => StartZoom > 0 ? EndZoom / StartZoom : 1.0;

    /// <summary>
    /// Human-readable zoom ratio display.
    /// </summary>
    public string ZoomRatioDisplay
    {
        get
        {
            var ratio = ZoomRatio;
            if (ratio >= 1_000_000)
                return $"{ratio / 1_000_000:F1}M×";
            if (ratio >= 1_000)
                return $"{ratio / 1_000:F1}K×";
            return $"{ratio:F1}×";
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PARAMETER ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private string parameterToAnimate = "power"; // More interesting than exponent

    [ObservableProperty]
    private double startParameterValue = 2.0; // Classic Mandelbrot power

    [ObservableProperty]
    private double endParameterValue = 5.0; // Good range for visual change

    // ═══════════════════════════════════════════════════════════════════════════════
    // PAN ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private double startCenterX = -0.5; // Default Mandelbrot center

    [ObservableProperty]
    private double startCenterY = 0.0;

    [ObservableProperty]
    private double endCenterX = -0.75; // Move toward interesting features

    [ObservableProperty]
    private double endCenterY = 0.1;

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDER STATE
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private bool isRendering;

    [ObservableProperty]
    private double renderProgress;

    [ObservableProperty]
    private string renderStatusMessage = "";

    [ObservableProperty]
    private string renderPhase = "";

    [ObservableProperty]
    private int currentFrame;

    [ObservableProperty]
    private int totalFrames;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCompletedFile))]
    private string? lastCompletedFilePath;

    /// <summary>
    /// True if a file was successfully rendered and can be opened.
    /// </summary>
    public bool HasCompletedFile => !string.IsNullOrWhiteSpace(LastCompletedFilePath) && 
                                     System.IO.File.Exists(LastCompletedFilePath);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalEstimatedTime))]
    private TimeSpan? estimatedTimeRemaining;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalEstimatedTime))]
    private TimeSpan elapsedTime;

    /// <summary>
    /// Total estimated time from start to finish (elapsed + remaining).
    /// </summary>
    public TimeSpan? TotalEstimatedTime
    {
        get
        {
            if (EstimatedTimeRemaining.HasValue)
            {
                return ElapsedTime + EstimatedTimeRemaining.Value;
            }
            return null;
        }
    }

    private System.Diagnostics.Stopwatch? _renderStopwatch;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PROPERTY CHANGED HANDLERS
    // ═══════════════════════════════════════════════════════════════════════════════

    partial void OnOutputPathChanged(string value)
    {
        // Save the directory for next time when user selects a file
        if (!string.IsNullOrWhiteSpace(value))
        {
            SaveLastUsedDirectory(value);
        }
    }

    partial void OnSelectedExportFormatChanged(ExportFormat value)
    {
        // Update file extension when export format changes
        if (!string.IsNullOrWhiteSpace(OutputPath))
        {
            var directory = System.IO.Path.GetDirectoryName(OutputPath);
            var fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(OutputPath);
            var newExtension = GetFileExtension(value);

            if (!string.IsNullOrEmpty(directory))
            {
                OutputPath = System.IO.Path.Combine(directory, $"{fileNameWithoutExt}.{newExtension}");
            }
            else
            {
                OutputPath = $"{fileNameWithoutExt}.{newExtension}";
            }
        }
    }

    partial void OnDurationSecondsChanged(double value)
    {
        // Update frame count when duration changes
        FrameCount = (int)Math.Ceiling(value * FrameRate);
    }

    partial void OnFrameRateChanged(int value)
    {
        // Update frame count when frame rate changes
        FrameCount = (int)Math.Ceiling(DurationSeconds * value);
    }

    partial void OnFrameCountChanged(int value)
    {
        // Update duration when frame count changes (manual override)
        if (FrameRate > 0)
        {
            DurationSeconds = (double)value / FrameRate;
        }
    }

    partial void OnStartZoomChanged(double value)
    {
        // When StartZoom changes, recalculate EndZoom to maintain the speed preset ratio
        // (unless we're already in the middle of an update, or preset is Custom)
        if (!_isUpdatingZoomValues && SelectedZoomSpeed != ZoomSpeedPreset.Custom)
        {
            _isUpdatingZoomValues = true;
            try
            {
                EndZoom = SelectedZoomSpeed switch
                {
                    ZoomSpeedPreset.VerySlow => value * 10,
                    ZoomSpeedPreset.Slow => value * 100,
                    ZoomSpeedPreset.Medium => value * 1000,
                    ZoomSpeedPreset.Fast => value * 10000,
                    ZoomSpeedPreset.VeryFast => value * 100000,
                    ZoomSpeedPreset.Extreme => value * 1000000,
                    _ => value * 1000
                };
            }
            finally
            {
                _isUpdatingZoomValues = false;
            }
        }
    }

    partial void OnEndZoomChanged(double value)
    {
        // When EndZoom is manually changed, switch to Custom preset
        // (unless we're already in the middle of an update)
        if (!_isUpdatingZoomValues && SelectedZoomSpeed != ZoomSpeedPreset.Custom)
        {
            // Calculate what preset this would correspond to
            double ratio = value / StartZoom;

            // Check if it matches a preset (with 10% tolerance)
            ZoomSpeedPreset matchingPreset = ratio switch
            {
                var r when Math.Abs(r - 10) < 1 => ZoomSpeedPreset.VerySlow,
                var r when Math.Abs(r - 100) < 10 => ZoomSpeedPreset.Slow,
                var r when Math.Abs(r - 1000) < 100 => ZoomSpeedPreset.Medium,
                var r when Math.Abs(r - 10000) < 1000 => ZoomSpeedPreset.Fast,
                var r when Math.Abs(r - 100000) < 10000 => ZoomSpeedPreset.VeryFast,
                var r when Math.Abs(r - 1000000) < 100000 => ZoomSpeedPreset.Extreme,
                _ => ZoomSpeedPreset.Custom
            };

            _isUpdatingZoomValues = true;
            try
            {
                SelectedZoomSpeed = matchingPreset;
            }
            finally
            {
                _isUpdatingZoomValues = false;
            }
        }
    }

    partial void OnSelectedZoomSpeedChanged(ZoomSpeedPreset value)
    {
        // Update EndZoom based on preset (keeping StartZoom constant)
        if (!_isUpdatingZoomValues)
        {
            _isUpdatingZoomValues = true;
            try
            {
                EndZoom = value switch
                {
                    ZoomSpeedPreset.VerySlow => StartZoom * 10,        // 10x zoom
                    ZoomSpeedPreset.Slow => StartZoom * 100,           // 100x zoom
                    ZoomSpeedPreset.Medium => StartZoom * 1000,        // 1000x zoom (original default)
                    ZoomSpeedPreset.Fast => StartZoom * 10000,         // 10,000x zoom
                    ZoomSpeedPreset.VeryFast => StartZoom * 100000,    // 100,000x zoom
                    ZoomSpeedPreset.Extreme => StartZoom * 1000000,    // 1,000,000x zoom
                    ZoomSpeedPreset.Custom => EndZoom,                 // Keep current value
                    _ => StartZoom * 1000
                };
            }
            finally
            {
                _isUpdatingZoomValues = false;
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    [RelayCommand(CanExecute = nameof(CanRenderAnimation))]
    private async Task RenderAnimationAsync()
    {
        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            _logger.LogWarning("Cannot render animation: output path not set");
            RenderStatusMessage = "Error: Please select an output file path";
            return;
        }

        try
        {
            IsRendering = true;
            RenderProgress = 0;
            RenderStatusMessage = "Preparing animation...";
            RenderPhase = "Starting";
            ElapsedTime = TimeSpan.Zero;

            _cancellationTokenSource = new CancellationTokenSource();
            _renderStopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Build animation settings from current properties
            var settings = BuildAnimationSettings();

            // Create progress reporter
            var progress = new Progress<AnimationProgress>(p =>
            {
                CurrentFrame = p.CurrentFrame;
                TotalFrames = p.TotalFrames;
                RenderProgress = p.ProgressPercentage;
                RenderPhase = p.Phase;
                RenderStatusMessage = p.StatusMessage ?? $"{p.Phase}: {p.CurrentFrame}/{p.TotalFrames}";
                EstimatedTimeRemaining = p.EstimatedTimeRemaining;

                // Update elapsed time
                if (_renderStopwatch != null)
                {
                    ElapsedTime = _renderStopwatch.Elapsed;
                }
            });

            _logger.LogInformation("Starting animation render: {Type}, {FrameCount} frames", 
                SelectedAnimationType, FrameCount);

            // Render and export animation
            var outputFile = await _animationService.CreateAnimationAsync(
                settings,
                progress,
                _cancellationTokenSource.Token);

            LastCompletedFilePath = outputFile;
            RenderStatusMessage = $"✓ Animation complete: {outputFile}";
            _logger.LogInformation("Animation render complete: {OutputFile}", outputFile);
        }
        catch (OperationCanceledException)
        {
            LastCompletedFilePath = null;
            RenderStatusMessage = "Animation cancelled";
            _logger.LogInformation("Animation render cancelled by user");
        }
        catch (Exception ex)
        {
            LastCompletedFilePath = null;
            RenderStatusMessage = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to render animation");
        }
        finally
        {
            IsRendering = false;
            _renderStopwatch?.Stop();
            _renderStopwatch = null;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancelAnimation))]
    private void CancelAnimation()
    {
        _cancellationTokenSource?.Cancel();
        _logger.LogInformation("Animation cancellation requested");
    }

    [RelayCommand(CanExecute = nameof(HasCompletedFile))]
    private async Task OpenCompletedFileAsync()
    {
        if (string.IsNullOrWhiteSpace(LastCompletedFilePath) || !System.IO.File.Exists(LastCompletedFilePath))
        {
            _logger.LogWarning("Cannot open file: path is invalid or file doesn't exist");
            return;
        }

        try
        {
            _logger.LogInformation("Opening completed animation file: {FilePath}", LastCompletedFilePath);

            // Use Windows.System.Launcher to open the file with the default associated app
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(LastCompletedFilePath);
            await Windows.System.Launcher.LaunchFileAsync(file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open file: {FilePath}", LastCompletedFilePath);
            RenderStatusMessage = $"Error opening file: {ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(HasCompletedFile))]
    private async Task ShowFileInExplorerAsync()
    {
        if (string.IsNullOrWhiteSpace(LastCompletedFilePath) || !System.IO.File.Exists(LastCompletedFilePath))
        {
            _logger.LogWarning("Cannot show file: path is invalid or file doesn't exist");
            return;
        }

        try
        {
            _logger.LogInformation("Showing file in Explorer: {FilePath}", LastCompletedFilePath);

            // Use Windows.System.Launcher to show the file in Explorer
            var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(
                System.IO.Path.GetDirectoryName(LastCompletedFilePath)!);
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show file in Explorer: {FilePath}", LastCompletedFilePath);
            RenderStatusMessage = $"Error opening Explorer: {ex.Message}";
        }
    }

    private bool CanRenderAnimation() => !IsRendering;

    private bool CanCancelAnimation() => IsRendering;

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Build RenderParameters from MainViewModel's current state.
    /// This captures all fractal settings, colors, and view parameters.
    /// </summary>
    private RenderParameters BuildRenderParametersFromMainView()
    {
        if (_mainViewModel == null)
        {
            _logger.LogWarning("MainViewModel not available, using default parameters");
            return new RenderParameters
            {
                FractalType = "Mandelbrot",
                CenterX = StartCenterX,
                CenterY = StartCenterY,
                Zoom = StartZoom,
                Width = 1920,
                Height = 1080,
                MaxIterations = 512,
                Palette = "Classic"
            };
        }

        // Get current fractal parameters from MainViewModel
        var baseParams = new RenderParameters
        {
            FractalType = _mainViewModel.SelectedFractalType,
            CenterX = _mainViewModel.CenterX,
            CenterY = _mainViewModel.CenterY,
            Zoom = _mainViewModel.Zoom,
            Width = 1920, // Animation resolution (could be made configurable)
            Height = 1080,
            MaxIterations = _mainViewModel.MaxIterations,

            // Julia mode settings
            IsJuliaMode = _mainViewModel.IsJuliaMode,
            JuliaCReal = _mainViewModel.JuliaCX,
            JuliaCImaginary = _mainViewModel.JuliaCY,

            // Color settings
            Palette = _mainViewModel.SelectedPalette,
            ColorCycleSpeed = _mainViewModel.ColorCycleSpeed,
            ColorOffset = _mainViewModel.ColorOffset,
            UseSmoothColoring = _mainViewModel.UseSmoothColoring,

            // Escape radius
            EscapeRadius = 2.0 // Standard default
        };

        // Copy any extended parameters from the parameter system
        if (_mainViewModel.CurrentParameters != null)
        {
            foreach (var param in _mainViewModel.CurrentParameters.Parameters)
            {
                var value = _mainViewModel.CurrentParameters.GetValue(param.Key);

                if (value == null) continue;

                if (param.Type == ParameterType.Double && value is double doubleValue)
                {
                    baseParams.SetExtended(param.Key, doubleValue);
                }
                else if (param.Type == ParameterType.Integer && value is int intValue)
                {
                    baseParams.SetExtended(param.Key, (double)intValue);
                }
                else if (param.Type == ParameterType.Boolean && value is bool boolValue)
                {
                    baseParams.SetExtended(param.Key, boolValue ? 1.0 : 0.0);
                }
            }
        }

        _logger.LogInformation(
            "Built render parameters: {FractalType} at ({CenterX}, {CenterY}) zoom {Zoom}x, {MaxIter} iterations",
            baseParams.FractalType, baseParams.CenterX, baseParams.CenterY, baseParams.Zoom, baseParams.MaxIterations);

        return baseParams;
    }

    /// <summary>
    /// Build AnimationSettings from current ViewModel properties.
    /// Integrates with MainViewModel to use current fractal view and settings.
    /// </summary>
    private AnimationSettings BuildAnimationSettings()
    {
        // Get current fractal parameters from MainViewModel (or defaults if not available)
        var baseParams = BuildRenderParametersFromMainView();

        var settings = new AnimationSettings
        {
            Type = SelectedAnimationType,
            FrameCount = FrameCount,
            FrameRate = FrameRate,
            Easing = SelectedEasing,
            ExportFormat = SelectedExportFormat,
            OutputPath = OutputPath,
            BaseParameters = baseParams
        };

        // Configure type-specific settings
        switch (SelectedAnimationType)
        {
            case AnimationType.Zoom:
                settings.StartParameters = baseParams.With(zoom: StartZoom);
                settings.EndParameters = baseParams.With(zoom: EndZoom);
                settings.ZoomSettings = new ZoomAnimationSettings
                {
                    StartZoom = StartZoom,
                    EndZoom = EndZoom
                };
                break;

            case AnimationType.Parameter:
                settings.StartParameters = baseParams;
                settings.EndParameters = baseParams;
                // Set extended parameter
                settings.StartParameters.SetExtended(ParameterToAnimate, StartParameterValue);
                settings.EndParameters.SetExtended(ParameterToAnimate, EndParameterValue);
                settings.ParameterSettings = new ParameterAnimationSettings
                {
                    ParameterName = ParameterToAnimate,
                    StartValue = StartParameterValue,
                    EndValue = EndParameterValue
                };
                break;

            case AnimationType.Pan:
                // Use current view center as start position if MainViewModel is available
                var startX = _mainViewModel?.CenterX ?? StartCenterX;
                var startY = _mainViewModel?.CenterY ?? StartCenterY;

                settings.StartParameters = baseParams.With(centerX: startX, centerY: startY);
                settings.EndParameters = baseParams.With(centerX: EndCenterX, centerY: EndCenterY);
                settings.PanSettings = new PanAnimationSettings
                {
                    StartCenter = (startX, startY),
                    EndCenter = (EndCenterX, EndCenterY),
                    LockZoom = true
                };
                break;

            default:
                throw new NotSupportedException($"Animation type {SelectedAnimationType} not yet implemented");
        }

        return settings;
    }

    /// <summary>
    /// Browse for output file path.
    /// This will be called from the UI's Browse button.
    /// </summary>
    public async Task BrowseOutputPathAsync()
    {
        // TODO: Implement file picker dialog in Task 1.5
        // For now, generate a default path
        var defaultPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"ManpWinUI_Animation_{DateTime.Now:yyyyMMdd_HHmmss}.{GetFileExtension(SelectedExportFormat)}");

        OutputPath = defaultPath;
        _logger.LogDebug("Default output path set: {OutputPath}", OutputPath);
    }

    private string GetFileExtension(ExportFormat format) => format switch
    {
        ExportFormat.MP4 => "mp4",
        ExportFormat.GIF => "gif",
        ExportFormat.PNGSequence => "zip", // Will contain PNG sequence
        ExportFormat.WebM => "webm",
        _ => "mp4"
    };
}
