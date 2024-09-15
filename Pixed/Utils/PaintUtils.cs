using Pixed.Models;
using Pixed.Services.History;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Utils;

internal static class PaintUtils
{
    public static List<Pixel> GetSimiliarConnectedPixels(Frame frame, int x, int y)
    {
        int targetColor = frame.GetPixel(x, y);

        List<Point> visited = [];
        return VisitConnectedPixels(frame.Layers[frame.SelectedLayer], x, y, p =>
        {
            if (visited.Contains(p))
            {
                return false;
            }

            visited.Add(p);
            return frame.GetPixel(p.X, p.Y) == targetColor;
        });
    }
    public static DynamicHistoryEntry PaintSimiliarConnected(Layer layer, int x, int y, int replacementColor)
    {
        int targetColor = layer.GetPixel(x, y);

        if (targetColor == replacementColor)
        {
            return new DynamicHistoryEntry();
        }

        DynamicHistoryEntry paintedPixels = VisitConnectedPixelsHistory(layer, x, y, pixel =>
        {
            var sourceColor = layer.GetPixel(pixel.X, pixel.Y);
            if (sourceColor == targetColor)
            {
                layer.SetPixel(pixel.X, pixel.Y, replacementColor);
                return true;
            }

            return false;
        });

        return paintedPixels;
    }

    public static DynamicHistoryEntry VisitConnectedPixelsHistory(Layer layer, int x, int y, Func<Point, bool> visitor)
    {
        Queue<Point> queue = [];
        DynamicHistoryEntry entry = new()
        {
            LayerId = layer.Id
        };
        int[] dy = [-1, 0, 1, 0];
        int[] dx = [0, 1, 0, -1];

        queue.Enqueue(new Point(x, y));
        int oldColor = layer.GetPixel(x, y);
        visitor.Invoke(new Point(x, y));
        int newColor = layer.GetPixel(x, y);
        entry.Add(x, y, oldColor, newColor);

        int loopCount = 0;
        int cellCount = layer.Width * layer.Height;
        while (queue.Count > 0)
        {
            loopCount++;

            var current = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nextX = current.X + dx[i];
                int nextY = current.Y + dy[i];
                try
                {
                    oldColor = layer.GetPixel(nextX, nextY);
                    bool isValid = visitor(new Point(nextX, nextY));
                    newColor = layer.GetPixel(nextX, nextY);

                    if (isValid)
                    {
                        queue.Enqueue(new Point(nextX, nextY));
                        entry.Add(nextX, nextY, oldColor, newColor);
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

        return entry;
    }

    public static List<Pixel> VisitConnectedPixels(Layer layer, int x, int y, Func<Point, bool> visitor)
    {
        Queue<Point> queue = [];
        List<Pixel> points = [];
        int[] dy = [-1, 0, 1, 0];
        int[] dx = [0, 1, 0, -1];

        queue.Enqueue(new Point(x, y));
        visitor.Invoke(new Point(x, y));
        points.Add(new Pixel(x, y, layer.GetPixel(x, y)));

        int loopCount = 0;
        int cellCount = layer.Width * layer.Height;
        while (queue.Count > 0)
        {
            loopCount++;

            var current = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nextX = current.X + dx[i];
                int nextY = current.Y + dy[i];
                try
                {
                    bool isValid = visitor(new Point(nextX, nextY));

                    if (isValid)
                    {
                        queue.Enqueue(new Point(nextX, nextY));
                        points.Add(new Pixel(nextX, nextY, layer.GetPixel(nextX, nextY)));
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

        return points;
    }
}
