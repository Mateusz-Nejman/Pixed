using Avalonia;
using Pixed.Common.Utils;

namespace Pixed.Application.Utils;
internal static class PointUtils
{
    public static bool EqualsPrecision(this Point value, Point value1, double precision = 0.001)
    {
        return value.X.EqualsPrecision(value1.X, precision) && value.Y.EqualsPrecision(value1.Y, precision);
    }
}