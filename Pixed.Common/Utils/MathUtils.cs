using Pixed.Common.Algorithms;
using Pixed.Common.Models;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Utils;

public static class MathUtils
{
    public static List<Point> FloodFill(Point startPosition, Point size, Func<Point, bool> predicate)
    {
        List<Point> points = [];

        FloodFill algorithm = new(startPosition, size);
        bool predicateInternal(Point point) => predicate(point);
        void action(Point point) => points.Add(point);
        algorithm.Execute(predicateInternal, action);

        return points;
    }

    public static List<Point> GetUniformLinePixels(Point point1, Point point2)
    {
        List<Point> pixels = [];

        var dx = Math.Abs(point2.X - point1.X) + 1;
        var dy = Math.Abs(point2.Y - point1.Y) + 1;

        var sx = point1.X < point2.X ? 1 : -1;
        var sy = point1.Y < point2.Y ? 1 : -1;

        var ratio = Math.Max(dx, dy) / (double)Math.Min(dx, dy);
        var step = Math.Round(ratio);
        if (step > Math.Min(dx, dy))
        {
            step = double.MaxValue;
        }

        var maxDistance = Point.Distance(point1, point2);

        var x = point1.X;
        var y = point1.Y;
        int i = 0;

        while (true)
        {
            i++;
            pixels.Add(new Point(x, y));
            if (Point.Distance(point1, new Point(x, y)) >= maxDistance)
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

    public static Tuple<Point, Point> GetOrderedRectangle(Point point1, Point point2)
    {
        return new Tuple<Point, Point>(new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y)), new Point(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y)));
    }

    public static List<Point> GetCircle(Point point1, Point point2)
    {
        //https://stackoverflow.com/questions/2914807/plot-ellipse-from-rectangle
        List<Point> pixels = [];
        int centerX1, centerY1, centerX, centerY;

        // Calculate height
        centerY1 = centerY = (point1.Y + point2.Y) / 2;
        int height = point2.Y - point1.Y;
        int qy = height;
        int dy = height / 2;
        if (height % 2 != 0)
        {
            centerY++;
        }

        // Calculate width
        centerX1 = centerX = (point1.X + point2.X) / 2;
        int width = point2.X - point1.X;
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

    public static bool EqualsPrecision(this double value, double value1, double precision = 0.001)
    {
        return Math.Abs(value - value1) < precision;
    }
}
