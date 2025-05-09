using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolCircle(ApplicationData applicationData) : ShapeTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_circle_48_regular.svg";
    public override string Name => "Circle tool";
    public override string Id => "tool_circle";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Circle", "Shift", "1 to 1 ratio");
    protected override List<Point> GetShapePoints(Point point, bool shiftPressed)
    {
        var tuple = MathUtils.GetOrderedRectangle(_start, point);
        var point1 = tuple.Item1;
        var point2 = tuple.Item2;

        if (shiftPressed)
        {
            int width = Math.Abs(point2.X - point1.X);
            int height = Math.Abs(point2.Y - point1.Y);
            int size = Math.Min(width, height);

            point2 = point1 + new Point(size);
        }

        return MathUtils.GetCircle(point1, point2);
    }
}
