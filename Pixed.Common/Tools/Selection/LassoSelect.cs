using Pixed.Common.Algorithms;
using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools.Selection;

internal class LassoSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    private Point _prev;
    private List<Point> _points = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-lasso-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lasso selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnDragSelectStart(Point point, Frame frame, ref SKBitmap overlay)
    {
        _points.Clear();
        _points.Add(point);

        _start = point;
        _prev = point;
    }

    public override void OnDragSelect(Point point, Frame frame, ref SKBitmap overlay)
    {
        AddPixel(point, frame);
        List<Pixel> pixels = [];

        foreach (var p in GetLassoPixels())
        {
            pixels.Add(new Pixel(p, frame.GetPixel(p)));
        }
        BaseSelection selection = new ShapeSelection(pixels);
        SetSelection(selection, ref overlay);
    }

    public override void OnDragSelectEnd(Point point, Frame frame, ref SKBitmap overlay)
    {
        AddPixel(point, frame);
        BaseSelection selection = new LassoSelection(GetLassoPixels(), frame);
        SetSelection(selection, ref overlay);
    }

    private List<Point> GetLassoPixels()
    {
        var line = BresenhamLine.Get(_prev, _start);
        return [.. _points, .. line];
    }

    private void AddPixel(Point point, Frame frame)
    {
        point.X = Math.Clamp(point.X, 0, frame.Width - 1);
        point.Y = Math.Clamp(point.Y, 0, frame.Height - 1);

        var interpolated = BresenhamLine.Get(point, _prev);
        _points = [.. _points, .. interpolated];
        _points = _points.Distinct().ToList();

        _prev = point;
    }

    private void SetSelection(BaseSelection selection, ref SKBitmap overlay)
    {
        _selection = selection;
        overlay.Clear();
        Subjects.SelectionCreating.OnNext(selection);
    }
}
