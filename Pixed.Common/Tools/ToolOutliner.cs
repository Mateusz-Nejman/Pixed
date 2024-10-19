using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolOutliner(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_FILL_CORNERS = "Fill corners";

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-outliner.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Outliner", "Ctrl", "Fill corners");
    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        var controlPressed = keyState.IsCtrl || GetProperty(PROP_FILL_CORNERS);
        var color = ToolColor;
        PaintUtils.OutlineSimiliarConnectedPixels(frame.CurrentLayer, x, y, color, controlPressed);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_FILL_CORNERS)
            ];
    }
}
