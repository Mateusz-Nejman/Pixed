using Pixed.Models;
using Pixed.Services.Keyboard;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Tools;
internal class ToolColorSwap(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Core/Resources/Icons/tools/tool-colorswap.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Replace colors", "Ctrl", "Apply to all layers", "Shift", "Apply to all frames");
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        var shiftPressed = keyState.IsShift || GetProperty(ToolProperties.PROP_APPLY_ALL_FRAMES);
        var controlPressed = keyState.IsCtrl || GetProperty(ToolProperties.PROP_APPLY_ALL_LAYERS);

        if (frame.ContainsPixel(x, y))
        {
            var oldColor = frame.GetPixel(x, y);
            var newColor = ToolColor;

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
