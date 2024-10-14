using Pixed.Models;
using Pixed.Selection;
using Pixed.Utils;
using System;

namespace Pixed.Tools.Transform;

internal class Crop(ApplicationData applicationData, SelectionManager selectionManager) : AbstractTransformTool(applicationData)
{
    private readonly SelectionManager _selectionManager = selectionManager;

    public override void ApplyTransformation(bool shiftPressed, bool controlPressed, bool altPressed)
    {
        int[] boundaries;
        if (_selectionManager.HasSelection)
        {
            boundaries = TransformUtils.GetBoundariesFromSelection(_selectionManager.Selection);
        }
        else
        {
            boundaries = TransformUtils.GetBoundaries([.. _applicationData.CurrentFrame.Layers]);
        }

        bool applied = ApplyTool(boundaries);

        if (applied)
        {
            Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        }
    }
    public override void ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        base.ApplyTool(altKey, allFrames, allLayers);
    }
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        throw new NotImplementedException();
    }

    private bool ApplyTool(int[] boundaries)
    {
        //return [minx, miny, maxx, maxy];

        if (boundaries[0] >= boundaries[2])
        {
            return false;
        }

        var model = _applicationData.CurrentModel;
        int width = 1 + boundaries[2] - boundaries[0];
        int height = 1 + boundaries[3] - boundaries[1];

        if (width == model.Width && height == model.Height)
        {
            return false;
        }

        foreach (var frame in _applicationData.CurrentModel.Frames)
        {
            foreach (var layer in frame.Layers)
            {
                TransformUtils.MoveLayerFixes(layer, -boundaries[0], -boundaries[1]);
                Subjects.LayerModified.OnNext(layer);
            }

            Subjects.FrameModified.OnNext(frame);
        }

        var newModel = ResizeUtils.ResizeModel(applicationData, model, 1 + boundaries[2] - boundaries[0], 1 + boundaries[3] - boundaries[1], false, ResizeUtils.Origin.TopLeft);
        Subjects.SelectionDismissed.OnNext(null);
        _applicationData.Models[_applicationData.CurrentModelIndex] = newModel;
        Subjects.ProjectModified.OnNext(newModel);

        foreach (var frame in _applicationData.CurrentModel.Frames)
        {
            frame.RefreshLayerRenderSources();
            frame.RefreshCurrentLayerRenderSource([]);
        }
        return true;
    }
}
