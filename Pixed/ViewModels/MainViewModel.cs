using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using System;
using System.IO;

namespace Pixed.ViewModels;

internal class MainViewModel : PropertyChangedBase
{
    public MainViewModel()
    {
        Global.DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pixed");

        if (!Directory.Exists(Global.DataFolder))
        {
            Directory.CreateDirectory(Global.DataFolder);
        }

        if (!Directory.Exists(Path.Combine(Global.DataFolder, "Palettes")))
        {
            Directory.CreateDirectory(Path.Combine(Global.DataFolder, "Palettes"));
        }

        Global.ShortcutService = new ShortcutService();
        Global.PaletteService = new PaletteService();
        Global.PaletteService.LoadAll();
    }
}
