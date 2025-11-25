using Pixed.Application.ViewModels;
using Pixed.Common.Services.Palette;

namespace Pixed.Application.Pages;

internal partial class PaletteList : Modal
{
    public PaletteList()
    {
        InitializeComponent();
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

        if (Provider != null)
        {
            var paletteService = Provider.Get<PaletteService>();
            var paletteViewModel = Provider.Get<PaletteSectionViewModel>();

            if (paletteService != null && paletteViewModel != null)
            {
                var viewModel = new PaletteListViewModel(paletteService, paletteViewModel);
                DataContext = viewModel;
            
                if (viewModel.Palettes.Count != 0)
                {
                    noPalettesInfo.IsVisible = false;
                }
                viewModel.CloseAction = async () => await Close();
            }
        }
    }
}
