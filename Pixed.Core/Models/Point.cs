namespace Pixed.Core.Models;
public struct Point(int x, int y) : IEquatable<Point>
{
    public int X { readonly get; set; } = x;
    public int Y { readonly get; set; } = y;

    public Point() : this(0, 0)
    {
    }

    public Point(int value) : this(value, value)
    {
    }

    public static Point operator -(Point a)
    {
        return new Point(-a.X, -a.Y);
    }
    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !(left == right);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static double Distance(Point value1, Point value2)
    {
        double distanceSquared = (value2.X - value1.X) * (value2.X - value1.X) +
                                 (value2.Y - value1.Y) * (value2.Y - value1.Y);
        return Math.Sqrt(distanceSquared);
    }

    public bool Equals(Point other)
    {
        return X == other.X &&
               Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point point && Equals(point);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}