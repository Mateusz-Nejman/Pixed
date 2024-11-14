using Pixed.Core.Models;

namespace Pixed.Core.Selection;

public class LassoSelection : BaseSelection
{

    public LassoSelection(List<Point> pixels, Frame frame) : base()
    {
        Pixels.Clear();
        if (pixels.Count <= 4)
        {
            Pixels.AddRange(pixels.Select(p => new Pixel(p, frame.GetPixel(p))));
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
                var point = new Point(x, y);
                lassoPixels.Add(new Pixel(point, frame.GetPixel(point)));
            }
        }

        return lassoPixels;
    }
}