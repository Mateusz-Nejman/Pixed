using Pixed.Input;
using Pixed.Models;
using System.Drawing;
using System.Linq;

namespace Pixed.Tools
{
    internal class ToolMove : BaseTool
    {
        private int _startX = -1;
        private int _startY = -1;
        private Layer _currentLayer;
        private Layer _currentLayerClone;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _startX = x;
            _startY = y;
            _currentLayer = frame.Layers[frame.SelectedLayer];
            _currentLayerClone = _currentLayer.Clone();
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            int diffX = x - _startX;
            int diffY = y - _startY;

            ShiftLayer(frame.Layers[frame.SelectedLayer], _currentLayerClone, diffX, diffY);
        }

        public override void ReleaseTool(int x, int y, Frame _, ref Bitmap overlay)
        {
            int diffX = x - _startX;
            int diffY = y - _startY;

            bool ctrlKey = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);
            bool shiftKey = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);

            Frame[] frames = shiftKey ? Global.CurrentModel.Frames.ToArray() : [Global.CurrentFrame];

            foreach (Frame frame in frames)
            {
                Layer[] layers = ctrlKey ? frame.Layers.ToArray() : [Global.CurrentLayer];

                foreach (Layer layer in layers)
                {
                    var reference = this._currentLayer == layer ? this._currentLayerClone : layer.Clone();
                    ShiftLayer(layer, reference, diffX, diffY);
                }

                Subjects.FrameModified.OnNext(frame);
            }
        }

        private static void ShiftLayer(Layer layer, Layer reference, int diffX, int diffY)
        {
            int color;
            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    int x1 = x - diffX;
                    int y1 = y - diffY;

                    if (Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Alt))
                    {
                        x1 = (x1 + layer.Width) % layer.Width;
                        y1 = (y1 + layer.Height) % layer.Height;
                    }

                    if (reference.ContainsPixel(x1, y1))
                    {
                        color = reference.GetPixel(x1, y1);
                    }
                    else
                    {
                        color = UniColor.Transparent;
                    }

                    layer.SetPixel(x, y, color);
                }
            }
        }
    }
}
