using Pixed.Common.Models;
using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public abstract class AbstractTransformTool(ApplicationData applicationData)
{
    protected ApplicationData _applicationData = applicationData;

    public virtual void ApplyTransformation(bool shiftPressed, bool controlPressed, bool altPressed)
    {
        ApplyTool(altPressed, shiftPressed, controlPressed);
        _applicationData.CurrentModel.AddHistory();
    }
    public virtual void ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        _applicationData.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, altKey);
        }, _applicationData);

        _applicationData.CurrentModel.AddHistory();
    }

    public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
}
