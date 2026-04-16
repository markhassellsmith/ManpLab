#pragma once

#include <string>
#include <sstream>
#include <iomanip>

namespace Native {

    /// <summary>
    /// Lightweight high-precision double for deep zoom support (Phase 2)
    /// This is a simplified implementation for demonstration purposes.
    /// Full MPFR integration will be done when connecting to ManpWIN64 engine.
    /// </summary>
    class SimpleBigDouble
    {
    public:
        SimpleBigDouble() : value(0.0), precision(16) {}
        SimpleBigDouble(double val) : value(val), precision(16) {}
        SimpleBigDouble(double val, int prec) : value(val), precision(prec) {}

        // For Phase 2, we use double internally
        // In later phases, this will be replaced with actual MPFR mpfr_t
        double value;
        int precision;  // Number of significant digits

        /// <summary>
        /// Convert to string representation with full precision
        /// </summary>
        std::string ToString() const
        {
            std::ostringstream oss;
            oss << std::setprecision(precision) << std::scientific << value;
            return oss.str();
        }

        /// <summary>
        /// Parse from string representation
        /// </summary>
        static SimpleBigDouble FromString(const std::string& str)
        {
            SimpleBigDouble result;
            result.value = std::stod(str);
            return result;
        }

        // Arithmetic operations (Phase 2 uses double, later will use MPFR)
        SimpleBigDouble operator+(const SimpleBigDouble& other) const
        {
            return SimpleBigDouble(value + other.value, precision);
        }

        SimpleBigDouble operator-(const SimpleBigDouble& other) const
        {
            return SimpleBigDouble(value - other.value, precision);
        }

        SimpleBigDouble operator*(const SimpleBigDouble& other) const
        {
            return SimpleBigDouble(value * other.value, precision);
        }

        SimpleBigDouble operator/(const SimpleBigDouble& other) const
        {
            return SimpleBigDouble(value / other.value, precision);
        }

        bool operator<(const SimpleBigDouble& other) const
        {
            return value < other.value;
        }

        bool operator>(const SimpleBigDouble& other) const
        {
            return value > other.value;
        }

        double ToDouble() const
        {
            return value;
        }
    };

} // namespace Native
