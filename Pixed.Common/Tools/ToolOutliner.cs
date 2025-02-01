using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolOutliner(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_FILL_CORNERS = "Fill corners";

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_draw_shape_24_regular.svg";
    public override string Name => "Outliner tool";
    public override string Id => "tool_outliner_tool";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Outliner", "Ctrl", "Fill corners");
    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        var controlPressed = keyState.IsCtrl || GetProperty(PROP_FILL_CORNERS);
        var color = ToolColor;
        PaintUtils.OutlineSimiliarConnectedPixels(frame.CurrentLayer, point, color, controlPressed);
        Subjects.FrameModified.OnNext(frame);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_FILL_CORNERS)
            ];
    }
}
