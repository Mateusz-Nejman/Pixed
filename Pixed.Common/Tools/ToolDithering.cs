﻿using Pixed.Common.Input;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolDithering(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_transparency_square_24_regular.svg";
    public override string Name => "Dithering tool";
    public override string Id => "tool_dithering";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Dithering");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _prev = point;
        var toolPoints = PaintUtils.GetToolPoints(point, _applicationData.ToolSize);

        foreach (var toolPoint in toolPoints)
        {
            if (!frame.ContainsPixel(toolPoint))
            {
                continue;
            }

            bool usePrimary = (toolPoint.X + toolPoint.Y) % 2 != 0;

            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                usePrimary = !usePrimary;
            }

            var color = usePrimary ? _applicationData.PrimaryColor : _applicationData.SecondaryColor;
            AddPixel(toolPoint, color);
        }
    }
}
