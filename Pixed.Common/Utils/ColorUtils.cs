namespace Pixed.Common.Utils;
internal static class ColorUtils
{
    public static Tuple<double, double, double> RgbToHsl(double r, double g, double b)
    {
        double num = Math.Min(Math.Min(r, g), b);
        double num2 = Math.Max(Math.Max(r, g), b);
        double num3 = num2 - num;
        double num4 = (num2 + num) / 2.0;
        if (num2 == 0.0)
        {
            return new Tuple<double, double, double>(-1.0, -1.0, 0.0);
        }

        if (num3 == 0.0)
        {
            return new Tuple<double, double, double>(-1.0, 0.0, num4);
        }

        double item = ((num4 <= 0.5) ? (num3 / (num2 + num)) : (num3 / (2.0 - num2 - num)));
        double num5 = ((r == num2) ? ((g - b) / 6.0 / num3) : ((g != num2) ? (0.66666668653488159 + (r - g) / 6.0 / num3) : (0.3333333432674408 + (b - r) / 6.0 / num3)));
        if (num5 < 0.0)
        {
            num5 += 1.0;
        }

        if (num5 > 1.0)
        {
            num5 -= 1.0;
        }

        num5 *= 360.0;
        return new Tuple<double, double, double>(num5, item, num4);
    }

    public static Tuple<double, double, double> HslToRgb(double h, double s, double l)
    {
        int num = (int)(h / 60.0);
        double num2 = (h - (double)(60 * num)) / 60.0;
        double num3 = ((l < 0.5) ? (l * (1.0 + s)) : (l + s - l * s));
        double num4 = 2.0 * l - num3;
        double num5 = num3 - num4;
        return num switch
        {
            0 => new Tuple<double, double, double>(num3, num5 * num2 + num4, num4),
            1 => new Tuple<double, double, double>(num5 * (1.0 - num2) + num4, num3, num4),
            2 => new Tuple<double, double, double>(num4, num3, num5 * num2 + num4),
            3 => new Tuple<double, double, double>(num4, num5 * (1.0 - num2) + num4, num3),
            4 => new Tuple<double, double, double>(num5 * num2 + num4, num4, num3),
            _ => new Tuple<double, double, double>(num3, num4, num5 * (1.0 - num2) + num4),
        };
    }
}
