using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolStroke : ShapeTool
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_line_48_regular.svg";
    public override string Name => "Line tool";
    public override string Id => "tool_stroke";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Line", "Shift", "Draw straight lines");
    public ToolStroke(ApplicationData applicationData) : base(applicationData)
    {
        PROP_SHIFT = "Draw straight lines";
    }

    protected override List<Point> GetShapePoints(Point point, bool shiftPressed)
    {
        if (shiftPressed)
        {
            return MathUtils.GetUniformLinePixels(_start, point);
        }

        return BresenhamLine.Get(point, _start);
    }
}
