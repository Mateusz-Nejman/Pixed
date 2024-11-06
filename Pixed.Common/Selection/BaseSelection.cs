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

    public void Move(Point diff)
    {
        for (int i = 0; i < Pixels.Count; i++)
        {
            var pixel = Pixels[i];
            pixel.Position += diff;
            Pixels[i] = pixel;
        }
    }

    public void FillSelectionFromFrame(Frame frame)
    {
        for (int i = 0; i < Pixels.Count; i++)
        {
            var pixel = Pixels[i];

            if (!frame.ContainsPixel(pixel.Position))
            {
                continue;
            }

            pixel.Color = frame.GetPixel(Pixels[i].Position);
        }
    }

    public SKBitmap ToBitmap()
    {
        var minX = Pixels.Min(p => p.Position.X);
        var minY = Pixels.Min(p => p.Position.Y);
        var maxX = Pixels.Max(p => p.Position.X);
        var maxY = Pixels.Max(p => p.Position.Y);

        SKBitmap bitmap = new(maxX - minX + 1, maxY - minY + 1, true);

        foreach (var pixel in Pixels)
        {
            bitmap.SetPixel(pixel.Position.X - minX, pixel.Position.Y - minY, (UniColor)pixel.Color);
        }

        return bitmap;
    }
}
