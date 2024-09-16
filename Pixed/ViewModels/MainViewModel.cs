using Avalonia.Controls;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Windows;
using System;
using System.IO;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class MainViewModel : PropertyChangedBase
{
    private NativeMenu? _menu;

    public NativeMenu? Menu
    {
        get => _menu;
        set
        {
            _menu = value;
            OnPropertyChanged();
        }
    }

    public ICommand QuitCommand => MainWindow.QuitCommand;
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
        StaticMenuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu;
        });
    }
}
