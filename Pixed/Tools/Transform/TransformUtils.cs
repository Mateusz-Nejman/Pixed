using Pixed.Models;
using Pixed.Selection;
using System;
using System.Collections.Generic;

namespace Pixed.Tools.Transform;

internal static class TransformUtils
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

                pixels.Add(new Pixel(pixelX, pixelY, clone.GetPixel(x, y)));
            }
        }

        layer.SetPixels(pixels);
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

                if (clone.ContainsPixel((int)newX, (int)newY))
                {
                    pixels.Add(new Pixel(x, y, clone.GetPixel((int)newX, (int)newY)));
                }
                else
                {
                    pixels.Add(new Pixel(x, y, UniColor.Transparent));
                }
            }
        }

        layer.SetPixels(pixels);
    }

    public static void Center(Layer layer)
    {
        var boundaries = GetBoundaries([layer]);

        int bw = (boundaries[2] - boundaries[0] + 1) / 2;
        int bh = (boundaries[3] - boundaries[1] + 1) / 2;
        int fw = layer.Width / 2;
        int fh = layer.Height / 2;

        int dx = fw - bw - boundaries[0];
        int dy = fh - bh - boundaries[1];

        MoveLayerFixes(layer, dx, dy);
    }

    public static void MoveLayerFixes(Layer layer, int dx, int dy)
    {
        Layer clone = layer.Clone();
        List<Pixel> pixels = [];

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                int newX = x - dx;
                int newY = y - dy;

                if (clone.ContainsPixel(newX, newY))
                {
                    pixels.Add(new Pixel(x, y, clone.GetPixel(newX, newY)));
                }
                else
                {
                    pixels.Add(new Pixel(x, y, UniColor.Transparent));
                }
            }
        }

        layer.SetPixels(pixels);
    }

    public static int[] GetBoundaries(Layer[] layers)
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
                    int color = layer.GetPixel(x, y);

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

        return [minx, miny, maxx, maxy];
    }

    public static int[] GetBoundariesFromSelection(BaseSelection selection)
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
                minx = Math.Min(minx, pixel.X);
                maxx = Math.Max(maxx, pixel.X);
                miny = Math.Min(miny, pixel.Y);
                maxy = Math.Max(maxy, pixel.Y);
            }
        }

        return [minx, miny, maxx, maxy];
    }
}
