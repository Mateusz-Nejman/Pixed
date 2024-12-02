using Pixed.Application.Utils;
using Pixed.Common.Menu;
using Pixed.Common.Services.Palette;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;
internal class PaletteMenuRegister(IMenuItemRegistry menuItemRegistry, PaletteService paletteService, DialogUtils dialogUtils)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly PaletteService _paletteService = paletteService;
    private readonly DialogUtils _dialogUtils = dialogUtils;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Add Primary color to palette", PaletteAddPrimaryAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Merge palette with current colors", PaletteAddCurrentAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Clear palette", PaletteClearAction);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Open palette from file", async () => await PaletteOpenAction());
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Save palette to file", async () => await PaletteSaveAction());
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
        var files = await _dialogUtils.OpenFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];
        await _paletteService.Load(file.Path.AbsolutePath);
    }

    private async Task PaletteSaveAction()
    {
        if (_paletteService.SelectedPalette.Colors.Count == 0)
        {
            return;
        }

        var file = await _dialogUtils.SaveFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (file != null)
        {
            await _paletteService.Save(file.Path.AbsolutePath);
        }
    }

    private void PaletteListAction()
    {
        RouterControl.Navigate("/palettes");
    }
}
