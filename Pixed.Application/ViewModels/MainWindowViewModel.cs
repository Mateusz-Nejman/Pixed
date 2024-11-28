using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.Menu;
using System;

namespace Pixed.Application.ViewModels;
internal class MainWindowViewModel : PixedViewModel, IDisposable
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

    public MainWindowViewModel()
    {
        var menuBuilder = App.ServiceProvider.Get<MenuBuilder>();
        _onMenuBuilt = menuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu;
        });
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
