using Pixed.Models;

namespace Pixed.Tools;
internal static class ToolProperties
{
    public static ToolProperty GetApplyToAllFrames()
    {
        return new ToolProperty("Apply to all frames");
    }

    public static ToolProperty GetApplyToAllLayers()
    {
        return new ToolProperty("Apply to all layers");
    }
}
