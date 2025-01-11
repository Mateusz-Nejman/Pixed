using Pixed.Application.ViewModels;
using Pixed.Common.Services.Palette;

namespace Pixed.Application.Pages;

internal partial class PaletteList : Modal
{
    private readonly PaletteListViewModel _viewModel;
    public PaletteList()
    {
        InitializeComponent();
        DataContext = _viewModel = new PaletteListViewModel(Provider.Get<PaletteService>(), Provider.Get<PaletteSectionViewModel>());

        if (_viewModel.Palettes.Count != 0)
        {
            noPalettesInfo.IsVisible = false;
        }
        _viewModel.CloseAction = async () => await Close();
    }
}
