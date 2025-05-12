using Pixed.Common.Models;
using Pixed.Common.Services;
using Pixed.Core.Models;
using System.Threading.Tasks;

namespace Pixed.Common.Tools.Transform;

public abstract class AbstractTransformTool(ApplicationData applicationData, IHistoryService historyService)
{
    protected ApplicationData _applicationData = applicationData;
    protected IHistoryService _historyService = historyService;

    public virtual async Task ApplyTransformation(bool shiftPressed, bool controlPressed, bool altPressed)
    {
        await ApplyTool(altPressed, shiftPressed, controlPressed);
        _applicationData.CurrentModel.ResetID();
        _applicationData.CurrentFrame.ResetID();
        _applicationData.CurrentLayer.ResetID();
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }
    public virtual async Task ApplyTool(bool altKey, bool allFrames, bool allLayers)
    {
        _applicationData.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, altKey);
        }, _applicationData);
    }

    public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
}
