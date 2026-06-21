using System.Collections.Generic;

namespace ManpWinUI.Models.LSystem;

/// <summary>
/// Library of classic L-System fractals with predefined rules and parameters.
/// Based on historical fractals from Lindenmayer, Prusinkiewicz, and others.
/// </summary>
public static class LSystemPresets
{
    /// <summary>
    /// Get all available L-System presets.
    /// </summary>
    public static List<LSystemDefinition> GetAllPresets()
    {
        return new List<LSystemDefinition>
        {
            KochSnowflake(),
            DragonCurve(),
            SierpinskiTriangle(),
            HilbertCurve(),
            FractalPlant(),
            KochCurve(),
            PeanoCurve()
        };
    }

    /// <summary>
    /// Koch Snowflake: Classic 60° triangular fractal.
    /// Rule: F → F+F--F+F
    /// Angle: 60°
    /// Axiom: F++F++F (equilateral triangle start)
    /// </summary>
    public static LSystemDefinition KochSnowflake()
    {
        return new LSystemDefinition
        {
            Name = "Koch Snowflake",
            Axiom = "F++F++F",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'F', Successor = "F+F--F+F" }
            },
            TurnAngle = 60.0,
            DefaultGenerations = 4,
            MaxGenerations = 7,
            InitialHeading = 0.0,
            Category = "Classic",
            Description = "Helge von Koch's 1904 continuous but nowhere-differentiable curve. One of the first fractal curves ever described."
        };
    }

    /// <summary>
    /// Dragon Curve: Space-filling dragon discovered by Heighway, Harter, and Davis.
    /// Rules: F → F+G, G → F-G
    /// Angle: 90°
    /// Axiom: F
    /// </summary>
    public static LSystemDefinition DragonCurve()
    {
        return new LSystemDefinition
        {
            Name = "Dragon Curve",
            Axiom = "F",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'F', Successor = "F+G" },
                new LSystemRule { Predecessor = 'G', Successor = "F-G" }
            },
            TurnAngle = 90.0,
            DefaultGenerations = 10,
            MaxGenerations = 16,
            InitialHeading = 0.0,
            Category = "Classic",
            Description = "Heighway dragon discovered in 1967. Self-similar space-filling curve that tiles the plane."
        };
    }

    /// <summary>
    /// Sierpinski Triangle: Arrowhead variant using L-System grammar.
    /// Rules: F → G-F-G, G → F+G+F
    /// Angle: 60°
    /// Axiom: F
    /// </summary>
    public static LSystemDefinition SierpinskiTriangle()
    {
        return new LSystemDefinition
        {
            Name = "Sierpinski Triangle",
            Axiom = "F",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'F', Successor = "G-F-G" },
                new LSystemRule { Predecessor = 'G', Successor = "F+G+F" }
            },
            TurnAngle = 60.0,
            DefaultGenerations = 6,
            MaxGenerations = 10,
            InitialHeading = 0.0,
            Category = "Classic",
            Description = "Sierpinski arrowhead curve, an L-System variant of Sierpinski's 1915 triangle fractal."
        };
    }

    /// <summary>
    /// Hilbert Curve: Space-filling curve discovered by David Hilbert (1891).
    /// Rules: X → -YF+XFX+FY-, Y → +XF-YFY-FX+
    /// Angle: 90°
    /// Axiom: X
    /// Note: X and Y are non-drawing symbols (used only for grammar expansion)
    /// </summary>
    public static LSystemDefinition HilbertCurve()
    {
        return new LSystemDefinition
        {
            Name = "Hilbert Curve",
            Axiom = "X",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'X', Successor = "-YF+XFX+FY-" },
                new LSystemRule { Predecessor = 'Y', Successor = "+XF-YFY-FX+" }
            },
            TurnAngle = 90.0,
            DefaultGenerations = 5,
            MaxGenerations = 8,
            InitialHeading = 0.0,
            Category = "Space-Filling",
            Description = "David Hilbert's 1891 continuous curve that fills a square. Every point in 2D space is arbitrarily close to the curve."
        };
    }

    /// <summary>
    /// Fractal Plant: Realistic branching plant structure using stack push/pop.
    /// Rule: F → FF, X → F-[[X]+X]+F[+FX]-X
    /// Angle: 25°
    /// Axiom: X
    /// Uses '[' to push state and ']' to pop (creates branches)
    /// </summary>
    public static LSystemDefinition FractalPlant()
    {
        return new LSystemDefinition
        {
            Name = "Fractal Plant",
            Axiom = "X",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'F', Successor = "FF" },
                new LSystemRule { Predecessor = 'X', Successor = "F-[[X]+X]+F[+FX]-X" }
            },
            TurnAngle = 25.0,
            DefaultGenerations = 5,
            MaxGenerations = 7,
            InitialHeading = 90.0,  // Start pointing up
            Category = "Plant",
            Description = "Aristid Lindenmayer's original plant model from 1968. Demonstrates biological branching patterns."
        };
    }

    /// <summary>
    /// Koch Curve: Single-edge variant (not closed snowflake).
    /// Rule: F → F+F-F-F+F
    /// Angle: 90°
    /// Axiom: F
    /// </summary>
    public static LSystemDefinition KochCurve()
    {
        return new LSystemDefinition
        {
            Name = "Koch Curve",
            Axiom = "F",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'F', Successor = "F+F-F-F+F" }
            },
            TurnAngle = 90.0,
            DefaultGenerations = 4,
            MaxGenerations = 6,
            InitialHeading = 0.0,
            Category = "Classic",
            Description = "Quadratic Koch variant with 90° angles, creating a square-based fractal edge."
        };
    }

    /// <summary>
    /// Peano Curve: Space-filling curve discovered by Giuseppe Peano (1890).
    /// Rules: X → XFYFX+F+YFXFY-F-XFYFX, Y → YFXFY-F-XFYFX+F+YFXFY
    /// Angle: 90°
    /// Axiom: X
    /// </summary>
    public static LSystemDefinition PeanoCurve()
    {
        return new LSystemDefinition
        {
            Name = "Peano Curve",
            Axiom = "X",
            Rules = new List<LSystemRule>
            {
                new LSystemRule { Predecessor = 'X', Successor = "XFYFX+F+YFXFY-F-XFYFX" },
                new LSystemRule { Predecessor = 'Y', Successor = "YFXFY-F-XFYFX+F+YFXFY" }
            },
            TurnAngle = 90.0,
            DefaultGenerations = 3,
            MaxGenerations = 5,
            InitialHeading = 0.0,
            Category = "Space-Filling",
            Description = "Giuseppe Peano's 1890 curve, the first space-filling curve ever discovered. Fills a square completely."
        };
    }

    /// <summary>
    /// Get preset by name (case-insensitive, ignores spaces and hyphens).
    /// Handles both "DragonCurve" and "Dragon Curve" formats.
    /// </summary>
    public static LSystemDefinition? GetPresetByName(string name)
    {
        var presets = GetAllPresets();

        // Normalize the search name: remove spaces, hyphens, and lowercase
        string normalizedSearchName = name.Replace(" ", "").Replace("-", "").ToLowerInvariant();

        // Find preset by normalized name comparison
        return presets.Find(p => 
        {
            string normalizedPresetName = p.Name.Replace(" ", "").Replace("-", "").ToLowerInvariant();
            return normalizedPresetName == normalizedSearchName;
        });
    }
}
