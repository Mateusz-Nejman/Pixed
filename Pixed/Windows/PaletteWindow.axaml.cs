﻿using Avalonia.Controls;
using Pixed.Services.Palette;
using Pixed.ViewModels;

namespace Pixed.Windows;

internal partial class PaletteWindow : Window
{
    private readonly PaletteService _paletteService;
    private readonly PaletteWindowViewModel _viewModel;
    public PaletteWindow(PaletteService paletteService)
    {
        InitializeComponent();
        _paletteService = paletteService;
        DataContext = _viewModel = new PaletteWindowViewModel(paletteService);

        if (_viewModel.Palettes.Count != 0)
        {
            noPalettesInfo.IsVisible = false;
        }
        _viewModel.PaletteAction = (select, model) =>
        {
            if (select)
            {
                _paletteService.Select(model);
                Close(true);
            }
            else
            {
                _paletteService.Remove(model);
            }
        };
        _viewModel.PaletteRenameAction = (model, newName) =>
        {
            _paletteService.Rename(model, newName);
        };
    }
}
