using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Common.Algos;
internal static class BresenhamLine
{
    public static List<Point> Get(int x0, int y0, int x1, int y1)
    {
        List<Point> points = [];

        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);

        var stepX = x0 < x1 ? 1 : -1;
        var stepY = y0 < y1 ? 1 : -1;

        var error = dx - dy;

        while (true)
        {
            points.Add(new Point(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            var error2 = error * 2;
            if (error2 > -dy)
            {
                error -= dy;
                x0 += stepX;
            }
            if (error2 < dx)
            {
                error += dx;
                y0 += stepY;
            }
        }

        return points;
    }
}