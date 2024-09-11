using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Utils
{
    internal static class MathUtils
    {
        public static List<Point> GetBresenhamLine(int x0, int y0, int x1, int y1)
        {
            List<Point> points = [];
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x0);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                points.Add(new Point(steep ? y : x, steep ? x : y));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            return points;
        }
    }
}
