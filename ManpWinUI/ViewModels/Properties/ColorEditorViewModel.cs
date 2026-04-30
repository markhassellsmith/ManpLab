#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// Defines available color palette types for fractal rendering.
    /// Week 7 Task 1: Foundation for palette selection.
    /// </summary>
    public enum PaletteType
    {
        Classic,        // Traditional rainbow/spectrum
        Fire,           // Red/orange/yellow gradient
        Ice,            // Blue/cyan/white gradient
        Grayscale,      // Black to white
        Ocean,          // Deep blue to turquoise
        Sunset,         // Purple/pink/orange
        Forest,         // Green/brown earth tones
        Custom          // User-defined gradient
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
        /// Week 7 Task 2: Will expand with actual color data.
        /// </summary>
        private void LoadDefaultPalettes()
        {
            Palettes.Clear();

            // Classic rainbow palette
            var classic = new PaletteItem
            {
                Name = "Classic",
                Type = PaletteType.Classic,
                Description = "Traditional rainbow spectrum",
                IsSelected = true
            };
            classic.PreviewColors.Add("#FF0000"); // Red
            classic.PreviewColors.Add("#FFFF00"); // Yellow
            classic.PreviewColors.Add("#00FF00"); // Green
            classic.PreviewColors.Add("#00FFFF"); // Cyan
            classic.PreviewColors.Add("#0000FF"); // Blue
            classic.PreviewColors.Add("#FF00FF"); // Magenta
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

            // Ice palette
            var ice = new PaletteItem
            {
                Name = "Ice",
                Type = PaletteType.Ice,
                Description = "Cool blue and cyan gradient"
            };
            ice.PreviewColors.Add("#000033"); // Dark blue
            ice.PreviewColors.Add("#000080"); // Navy
            ice.PreviewColors.Add("#0000FF"); // Blue
            ice.PreviewColors.Add("#00BFFF"); // Deep sky blue
            ice.PreviewColors.Add("#87CEEB"); // Sky blue
            ice.PreviewColors.Add("#FFFFFF"); // White
            Palettes.Add(ice);

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

            // Sunset palette
            var sunset = new PaletteItem
            {
                Name = "Sunset",
                Type = PaletteType.Sunset,
                Description = "Purple, pink, and orange twilight"
            };
            sunset.PreviewColors.Add("#191970"); // Midnight blue
            sunset.PreviewColors.Add("#4B0082"); // Indigo
            sunset.PreviewColors.Add("#8B008B"); // Dark magenta
            sunset.PreviewColors.Add("#FF1493"); // Deep pink
            sunset.PreviewColors.Add("#FF6347"); // Tomato
            sunset.PreviewColors.Add("#FFD700"); // Gold
            Palettes.Add(sunset);

            // Forest palette
            var forest = new PaletteItem
            {
                Name = "Forest",
                Type = PaletteType.Forest,
                Description = "Earth tones and greenery"
            };
            forest.PreviewColors.Add("#1a1a0d"); // Dark brown
            forest.PreviewColors.Add("#2F4F2F"); // Dark olive green
            forest.PreviewColors.Add("#228B22"); // Forest green
            forest.PreviewColors.Add("#32CD32"); // Lime green
            forest.PreviewColors.Add("#90EE90"); // Light green
            forest.PreviewColors.Add("#F0E68C"); // Khaki
            Palettes.Add(forest);

            // Set Classic as default selection
            _selectedPalette = classic;

            System.Diagnostics.Debug.WriteLine($"[ColorEditorViewModel] Loaded {Palettes.Count} default palettes");
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
