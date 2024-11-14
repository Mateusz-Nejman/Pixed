using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;

namespace Pixed.Common.Tools;
public class ToolCircle(ApplicationData applicationData) : ShapeTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-circle.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Circle", "Shift", "1 to 1 ratio");
    protected override void Draw(Point point, uint color, bool isShift, Action<Point, uint> setPixelAction)
    {
        var tuple = MathUtils.GetOrderedRectangle(_start, point);
        var point1 = tuple.Item1;
        var point2 = tuple.Item2;

        if (isShift)
        {
            int width = Math.Abs(point2.X - point1.X);
            int height = Math.Abs(point2.Y - point1.Y);
            int size = Math.Min(width, height);

            point2 = point1 + new Point(size);
        }

        var circle = MathUtils.GetCircle(point1, point2);

        foreach (var p in circle)
        {
            setPixelAction?.Invoke(p, color);
        }
    }
}
