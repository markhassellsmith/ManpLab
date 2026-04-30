#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// Defines available color palette types for fractal rendering.
    /// Week 7 Task 1: Foundation for palette selection.
    /// Week 7 Task 2: Aligned with ManpCore.Native ColorPalette enum.
    /// Week 7 Task 3: Added Spectrum palette.
    /// </summary>
    public enum PaletteType
    {
        Grayscale,      // Black to white
        Classic,        // Traditional blue/cyan fractal colors
        Fire,           // Red/orange/yellow gradient
        Ocean,          // Deep blue to turquoise
        Rainbow,        // Full spectrum colors
        Psychedelic,    // Vibrant, high-contrast colors
        Spectrum        // Pure HSV color wheel (S=100%, L=50%)
    }

    /// <summary>
    /// Represents a color palette preset with preview information.
    /// Week 7 Task 1: Individual palette item for UI binding.
    /// </summary>
    public class PaletteItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>
        /// Name of the palette.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type identifier for the palette.
        /// </summary>
        public PaletteType Type { get; set; }

        /// <summary>
        /// Description of the color scheme.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether this palette is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// Preview colors for this palette (for UI display).
        /// </summary>
        public ObservableCollection<string> PreviewColors { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// ViewModel for the color editor panel.
    /// Week 7 Task 1: Foundation setup for palette selection and color customization.
    /// </summary>
    public class ColorEditorViewModel : INotifyPropertyChanged
    {
        private PaletteItem? _selectedPalette;
        private int _colorCycleSpeed = 50;
        private int _colorOffset = 0;

        /// <summary>
        /// Collection of available color palettes.
        /// </summary>
        public ObservableCollection<PaletteItem> Palettes { get; } = new();

        /// <summary>
        /// Currently selected color palette.
        /// Week 7 Task 2: Will trigger palette application to render.
        /// </summary>
        public PaletteItem? SelectedPalette
        {
            get => _selectedPalette;
            set
            {
                if (_selectedPalette != value)
                {
                    // Deselect previous
                    if (_selectedPalette != null)
                        _selectedPalette.IsSelected = false;

                    _selectedPalette = value;

                    // Select new
                    if (_selectedPalette != null)
                        _selectedPalette.IsSelected = true;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPalette)));

                    // Week 7 Task 2: Will notify for re-render with new palette
                    PaletteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Color cycle speed (0-100, affects how quickly colors transition).
        /// Week 7 Task 2: For dynamic color animation effects.
        /// </summary>
        public int ColorCycleSpeed
        {
            get => _colorCycleSpeed;
            set
            {
                if (_colorCycleSpeed != value)
                {
                    _colorCycleSpeed = Math.Clamp(value, 0, 100);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorCycleSpeed)));
                    ColorSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Color offset (0-360, rotates the color palette).
        /// Week 7 Task 2: For palette rotation effects.
        /// </summary>
        public int ColorOffset
        {
            get => _colorOffset;
            set
            {
                if (_colorOffset != value)
                {
                    _colorOffset = Math.Clamp(value, 0, 360);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorOffset)));
                    ColorSettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Event raised when the palette selection changes.
        /// Week 7 Task 2: MainPage will subscribe to trigger re-render.
        /// </summary>
        public event EventHandler? PaletteChanged;

        /// <summary>
        /// Event raised when color settings (speed, offset) change.
        /// Week 7 Task 2: For real-time color adjustments.
        /// </summary>
        public event EventHandler? ColorSettingsChanged;

        public ColorEditorViewModel()
        {
            LoadDefaultPalettes();
        }

        /// <summary>
        /// Load default color palette presets.
        /// Week 7 Task 1: Initialize with built-in palettes.
        /// Week 7 Task 2: Aligned with ManpCore.Native palettes.
        /// </summary>
        private void LoadDefaultPalettes()
        {
            Palettes.Clear();

            // Grayscale palette
            var grayscale = new PaletteItem
            {
                Name = "Grayscale",
                Type = PaletteType.Grayscale,
                Description = "Black to white monochrome"
            };
            grayscale.PreviewColors.Add("#000000"); // Black
            grayscale.PreviewColors.Add("#404040"); // Dark gray
            grayscale.PreviewColors.Add("#808080"); // Gray
            grayscale.PreviewColors.Add("#C0C0C0"); // Light gray
            grayscale.PreviewColors.Add("#FFFFFF"); // White
            Palettes.Add(grayscale);

            // Classic fractal palette (default)
            var classic = new PaletteItem
            {
                Name = "Classic",
                Type = PaletteType.Classic,
                Description = "Traditional blue and cyan fractal colors",
                IsSelected = true
            };
            classic.PreviewColors.Add("#000040"); // Dark blue
            classic.PreviewColors.Add("#0000FF"); // Blue
            classic.PreviewColors.Add("#0080FF"); // Light blue
            classic.PreviewColors.Add("#00FFFF"); // Cyan
            classic.PreviewColors.Add("#80FFFF"); // Light cyan
            classic.PreviewColors.Add("#FFFFFF"); // White
            Palettes.Add(classic);

            // Fire palette
            var fire = new PaletteItem
            {
                Name = "Fire",
                Type = PaletteType.Fire,
                Description = "Warm red, orange, and yellow gradient"
            };
            fire.PreviewColors.Add("#000000"); // Black
            fire.PreviewColors.Add("#8B0000"); // Dark red
            fire.PreviewColors.Add("#FF4500"); // Orange red
            fire.PreviewColors.Add("#FFA500"); // Orange
            fire.PreviewColors.Add("#FFFF00"); // Yellow
            fire.PreviewColors.Add("#FFFFFF"); // White
            Palettes.Add(fire);

            // Ocean palette
            var ocean = new PaletteItem
            {
                Name = "Ocean",
                Type = PaletteType.Ocean,
                Description = "Deep sea to tropical waters"
            };
            ocean.PreviewColors.Add("#001a33"); // Deep ocean
            ocean.PreviewColors.Add("#003366"); // Dark blue
            ocean.PreviewColors.Add("#0066CC"); // Ocean blue
            ocean.PreviewColors.Add("#00CED1"); // Turquoise
            ocean.PreviewColors.Add("#7FFFD4"); // Aquamarine
            ocean.PreviewColors.Add("#E0FFFF"); // Light cyan
            Palettes.Add(ocean);

            // Rainbow palette
            var rainbow = new PaletteItem
            {
                Name = "Rainbow",
                Type = PaletteType.Rainbow,
                Description = "Full spectrum from red to violet"
            };
            rainbow.PreviewColors.Add("#FF0000"); // Red
            rainbow.PreviewColors.Add("#FF7F00"); // Orange
            rainbow.PreviewColors.Add("#FFFF00"); // Yellow
            rainbow.PreviewColors.Add("#00FF00"); // Green
            rainbow.PreviewColors.Add("#0000FF"); // Blue
            rainbow.PreviewColors.Add("#8B00FF"); // Violet
            Palettes.Add(rainbow);

            // Psychedelic palette
            var psychedelic = new PaletteItem
            {
                Name = "Psychedelic",
                Type = PaletteType.Psychedelic,
                Description = "Vibrant, high-contrast colors"
            };
            psychedelic.PreviewColors.Add("#FF00FF"); // Magenta
            psychedelic.PreviewColors.Add("#00FF00"); // Green
            psychedelic.PreviewColors.Add("#FFFF00"); // Yellow
            psychedelic.PreviewColors.Add("#00FFFF"); // Cyan
            psychedelic.PreviewColors.Add("#FF0000"); // Red
            psychedelic.PreviewColors.Add("#0000FF"); // Blue
            Palettes.Add(psychedelic);

            // Spectrum palette (Week 7 Task 3 enhancement)
            var spectrum = new PaletteItem
            {
                Name = "Spectrum",
                Type = PaletteType.Spectrum,
                Description = "Pure HSV color wheel at full saturation"
            };
            spectrum.PreviewColors.Add("#FF0000"); // Red (0°)
            spectrum.PreviewColors.Add("#FFFF00"); // Yellow (60°)
            spectrum.PreviewColors.Add("#00FF00"); // Green (120°)
            spectrum.PreviewColors.Add("#00FFFF"); // Cyan (180°)
            spectrum.PreviewColors.Add("#0000FF"); // Blue (240°)
            spectrum.PreviewColors.Add("#FF00FF"); // Magenta (300°)
            Palettes.Add(spectrum);

            // Set Classic as default selection
            _selectedPalette = classic;

            System.Diagnostics.Debug.WriteLine($"[ColorEditorViewModel] Loaded {Palettes.Count} default palettes (aligned with ManpCore.Native)");
        }

        /// <summary>
        /// Reset color settings to defaults.
        /// Week 7 Task 1: Basic reset functionality.
        /// </summary>
        public void ResetToDefaults()
        {
            ColorCycleSpeed = 50;
            ColorOffset = 0;
            SelectedPalette = Palettes.Count > 0 ? Palettes[0] : null;

            System.Diagnostics.Debug.WriteLine("[ColorEditorViewModel] Reset to default settings");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
