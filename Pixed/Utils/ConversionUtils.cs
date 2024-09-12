using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
