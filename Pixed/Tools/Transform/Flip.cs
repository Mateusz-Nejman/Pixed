using Pixed.Models;

namespace Pixed.Tools.Transform
{
    internal class Flip : AbstractTransformTool
    {
        public override void ApplyToolOnLayer(Layer layer, bool altKey)
        {
            TransformUtils.Axis axis = TransformUtils.Axis.Vertical;

            if (altKey)
            {
                axis = TransformUtils.Axis.Horizontal;
            }

            TransformUtils.Flip(ref layer, axis);
            Subjects.RefreshCanvas.OnNext(true);
        }
    }
}