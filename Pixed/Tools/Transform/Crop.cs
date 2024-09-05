using Pixed.Models;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools.Transform
{
    internal class Crop : AbstractTransformTool
    {
        public override void ApplyTransformation()
        {

            var layers = Getlayers();

            int[] boundaries;
            if(Global.SelectionManager.HasSelection)
            {
                boundaries = TransformUtils.GetBoundariesFromSelection(Global.SelectionManager.Selection);
            }
            else
            {
                boundaries = TransformUtils.GetBoundaries(layers.ToArray());
            }

            bool applied = ApplyTool(layers, boundaries);

            if (applied)
            {
                Subjects.RefreshCanvas.OnNext(true);
                Global.CurrentFrame.RefreshRenderSource();
                Subjects.FrameChanged.OnNext(Global.CurrentFrameIndex);
                //TODO undo/redo
            }
        }
        public override void ApplyTool(bool altKey, bool allFrames, bool allLayers)
        {
            base.ApplyTool(altKey, allFrames, allLayers);
        }
        public override void ApplyToolOnLayer(Layer layer, bool altKey)
        {
            throw new NotImplementedException();
        }

        private ObservableCollection<Layer> Getlayers()
        {
            return new ObservableCollection<Layer>(Global.CurrentModel.Frames.SelectMany(f => f.Layers));
        }

        private bool ApplyTool(ObservableCollection<Layer> layers, int[] boundaries)
        {
            //return [minx, miny, maxx, maxy];

            if (boundaries[0] >= boundaries[2])
            {
                return false;
            }

            var model = Global.CurrentModel;
            int width = 1 + boundaries[2] - boundaries[0];
            int height = 1 + boundaries[3] - boundaries[1];

            if(width == model.Width && height == model.Height)
            {
                return false;
            }

            foreach(var layer in layers)
            {
                TransformUtils.MoveLayerFixes(layer, -boundaries[0], -boundaries[1]);
            }
            
            var newModel = ResizeUtils.ResizeModel(model, 1 + boundaries[2] - boundaries[0], 1 + boundaries[3] - boundaries[1], false, ResizeUtils.Origin.TopLeft);
            Subjects.SelectionDismissed.OnNext(null);
            Global.Models[Global.CurrentModelIndex] = newModel;
            return true;
        }
    }
}
