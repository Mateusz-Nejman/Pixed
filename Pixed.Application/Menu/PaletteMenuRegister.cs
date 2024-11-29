using Pixed.Application.Utils;
using Pixed.Application.Windows;
using Pixed.Common.Menu;
using Pixed.Common.Services.Palette;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;
internal class PaletteMenuRegister(IMenuItemRegistry menuItemRegistry, PaletteService paletteService)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly PaletteService _paletteService = paletteService;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Add Primary color to palette", PaletteAddPrimaryAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Merge palette with current colors", PaletteAddCurrentAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Clear palette", PaletteClearAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Open palette from file", async() => await PaletteOpenAction());
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Save palette to file", async() => await PaletteSaveAction());
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Palettes list", PaletteListAction);
    }

    private void PaletteAddPrimaryAction()
    {
        _paletteService.AddPrimaryColor();
        //TODO OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteAddCurrentAction()
    {
        _paletteService.AddColorsFromPalette(_paletteService.CurrentColorsPalette);
        //TODO OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteClearAction()
    {
        _paletteService.ClearPalette();
        //TODO OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private async Task PaletteOpenAction()
    {
        var files = await DialogUtils.OpenFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];
        _paletteService.Load(file.Path.AbsolutePath);
    }

    private async Task PaletteSaveAction()
    {
        if (_paletteService.SelectedPalette.Colors.Count == 0)
        {
            return;
        }

        var file = await DialogUtils.SaveFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (file != null)
        {
            _paletteService.Save(file.Path.AbsolutePath);
        }
    }

    private void PaletteListAction()
    {
        RouterControl.Navigate("/palettes");
    }
}
