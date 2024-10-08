using Pixed.Models;

namespace Pixed.Tools;
internal static class ToolProperties
{
    public const string PROP_APPLY_ALL_FRAMES = "Apply to all frames";
    public const string PROP_APPLY_ALL_LAYERS = "Apply to all layers";
    public static ToolProperty GetApplyToAllFrames()
    {
        return new ToolProperty(PROP_APPLY_ALL_FRAMES);
    }

    public static ToolProperty GetApplyToAllLayers()
    {
        return new ToolProperty(PROP_APPLY_ALL_LAYERS);
    }
}
