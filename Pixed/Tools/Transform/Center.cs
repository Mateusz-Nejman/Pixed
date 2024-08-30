using Pixed.Models;

namespace Pixed.Tools.Transform
{
    internal class Center : AbstractTransformTool
    {
        public override void ApplyToolOnLayer(Layer layer, bool altKey)
        {
            TransformUtils.Center(layer);
            Subjects.RefreshCanvas.OnNext(true);
        }
    }
}
