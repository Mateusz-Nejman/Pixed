using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Pixed.Utils;

internal static class PaintUtils
{
    public static List<Point> GetToolPoints(int x, int y, int toolSize)
    {
        List<Point> points = [];
        for (int y1 = 0; y1 < toolSize; y1++)
        {
            for (int x1 = 0; x1 < toolSize; x1++)
            {
                points.Add(new(x - (int)Math.Floor((double)toolSize / 2.0d) + x1, y - (int)Math.Floor((double)toolSize / 2.0d) + y1));
            }
        }

        return points;
    }
    public static List<Pixel> GetSimiliarConnectedPixels(Frame frame, int x, int y)
    {
        return GetSimiliarConnectedPixels(frame.CurrentLayer, x, y);
    }

    public static List<Pixel> GetSimiliarConnectedPixels(Layer layer, int x, int y)
    {
        uint targetColor = layer.GetPixel(x, y);

        var points = MathUtils.FloodFill(new Point(x, y), new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos.X, pos.Y) == targetColor);
        var pixels = points.Select(point => new Pixel(point.X, point.Y, targetColor)).ToList();
        return pixels;
    }

    public static void PaintSimiliarConnected(Layer layer, int x, int y, uint replacementColor)
    {
        uint targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return;
        }

        var points = MathUtils.FloodFill(new Point(x, y), new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos.X, pos.Y) == targetColor);
        var pixels = points.Select(p => new Pixel(p.X, p.Y, replacementColor)).ToList();
        layer.SetPixels(pixels);
    }

    public static UniColor GetNoiseColor(UniColor primary, UniColor secondary)
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        double[] colors = [0, 0.25, 0.5, 0.75, 1];
        double color = colors[(int)Math.Floor(random.NextDouble() * colors.Length)];
        return primary.Blend(secondary, color);
    }

    public static void PaintNoiseSimiliarConnected(Layer layer, int x, int y, UniColor primaryColor, UniColor secondaryColor)
    {
        uint targetColor = layer.GetPixel(x, y);

        var points = MathUtils.FloodFill(new Point(x, y), new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos.X, pos.Y) == targetColor);
        var pixels = points.Select(point => new Pixel(point.X, point.Y, GetNoiseColor(primaryColor, secondaryColor))).ToList();
        layer.SetPixels(pixels);
    }

    public static void OutlineSimiliarConnectedPixels(Layer layer, int x, int y, uint replacementColor, bool fillCorners)
    {
        var targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return;
        }

        bool neighbourCheck(Pixel pixel)
        {
            for (int y1 = -1; y1 <= 1; y1++)
            {
                for (int x1 = -1; x1 <= 1; x1++)
                {
                    if (!layer.ContainsPixel(pixel.X + x1, pixel.Y + y1))
                    {
                        continue;
                    }

                    if (fillCorners || (x1 == 0 || y1 == 0))
                    {
                        var pixelColor = layer.GetPixel(pixel.X + x1, pixel.Y + y1);

                        if (pixelColor != targetColor)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        var pixels = GetSimiliarConnectedPixels(layer, x, y);
        pixels = pixels.Where(neighbourCheck).Select(p => new Pixel(p.X, p.Y, replacementColor)).ToList();

        layer.SetPixels(pixels);
    }
}
