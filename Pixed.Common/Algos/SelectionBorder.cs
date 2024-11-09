using Pixed.Common.Models;
using Pixed.Common.Selection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Algos;
public static class SelectionBorder
{
    private readonly struct FreeSides(bool top, bool bottom, bool left, bool right)
    {
        public bool Top { get; } = top;
        public bool Bottom { get; } = bottom;
        public bool Left { get; } = left;
        public bool Right { get; } = right;
    }

    public static List<Tuple<SKPoint, SKPoint>> Get(BaseSelection selection)
    {
        var minX = selection.Pixels.Min(p => p.Position.X);
        var maxX = selection.Pixels.Max(p => p.Position.X);
        var minY = selection.Pixels.Min(p => p.Position.Y);
        var maxY = selection.Pixels.Max(p => p.Position.Y);
        int width = Math.Max(1, (maxX - minX) + 1);
        int height = Math.Max(1, (maxY - minY) + 1);

        bool[,] map = new bool[width, height];

        foreach (var pixel in selection.Pixels)
        {
            var pos = pixel.Position;
            map[pos.X - minX, pos.Y - minY] = true;
        }

        return Get(map).Select(tuple => new Tuple<SKPoint, SKPoint>(tuple.Item1 + new SKPoint(minX, minY), tuple.Item2 + new SKPoint(minX, minY))).ToList();
    }

    public static List<Tuple<SKPoint, SKPoint>> Get(bool[,] map)
    {
        List<Tuple<SKPoint, SKPoint>> border = [];

        ForEach(ref border, map, state => state.Left || state.Right, state => state.Right, new SKPoint(0, 1), new SKPoint(1, 0), false);
        ForEach(ref border, map, state => state.Top || state.Bottom, state => state.Bottom, new SKPoint(1, 0), new SKPoint(0, 1), true);

        return border;
    }

    private static void ForEach(ref List<Tuple<SKPoint, SKPoint>> border, bool[,] map, Func<FreeSides, bool> stateCheck, Func<FreeSides, bool> endStateCheck, SKPoint normalOffset, SKPoint endOffset, bool reversed)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        if (reversed)
        {
            (width, height) = (height, width);
        }

        Point getPoint(int x, int y)
        {
            if (reversed)
            {
                return new Point(y, x);
            }

            return new Point(x, y);
        }

        SKPoint getSKPoint(int x, int y)
        {
            if (reversed)
            {
                return new SKPoint(y, x);
            }

            return new SKPoint(x, y);
        }

        bool getMapCell(int x, int y)
        {
            if (reversed)
            {
                return map[y, x];
            }

            return map[x, y];
        }

        SKPoint? beginPoint = null;
        SKPoint? endPoint = null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var state = GetState(map, getPoint(x, y));
                SKPoint point = getSKPoint(x, y);

                if (getMapCell(x, y) && stateCheck(state))
                {
                    if (!beginPoint.HasValue)
                    {
                        beginPoint = point;

                        if (endStateCheck(state))
                        {
                            beginPoint += endOffset;
                        }
                        continue;
                    }

                    endPoint = point + normalOffset;

                    if (endStateCheck(state))
                    {
                        endPoint += endOffset;
                    }
                }
                else if (beginPoint.HasValue)
                {
                    if (!endPoint.HasValue)
                    {
                        endPoint = beginPoint + normalOffset;
                    }
                    border.Add(new Tuple<SKPoint, SKPoint>(beginPoint.Value, endPoint.Value));
                    endPoint = null;
                    beginPoint = null;
                }
            }

            if (beginPoint.HasValue)
            {
                if (!endPoint.HasValue)
                {
                    endPoint = beginPoint + normalOffset;
                }

                border.Add(new Tuple<SKPoint, SKPoint>(beginPoint.Value, endPoint.Value));
                endPoint = null;
                beginPoint = null;
            }
        }

        if (beginPoint.HasValue && !endPoint.HasValue)
        {
            endPoint = beginPoint + endOffset;
            border.Add(new Tuple<SKPoint, SKPoint>(beginPoint.Value, endPoint.Value));
        }
    }

    private static FreeSides GetState(bool[,] map, Point point)
    {
        bool top = true;
        bool bottom = true;
        bool left = true;
        bool right = true;

        if (point.X > 0)
        {
            left = !map[point.X - 1, point.Y];
        }

        if (point.Y > 0)
        {
            top = !map[point.X, point.Y - 1];
        }

        if (point.X < map.GetLength(0) - 1)
        {
            right = !map[point.X + 1, point.Y];
        }

        if (point.Y < map.GetLength(1) - 1)
        {
            bottom = !map[point.X, point.Y + 1];
        }

        return new FreeSides(top, bottom, left, right);
    }
}
