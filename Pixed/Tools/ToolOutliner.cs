using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Tools;
internal class ToolOutliner(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_FILL_CORNERS = "Fill corners";

    public override bool ControlHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        controlPressed = controlPressed || GetProperty(PROP_FILL_CORNERS);
        var color = GetToolColor();
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
