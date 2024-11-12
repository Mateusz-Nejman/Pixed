using Pixed.Common.Models;

namespace Pixed.Application.Utils;
internal static class ConversionUtils
{
    public static Point ToPixedPoint(this Avalonia.Point point)
    {
        return new Point((int)point.X, (int)point.Y);
    }

    public static float ToFloat(this double value)
    {
        return (float)value;
    }

    public static float ToFloat(this int value)
    {
        return value;
    }
}
