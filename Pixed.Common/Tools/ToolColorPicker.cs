using Pixed.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolColorPicker(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Common/Resources/Icons/tools/tool-colorpicker.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Color picker");
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override bool AddToHistory { get; protected set; } = false;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        if (frame.ContainsPixel(x, y))
        {
            var color = frame.GetPixel(x, y);

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
