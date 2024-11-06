using Pixed.Common.Algos;
using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Drawing.Point;

namespace Pixed.Common.Tools.Selection;

internal class LassoSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    private int _prevX;
    private int _prevY;
    private List<Point> _points = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-lasso-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lasso selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnDragSelectStart(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        _points.Clear();
        _points.Add(new Point(x, y));

        _startX = x;
        _startY = y;

        _prevX = x;
        _prevY = y;
    }

    public override void OnDragSelect(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        AddPixel(x, y, frame);
        List<Pixel> pixels = [];

        foreach (var point in GetLassoPixels())
        {
            pixels.Add(new Pixel(point.X, point.Y, frame.GetPixel(point.X, point.Y)));
        }
        BaseSelection selection = new ShapeSelection(pixels);
        SetSelection(selection, ref overlay);
    }

    public override void OnDragSelectEnd(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        AddPixel(x, y, frame);
        BaseSelection selection = new LassoSelection(GetLassoPixels(), frame);
        SetSelection(selection, ref overlay);
    }

    private List<Point> GetLassoPixels()
    {
        var line = BresenhamLine.Get(_prevX, _prevY, _startX, _startY);
        return [.. _points, .. line];
    }

    private void AddPixel(int x, int y, Frame frame)
    {
        x = Math.Clamp(x, 0, frame.Width - 1);
        y = Math.Clamp(y, 0, frame.Height - 1);

        var interpolated = BresenhamLine.Get(x, y, _prevX, _prevY);
        _points = [.. _points, .. interpolated];
        _points = _points.Distinct().ToList();

        _prevX = x;
        _prevY = y;
    }

    private void SetSelection(BaseSelection selection, ref SKBitmap overlay)
    {
        _selection = selection;
        overlay.Clear();
        Subjects.SelectionCreated.OnNext(selection);
        DrawSelectionOnOverlay(ref overlay);
    }
}
