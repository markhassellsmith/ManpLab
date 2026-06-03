//=============================================================================
// AI AGENT HELPER - FRACTAL REGISTRATION TEMPLATE
//=============================================================================
// This file provides templates and guidance for AI agents adding new fractals
// to the ManpLab application.
//=============================================================================

#pragma once

/*

QUICK START FOR AI AGENTS
==========================

When adding a new fractal to the application, follow these steps:

1. STORE INITIAL CONDITIONS
   ------------------------
   Add the initial view conditions to the InitialConditionsService:

   InitialConditionsService::Set("YourFractalName", 
       centerX,    // X coordinate of interesting region
       centerY,    // Y coordinate 
       zoom);      // Zoom level (viewport_width = 4.0 / zoom)

   This automatically persists to: FractalData\InitialConditions.txt


2. CREATE FRACTAL REGISTRATION FUNCTION
   ------------------------------------
   In an appropriate *Family.cpp file, add your registration function:

   #include "FractalRegistry.h"
   #include "MandelbrotCalculator.h"
   #include "InitialConditionsService.h"
   #include <cmath>

   void RegisterYourFractalsFamily()
   {
       FractalSpec spec;

       // Basic metadata
       spec.name = "YourFractalName";              // Internal ID (no spaces)
       spec.displayName = "Your Fractal Display Name";
       spec.category = "Your Category";
       spec.type = FractalCategory::EscapeTime2D;  // Or other appropriate type
       spec.description = "Brief description of the fractal";
       spec.formula = "Mathematical formula (e.g., z = z² + c)";
       spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";  // LaTeX version

       // Calculator function
       spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                           ComplexD juliaC, const ParamMap& params) -> double 
       {
           ComplexD z(0.0, 0.0);
           const double bailout = 256.0;

           for (int i = 0; i < maxIter; ++i)
           {
               double magSq = z.real * z.real + z.imag * z.imag;
               if (magSq > bailout)
                   return static_cast<double>(i);

               // Your iteration formula here
               ComplexD temp = z;
               z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
               z.imag = 2.0 * temp.real * temp.imag + c.imag;
           }

           return static_cast<double>(maxIter);
       };

       // View settings from InitialConditionsService
       spec.supportsJulia = false;  // or true
       auto initialConditions = InitialConditionsService::Get("YourFractalName");
       spec.defaultCenterX = initialConditions.centerX;
       spec.defaultCenterY = initialConditions.centerY;
       spec.defaultZoom = initialConditions.zoom;
       spec.defaultBailout = 256.0;
       spec.hasSymmetry = true;  // or false

       // Register it
       FractalRegistry::Register(spec);
   }


3. CALL YOUR REGISTRATION FUNCTION
   --------------------------------
   Add a call to your function in FractalRegistry.cpp:

   void FractalRegistry::InitializeBuiltins()
   {
       // ... existing registrations ...
       RegisterYourFractalsFamily();
   }


ZOOM CALCULATION GUIDE
======================

The zoom value determines the viewport width shown to the user:

    viewport_width = 4.0 / zoom

Examples:
    zoom = 4.0    → viewport width = 1.0   (very zoomed in)
    zoom = 2.0    → viewport width = 2.0   (zoomed in)
    zoom = 1.333  → viewport width = 3.0   (moderate)
    zoom = 1.0    → viewport width = 4.0   (default)
    zoom = 0.667  → viewport width = 6.0   (zoomed out)
    zoom = 0.5    → viewport width = 8.0   (very zoomed out)

For a standard Mandelbrot view (-2 to 1 on X axis):
    - Range = 3.0 units
    - Use zoom = 4.0 / 3.0 = 1.333


FRACTAL CATEGORIES
==================

Choose the appropriate FractalCategory:

    FractalCategory::EscapeTime2D       - Standard 2D escape-time fractals
                                          (Mandelbrot, Julia, etc.)

    FractalCategory::Sequence2D         - Sequence-based fractals
                                          (Hailstone, bifurcation diagrams)

    FractalCategory::AttractorBased3D   - 3D strange attractors
                                          (Lorenz, Rössler - legacy rendering)

    FractalCategory::HistogramBased     - Orbit accumulation rendering
                                          (Strange attractors, flame fractals)

    FractalCategory::Special            - Special rendering techniques
                                          (Perturbation theory, Buddhabrot)


CALCULATOR FUNCTION SIGNATURES
===============================

Escape-Time Fractals:
---------------------
Return smooth iteration count, or maxIter if point is in the set.

double calculator(ComplexD c, int maxIter, bool isJulia, 
                 ComplexD juliaC, const ParamMap& params)
{
    // Returns: iteration count (0 to maxIter)
}


Histogram-Based Fractals:
-------------------------
Update coordinates in-place for orbit tracing.

void orbitIterator(double& x, double& y, double& z, const ParamMap& params)
{
    // Modifies x, y, z according to dynamical system
}


COMMON PATTERNS
===============

Smooth Coloring:
---------------
For continuous coloring at escape:

    double mu = i + 1 - std::log(std::log(mag)) / std::log(2.0);
    return mu;


Julia Set Support:
-----------------
Use the isJulia and juliaC parameters:

    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
    ComplexD constant = isJulia ? juliaC : c;


Orbit Trapping:
--------------
Track minimum distance to some shape:

    double minDist = 1000.0;
    for (int i = 0; i < maxIter; ++i)
    {
        // ... iteration ...
        double dist = /* distance to trap shape */;
        if (dist < minDist)
            minDist = dist;
    }
    return minDist * maxIter;


FILE LOCATIONS
==============

Fractal Family Files:   ManpCore.Native\*Family.cpp
Registry:               ManpCore.Native\FractalRegistry.cpp
Initial Conditions:     ManpCore.Native\Resources\InitialConditions.txt
                       (runtime copy: FractalData\InitialConditions.txt)


TESTING
=======

After adding your fractal:

1. Build the project (should compile without errors)
2. Run the application
3. Verify your fractal appears in the appropriate category
4. Check that the initial view shows an interesting region
5. Test zooming and panning


DOCUMENTATION
=============

For complete details, see:
- ManpCore.Native\Documentation\INITIAL_CONDITIONS_SERVICE.md
- ManpCore.Native\FractalRegistry.h
- ManpCore.Native\Documentation\ADDING_NEW_FRACTALS.md (if exists)

*/
