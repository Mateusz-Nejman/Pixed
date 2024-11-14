using Pixed.Core.Models;
using Pixed.Core.Utils;

namespace Pixed.Core.Selection;

public class RectangularSelection : BaseSelection
{
    private Point _point1;
    private Point _point2;

    public RectangularSelection(Point point1, Point point2, Frame frame) : base()
    {
        SetOrderedRectangleCoordinates(point1, point2);

        for (int x = _point1.X; x <= _point2.X; x++)
        {
            for (int y = _point1.Y; y <= _point2.Y; y++)
            {
                var point = new Point(x, y);
                Pixels.Add(new Pixel(point, frame.GetPixel(point)));
            }
        }
    }

    private void SetOrderedRectangleCoordinates(Point point1, Point point2)
    {
        var tuple = MathUtils.GetOrderedRectangle(point1, point2);
        _point1 = tuple.Item1;
        _point2 = tuple.Item2;
    }
}
