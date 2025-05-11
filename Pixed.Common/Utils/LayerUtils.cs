using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Utils;

public static class LayerUtils
{
    public static Layer Resize(Layer layer, Point targetSize)
    {
        var oldBitmap = layer.Render();

        Layer newLayer = new(targetSize.X, targetSize.Y);
        var canvas = newLayer.GetCanvas();
        canvas.DrawBitmap(oldBitmap, SKRect.Create(oldBitmap.Width, oldBitmap.Height), SKRect.Create(targetSize.X, targetSize.Y));
        canvas.Dispose();

        return newLayer;
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
