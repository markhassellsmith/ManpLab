// BigDoubleSupport.cpp
// Compile ManpWIN64 BigDouble/BigComplex sources without managed code

// Disable CLR compilation for this file to allow pure native MPFR code
#pragma unmanaged

#include "../ManpWIN64/BigDouble.cpp"
#include "../ManpWIN64/BigComplex.cpp"

// Define the global decimals variable if not already defined
#ifndef DECIMALS_DEFINED
#define DECIMALS_DEFINED
int decimals = 50;  // Default precision for deep zoom
#endif

#pragma managed
