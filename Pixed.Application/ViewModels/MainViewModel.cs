using Pixed.Application.Controls;
using Pixed.Application.Menu;
using Pixed.Application.Windows;
using Pixed.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class MainViewModel : PixedViewModel, IDisposable
{
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _menuBuilt;
    private bool _disposedValue;
    private string _title;
    private List<Avalonia.Controls.MenuItem>? _menu;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public List<Avalonia.Controls.MenuItem>? Menu
    {
        get => _menu;
        set
        {
            _menu = value;
            OnPropertyChanged();
        }
    }

    public ICommand QuitCommand => MainPage.QuitCommand; //For binding
    public MainViewModel(MenuBuilder menuBuilder)
    {
        _projectChanged = Subjects.ProjectChanged.Subscribe(model =>
        {
            Title = model.FileName + " - Pixed";
        });

        _menuBuilt = menuBuilder.OnMenuBuilt.Subscribe(menu =>
        {
            Menu = menu.Select(m => m.ToAvaloniaMenuItem()).ToList();
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
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
