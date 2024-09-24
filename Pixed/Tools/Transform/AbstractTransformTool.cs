using Pixed.Models;

namespace Pixed.Tools.Transform;

internal abstract class AbstractTransformTool
{
    public virtual void ApplyTransformation(bool shiftPressed, bool controlPressed, bool altPressed)
    {
        ApplyTool(altPressed, shiftPressed, controlPressed);
        Global.CurrentModel.AddHistory();
    }
    public virtual void ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        Global.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, altKey);
        });

        Global.CurrentModel.AddHistory();
    }

    public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
}
