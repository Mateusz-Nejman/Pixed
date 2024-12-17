using Pixed.Application.ViewModels;
using Pixed.Common.Menu;

namespace Pixed.Application.Menu;
internal class PaletteMenuRegister(IMenuItemRegistry menuItemRegistry, PaletteSectionViewModel paletteSectionViewModel)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly PaletteSectionViewModel _paletteSectionViewModel = paletteSectionViewModel;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Add Primary color to palette", _paletteSectionViewModel.PaletteAddPrimaryAction, new("avares://Pixed.Application/Resources/Icons/plus-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Merge palette with current colors", _paletteSectionViewModel.PaletteAddCurrentAction, new("avares://Pixed.Application/Resources/Icons/merge_palettes-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Clear palette", _paletteSectionViewModel.PaletteClearAction, new("avares://Pixed.Application/Resources/Icons/cross-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Open palette from file", async () => await _paletteSectionViewModel.PaletteOpenAction(), new("avares://Pixed.Application/Resources/Icons/folder-open-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Save palette to file", async () => await _paletteSectionViewModel.PaletteSaveAction(), new("avares://Pixed.Application/Resources/Icons/floppy-disk-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Palettes list", _paletteSectionViewModel.PaletteListAction, new("avares://Pixed.Application/Resources/Icons/database-menu.png"));
    }
}
