namespace Pixed.Tools;

internal class ToolEraser : ToolPen
{
    public override UniColor GetToolColor()
    {
        return UniColor.Transparent;
    }
}
