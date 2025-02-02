using Pixed.Core;
using Pixed.Core.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Utils;

public static class LayerUtils
{
    public static Layer Resize(Layer layer, Point targetSize)
    {
        layer.Render(out SKBitmap oldBitmap);

        SKBitmap newBitmap = new(targetSize.X, targetSize.Y, true);
        SKCanvas canvas = new(newBitmap);
        canvas.DrawBitmap(oldBitmap, SKRect.Create(oldBitmap.Width, oldBitmap.Height), SKRect.Create(targetSize.X, targetSize.Y));
        canvas.Dispose();

        Layer newLayer = new(targetSize.X, targetSize.Y);
        List<Pixel> pixels = [];
        for (int x = 0; x < targetSize.X; x++)
        {
            for (int y = 0; y < targetSize.Y; y++)
            {
                pixels.Add(new Pixel(new Point(x, y), (UniColor)newBitmap.GetPixel(x, y)));
            }
        }

        newLayer.SetPixels(pixels);

        return newLayer;
    }

    public static uint[] GetRectangleColors(this Layer layer, Point point, Point size)
    {
        var pixels = layer.GetPixels();

        return pixels.GetRectangleColors(new Point(layer.Width, layer.Height), point, size);
    }

    public static uint[] GetRectangleColors(this uint[] pixels, Point sourceSize, Point point, Point destinationSize)
    {
        if (point.X + destinationSize.X > sourceSize.X || point.Y + destinationSize.Y > sourceSize.Y)
        {
            return [];
        }

        IList<uint> colors = [];

        for (int y = point.Y; y < point.Y + destinationSize.Y; y++)
        {
            for (int x = point.X; x < point.X + destinationSize.X; x++)
            {
                colors.Add(pixels[y * sourceSize.X + x]);
            }
        }

        return [.. colors];
    }
}
