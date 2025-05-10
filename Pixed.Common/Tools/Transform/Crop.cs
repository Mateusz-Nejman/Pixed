using Pixed.Common.Services;
using Pixed.Common.Utils;
using Pixed.Core.Models;
using System;
using System.Threading.Tasks;

namespace Pixed.Common.Tools.Transform;

public class Crop(ApplicationData applicationData, SelectionManager selectionManager, IHistoryService historyService) : AbstractTransformTool(applicationData, historyService)
{
    private readonly SelectionManager _selectionManager = selectionManager;

    public override async Task ApplyTransformation(bool shiftPressed, bool controlPressed, bool altPressed)
    {
        Tuple<Point, Point> boundaries;
        if (_selectionManager.HasSelection)
        {
            boundaries = TransformUtils.GetBoundariesFromSelection(_selectionManager.Selection);
        }
        else
        {
            boundaries = TransformUtils.GetBoundaries([.. _applicationData.CurrentFrame.Layers]);
        }

        bool applied = await ApplyTool(boundaries);

        if (applied)
        {
            Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        }
    }
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> ApplyTool(Tuple<Point, Point> boundaries)
    {
        //return [minx, miny, maxx, maxy];

        if (boundaries.Item1.X >= boundaries.Item2.X)
        {
            return false;
        }

        var model = _applicationData.CurrentModel;
        int width = 1 + boundaries.Item2.X - boundaries.Item1.X;
        int height = 1 + boundaries.Item2.Y - boundaries.Item1.Y;

        if (width == model.Width && height == model.Height)
        {
            return false;
        }

        foreach (var frame in _applicationData.CurrentModel.Frames)
        {
            foreach (var layer in frame.Layers)
            {
                TransformUtils.MoveLayerFixes(layer, -boundaries.Item1);
                Subjects.LayerModified.OnNext(layer);
            }

            Subjects.FrameModified.OnNext(frame);
        }

        var newModel = ResizeUtils.ResizeModel(applicationData, model, new Point(1 + boundaries.Item2.X - boundaries.Item1.X, 1 + boundaries.Item2.Y - boundaries.Item1.Y), false, ResizeUtils.Origin.TopLeft);

        historyService.Register(newModel);
        historyService.CopyHistoryFrom(model, newModel);
        await historyService.AddToHistory(newModel);

        Subjects.SelectionDismissed.OnNext(null);
        _applicationData.Models[_applicationData.CurrentModelIndex] = newModel;
        Subjects.ProjectModified.OnNext(newModel);
        return true;
    }
}
