﻿using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolPen(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    private const string PROP_VERTICAL = "Use vertical mirror";
    private const string PROP_HORIZONTAL = "Use horizontal mirror";
    private const string PROP_BOTH = "Use both vertical and horizontal mirror";
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg";
    public override string Name => "Pen tool";
    public override string Id => "tool_pen";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Pen tool", "Ctrl", PROP_VERTICAL, "Alt", PROP_HORIZONTAL, "Shift", PROP_BOTH);

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_VERTICAL),
            new ToolProperty(PROP_HORIZONTAL),
            new ToolProperty(PROP_BOTH)
        ];
    }

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        base.ApplyTool(point, frame, ref overlay, keyState);
        bool shiftPressed = keyState.IsShift || GetProperty(PROP_BOTH);
        bool controlPressed = keyState.IsCtrl || GetProperty(PROP_VERTICAL) || shiftPressed;
        bool altPressed = keyState.IsAlt || GetProperty(PROP_HORIZONTAL) || shiftPressed;

        var color = ToolColor;

        bool mirror = controlPressed || altPressed || shiftPressed;

        if (mirror)
        {
            int symX = GetSymmetricX(point.X, frame);
            int symY = GetSymmetricY(point.Y, frame);

            if (controlPressed)
            {
                DrawOnOverlay(color, new Point(symX, point.Y), frame, ref overlay);
            }

            if (altPressed)
            {
                DrawOnOverlay(color, new Point(point.X, symY), frame, ref overlay);
            }

            if (shiftPressed)
            {
                DrawOnOverlay(color, new Point(symX, symY), frame, ref overlay);
            }

            Subjects.OverlayModified.OnNext(overlay);

            _prev = point;
        }
    }

    private static int GetSymmetricX(int x, Frame frame)
    {
        return frame.Width - 1 - x;
    }

    private static int GetSymmetricY(int y, Frame frame)
    {
        return frame.Height - 1 - y;
    }
}
