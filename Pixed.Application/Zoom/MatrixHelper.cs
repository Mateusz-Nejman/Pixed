using Avalonia;

namespace Pixed.Application.Zoom;
public static class MatrixHelper
{
    public static Matrix Translate(double offsetX, double offsetY)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
    }
    public static Matrix TranslatePrepend(Matrix matrix, double offsetX, double offsetY)
    {
        return Translate(offsetX, offsetY) * matrix;
    }
    public static Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }
    public static Matrix ScaleAtPrepend(Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
    {
        return ScaleAt(scaleX, scaleY, centerX, centerY) * matrix;
    }
    public static Matrix ScaleAndTranslate(double scaleX, double scaleY, double offsetX, double offsetY)
    {
        return new Matrix(scaleX, 0.0, 0.0, scaleY, offsetX, offsetY);
    }
}