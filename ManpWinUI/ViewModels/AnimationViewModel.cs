using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManpWinUI.Models.Animation;
using ManpWinUI.Models.Parameters;
using ManpWinUI.Services.Animation;
using Microsoft.Extensions.Logging;
using System;
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
    private CancellationTokenSource? _cancellationTokenSource;

    public AnimationViewModel(
        AnimationService animationService,
        ILogger<AnimationViewModel> logger)
    {
        _animationService = animationService;
        _logger = logger;
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
    private int frameCount = 150; // 5 seconds at 30fps

    [ObservableProperty]
    private int frameRate = 30;

    [ObservableProperty]
    private double durationSeconds = 5.0;

    [ObservableProperty]
    private EasingFunction selectedEasing = EasingFunction.Linear;

    [ObservableProperty]
    private ExportFormat selectedExportFormat = ExportFormat.MP4;

    [ObservableProperty]
    private string outputPath = "";

    // ═══════════════════════════════════════════════════════════════════════════════
    // ZOOM ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private double startZoom = 1.0;

    [ObservableProperty]
    private double endZoom = 100.0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PARAMETER ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private string parameterToAnimate = "exponent";

    [ObservableProperty]
    private double startParameterValue = 2.0;

    [ObservableProperty]
    private double endParameterValue = 4.0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PAN ANIMATION SETTINGS
    // ═══════════════════════════════════════════════════════════════════════════════

    [ObservableProperty]
    private double startCenterX = -0.5;

    [ObservableProperty]
    private double startCenterY = 0.0;

    [ObservableProperty]
    private double endCenterX = -0.7;

    [ObservableProperty]
    private double endCenterY = 0.3;

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
    private TimeSpan? estimatedTimeRemaining;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PROPERTY CHANGED HANDLERS
    // ═══════════════════════════════════════════════════════════════════════════════

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

            _cancellationTokenSource = new CancellationTokenSource();

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
            });

            _logger.LogInformation("Starting animation render: {Type}, {FrameCount} frames", 
                SelectedAnimationType, FrameCount);

            // Render and export animation
            var outputFile = await _animationService.CreateAnimationAsync(
                settings,
                progress,
                _cancellationTokenSource.Token);

            RenderStatusMessage = $"Animation complete: {outputFile}";
            _logger.LogInformation("Animation render complete: {OutputFile}", outputFile);
        }
        catch (OperationCanceledException)
        {
            RenderStatusMessage = "Animation cancelled";
            _logger.LogInformation("Animation render cancelled by user");
        }
        catch (Exception ex)
        {
            RenderStatusMessage = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to render animation");
        }
        finally
        {
            IsRendering = false;
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

    private bool CanRenderAnimation() => !IsRendering;

    private bool CanCancelAnimation() => IsRendering;

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Build AnimationSettings from current ViewModel properties.
    /// This will be enhanced in future tasks to get current fractal parameters from MainViewModel.
    /// </summary>
    private AnimationSettings BuildAnimationSettings()
    {
        // TODO: In Task 1.6, integrate with MainViewModel to get current RenderParameters
        // For now, create default parameters

        var baseParams = new RenderParameters
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
                settings.StartParameters = baseParams.With(centerX: StartCenterX, centerY: StartCenterY);
                settings.EndParameters = baseParams.With(centerX: EndCenterX, centerY: EndCenterY);
                settings.PanSettings = new PanAnimationSettings
                {
                    StartCenter = (StartCenterX, StartCenterY),
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
