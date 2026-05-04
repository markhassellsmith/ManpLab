#nullable enable
using System;
using System.ComponentModel;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// Defines render modes for fractal visualization.
    /// Week 7 Task 1: Foundation for render mode selection.
    /// Week 7 Task 3: Will expand based on C++ engine capabilities.
    /// </summary>
    public enum RenderMode
    {
        EscapeTime,         // Standard iteration count coloring
        SmoothColoring,     // Continuous/smooth gradient coloring
        DistanceEstimation, // Edge detection/highlighting
        OrbitTrap           // Based on closest approach to trap shape
    }

    /// <summary>
    /// Defines antialiasing quality levels.
    /// Week 7 Task 4: Performance vs quality trade-off.
    /// </summary>
    public enum AntialiasingLevel
    {
        None,       // No antialiasing (fastest)
        MSAA2x,     // 2x multisampling
        MSAA4x,     // 4x multisampling
        MSAA8x      // 8x multisampling (highest quality)
    }

    /// <summary>
    /// ViewModel for render settings and quality options.
    /// Week 7 Task 1: Foundation setup for render configuration.
    /// </summary>
    public class RenderSettingsViewModel : INotifyPropertyChanged
    {
        private readonly ManpWinUI.Services.IAppSettingsService? _settingsService;
        private RenderMode _selectedRenderMode = RenderMode.EscapeTime;
        private AntialiasingLevel _antialiasingLevel = AntialiasingLevel.None;
        private bool _useDeepZoom = false;
        private bool _useSmoothColoring = false;
        private int _renderWidth = 1280;
        private int _renderHeight = 720;

        /// <summary>
        /// Currently selected render mode.
        /// Week 7 Task 3: Determines coloring algorithm.
        /// </summary>
        public RenderMode SelectedRenderMode
        {
            get => _selectedRenderMode;
            set
            {
                if (_selectedRenderMode != value)
                {
                    _selectedRenderMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRenderMode)));
                    RenderModeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Antialiasing quality level.
        /// Week 7 Task 4: Controls edge smoothness.
        /// </summary>
        public AntialiasingLevel AntialiasingLevel
        {
            get => _antialiasingLevel;
            set
            {
                if (_antialiasingLevel != value)
                {
                    _antialiasingLevel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AntialiasingLevel)));
                    RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Enable arbitrary precision for deep zoom.
        /// Week 7 Task 4: For extreme magnification levels.
        /// Week 9: Persisted setting restored at startup.
        /// </summary>
        public bool UseDeepZoom
        {
            get => _useDeepZoom;
            set
            {
                if (_useDeepZoom != value)
                {
                    _useDeepZoom = value;
                    _settingsService?.SetUseDeepZoom(value); // Persist immediately
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseDeepZoom)));
                    RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Enable smooth/continuous coloring.
        /// Week 7 Task 3: Alternative to standard escape-time banding.
        /// </summary>
        public bool UseSmoothColoring
        {
            get => _useSmoothColoring;
            set
            {
                if (_useSmoothColoring != value)
                {
                    _useSmoothColoring = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseSmoothColoring)));
                    RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Render output width in pixels.
        /// Week 7 Task 4: Resolution control.
        /// </summary>
        public int RenderWidth
        {
            get => _renderWidth;
            set
            {
                if (_renderWidth != value && value > 0)
                {
                    _renderWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderWidth)));
                    RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Render output height in pixels.
        /// Week 7 Task 4: Resolution control.
        /// </summary>
        public int RenderHeight
        {
            get => _renderHeight;
            set
            {
                if (_renderHeight != value && value > 0)
                {
                    _renderHeight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderHeight)));
                    RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Description of the current render mode.
        /// Week 7 Task 3: For UI tooltips and help text.
        /// </summary>
        public string RenderModeDescription => SelectedRenderMode switch
        {
            RenderMode.EscapeTime => "Standard iteration count coloring - fastest and most common",
            RenderMode.SmoothColoring => "Continuous gradient coloring - eliminates banding artifacts",
            RenderMode.DistanceEstimation => "Edge detection and highlighting - emphasizes fractal boundaries",
            RenderMode.OrbitTrap => "Colors based on orbit proximity to trap shapes - creative effects",
            _ => "Unknown render mode"
        };

        /// <summary>
        /// Description of the current antialiasing level.
        /// Week 7 Task 4: For UI tooltips.
        /// </summary>
        public string AntialiasingDescription => AntialiasingLevel switch
        {
            AntialiasingLevel.None => "No antialiasing - fastest performance",
            AntialiasingLevel.MSAA2x => "2x multisampling - slight quality improvement",
            AntialiasingLevel.MSAA4x => "4x multisampling - balanced quality and performance",
            AntialiasingLevel.MSAA8x => "8x multisampling - highest quality, slower",
            _ => "Unknown antialiasing level"
        };

        /// <summary>
        /// Event raised when render mode changes.
        /// Week 7 Task 3: Triggers re-render with new algorithm.
        /// </summary>
        public event EventHandler? RenderModeChanged;

        /// <summary>
        /// Event raised when render settings change.
        /// Week 7 Task 4: Triggers quality/resolution updates.
        /// </summary>
        public event EventHandler? RenderSettingsChanged;

        public RenderSettingsViewModel(ManpWinUI.Services.IAppSettingsService? settingsService = null)
        {
            _settingsService = settingsService;

            // Restore persisted deep zoom setting
            if (_settingsService != null)
            {
                _useDeepZoom = _settingsService.GetUseDeepZoom();
                System.Diagnostics.Debug.WriteLine($"[RenderSettingsViewModel] Restored UseDeepZoom: {_useDeepZoom}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[RenderSettingsViewModel] No settings service - using defaults");
            }
        }

        /// <summary>
        /// Reset all settings to default values.
        /// Week 7 Task 1: Basic reset functionality.
        /// </summary>
        public void ResetToDefaults()
        {
            SelectedRenderMode = RenderMode.EscapeTime;
            AntialiasingLevel = AntialiasingLevel.None;
            UseDeepZoom = false;
            UseSmoothColoring = false;
            RenderWidth = 1280;
            RenderHeight = 720;

            System.Diagnostics.Debug.WriteLine("[RenderSettingsViewModel] Reset to default settings");
        }

        /// <summary>
        /// Apply resolution preset.
        /// Week 7 Task 4: Quick resolution selection.
        /// </summary>
        public void ApplyResolutionPreset(string presetName)
        {
            switch (presetName.ToLower())
            {
                case "sd":
                    RenderWidth = 1280;
                    RenderHeight = 720;
                    break;
                case "hd":
                    RenderWidth = 1280;
                    RenderHeight = 720;
                    break;
                case "fullhd":
                    RenderWidth = 1920;
                    RenderHeight = 1080;
                    break;
                case "4k":
                    RenderWidth = 3840;
                    RenderHeight = 2160;
                    break;
                case "custom":
                    // Keep current values
                    break;
                default:
                    RenderWidth = 1280;
                    RenderHeight = 720;
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"[RenderSettingsViewModel] Applied resolution preset: {presetName} ({RenderWidth}x{RenderHeight})");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
