using Avalonia.Input;
using Pixed.Input;
using Pixed.Models;

namespace Pixed.Tools.Transform;

internal abstract class AbstractTransformTool
{
    public virtual void ApplyTransformation()
    {
        bool allFrames = Keyboard.Modifiers.HasFlag(KeyModifiers.Shift);
        bool allLayers = Keyboard.Modifiers.HasFlag(KeyModifiers.Control);

        ApplyTool(Keyboard.Modifiers.HasFlag(KeyModifiers.Alt), allFrames, allLayers);
        Global.CurrentModel.AddHistory();
    }
    public virtual void ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        Global.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, altKey);
        });
    }

    public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
}
