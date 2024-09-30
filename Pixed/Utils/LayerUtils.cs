using Pixed.Models;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Utils;

internal static class LayerUtils
{
    public static Layer Resize(Layer layer, int targetWidth, int targetHeight)
    {
        var oldBitmap = layer.Render();

        Bitmap newBitmap = new(targetWidth, targetHeight);
        Graphics graphics = Graphics.FromImage(newBitmap);
        graphics.DrawImage(oldBitmap, new Rectangle(0, 0, targetWidth, targetHeight), new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height), GraphicsUnit.Pixel);
        graphics.Dispose();

        Layer newLayer = new(targetWidth, targetHeight);
        List<Pixel> pixels = [];
        for (int x = 0; x < targetWidth; x++)
        {
            for (int y = 0; y < targetHeight; y++)
            {
                pixels.Add(new Pixel(x, y, newBitmap.GetPixel(x, y).ToArgb()));
            }
        }

        newLayer.SetPixels(pixels);

        return newLayer;
    }
}
