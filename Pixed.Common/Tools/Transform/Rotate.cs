using Pixed.Common.Services;
using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public class Rotate(ApplicationData applicationData, IHistoryService historyService) : AbstractTransformTool(applicationData, historyService)
{
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        TransformUtils.Direction direction = TransformUtils.Direction.Clockwise;

        if (altKey)
        {
            direction = TransformUtils.Direction.CounterClockwise;
        }

        TransformUtils.Rotate(ref layer, direction);
        Subjects.LayerModified.OnNext(layer);
    }
}
