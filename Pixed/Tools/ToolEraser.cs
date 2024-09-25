using Pixed.Models;

namespace Pixed.Tools;

internal class ToolEraser(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override UniColor GetToolColor()
    {
        return UniColor.Transparent;
    }
}
