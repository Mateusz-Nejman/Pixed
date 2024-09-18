using Pixed.Models;
using Pixed.Services.History;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

namespace Pixed.Utils;

internal static class PaintUtils
{
    public static List<Pixel> GetSimiliarConnectedPixels(Frame frame, int x, int y)
    {
        return GetSimiliarConnectedPixels(frame.Layers[frame.SelectedLayer], x, y);
    }

    public static List<Pixel> GetSimiliarConnectedPixels(Layer layer, int x, int y)
    {
        int targetColor = layer.GetPixel(x, y);

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

    public static DynamicHistoryEntry PaintSimiliarConnected(Layer layer, int x, int y, int replacementColor)
    {
        int targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return new DynamicHistoryEntry();
        }

        DynamicHistoryEntry entry = new()
        {
            LayerId = layer.Id
        };
        var pixels = VisitConnectedPixels(layer, x, y, pixel =>
        {
            var sourceColor = layer.GetPixel(pixel.X, pixel.Y);
            if (sourceColor == targetColor)
            {
                return true;
            }

            return false;
        });

        foreach (var pixel in pixels)
        {
            layer.SetPixel(pixel.X, pixel.Y, replacementColor);
            entry.Add(pixel.X, pixel.Y, pixel.Color, replacementColor);
        }

        return entry;
    }

    public static List<Pixel> VisitConnectedPixels(Layer layer, int x, int y, Func<Pixel, bool> visitor) //TODO optimize
    {
        List<Point> toVisit = [];
        List<Point> visited = [];
        List<Pixel> pixels = [];
        int[] dy = [-1, 0, 1, 0];
        int[] dx = [0, 1, 0, -1];

        toVisit.Add(new Point(x, y));
        int color = layer.GetPixel(x, y);
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

    public static UniColor GetNoiseColor()
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        double[] colors = [0, 0.25, 0.5, 0.75, 1];
        double color = colors[(int)Math.Floor(random.NextDouble() * colors.Length)];
        return Global.PrimaryColor.Blend(Global.SecondaryColor, color);
    }

    public static DynamicHistoryEntry PaintNoiseSimiliarConnected(Layer layer, int x, int y)
    {
        int targetColor = layer.GetPixel(x, y);

        DynamicHistoryEntry entry = new()
        {
            LayerId = layer.Id
        };
        var pixels = VisitConnectedPixels(layer, x, y, pixel =>
        {
            var sourceColor = layer.GetPixel(pixel.X, pixel.Y);
            if (sourceColor == targetColor)
            {
                return true;
            }

            return false;
        });

        foreach (var pixel in pixels)
        {
            var color = GetNoiseColor();
            layer.SetPixel(pixel.X, pixel.Y, color);
            entry.Add(pixel.X, pixel.Y, pixel.Color, color);
        }

        return entry;
    }

    public static DynamicHistoryEntry OutlineSimiliarConnectedPixels(Layer layer, int x, int y, int replacementColor, bool fillCorners)
    {
        DynamicHistoryEntry entry = new DynamicHistoryEntry();
        entry.LayerId = layer.Id;

        var targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return entry;
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
        pixels = pixels.Where(neighbourCheck).ToList();

        foreach(var pixel in pixels)
        {
            int oldColor = layer.GetPixel(pixel.X, pixel.Y);
            layer.SetPixel(pixel.X, pixel.Y, replacementColor);
            entry.Add(pixel.X, pixel.Y, oldColor, replacementColor);
        }

        return entry;
    }
}
