using Avalonia.Input;
using Pixed.Input;
using Pixed.Models;
using System.Collections.ObjectModel;

namespace Pixed.Tools.Transform;

internal abstract class AbstractTransformTool
{
    public virtual void ApplyTransformation()
    {
        bool allFrames = Keyboard.Modifiers == KeyModifiers.Shift;
        bool allLayers = Keyboard.Modifiers == KeyModifiers.Control;

        ApplyTool(Keyboard.Modifiers == KeyModifiers.Alt, allFrames, allLayers);
    }
    public virtual void ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        Global.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, altKey);
            return null;
        });
    }

    public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
}
