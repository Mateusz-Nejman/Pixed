using Pixed.Common.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Selection;
public class BaseSelection
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

    public SKBitmap ToBitmap()
    {
        var minX = Pixels.Min(p => p.X);
        var minY = Pixels.Min(p => p.Y);
        var maxX = Pixels.Max(p => p.X);
        var maxY = Pixels.Max(p => p.Y);

        SKBitmap bitmap = new(maxX - minX + 1, maxY - minY + 1, true);

        foreach (var pixel in Pixels)
        {
            bitmap.SetPixel(pixel.X - minX, pixel.Y - minY, (UniColor)pixel.Color);
        }

        return bitmap;
    }
}
