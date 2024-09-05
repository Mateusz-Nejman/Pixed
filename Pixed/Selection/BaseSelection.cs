using Pixed.Models;
using System.Drawing;

namespace Pixed.Selection
{
    internal class BaseSelection
    {
        public List<Pixel> Pixels { get; }

        public BaseSelection()
        {
            Pixels = [];
            Reset();
        }

        public void Reset()
        {
            Pixels.Clear();
        }

        public void Move(int xDiff, int yDiff)
        {
            for (int i = 0; i < Pixels.Count; i++)
            {
                var pixel = Pixels[i];
                pixel.X += xDiff;
                pixel.Y += yDiff;
                Pixels[i] = pixel;
            }
        }

        public void FillSelectionFromFrame(Frame frame)
        {
            for (int i = 0; i < Pixels.Count; i++)
            {
                var pixel = Pixels[i];

                if (!frame.ContainsPixel(pixel.X, pixel.Y))
                {
                    continue;
                }

                pixel.Color = frame.GetPixel(Pixels[i].X, Pixels[i].Y);
            }
        }

        public Bitmap ToBitmap()
        {
            var minX = Pixels.Min(p => p.X);
            var minY = Pixels.Min(p => p.Y);
            var maxX = Pixels.Max(p => p.X);
            var maxY = Pixels.Max(p => p.Y);

            Bitmap bitmap = new((maxX - minX) + 1, (maxY - minY) + 1);

            foreach (var pixel in Pixels)
            {
                bitmap.SetPixel(pixel.X - minX, pixel.Y - minY, (UniColor)pixel.Color);
            }

            return bitmap;
        }
    }
}
