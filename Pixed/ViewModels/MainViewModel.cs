using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Windows;
using System;
using System.IO;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class MainViewModel : PixedViewModel, IDisposable
{
    private readonly IDisposable _onMenuBuilt;
    private NativeMenu? _menu;
    private bool _disposedValue;

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
        _onMenuBuilt = StaticMenuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu;
        });
    }

    public override void OnInitialized()
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
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _onMenuBuilt?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
