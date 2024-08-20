using Pixed.Models;
using Pixed.Services.Keyboard;

namespace Pixed
{
    internal static class Global
    {
        public static ShortcutService? ShortcutService { get; set; }
        public static Settings UserSettings { get; set; } = new Settings();
    }
}
