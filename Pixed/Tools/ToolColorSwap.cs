using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolColorSwap : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (frame.ContainsPixel(x, y))
            {
                var oldColor = frame.GetPixel(x, y);
                var newColor = GetToolColor();

                bool allLayers = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);
                bool allFrames = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);

                SwapColors(oldColor, newColor, allLayers, allFrames);
            }
        }

        private void SwapColors(int oldColor, int newColor, bool allLayers, bool allFrames)
        {
            Global.CurrentModel.Process(allFrames, allLayers, (frame, layer) =>
            {
                ApplyToolOnLayer(layer, oldColor, newColor);
            });
        }

        private void ApplyToolOnLayer(Layer layer, int oldColor, int newColor)
        {
            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    if (layer.GetPixel(x, y) == oldColor)
                    {
                        layer.SetPixel(x, y, newColor);
                    }
                }
            }
        }
    }
}
