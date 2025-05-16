using Pixed.Common.Services;
using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public class Flip(ApplicationData applicationData, IHistoryService historyService) : AbstractTransformTool(applicationData, historyService)
{
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        TransformUtils.Axis axis = TransformUtils.Axis.Vertical;

        if (altKey)
        {
            axis = TransformUtils.Axis.Horizontal;
        }

        TransformUtils.Flip(ref layer, axis);
        Subjects.LayerModified.OnNext(layer);
    }
}
