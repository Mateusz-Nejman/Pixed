using Pixed.Common.Services;
using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public class Center(ApplicationData applicationData, IHistoryService historyService) : AbstractTransformTool(applicationData, historyService)
{
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        TransformUtils.Center(layer);
        Subjects.LayerModified.OnNext(layer);
    }
}
