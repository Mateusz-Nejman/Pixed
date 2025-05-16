using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;
public class ToolLighten(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    private const string PROP_DARKEN = "Darken";

    private readonly ConcurrentBag<Pixel> _modified = [];
    private SKBitmap? _render = null;

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_subtract_circle_48_regular.svg";
    public override string Name => "Lighten tool";
    public override string Id => "tool_lighten";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lighten", "Shift", "Darken");
    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();
        var frame = model.CurrentFrame;
        var shiftPressed = keyState.IsShift || GetProperty(PROP_DARKEN);

        if (!frame.ContainsPixel(point))
        {
            return;
        }

        _prev = point;

        if (!_modified.Contains(p => p.Position == point))
        {
            if (_handle == null)
            {
                _render = frame.CurrentLayer.Render();
                _handle = frame.GetHandle();
            }
            _modified.AddRange(GetModifiedPixels(point, frame, shiftPressed, selection));
        }
    }

    public override void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        _handle?.SetPixels([.. _modified]);
        base.ToolEnd(point, model, keyState, selection);
        _modified.Clear();
        _render?.Dispose();
        _render = null;
    }

    public override void OnOverlay(SKCanvas canvas)
    {
        base.OnOverlay(canvas);
        canvas.DrawPixels([.. _modified]);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_DARKEN),
        ];
    }
    private List<Pixel> GetModifiedPixels(Point point, Frame frame, bool isDarken, BaseSelection? selection)
    {
        if (selection != null && !selection.InSelection(point))
        {
            return [];
        }

        var points = PaintUtils.GetToolPoints(point, _applicationData.ToolSize).Where(p => !_modified.Contains(p1 => p1.Position == p));

        if (selection != null)
        {
            points = [.. points.Where(selection.InSelection)];
        }

        List<Pixel> pixels = [];

        foreach (var p in points)
        {
            UniColor pixelColor = frame.GetPixel(p);
            pixels.Add(new Pixel(p, isDarken ? pixelColor.Darken(6) : pixelColor.Lighten(6)));
        }

        return pixels;
    }
}
