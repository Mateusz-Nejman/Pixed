using System.Drawing;

namespace Pixed.Utils
{
    internal static class ConversionUtils
    {
        public static Point ToSystemPoint(this Avalonia.Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }
    }
}
