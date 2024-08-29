using Pixed.Models;
using Pixed.Services.History;
using System.Drawing;

namespace Pixed.Utils
{
    internal static class PaintUtils
    {
        public static DynamicHistoryEntry PaintSimiliarConnected(Layer layer, int x, int y, int replacementColor)
        {
            int targetColor = layer.GetPixel(x, y);

            if (targetColor == replacementColor)
            {
                return new DynamicHistoryEntry();
            }

            DynamicHistoryEntry paintedPixels = VisitConnectedPixels(layer, x, y, pixel =>
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

        public static DynamicHistoryEntry VisitConnectedPixels(Layer layer, int x, int y, Func<Point, bool> visitor)
        {
            Queue<Point> queue = [];
            DynamicHistoryEntry entry = new DynamicHistoryEntry();
            entry.LayerId = layer.Id;
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
    }
}
