using Pixed.Models;
using Pixed.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Selection;

internal class LassoSelection : BaseSelection
{
    private const int OUTSIDE = -1;
    private const int INSIDE = 1;
    private const int VISITED = 2;

    private readonly int[,] _pixels;

    public LassoSelection(List<Point> pixels, Frame frame) : base()
    {
        _pixels = new int[frame.Width, frame.Height];

        foreach (var p in pixels)
        {
            SetPixel(p, INSIDE);
        }

        Pixels.Clear();
        Pixels.AddRange(GetLassoPixels(frame));
    }

    private List<Pixel> GetLassoPixels(Frame frame)
    {
        List<Pixel> pixels = [];

        for (int x = 0; x < frame.Width; x++)
        {
            for (int y = 0; y < frame.Height; y++)
            {
                if (IsInSelection(new Point(x, y), frame))
                {
                    pixels.Add(new Pixel(x, y, frame.GetPixel(x, y)));
                }
            }
        }

        return pixels;
    }

    private bool IsInSelection(Point point, Frame frame)
    {
        bool visited = GetPixel(point) == VISITED;

        if (!visited)
        {
            VisitPixel(point, frame);
        }

        return GetPixel(point) == INSIDE;
    }

    private void VisitPixel(Point point, Frame frame)
    {
        bool frameBorderReached = false;
        var entry = PaintUtils.VisitConnectedPixelsHistory(frame.Layers[frame.SelectedLayer], point.X, point.Y, p =>
        {
            var alreadyVisited = GetPixel(point);

            if (alreadyVisited == VISITED)
            {
                return false;
            }

            if (!frame.ContainsPixel(point.X, point.Y))
            {
                frameBorderReached = true;
                return false;
            }

            SetPixel(point, VISITED);
            return true;
        });

        for (int a = 0; a < entry.PixelX.Count; a++)
        {
            int x = entry.PixelX[a];
            int y = entry.PixelY[a];

            SetPixel(new Point(x, y), frameBorderReached ? OUTSIDE : INSIDE);
        }
    }

    private void SetPixel(Point point, int value)
    {
        _pixels[point.X, point.Y] = value;
    }

    private int GetPixel(Point point)
    {
        return _pixels[point.X, point.Y];
    }
}