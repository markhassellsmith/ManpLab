#pragma once

#include <string>
#include <sstream>
#include <iomanip>
#include "../ManpWIN64/BigDouble.h"

// Forward declare to avoid ambiguity
class BigDouble;

namespace Native {

    /// <summary>
    /// MPFR-backed high-precision double for deep zoom support
    /// This wraps the real BigDouble class from ManpWIN64 with actual arbitrary precision.
    /// </summary>
    class MPFRBigDouble
    {
    public:
        MPFRBigDouble() {
            // BigDouble constructor handles initialization
        }

        MPFRBigDouble(double val) : value(val) {}

        MPFRBigDouble(double val, int prec) : value(val) {
            value.ChangePrecision(prec);
        }

        // The actual MPFR-backed BigDouble from ManpWIN64
        ::BigDouble value;

        /// <summary>
        /// Convert to string representation with full precision
        /// </summary>
        std::string ToString() const
        {
            char buf[512];
            // BigDouble::ToString is not const, need to use workaround
            ::BigDouble temp = value;
            temp.ToString(buf, sizeof(buf), true); // scientific notation
            return std::string(buf);
        }

        /// <summary>
        /// Parse from string representation
        /// </summary>
        static MPFRBigDouble FromString(const std::string& str)
        {
            MPFRBigDouble result;
            result.value = std::stod(str); // BigDouble has operator= from double
            return result;
        }

        // Arithmetic operations using MPFR
        MPFRBigDouble operator+(const MPFRBigDouble& other) const
        {
            MPFRBigDouble result;
            ::BigDouble temp1 = value;  // Copy to mutable
            ::BigDouble temp2 = other.value;
            result.value = temp1 + temp2;
            return result;
        }

        MPFRBigDouble operator-(const MPFRBigDouble& other) const
        {
            MPFRBigDouble result;
            ::BigDouble temp1 = value;
            ::BigDouble temp2 = other.value;
            result.value = temp1 - temp2;
            return result;
        }

        MPFRBigDouble operator*(const MPFRBigDouble& other) const
        {
            MPFRBigDouble result;
            ::BigDouble temp1 = value;
            ::BigDouble temp2 = other.value;
            result.value = temp1 * temp2;
            return result;
        }

        MPFRBigDouble operator/(const MPFRBigDouble& other) const
        {
            MPFRBigDouble result;
            ::BigDouble temp1 = value;
            ::BigDouble temp2 = other.value;
            result.value = temp1 / temp2;
            return result;
        }

        bool operator<(const MPFRBigDouble& other) const
        {
            ::BigDouble temp1 = value;
            ::BigDouble temp2 = other.value;
            return temp1 < temp2;
        }

        bool operator>(const MPFRBigDouble& other) const
        {
            ::BigDouble temp1 = value;
            ::BigDouble temp2 = other.value;
            return temp1 > temp2;
        }

        double ToDouble() const
        {
            ::BigDouble temp = value;
            return temp.BigDoubleToDouble();
        }
    };

    // Keep SimpleBigDouble for backward compatibility if needed
    using SimpleBigDouble = MPFRBigDouble;

} // namespace Native
