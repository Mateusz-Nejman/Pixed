using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolColorSwap(ApplicationData applicationData) : BaseTool(applicationData)
    {
        public override bool ShiftHandle { get; protected set; } = true;
        public override bool ControlHandle { get; protected set; } = true;
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            if (frame.ContainsPixel(x, y))
            {
                var oldColor = frame.GetPixel(x, y);
                var newColor = GetToolColor();

                SwapColors(oldColor, newColor, shiftPressed, controlPressed);
            }
        }

        private void SwapColors(int oldColor, int newColor, bool shiftPressed, bool controlPressed)
        {
            _applicationData.CurrentModel.Process(shiftPressed, controlPressed, (frame, layer) =>
            {
                ApplyToolOnLayer(layer, oldColor, newColor);
            }, _applicationData);
        }

        private static void ApplyToolOnLayer(Layer layer, int oldColor, int newColor)
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
