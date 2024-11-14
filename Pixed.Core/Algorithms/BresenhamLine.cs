using Pixed.Core.Models;

namespace Pixed.Core.Algorithms;
public static class BresenhamLine
{
    public static List<Point> Get(Point point1, Point point2)
    {
        List<Point> points = [];

        var dx = Math.Abs(point2.X - point1.X);
        var dy = Math.Abs(point2.Y - point1.Y);

        var stepX = point1.X < point2.X ? 1 : -1;
        var stepY = point1.Y < point2.Y ? 1 : -1;

        var error = dx - dy;

        while (true)
        {
            points.Add(new Point(point1.X, point1.Y));

            if (point1.X == point2.X && point1.Y == point2.Y) break;

            var error2 = error * 2;
            if (error2 > -dy)
            {
                error -= dy;
                point1.X += stepX;
            }
            if (error2 < dx)
            {
                error += dx;
                point1.Y += stepY;
            }
        }

        return points;
    }
}