using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Tools;

namespace Pixed
{
    internal static class Global
    {
        public static ShortcutService? ShortcutService { get; set; }
        public static Settings UserSettings { get; set; } = new Settings();
        public static BaseTool ToolSelected { get; set; }
        public static List<PixedModel> Models { get; } = [];
    }
}
