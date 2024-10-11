using Pixed.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Tools;
internal class ToolColorSwap(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        shiftPressed = shiftPressed || GetProperty(ToolProperties.PROP_APPLY_ALL_FRAMES);
        controlPressed = controlPressed || GetProperty(ToolProperties.PROP_APPLY_ALL_LAYERS);

        if (frame.ContainsPixel(x, y))
        {
            var oldColor = frame.GetPixel(x, y);
            var newColor = GetToolColor();

            SwapColors(oldColor, newColor, shiftPressed, controlPressed);
        }
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            ToolProperties.GetApplyToAllLayers(),
            ToolProperties.GetApplyToAllFrames()
            ];
    }

    private void SwapColors(uint oldColor, uint newColor, bool shiftPressed, bool controlPressed)
    {
        _applicationData.CurrentModel.Process(shiftPressed, controlPressed, (frame, layer) =>
        {
            ApplyToolOnLayer(layer, oldColor, newColor);
        }, _applicationData);
    }

    private static void ApplyToolOnLayer(Layer layer, uint oldColor, uint newColor)
    {
        List<Pixel> pixels = [];
        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                if (layer.GetPixel(x, y) == oldColor)
                {
                    pixels.Add(new Pixel(x, y, newColor));
                }
            }
        }

        SetPixels(layer, pixels);
    }
}
