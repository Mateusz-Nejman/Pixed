using Avalonia;

namespace Pixed.Application.Zoom;
public static class MatrixHelper
{
    public static Matrix TranslatePrepend(Matrix matrix, double offsetX, double offsetY)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY) * matrix;
    }
    public static Matrix ScaleAtPrepend(Matrix matrix, double scale, double centerX, double centerY)
    {
        return matrix.Prepend(new Matrix(scale, 0, 0, scale, centerX - (scale * centerX), centerY - (scale * centerY)));
    }
    public static Matrix ScaleAndTranslate(double scale, double offsetX, double offsetY)
    {
        return new Matrix(scale, 0.0, 0.0, scale, offsetX, offsetY);
    }
}