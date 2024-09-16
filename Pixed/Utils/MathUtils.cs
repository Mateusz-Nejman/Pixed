using Avalonia.Media.TextFormatting.Unicode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Pixed.Utils;

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

    public static List<Point> GetUniformLinePixels(int x0, int y0, int x1, int y1)
    {
        List<Point> pixels = [];

        var dx = Math.Abs(x1 - x0) + 1;
        var dy = Math.Abs(y1 - y0) + 1;

        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;

        var ratio = (double)Math.Max(dx, dy) / (double)Math.Min(dx, dy);
        var step = Math.Round(ratio);
        if(step > Math.Min(dx, dy))
        {
            step = double.MaxValue;
        }

        var maxDistance = Distance(x0, y0, x1, y1);

        var x = x0;
        var y = y0;
        int i = 0;

        while(true)
        {
            i++;
            pixels.Add(new Point(x, y));
            if (Distance(x0, y0, x, y) >= maxDistance)
            {
                break;
            }

            bool isAtStep = (int)(i % step) == 0;

            if(dx >= dy || isAtStep)
            {
                x += sx;
            }

            if(dy >= dx || isAtStep)
            {
                y += sy;
            }
        }

        return pixels;
    }

    public static double Distance(int x0, int y0, int x1, int y1)
    {
        var dx = Math.Abs((double)x1 - (double)x0);
        var dy = Math.Abs((double)y1 - (double)y0);
        return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
    }
}
