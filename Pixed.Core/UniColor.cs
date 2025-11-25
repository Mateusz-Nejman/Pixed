using Avalonia;

namespace Pixed.Core;

public struct UniColor(byte alpha, byte red, byte green, byte blue) : IEquatable<UniColor>
{
    public readonly struct Hsl(double h, double s, double l)
    {
        public double H { get; } = h;
        public double S { get; } = s;
        public double L { get; } = l;

        public override string ToString()
        {
            return "[" + H + ", " + S + ", " + L + "]";
        }
    }

    public readonly static UniColor Transparent = new();
    public readonly static UniColor Black = new(255, 0);
    public readonly static UniColor White = new(255, 255);
    public readonly static UniColor CornflowerBlue = new(100, 149, 237);

    public byte A { get; set; } = alpha;
    public byte R { get; set; } = red;
    public byte B { get; set; } = blue;
    public byte G { get; set; } = green;
    public readonly bool IsGrayscale => R == G && G == B;
    public UniColor() : this(0, 0, 0, 0)
    { }

    public UniColor(byte alpha, byte value) : this(alpha, value, value, value)
    { }

    public UniColor(byte alpha, UniColor color) : this(alpha, color.R, color.G, color.B)
    { }

    public UniColor(byte value) : this(255, value)
    { }

    public UniColor(byte red, byte green, byte blue) : this(255, red, green, blue)
    { }

    public UniColor(int red, int green, int blue) : this((byte)red, (byte)green, (byte)blue) { }
    public UniColor(double alpha, double red, double green, double blue) : this()
    {
        A = (byte)(alpha * 255d);
        R = (byte)(red * 255d);
        G = (byte)(green * 255d);
        B = (byte)(blue * 255d);
    }

    public UniColor(uint argb) : this()
    {
        A = (byte)(argb >> 24 & 0xFFu);
        R = (byte)(argb >> 16 & 0xFFu);
        G = (byte)(argb >> 8 & 0xFFu);
        B = (byte)(argb & 0xFFu);
    }

    public UniColor(int argb) : this()
    {
        A = (byte)(argb >> 24 & 0xFF);
        R = (byte)(argb >> 16 & 0xFF);
        G = (byte)(argb >> 8 & 0xFF);
        B = (byte)(argb & 0xFF);
    }

    public UniColor(Hsl hsl) : this(255, hsl) { }

    public UniColor(byte alpha, Hsl hsl) : this()
    {
        var hueCircleSegment = (int)(hsl.H / 60);
        var circleSegmentFraction = (hsl.H - 60 * hueCircleSegment) / 60;

        var maxRGB = hsl.L < 0.5 ? hsl.L * (1 + hsl.S) : hsl.L + hsl.S - hsl.L * hsl.S;
        var minRGB = 2 * hsl.L - maxRGB;
        var delta = maxRGB - minRGB;
        double r, g, b;
        switch (hueCircleSegment)
        {
            case 0:
                r = maxRGB * 255d;
                g = (delta * circleSegmentFraction + minRGB) * 255d;
                b = minRGB * 255d;
                break;
            case 1:
                r = (delta * (1 - circleSegmentFraction) + minRGB) * 255d;
                g = maxRGB * 255d;
                b = minRGB * 255d;
                break;
            case 2:
                r = minRGB * 255d;
                g = maxRGB * 255d;
                b = (delta * circleSegmentFraction + minRGB) * 255d;
                break;
            case 3:
                r = minRGB * 255d;
                g = (delta * (1 - circleSegmentFraction) + minRGB) * 255d;
                b = maxRGB * 255d;
                break;
            case 4:
                r = (delta * circleSegmentFraction + minRGB) * 255d;
                g = minRGB * 255d;
                b = maxRGB * 255d;
                break;
            default:
                r = maxRGB * 255d;
                g = minRGB * 255d;
                b = (delta * (1 - circleSegmentFraction) + minRGB) * 255d;
                break;
        }

        A = alpha;
        R = (byte)Math.Round(r);
        G = (byte)Math.Round(g);
        B = (byte)Math.Round(b);
    }

    public readonly Hsl ToHsl()
    {
        double r = R / 255d;
        double g = G / 255d;
        double b = B / 255d;

        double h, s, l;

        var min = Math.Min(Math.Min(r, g), b);
        var max = Math.Max(Math.Max(r, g), b);
        var delta = max - min;
        l = (max + min) / 2;

        if (max == 0)
            //pure black
            return new Hsl(-1, -1, 0);

        if (delta == 0)
            //gray
            return new Hsl(-1, 0, l);

        //magic
        s = l <= 0.5 ? delta / (max + min) : delta / (2 - max - min);

        if (r == max)
            h = (g - b) / 6 / delta;
        else if (g == max)
            h = 1.0f / 3 + (b - r) / 6 / delta;
        else
            h = 2.0f / 3 + (r - g) / 6 / delta;

        if (h < 0)
            h += 1;
        if (h > 1)
            h -= 1;

        h *= 360;

        return new Hsl(h, s, l);
    }

    public readonly UniColor Darken(int amount)
    {
        var hsl = ToHsl();
        double l = hsl.L - amount / 100d;
        l = Math.Min(1.0d, Math.Max(0d, l));
        hsl = new Hsl(hsl.H, hsl.S, l);
        return new UniColor(hsl);
    }

    public readonly UniColor Lighten(int amount)
    {
        var hsl = ToHsl();
        double l = hsl.L + amount / 100d;
        l = Math.Min(1.0d, Math.Max(0d, l));
        hsl = new Hsl(hsl.H, hsl.S, l);
        return new UniColor(hsl);
    }

    public readonly UniColor Blend(UniColor color, double amount)
    {
        byte r = (byte)(R * amount + color.R * (1 - amount));
        byte g = (byte)(G * amount + color.G * (1 - amount));
        byte b = (byte)(B * amount + color.B * (1 - amount));
        return new UniColor(r, g, b);
    }

    public readonly uint ToUInt()
    {
        return (uint)(A << 24 | R << 16 | G << 8 | B);
    }

    public readonly bool Equals(UniColor other)
    {
        return other.R == R && other.G == G && other.B == B && other.A == A;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is UniColor color && Equals(color);
    }

    public override readonly string ToString()
    {
        return "[" + A + ", " + R + ", " + G + ", " + B + "]";
    }

    public static UniColor WithAlpha(byte alpha, UniColor color)
    {
        return new UniColor(alpha, color.R, color.G, color.B);
    }

    public static UniColor WithAlpha(byte alpha, int color)
    {
        UniColor uniColor = color;
        uniColor.A = alpha;
        return uniColor;
    }

    public static implicit operator UniColor(int value)
    {
        return new UniColor(value);
    }

    public static implicit operator UniColor(uint value)
    {
        return new UniColor(value);
    }

    public static implicit operator UniColor(SkiaSharp.SKColor color)
    {
        return new UniColor(color.Alpha, color.Red, color.Green, color.Blue);
    }

    public static implicit operator UniColor(System.Drawing.Color color)
    {
        return new UniColor(color.A, color.R, color.G, color.B);
    }

    public static implicit operator UniColor(Avalonia.Media.Color color)
    {
        return new UniColor(color.A, color.R, color.G, color.B);
    }

    public static implicit operator int(UniColor color)
    {
        int value = 0;
        value |= color.B;
        value |= color.G << 8;
        value |= color.R << 16;
        value |= color.A << 24;

        return value;
    }

    public static implicit operator uint(UniColor color)
    {
        uint value = 0;
        value |= color.B;
        value |= (uint)color.G << 8;
        value |= (uint)color.R << 16;
        value |= (uint)color.A << 24;

        return value;
    }

    public static implicit operator SkiaSharp.SKColor(UniColor color)
    {
        return new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
    }

    public static implicit operator System.Drawing.Color(UniColor color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static implicit operator Avalonia.Media.Color(UniColor color)
    {
        return Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static UniColor GetFromResources(string name)
    {
        if (Application.Current != null && Application.Current.Resources.TryGetResource(name, Application.Current.ActualThemeVariant, out object? obj))
        {
            if (obj is Avalonia.Media.Color color)
            {
                return color;
            }
        }
        return new UniColor();
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(A, R, G, B);
    }
}
