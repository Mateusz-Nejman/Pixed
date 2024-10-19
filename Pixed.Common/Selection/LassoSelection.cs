using Pixed.Common.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Pixed.Common.Selection;

internal class LassoSelection : BaseSelection
{

    public LassoSelection(List<Point> pixels, Frame frame) : base()
    {
        Pixels.Clear();
        if (pixels.Count <= 4)
        {
            Pixels.AddRange(pixels.Select(p => new Pixel(p.X, p.Y, frame.GetPixel(p.X, p.Y))));
        }
        else
        {
            var lasso = GetLassoPixels(pixels, frame);
            Pixels.AddRange(lasso);
        }
    }

    private List<Pixel> GetLassoPixels(List<Point> lassoPoints, Frame frame)
    {
        Point maxTopPoint = lassoPoints.MinBy(p => p.Y);
        Point maxBottomPoint = lassoPoints.MaxBy(p => p.Y);

        List<Pixel> lassoPixels = [];
        for (int y = maxTopPoint.Y; y <= maxBottomPoint.Y; y++)
        {
            var row = lassoPoints.Where(p => p.Y == y);
            int minX = row.Min(p => p.X);
            int maxX = row.Max(p => p.X);

            for (int x = minX; x <= maxX; x++)
            {
                lassoPixels.Add(new Pixel(x, y, frame.GetPixel(x, y)));
            }
        }

        return lassoPixels;
    }
}