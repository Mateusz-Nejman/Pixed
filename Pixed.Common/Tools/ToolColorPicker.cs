using Pixed.Common.Input;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolColorPicker(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-colorpicker.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Color picker");
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override bool AddToHistory { get; protected set; } = false;
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        if (frame.ContainsPixel(point))
        {
            var color = frame.GetPixel(point);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Subjects.PrimaryColorChange.OnNext(color);
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                Subjects.SecondaryColorChange.OnNext(color);
            }
        }
    }
}
