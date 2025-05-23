﻿using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolRectangle(ApplicationData applicationData) : ShapeTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_square_48_regular.svg";
    public override string Name => "Rectangle tool";
    public override string Id => "tool_rectangle";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Rectangle", "Shift", "1 to 1 ratio");
    protected override List<Point> GetShapePoints(Point point, bool shiftPressed)
    {
        List<Point> points = [];

        var tuple = MathUtils.GetOrderedRectangle(_start, point);
        var point1 = tuple.Item1;
        var point2 = tuple.Item2;

        if (shiftPressed)
        {
            int width = Math.Abs(point2.X - point1.X);
            int height = Math.Abs(point2.Y - point1.Y);
            int size = Math.Min(width, height);

            point2.X = point1.X + size;
            point2.Y = point1.Y + size;
        }

        for (int rx = point1.X; rx <= point2.X; rx++)
        {
            for (int ry = point1.Y; ry <= point2.Y; ry++)
            {
                if (rx == point1.X || rx == point2.X || ry == point1.Y || ry == point2.Y)
                {
                    points.Add(new Point(rx, ry));
                }
            }
        }

        return points;
    }
}
