using Pixed.Models;
using Pixed.Selection;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Tools;

namespace Pixed
{
    internal static class Global
    {
        public static ShortcutService? ShortcutService { get; set; }
        public static PaletteService? PaletteService { get; set; }
        public static SelectionManager? SelectionManager { get; set; }
        public static Settings UserSettings { get; set; } = new Settings();
        public static BaseTool ToolSelected { get; set; }
        public static List<PixedModel> Models { get; } = [];
        public static int CurrentModelIndex { get; set; }
        public static int CurrentFrameIndex { get; set; }
        public static int CurrentLayerIndex { get; set; }
        public static PixedModel CurrentModel => Models[CurrentModelIndex];
        public static Frame CurrentFrame => CurrentModel.Frames[CurrentFrameIndex];
        public static Layer CurrentLayer => CurrentFrame.Layers[CurrentLayerIndex];
        public static ToolSelector ToolSelector { get; set; }
        public static UniColor PrimaryColor { get; set; }
        public static UniColor SecondaryColor { get; set; }
    }
}
