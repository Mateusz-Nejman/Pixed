using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.Menu;
using Pixed.Application.Services;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Services.Palette;
using Pixed.Core.Models;
using System;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class MainViewModel : PixedViewModel, IDisposable
{
    private readonly IDisposable _projectChanged;
    private bool _disposedValue;
    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public ICommand QuitCommand => MainView.QuitCommand;
    public MainViewModel()
    {
        _projectChanged = Subjects.ProjectChanged.Subscribe(model =>
        {
            Title = model.FileName + " - Pixed";
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
