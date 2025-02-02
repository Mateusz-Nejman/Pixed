﻿using Pixed.Core.Models;

namespace Pixed.Common.Tools.Transform;

public class Flip(ApplicationData applicationData) : AbstractTransformTool(applicationData)
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
