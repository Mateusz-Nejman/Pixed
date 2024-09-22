using Avalonia.Controls;
using Pixed.Models;
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
        Global.RecentFilesService = new Services.RecentFilesService();
        Global.RecentFilesService.Load();
        Global.PaletteService.LoadAll();
        StaticMenuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu;
        });
    }
}
