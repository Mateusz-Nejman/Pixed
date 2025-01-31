using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools.Selection;

public class ToolSelectLasso(ApplicationData applicationData) : ToolSelectBase(applicationData)
{
    private List<Point> _points = [];
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_lasso_28_regular.svg";
    public override string Name => "Lasso selection";
    public override string Id => "tool_lasso_select";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lasso selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnSelectionBegin(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        _points.Clear();
        _points.Add(currentPoint);
        _selection = new ShapeSelection(_points.Select(p => new Pixel(p, frame.GetPixel(p))).ToList());
        Subjects.SelectionStarted.OnNext(_selection);
    }

    public override void OnSelection(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        if (Math.Abs(currentPoint.X - previousPoint.X) > 1 || Math.Abs(currentPoint.Y - previousPoint.Y) > 1)
        {
            var points = BresenhamLine.Get(previousPoint, currentPoint);

            foreach (var point in points)
            {
                currentPoint = point;
                OnSelectionBase(startPoint, currentPoint, previousPoint, frame);
                previousPoint = point;
            }

            return;
        }

        OnSelectionBase(startPoint, currentPoint, previousPoint, frame);
    }

    public override void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        currentPoint = AddPixel(currentPoint, frame);
        _selection = new LassoSelection(GetLassoPixels(startPoint, currentPoint), frame);
        Subjects.SelectionCreated.OnNext(_selection);
    }

    private List<Point> GetLassoPixels(Point startPoint, Point currentPoint)
    {
        var line = BresenhamLine.Get(currentPoint, startPoint);
        return [.. _points, .. line];
    }

    private Point AddPixel(Point currentPoint, Frame frame)
    {
        Point prev = new(currentPoint.X, currentPoint.Y);
        currentPoint.X = Math.Clamp(currentPoint.X, 0, frame.Width - 1);
        currentPoint.Y = Math.Clamp(currentPoint.Y, 0, frame.Height - 1);

        var interpolated = BresenhamLine.Get(currentPoint, prev);
        _points = [.. _points, .. interpolated];
        _points = _points.Distinct().ToList();

        return currentPoint;
    }

    private void OnSelectionBase(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        currentPoint = AddPixel(currentPoint, frame);
        List<Pixel> pixels = [];

        foreach (var p in GetLassoPixels(startPoint, currentPoint))
        {
            pixels.Add(new Pixel(p, frame.GetPixel(p)));
        }
        _selection = new ShapeSelection(pixels);
        Subjects.SelectionCreating.OnNext(_selection);
    }
}
