using Avalonia.Controls;
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
        _viewModel = (PaletteWindowViewModel)DataContext;
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
