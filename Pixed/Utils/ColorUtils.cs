using System.Windows.Media;

namespace Pixed.Utils
{
    internal static class ColorUtils
    {
        public readonly struct Hsl(float h, float s, float l)
        {
            public float H { get; } = h;
            public float S { get; } = s;
            public float L { get; } = l;
        }
        public static Hsl ToHsl(int color)
        {
            return ToHsl(System.Drawing.Color.FromArgb(color));
        }

        public static Hsl ToHsl(Color color)
        {
            float r = color.R;
            float g = color.G;
            float b = color.B;

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

        public static Hsl ToHsl(System.Drawing.Color color)
        {
            float r = color.R;
            float g = color.G;
            float b = color.B;

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
    }
}
