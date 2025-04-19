using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolBucket(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_REPLACE = "Replace color in current layer";
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_paint_bucket_24_regular.svg";
    public override string Name => "Paint bucket tool";
    public override string Id => "tool_paint_bucket";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Fill color", "Shift", PROP_REPLACE);
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, ref overlay, keyState, selection);
        uint color = ToolColor;
        uint targetColor = frame.GetPixel(point);

        if (color == targetColor)
        {
            return;
        }

        bool replace = keyState.IsShift || GetProperty(PROP_REPLACE);

        if (replace)
        {
            List<Pixel> pixels = [];
            for (int x = 0; x < frame.Width; x++)
            {
                for (int y = 0; y < frame.Height; y++)
                {
                    var replacePoint = new Point(x, y);
                    if (frame.GetPixel(replacePoint) == targetColor)
                    {
                        pixels.Add(new Pixel(replacePoint, color));
                    }
                }
            }

            SetPixels(frame, pixels);
        }
        else
        {
            PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, point, color);
        }
        Subjects.FrameModified.OnNext(frame);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_REPLACE)
        ];
    }
}
