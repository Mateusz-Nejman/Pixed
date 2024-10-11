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

        List<Pixel> visited = [];
        return VisitConnectedPixels(layer, x, y, p =>
        {
            if (visited.Contains(p))
            {
                return false;
            }

            visited.Add(p);
            return layer.GetPixel(p.X, p.Y) == targetColor;
        });
    }

    public static void PaintSimiliarConnected(Layer layer, int x, int y, uint replacementColor)
    {
        uint targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return;
        }

        var pixels = VisitConnectedPixels(layer, x, y, pixel =>
        {
            var sourceColor = layer.GetPixel(pixel.X, pixel.Y);
            if (sourceColor == targetColor)
            {
                return true;
            }

            return false;
        });

        pixels = pixels.Select(p => new Pixel(p.X, p.Y, replacementColor)).ToList();
        layer.SetPixels(pixels);

        Subjects.LayerModified.OnNext(layer);
    }

    public static List<Pixel> VisitConnectedPixels(Layer layer, int x, int y, Func<Pixel, bool> visitor) //TODO optimize
    {
        List<Point> toVisit = [];
        List<Point> visited = [];
        List<Pixel> pixels = [];
        int[] dy = [-1, 0, 1, 0];
        int[] dx = [0, 1, 0, -1];

        toVisit.Add(new Point(x, y));
        uint color = layer.GetPixel(x, y);
        visitor.Invoke(new Pixel(x, y, color));
        pixels.Add(new Pixel(x, y, color));

        int loopCount = 0;
        int cellCount = layer.Width * layer.Height;
        while (toVisit.Count > 0)
        {
            var current = toVisit.Pop();

            if (visited.Contains(current)) continue;

            visited.Add(current);

            loopCount++;

            for (int i = 0; i < 4; i++)
            {
                int nextX = current.X + dx[i];
                int nextY = current.Y + dy[i];
                try
                {
                    color = layer.GetPixel(nextX, nextY);
                    bool isValid = visitor(new Pixel(nextX, nextY, color));

                    if (isValid && layer.ContainsPixel(nextX, nextY))
                    {
                        toVisit.Add(new Point(nextX, nextY));
                        pixels.Add(new Pixel(nextX, nextY, color));
                    }
                }
                catch (Exception e)
                {
                    //Ignored
                }
            }

            if (loopCount > 10 * cellCount)
            {
                break;
            }
        }

        pixels = pixels.DistinctBy(p => p.X + ";" + p.Y).ToList();
        return pixels;
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

        var pixels = VisitConnectedPixels(layer, x, y, pixel =>
        {
            var sourceColor = layer.GetPixel(pixel.X, pixel.Y);
            if (sourceColor == targetColor)
            {
                return true;
            }

            return false;
        });

        pixels = pixels.Select(p => new Pixel(p.X, p.Y, GetNoiseColor(primaryColor, secondaryColor))).ToList();
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
