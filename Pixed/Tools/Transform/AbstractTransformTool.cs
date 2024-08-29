using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Tools.Transform
{
    internal abstract class AbstractTransformTool
    {
        public virtual void ApplyTransformation()
        {
            bool allFrames = Keyboard.Modifiers == ModifierKeys.Shift;
            bool allLayers = Keyboard.Modifiers == ModifierKeys.Control;

            ApplyTool(Keyboard.Modifiers == ModifierKeys.Alt, allFrames, allLayers);
        }
        public virtual void ApplyTool(bool altKey, bool allFrames, bool allLayers)
        {
            var model = Global.CurrentModel;
            ObservableCollection<Frame> frames = allFrames ? model.Frames : [Global.CurrentFrame];

            foreach (Frame frame in frames)
            {
                ObservableCollection<Layer> layers = allLayers ? frame.Layers : [Global.CurrentLayer];

                foreach (Layer layer in layers)
                {
                    ApplyToolOnLayer(layer, altKey);
                }
            }
        }

        public abstract void ApplyToolOnLayer(Layer layer, bool altKey);
    }
}
