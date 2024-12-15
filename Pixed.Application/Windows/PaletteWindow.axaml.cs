using Pixed.Application.ViewModels;
using Pixed.Common.Services.Palette;

namespace Pixed.Application.Windows;

internal partial class PaletteWindow : PixedWindow
{
    private readonly PaletteWindowViewModel _viewModel;
    public PaletteWindow()
    {
        InitializeComponent();
        DataContext = _viewModel = new PaletteWindowViewModel(Provider.Get<PaletteService>(), Provider.Get<PaletteSectionViewModel>());

        if (_viewModel.Palettes.Count != 0)
        {
            noPalettesInfo.IsVisible = false;
        }
        _viewModel.CloseAction = async() => await Close();
    }
}
