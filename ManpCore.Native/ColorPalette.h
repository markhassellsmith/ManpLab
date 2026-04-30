#pragma once

#include <cmath>

// Native C++ color palette system for fractal coloring
// Provides various classic fractal color schemes

namespace Native {

    // RGB color structure
    struct ColorRGB
    {
        unsigned char r;
        unsigned char g;
        unsigned char b;

        ColorRGB() : r(0), g(0), b(0) {}
        ColorRGB(unsigned char red, unsigned char green, unsigned char blue)
            : r(red), g(green), b(blue) {}
    };

    // Color palette types
    enum class PaletteType
    {
        Grayscale,      // Simple black to white
        Classic,        // Classic blue/cyan fractal colors
        Fire,           // Hot colors: black -> red -> yellow -> white
        Ocean,          // Cool colors: deep blue -> cyan -> white
        Rainbow,        // Full spectrum
        Psychedelic,    // Vibrant, high-contrast colors
        Spectrum        // Pure HSV spectrum at full saturation (S=100%, L=50%)
    };

    /// <summary>
    /// Color palette generator for fractal coloring
    /// </summary>
    class ColorPalette
    {
    public:
        /// <summary>
        /// Generate color based on smooth iteration value
        /// </summary>
        /// <param name="iteration">Smooth iteration count (0.0 to maxIter)</param>
        /// <param name="maxIter">Maximum iterations</param>
        /// <param name="palette">Color palette to use</param>
        /// <returns>RGB color</returns>
        static ColorRGB GetColor(double iteration, int maxIter, PaletteType palette)
        {
            // Points in the set are always black
            if (iteration >= maxIter)
                return ColorRGB(0, 0, 0);

            // Normalize iteration to 0.0 - 1.0
            double t = iteration / maxIter;

            switch (palette)
            {
            case PaletteType::Grayscale:
                return GetGrayscaleColor(t);
            
            case PaletteType::Classic:
                return GetClassicColor(t);
            
            case PaletteType::Fire:
                return GetFireColor(t);
            
            case PaletteType::Ocean:
                return GetOceanColor(t);
            
            case PaletteType::Rainbow:
                return GetRainbowColor(t);
            
            case PaletteType::Psychedelic:
                return GetPsychedelicColor(t);

            case PaletteType::Spectrum:
                return GetSpectrumColor(t);

            default:
                return GetGrayscaleColor(t);
            }
        }

    private:
        // Grayscale palette (our current implementation)
        static ColorRGB GetGrayscaleColor(double t)
        {
            int gray = (int)(t * 255.0);
            return ColorRGB(gray, gray, gray);
        }

        // Classic fractal palette (blue to cyan to white)
        static ColorRGB GetClassicColor(double t)
        {
            // Create smooth gradient through classic fractal colors
            t = t * 3.0; // Stretch range

            if (t < 1.0)
            {
                // Dark blue to bright blue
                int b = (int)(64 + t * 191);
                return ColorRGB(0, 0, b);
            }
            else if (t < 2.0)
            {
                // Bright blue to cyan
                t = t - 1.0;
                int g = (int)(t * 255);
                return ColorRGB(0, g, 255);
            }
            else
            {
                // Cyan to white
                t = t - 2.0;
                int r = (int)(t * 255);
                return ColorRGB(r, 255, 255);
            }
        }

        // Fire palette (black -> red -> orange -> yellow -> white)
        static ColorRGB GetFireColor(double t)
        {
            t = t * 4.0;

            if (t < 1.0)
            {
                // Black to red
                int r = (int)(t * 255);
                return ColorRGB(r, 0, 0);
            }
            else if (t < 2.0)
            {
                // Red to orange
                t = t - 1.0;
                int g = (int)(t * 165);
                return ColorRGB(255, g, 0);
            }
            else if (t < 3.0)
            {
                // Orange to yellow
                t = t - 2.0;
                int g = (int)(165 + t * 90);
                return ColorRGB(255, g, 0);
            }
            else
            {
                // Yellow to white
                t = t - 3.0;
                int b = (int)(t * 255);
                return ColorRGB(255, 255, b);
            }
        }

        // Ocean palette (deep blue -> cyan -> white)
        static ColorRGB GetOceanColor(double t)
        {
            t = t * 3.0;

            if (t < 1.0)
            {
                // Deep blue to medium blue
                int r = 0;
                int g = (int)(t * 128);
                int b = (int)(128 + t * 127);
                return ColorRGB(r, g, b);
            }
            else if (t < 2.0)
            {
                // Medium blue to cyan
                t = t - 1.0;
                int g = (int)(128 + t * 127);
                return ColorRGB(0, g, 255);
            }
            else
            {
                // Cyan to white
                t = t - 2.0;
                int r = (int)(t * 255);
                int b = (int)(255 - t * 128);
                return ColorRGB(r, 255, b);
            }
        }

        // Rainbow palette (full spectrum)
        static ColorRGB GetRainbowColor(double t)
        {
            // Use HSV to RGB conversion for smooth rainbow
            // H varies from 0 to 360, S=1, V=1
            double h = t * 360.0;
            double s = 1.0;
            double v = 1.0;

            return HSVtoRGB(h, s, v);
        }

        // Psychedelic palette (high contrast, vibrant)
        static ColorRGB GetPsychedelicColor(double t)
        {
            // Use multiple sine waves for psychedelic effect
            double r = (sin(t * 8.0 * 3.14159) + 1.0) * 127.5;
            double g = (sin(t * 11.0 * 3.14159 + 2.0) + 1.0) * 127.5;
            double b = (sin(t * 13.0 * 3.14159 + 4.0) + 1.0) * 127.5;

            return ColorRGB((unsigned char)r, (unsigned char)g, (unsigned char)b);
        }

        // Spectrum palette (pure HSV wheel at S=100%, L=50%)
        // Matches Spectrum360 progression: smooth hue rotation through full color wheel
        static ColorRGB GetSpectrumColor(double t)
        {
            // Map iteration to full 360° hue rotation
            // Using HSV with fixed S=1.0 (100% saturation), V=1.0 (50% lightness equivalent)
            double hue = t * 360.0;
            return HSVtoRGB(hue, 1.0, 1.0);
        }

        // HSV to RGB conversion helper
        static ColorRGB HSVtoRGB(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1.0 - fabs(fmod(h / 60.0, 2.0) - 1.0));
            double m = v - c;

            double r1, g1, b1;

            if (h < 60.0)
            {
                r1 = c; g1 = x; b1 = 0;
            }
            else if (h < 120.0)
            {
                r1 = x; g1 = c; b1 = 0;
            }
            else if (h < 180.0)
            {
                r1 = 0; g1 = c; b1 = x;
            }
            else if (h < 240.0)
            {
                r1 = 0; g1 = x; b1 = c;
            }
            else if (h < 300.0)
            {
                r1 = x; g1 = 0; b1 = c;
            }
            else
            {
                r1 = c; g1 = 0; b1 = x;
            }

            unsigned char r = (unsigned char)((r1 + m) * 255.0);
            unsigned char g = (unsigned char)((g1 + m) * 255.0);
            unsigned char b = (unsigned char)((b1 + m) * 255.0);

            return ColorRGB(r, g, b);
        }
    };

} // namespace Native
