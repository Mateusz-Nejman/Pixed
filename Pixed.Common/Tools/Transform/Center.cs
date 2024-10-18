using Pixed.Models;

namespace Pixed.Tools.Transform;

internal class Center(ApplicationData applicationData) : AbstractTransformTool(applicationData)
{
    public override void ApplyToolOnLayer(Layer layer, bool altKey)
    {
        TransformUtils.Center(layer);
        Subjects.LayerModified.OnNext(layer);
    }
}
