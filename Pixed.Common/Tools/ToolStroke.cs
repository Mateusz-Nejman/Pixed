﻿using Pixed.Common.Algos;
using Pixed.Common.Models;
using Pixed.Common.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Common.Tools;
public class ToolStroke : ShapeTool
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-stroke.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Line", "Shift", "Draw straight lines");
    public ToolStroke(ApplicationData applicationData) : base(applicationData)
    {
        PROP_SHIFT = "Draw straight lines";
    }
    protected override void Draw(int x, int y, uint color, bool isShift, Action<int, int, uint> setPixelAction)
    {
        List<Point> linePixels;

        if (isShift)
        {
            linePixels = MathUtils.GetUniformLinePixels(_startX, _startY, x, y);
        }
        else
        {
            linePixels = BresenhamLine.Get(x, y, _startX, _startY);
        }

        setPixelAction.Invoke(linePixels[0].X, linePixels[0].Y, color);
        setPixelAction.Invoke(linePixels[^1].X, linePixels[^1].Y, color);

        foreach (var point in linePixels)
        {
            setPixelAction.Invoke(point.X, point.Y, color);
        }
    }
}
