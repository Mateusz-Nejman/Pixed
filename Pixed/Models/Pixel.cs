namespace Pixed.Models
{
    internal class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
            Color = Constants.TRANSPARENT_COLOR.ToArgb();
        }

        public Pixel(int x, int y, int color) : this(x, y)
        {
            Color = color;
        }
    }
}
