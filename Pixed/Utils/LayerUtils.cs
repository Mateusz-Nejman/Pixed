using Pixed.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Utils;

internal static class LayerUtils
{
    public static Layer Resize(Layer layer, int targetWidth, int targetHeight)
    {
        layer.Render(out SKBitmap oldBitmap);

        SKBitmap newBitmap = new(targetWidth, targetHeight, true);
        SKCanvas canvas = new SKCanvas(newBitmap);
        canvas.DrawBitmap(oldBitmap, new SKRect(0, 0, oldBitmap.Width, oldBitmap.Height), new SKRect(0, 0, targetWidth, targetHeight));
        canvas.Dispose();

        Layer newLayer = new(targetWidth, targetHeight);
        List<Pixel> pixels = [];
        for (int x = 0; x < targetWidth; x++)
        {
            for (int y = 0; y < targetHeight; y++)
            {
                pixels.Add(new Pixel(x, y, (UniColor)newBitmap.GetPixel(x, y)));
            }
        }

        newLayer.SetPixels(pixels);

        return newLayer;
    }
}
