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
}
