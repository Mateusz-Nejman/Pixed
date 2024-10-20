﻿using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.Menu;
using Pixed.Application.Services;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Models;
using Pixed.Common.Services.Palette;
using System;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

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
