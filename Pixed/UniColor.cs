using System;

namespace Pixed
{
    internal struct UniColor : IEquatable<UniColor>
    {
        public readonly struct Hsl(float h, float s, float l)
        {
            public float H { get; } = h;
            public float S { get; } = s;
            public float L { get; } = l;
        }

        public readonly static UniColor Transparent = new();
        public readonly static UniColor Black = new(255, 0);
        public readonly static UniColor White = new(255, 255);
        public readonly static UniColor CornflowerBlue = new(100, 149, 237);

        public byte A { get; private set; }
        public byte R { get; private set; }
        public byte B { get; private set; }
        public byte G { get; private set; }
        public UniColor() : this(0, 0, 0, 0)
        { }

        public UniColor(byte alpha, byte value) : this(alpha, value, value, value)
        { }

        public UniColor(byte value) : this(255, value)
        { }

        public UniColor(byte red, byte green, byte blue) : this(255, red, green, blue)
        { }

        public UniColor(byte alpha, byte red, byte green, byte blue)
        {
            A = alpha;
            R = red;
            B = blue;
            G = green;
        }

        public UniColor(int red, int green, int blue) : this((byte)red, (byte)green, (byte)blue) { }

        public Hsl ToHsl()
        {
            float r = R;
            float g = G;
            float b = B;

            r /= 255f;
            g /= 255f;
            b /= 255f;

            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;
            float h;
            if (delta == 0)
            {
                h = 0;
            }
            else if (max == r)
            {
                h = ((g - b) / delta) % 6;
            }
            else if (max == g)
            {
                h = (b - r) / delta + 2;
            }
            else
            {
                h = (r - g) / delta + 4;
            }

            h = MathF.Round(h * 60f);

            if (h < 0)
            {
                h += 360f;
            }

            float l = (max + min) / 2;
            float s = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * l - 1));

            s = +(s * 100);
            l = +(l * 100);

            return new Hsl(h, s, l);
        }

        public int ToInt()
        {
            return (int)this;
        }

        public bool Equals(UniColor other)
        {
            return (other == null && this == null) || (other.R == R && other.G == G && other.B == B && other.A == A);
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
            byte[] bgra = BitConverter.GetBytes(value);
            return new UniColor(bgra[3], bgra[2], bgra[1], bgra[0]);
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
            byte[] bgra = [color.B, color.G, color.R, color.A];
            return BitConverter.ToInt32(bgra, 0);
        }

        public static implicit operator System.Drawing.Color(UniColor color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static implicit operator Avalonia.Media.Color(UniColor color)
        {
            return Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
