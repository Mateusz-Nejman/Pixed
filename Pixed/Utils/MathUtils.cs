using System;
using System.Collections.Generic;
using System.Drawing;

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
        if (step > Math.Min(dx, dy))
        {
            step = double.MaxValue;
        }

        var maxDistance = Distance(x0, y0, x1, y1);

        var x = x0;
        var y = y0;
        int i = 0;

        while (true)
        {
            i++;
            pixels.Add(new Point(x, y));
            if (Distance(x0, y0, x, y) >= maxDistance)
            {
                break;
            }

            bool isAtStep = (int)(i % step) == 0;

            if (dx >= dy || isAtStep)
            {
                x += sx;
            }

            if (dy >= dx || isAtStep)
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

    public static int[] GetOrderedRectangle(int x0, int y0, int x1, int y1)
    {
        return [Math.Min(x0, x1), Math.Min(y0, y1), Math.Max(x0, x1), Math.Max(y0, y1)];
    }

    public static List<Point> GetCircle(int x0, int y0, int x1, int y1)
    {
        //https://stackoverflow.com/questions/2914807/plot-ellipse-from-rectangle
        List<Point> pixels = [];
        int centerX1, centerY1, centerX, centerY;

        // Calculate height
        centerY1 = centerY = (y0 + y1) / 2;
        int height = y1 - y0;
        int qy = height;
        int dy = height / 2;
        if (height % 2 != 0)
        {
            centerY++;
        }

        // Calculate width
        centerX1 = centerX = (x0 + x1) / 2;
        int width = x1 - x0;
        int qx = width % 2;
        int dx = 0;
        long qt = (long)width * width + (long)height * height - 2L * width * width * height;

        if (qx != 0)
        {
            centerX++;
            qt += 3L * height * height;
        }

        // Start at (dx, dy) = (0, b) and iterate until (a, 0) is reached
        while (qy >= 0 && qx <= width)
        {
            pixels.Add(new Point(centerX1 - dx, centerY1 - dy));
            if (dx != 0 || centerX1 != centerX)
            {
                pixels.Add(new Point(centerX + dx, centerY1 - dy));
                if (dy != 0 || centerY1 != centerY)
                    pixels.Add(new Point(centerX + dx, centerY + dy));
            }
            if (dy != 0 || centerY1 != centerY)
                pixels.Add(new Point(centerX1 - dx, centerY + dy));

            // If a (+1, 0) step stays inside the ellipse, do it
            if (qt + 2L * height * height * qx + 3L * height * height <= 0L ||
                qt + 2L * width * width * qy - (long)width * width <= 0L)
            {
                qt += 8L * height * height + 4L * height * height * qx;
                dx++;
                qx += 2;
                // If a (0, -1) step stays outside the ellipse, do it
            }
            else if (qt - 2L * width * width * qy + 3L * width * width > 0L)
            {
                qt += 8L * width * width - 4L * width * width * qy;
                dy--;
                qy -= 2;
                // Else step (+1, -1)
            }
            else
            {
                qt += 8L * height * height + 4L * height * height * qx + 8L * width * width - 4L * width * width * qy;
                dx++;
                qx += 2;
                dy--;
                qy -= 2;
            }
        }   // End of while loop
        return pixels;
    }
}
