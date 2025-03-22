using Avalonia;
using System;

namespace Pixed.Application.Zoom;
public static class MatrixHelper
{
    public static Matrix Create(double scale, double centerX, double centerY)
    {
        return new Matrix(scale, 0, 0, scale, centerX - (scale * centerX), centerY - (scale * centerY));
    }
    public static Matrix TranslatePrepend(Matrix matrix, double offsetX, double offsetY)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY) * matrix;
    }
    public static Matrix ScaleAtPrepend(Matrix matrix, double scale, double centerX, double centerY)
    {
        return matrix.Prepend(Create(scale, centerX, centerY));
    }
    public static Matrix ScaleAndTranslate(double scale, double offsetX, double offsetY)
    {
        return new Matrix(scale, 0.0, 0.0, scale, offsetX, offsetY);
    }

    public static void ToString(Matrix matrix, string linePrefix = "")
    {
        Console.WriteLine(linePrefix + " first row: [" + matrix.M11.ToString("0.00") + "][" + matrix.M21.ToString("0.00") + "][" + matrix.M31.ToString("0.00") + "]");
        Console.WriteLine(linePrefix + " second row: [" + matrix.M12.ToString("0.00") + "][" + matrix.M22.ToString("0.00") + "][" + matrix.M32.ToString("0.00") + "]");
        Console.WriteLine(linePrefix + " third row: [" + matrix.M13.ToString("0.00") + "][" + matrix.M23.ToString("0.00") + "][" + matrix.M33.ToString("0.00") + "]");
    }
}