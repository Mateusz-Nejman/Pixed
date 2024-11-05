using System.Drawing;

namespace Pixed.Application.Utils;
internal static class ConversionUtils
{
    public static Point ToSystemPoint(this Avalonia.Point point)
    {
        return new Point((int)point.X, (int)point.Y);
    }

    public static float ToFloat(this double value)
    {
        return (float)value;
    }
}
