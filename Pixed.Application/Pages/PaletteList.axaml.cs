using Pixed.Application.ViewModels;
using Pixed.Common.Services.Palette;

namespace Pixed.Application.Pages;

internal partial class PaletteList : Modal
{
    private readonly PaletteListViewModel _viewModel;
    public PaletteList()
    {
        InitializeComponent();
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        DataContext = _viewModel = new PaletteListViewModel(Provider.Get<PaletteService>(), Provider.Get<PaletteSectionViewModel>());

        if (_viewModel.Palettes.Count != 0)
        {
            noPalettesInfo.IsVisible = false;
        }
        _viewModel.CloseAction = async () => await Close();
    }
}
