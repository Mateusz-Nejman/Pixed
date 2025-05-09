using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools.Transform;

public static class TransformUtils
{
    public enum Axis
    {
        Vertical,
        Horizontal
    }

    public enum Direction
    {
        Clockwise,
        CounterClockwise
    }

    public static void Flip(ref Layer layer, Axis axis)
    {
        Layer clone = layer.Clone();
        List<Pixel> pixels = [];

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                int pixelX = x;
                int pixelY = y;

                if (axis == Axis.Vertical)
                {
                    pixelX = layer.Width - x - 1;
                }
                else if (axis == Axis.Horizontal)
                {
                    pixelY = layer.Height - y - 1;
                }

                pixels.Add(new Pixel(new Point(pixelX, pixelY), clone.GetPixel(new Point(x, y))));
            }
        }

        SetPixelsOpaque(layer, pixels);
    }

    public static void Rotate(ref Layer layer, Direction direction)
    {
        Layer clone = layer.Clone();
        int w = clone.Width;
        int h = clone.Height;

        int max = Math.Max(w, h);
        double deltaX = Math.Ceiling((max - w) / 2.0);
        double deltaY = Math.Ceiling((max - h) / 2.0);

        List<Pixel> pixels = [];
        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var point = new Point(x, y);
                double newX = x + deltaX;
                double newY = y + deltaY;

                double tempX = newX;
                double tempY = newY;

                if (direction == Direction.Clockwise)
                {
                    newX = tempY;
                    newY = max - 1.0 - tempX;
                }
                else if (direction == Direction.CounterClockwise)
                {
                    newY = tempX;
                    newX = max - 1.0 - tempY;
                }

                newX -= deltaX;
                newY -= deltaY;

                var newPoint = new Point((int)newX, (int)newY);
                if (clone.ContainsPixel(newPoint))
                {
                    pixels.Add(new Pixel(point, clone.GetPixel(newPoint)));
                }
                else
                {
                    pixels.Add(new Pixel(point, UniColor.Transparent));
                }
            }
        }

        SetPixelsOpaque(layer, pixels);
    }

    public static void Center(Layer layer)
    {
        var boundaries = GetBoundaries([layer]);
        var min = boundaries.Item1;
        var max = boundaries.Item2;

        int bw = (max.X - min.X + 1) / 2;
        int bh = (max.Y - min.Y + 1) / 2;
        int fw = layer.Width / 2;
        int fh = layer.Height / 2;

        MoveLayerFixes(layer, new Point(fw - bw - min.X, fh - bh - min.Y));
    }

    public static void MoveLayerFixes(Layer layer, Point diff)
    {
        Layer clone = layer.Clone();
        List<Pixel> pixels = [];

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var point = new Point(x, y);
                var newPoint = point - diff;

                if (clone.ContainsPixel(newPoint))
                {
                    pixels.Add(new Pixel(point, clone.GetPixel(newPoint)));
                }
                else
                {
                    pixels.Add(new Pixel(point, UniColor.Transparent));
                }
            }
        }

        SetPixelsOpaque(layer, pixels);
    }

    public static Tuple<Point, Point> GetBoundaries(Layer[] layers)
    {
        int minx = int.MaxValue;
        int miny = int.MaxValue;
        int maxx = 0;
        int maxy = 0;

        int transparent = UniColor.Transparent;

        for (int l = 0; l < layers.Length; l++)
        {
            Layer layer = layers[l];
            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    uint color = layer.GetPixel(new Point(x, y));

                    if (color != transparent)
                    {
                        minx = Math.Min(minx, x);
                        maxx = Math.Max(maxx, x);
                        miny = Math.Min(miny, y);
                        maxy = Math.Max(maxy, y);
                    }
                }
            }
        }

        return new Tuple<Point, Point>(new Point(minx, miny), new Point(maxx, maxy));
    }

    public static Tuple<Point, Point> GetBoundariesFromSelection(BaseSelection selection)
    {
        int minx = int.MaxValue;
        int miny = int.MaxValue;
        int maxx = 0;
        int maxy = 0;

        int transparent = UniColor.Transparent;

        foreach (var pixel in selection.Pixels)
        {
            if (pixel.Color != transparent)
            {
                minx = Math.Min(minx, pixel.Position.X);
                maxx = Math.Max(maxx, pixel.Position.X);
                miny = Math.Min(miny, pixel.Position.Y);
                maxy = Math.Max(maxy, pixel.Position.Y);
            }
        }

        return new Tuple<Point, Point>(new Point(minx, miny), new Point(maxx, maxy));
    }

    private static void SetPixelsOpaque(Layer layer, List<Pixel> pixels)
    {
        SKBitmap opaqued = new(layer.Width, layer.Height, true);
        SKCanvas opaquedCanvas = new(opaqued);
        opaquedCanvas.DrawPixels(pixels);
        opaquedCanvas.Dispose();

        var layerCanvas = layer.GetCanvas();
        layerCanvas.DrawBitmap(opaqued, SKPoint.Empty);
        layerCanvas.Dispose();
    }
}
