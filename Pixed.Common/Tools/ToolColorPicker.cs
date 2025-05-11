using Pixed.Common.Input;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolColorPicker(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_eyedropper_24_regular.svg";
    public override string Name => "Color picker";
    public override string Id => "tool_colorpicker";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties(Name);
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override bool AddToHistory { get; protected set; } = false;
    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();
        var frame = model.CurrentFrame;
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
