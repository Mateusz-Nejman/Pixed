using Pixed.Models;

namespace Pixed.Tools.Transform
{
    internal class Rotate : AbstractTransformTool
    {
        public override void ApplyToolOnLayer(Layer layer, bool altKey)
        {
            TransformUtils.Direction direction = TransformUtils.Direction.Clockwise;

            if (altKey)
            {
                direction = TransformUtils.Direction.CounterClockwise;
            }

            TransformUtils.Rotate(ref layer, direction);
            Subjects.RefreshCanvas.OnNext(true);
        }
    }
}
