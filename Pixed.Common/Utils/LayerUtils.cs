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

        if (point.X + size.X > layer.Width || point.Y + size.Y > layer.Height)
        {
            return [];
        }

        IList<uint> colors = [];

        for (int y = point.Y; y < point.Y + size.Y; y++)
        {
            for (int x = point.X; x < point.X + size.X; x++)
            {
                colors.Add(pixels[y * layer.Width + x]);
            }
        }

        return [.. colors];
    }
}
