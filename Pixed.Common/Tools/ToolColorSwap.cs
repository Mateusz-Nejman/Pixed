using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolColorSwap(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-colorswap.png";
    public override string Name => "Paint all pixels of the same color";
    public override string Id => "tool_colorswap";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Replace colors", "Ctrl", "Apply to all layers", "Shift", "Apply to all frames");
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        var shiftPressed = keyState.IsShift || GetProperty(ToolProperties.PROP_APPLY_ALL_FRAMES);
        var controlPressed = keyState.IsCtrl || GetProperty(ToolProperties.PROP_APPLY_ALL_LAYERS);

        if (frame.ContainsPixel(point))
        {
            var oldColor = frame.GetPixel(point);
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
                var point = new Point(x, y);
                if (layer.GetPixel(point) == oldColor)
                {
                    pixels.Add(new Pixel(point, newColor));
                }
            }
        }

        SetPixels(layer, pixels);
    }
}
