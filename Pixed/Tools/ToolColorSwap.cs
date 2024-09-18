using Pixed.Input;
using Pixed.Models;
using Pixed.Services.History;
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
                DynamicHistoryEntry entry = new()
                {
                    FrameId = frame.Id,
                    LayerId = layer.Id
                };
                ApplyToolOnLayer(layer, oldColor, newColor, ref entry);
                return entry.ToEntry();
            });
        }

        private void ApplyToolOnLayer(Layer layer, int oldColor, int newColor, ref DynamicHistoryEntry historyEntry)
        {
            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    if (layer.GetPixel(x, y) == oldColor)
                    {
                        layer.SetPixel(x, y, newColor);
                        historyEntry.Add(x, y, oldColor, newColor);
                    }
                }
            }
        }
    }
}
