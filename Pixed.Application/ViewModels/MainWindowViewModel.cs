using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public MainWindowViewModel(MenuBuilder menuBuilder)
    {
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
