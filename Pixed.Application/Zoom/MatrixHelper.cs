using Avalonia;
using System;

namespace Pixed.Application.Zoom;
public static class MatrixHelper
{
    public static Matrix Create(double scale, Point center)
    {
        return new Matrix(scale, 0, 0, scale, center.X - (scale * center.X), center.Y - (scale * center.Y));
    }
    public static Matrix TranslatePrepend(Matrix matrix, Point offset)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offset.X, offset.Y) * matrix;
    }
    public static Matrix ScaleAtPrepend(Matrix matrix, double scale, Point center)
    {
        return matrix.Prepend(Create(scale, center));
    }
    public static Matrix ScaleAndTranslate(double scale, Point offset)
    {
        return new Matrix(scale, 0.0, 0.0, scale, offset.X, offset.Y);
    }

    public static void ToString(Matrix matrix, string linePrefix = "")
    {
        Console.WriteLine(linePrefix + " first row: [" + matrix.M11.ToString("0.00") + "][" + matrix.M21.ToString("0.00") + "][" + matrix.M31.ToString("0.00") + "]");
        Console.WriteLine(linePrefix + " second row: [" + matrix.M12.ToString("0.00") + "][" + matrix.M22.ToString("0.00") + "][" + matrix.M32.ToString("0.00") + "]");
        Console.WriteLine(linePrefix + " third row: [" + matrix.M13.ToString("0.00") + "][" + matrix.M23.ToString("0.00") + "][" + matrix.M33.ToString("0.00") + "]");
    }
}