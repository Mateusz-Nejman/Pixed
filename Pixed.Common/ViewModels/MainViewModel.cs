using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Menu;
using Pixed.Models;
using Pixed.Services;
using Pixed.Services.Palette;
using Pixed.Windows;
using System;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class MainViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly RecentFilesService _recentFilesService;
    private readonly PaletteService _paletteService;
    private readonly IDisposable _onMenuBuilt;
    private readonly IDisposable _projectChanged;
    private NativeMenu? _menu;
    private bool _disposedValue;
    private string _title;

    public NativeMenu? Menu
    {
        get => _menu;
        set
        {
            _menu = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public ICommand QuitCommand => MainWindow.QuitCommand;
    public MainViewModel(ApplicationData data, RecentFilesService recentFilesService, PaletteService paletteService, MenuBuilder menuBuilder)
    {
        _applicationData = data;
        _recentFilesService = recentFilesService;
        _paletteService = paletteService;
        _onMenuBuilt = menuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu;
        });

        _projectChanged = Subjects.ProjectChanged.Subscribe(model =>
        {
            Title = model.FileName + " - Pixed";
        });
    }

    public async override void OnInitialized()
    {
        _recentFilesService.Load();
        _paletteService.LoadAll();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _onMenuBuilt?.Dispose();
                _projectChanged?.Dispose();
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
