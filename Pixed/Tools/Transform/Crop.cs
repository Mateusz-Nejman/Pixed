using Pixed.Models;
using Pixed.Utils;
using System;

namespace Pixed.Tools.Transform;

internal class Crop : AbstractTransformTool
{
    public override void ApplyTransformation()
    {
        int[] boundaries;
        if (Global.SelectionManager.HasSelection)
        {
            boundaries = TransformUtils.GetBoundariesFromSelection(Global.SelectionManager.Selection);
        }
        else
        {
            boundaries = TransformUtils.GetBoundaries([.. Global.CurrentFrame.Layers]);
        }

        bool applied = ApplyTool(boundaries);

        if (applied)
        {
            Subjects.RefreshCanvas.OnNext(null);
            //TODO undo/redo
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

    private static bool ApplyTool(int[] boundaries)
    {
        //return [minx, miny, maxx, maxy];

        if (boundaries[0] >= boundaries[2])
        {
            return false;
        }

        var model = Global.CurrentModel;
        int width = 1 + boundaries[2] - boundaries[0];
        int height = 1 + boundaries[3] - boundaries[1];

        if (width == model.Width && height == model.Height)
        {
            return false;
        }

        foreach (var frame in Global.CurrentModel.Frames)
        {
            foreach (var layer in frame.Layers)
            {
                TransformUtils.MoveLayerFixes(layer, -boundaries[0], -boundaries[1]);
            }
        }

        var newModel = ResizeUtils.ResizeModel(model, 1 + boundaries[2] - boundaries[0], 1 + boundaries[3] - boundaries[1], false, ResizeUtils.Origin.TopLeft);
        Subjects.SelectionDismissed.OnNext(null);
        Global.Models[Global.CurrentModelIndex] = newModel;

        foreach (var frame in Global.CurrentModel.Frames)
        {
            frame.RefreshLayerRenderSources();
            frame.RefreshRenderSource();
            Subjects.FrameModified.OnNext(frame);
        }
        return true;
    }
}
