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
        private bool _useSmoothColoring = true;  // Default to anti-banding ON (smooth)

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
                    _settingsService?.SetUseSmoothColoring(value); // Persist immediately
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseSmoothColoring)));
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
            RenderMode.SmoothColoring => "Continuous gradient algorithm - smooth color transitions without discrete bands",
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

            // Restore persisted settings
            if (_settingsService != null)
            {
                _useDeepZoom = _settingsService.GetUseDeepZoom();
                _useSmoothColoring = _settingsService.GetUseSmoothColoring();
                System.Diagnostics.Debug.WriteLine($"[RenderSettingsViewModel] Restored UseDeepZoom: {_useDeepZoom}, UseSmoothColoring: {_useSmoothColoring}");
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

            System.Diagnostics.Debug.WriteLine("[RenderSettingsViewModel] Reset to default settings");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
