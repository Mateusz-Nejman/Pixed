using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public class Center(ApplicationData applicationData) : AbstractTransformTool(applicationData)
{
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        TransformUtils.Center(layer);
        Subjects.LayerModified.OnNext(layer);
    }
}
