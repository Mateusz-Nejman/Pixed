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
    protected override void Draw(Point point, uint color, bool isShift, Action<Point, uint> setPixelAction)
    {
        List<Point> linePixels;

        if (isShift)
        {
            linePixels = MathUtils.GetUniformLinePixels(_start, point);
        }
        else
        {
            linePixels = BresenhamLine.Get(point, _start);
        }

        setPixelAction.Invoke(linePixels[0], color);
        setPixelAction.Invoke(linePixels[^1], color);

        foreach (var p in linePixels)
        {
            setPixelAction.Invoke(p, color);
        }
    }
}
