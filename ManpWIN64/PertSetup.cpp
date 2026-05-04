/****************************************************
    PERTSETUP.CPP a module to explore Perturbation
    Thanks to Shirom Makkad fractaltodesktop@gmail.com
    Written in Microsoft Visual C++ by Paul de Leeuw.
****************************************************/

#include <atomic>
#include <vector>
#include "PertEngine.h"
#include "Timer.h"

#define min(a,b)    (((a) < (b)) ? (a) : (b))

std::vector<ExpComplex>	ExpXSubN {0};		// references
std::vector<Complex>	XSubN {0};
int	ArithType = DOUBLE;
int	MaxRefIteration = 0;
BLAS    Bla;					// Bilinear Approximation
int	SlopeDegree = 2;

extern	int	subtype;
extern	double	param[];
extern	WORD	degree;				// power
extern	bool	EnableApproximation;		// use BLA on perturbation
extern	int	xdots, ydots;
extern	char	PertStatus[];

extern	std::atomic<bool> gStopRequested;	// force early exit
extern	std::vector<CPerturbation> PertCalculator;

//////////////////////////////////////////////////////////////////////
// Determine the type of arithmetic to use
//////////////////////////////////////////////////////////////////////

void	PertSetupArithType(int &ArithType, int subtype, long MaxIteration, int precision, BYTE BigNumFlag)
    {
    // okay, we are going to do some complicated arithmetic calculations for the various ways of calculating fractals
    if (!BigNumFlag || (BigNumFlag && precision <= 300))
	{
	if (subtype == 0 || subtype == 1)
	    ArithType = DOUBLE;
	else
	    ArithType = DBL_UNSUPPORTED;		// use Use BLA perturbation code but don't process via BLA table
	}
    else
	{
	if ((subtype == 0 || subtype == 1))		// only for Mandelbrot and power so far
	    ArithType = FLOATEXP;
	else
	    ArithType = EXP_UNSUPPORTED;		// use Use BLA perturbation code but don't process via BLA table
	}
 
    if (ArithType == EXP_UNSUPPORTED || ArithType == FLOATEXP) 
	{
	// get memory for Z array
	ExpXSubN.resize(MaxIteration + 1);
	}
    else
	{
	// get memory for Z array
	XSubN.resize(MaxIteration + 1);
	}
    if (EnableApproximation)
	{
	if (ArithType == EXP_UNSUPPORTED || ArithType == DBL_UNSUPPORTED)
	    EnableApproximation = false;		// we don't support these fractals with BLA
	}
    char	buf[256];
    sprintf(buf, "PertSetupArithType: precision = %d, ArithType = %d\n", precision, ArithType);
    OutputDebugStringA(buf);
    }

//////////////////////////////////////////////////////////////////////
// Reference Zoom Point
//////////////////////////////////////////////////////////////////////

int	ReferenceZoomPoint(BigComplex& centre, int maxIteration, int user_data(HWND hwnd), char* StatusBarInfo, int *pPertProgress, double bailout, int ArithType, int power, BigDouble BigWidth, int &SlopeDegree)
    {
    // Raising this number makes more calculations, but less variation between each calculation (less chance of mis-identifying a glitched point).
    BigComplex	zBig;
    bool	WeHaveMaxRefIteration = false;
    double	CoordinateMagnitudeSquared;
    int		lastChecked = -1;
    constexpr	DWORD	REF_UPDATE_INTERVAL_MS = 250;
    DWORD	lastUpdateTick = GetTickCount();
    int		lastPercent = -1;
    double	ZoomRadius = mpfr_get_d(BigWidth.x, MPFR_RNDN);
    char	buf[256];

    SimpleTimer  tRef;
    tRef.start();

    if (ArithType == EXP_UNSUPPORTED || ArithType == FLOATEXP)
	{
	// get memory for Z array
	ExpXSubN.resize(maxIteration + 1);
	}
    else
	{
	// get memory for Z array
	XSubN.resize(maxIteration + 1);
	}

    zBig = 0.0;
    for (int i = 0; i <= maxIteration; i++)
	{
	if (gStopRequested)
	    return -1;
	if (ArithType == FLOATEXP || ArithType == EXP_UNSUPPORTED)
	    {
	    PertCalculator[0].BigComplex2ExpComplex(&ExpXSubN[i], zBig);
	    }
	else
	    {
	    XSubN[i] = zBig.CBig2Double();
	    }

	DWORD now = GetTickCount();
	if (now - lastUpdateTick >= 250)	// just check user input and progress every 250 mS
	    {
	    lastUpdateTick = now;

	    // 1. user input
	    if (user_data(NULL) < 0)
		return -1;

	    // 2. progress
	    double progress = (double)i / maxIteration;
	    int percent = (int)(progress * 100.0);

	    _snprintf_s(PertStatus, MAXLINE, _TRUNCATE, "Ref=(%d%%)", percent);
//	    *pPertProgress = percent;
	    }

	// Calculate the set
	PertCalculator[0].RefFunctions(&centre, &zBig, SlopeDegree);
	if (EnableApproximation && !WeHaveMaxRefIteration)			// only needed for BLA
	    {
	    CoordinateMagnitudeSquared = zBig.CSumSqr();			// no point in further testing once we have MaxRefIteration
	    if ((CoordinateMagnitudeSquared) > bailout && !WeHaveMaxRefIteration)
		{
		MaxRefIteration = i;						// needed for BLA
		WeHaveMaxRefIteration = true;
		break;								// the rest of the table isn't needed for BLA
		}
	    }
	}

    if (!WeHaveMaxRefIteration)
	MaxRefIteration = maxIteration;	// MaxRefIteration not found in main reference loop

    double refSeconds = tRef.stop_ms();
    auto s = FormatElapsed(refSeconds);
    sprintf(buf, "Reference build: %s, MaxIter = %d, MaxRefIter = %d\n", s.c_str(), maxIteration, MaxRefIteration);
	OutputDebugStringA(buf);

	SimpleTimer  tBla;
	tBla.start();

	// Debug logging for adaptive BLA
	double zoom_magnitude_preview = log10(3.0 / ZoomRadius);
	double adaptive_scale_preview = max(0.1, pow(0.8, zoom_magnitude_preview / 10.0));
	sprintf(buf, "BLA adaptive scaling: zoom depth E-%.1f, scale factor %.3f\n", 
			zoom_magnitude_preview, adaptive_scale_preview);
	OutputDebugStringA(buf);

	if (EnableApproximation)
	{
	Bla.clear();
	int M = MaxRefIteration; // the period
	int image_size = min(xdots, ydots);

	// Adaptive BLA tile sizing based on zoom depth
	// Deeper zooms need finer tiles to avoid visible square artifacts
	// zoom_magnitude: E-6 = 6, E-14 = 14, E-20 = 20, etc.
	double zoom_magnitude = log10(3.0 / ZoomRadius);  // Full Mandelbrot set width = 3.0
	// Scale factor: 0.8^(depth/10) means 20% reduction per 10 orders of magnitude
	// E-6 → 0.93 (minimal change), E-14 → 0.44 (half size), E-20 → 0.26 (quarter size)
	double adaptive_scale = pow(0.8, zoom_magnitude / 10.0);
	adaptive_scale = max(0.1, adaptive_scale);  // Floor at 10% to prevent excessive subdivision

	if (ArithType == FLOATEXP)
		{
		// set up parameters for BLA table with adaptive sizing
		ExpComplex temp_size_image_size = ExpComplex(ZoomRadius / (double)image_size, ZoomRadius / (double)image_size);
		floatexp baseBlaSize = baseBlaSize.hypotExp(temp_size_image_size.x * xdots * 0.5, temp_size_image_size.y * ydots * 0.5);
		floatexp blaSize = baseBlaSize * adaptive_scale;
		Bla.initExp(M, ExpXSubN, blaSize, power, subtype, maxIteration, param);
		}
	else if (ArithType == DOUBLE)
		{
		Complex temp_size_image_size = Complex(ZoomRadius / (double)image_size, ZoomRadius / (double)image_size);
		double baseBlaSize = hypot(temp_size_image_size.x * xdots * 0.5, temp_size_image_size.y * ydots * 0.5);
		double blaSize = baseBlaSize * adaptive_scale;
		Bla.init(M, XSubN, blaSize, power, subtype, maxIteration, param);
		}
	}

    double blaSeconds = tBla.stop_ms();
    s = FormatElapsed(blaSeconds);
    sprintf(buf, "BLA build: %s\n", s.c_str());
    OutputDebugStringA(buf);

    return 0;
    }

