using Pixed.Application.ViewModels;
using Pixed.Common.Menu;

namespace Pixed.Application.Menu;
internal class PaletteMenuRegister(IMenuItemRegistry menuItemRegistry, PaletteSectionViewModel paletteSectionViewModel)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly PaletteSectionViewModel _paletteSectionViewModel = paletteSectionViewModel;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Add Primary color to palette", _paletteSectionViewModel.PaletteAddPrimaryAction, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Merge palette with current colors", _paletteSectionViewModel.PaletteAddCurrentAction, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_layer_diagonal_sparkle_24_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Clear palette", _paletteSectionViewModel.PaletteClearAction, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_square_eraser_20_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Open palette from file", async () => await _paletteSectionViewModel.PaletteOpenAction(), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_folder_open_28_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Save palette to file", async () => await _paletteSectionViewModel.PaletteSaveAction(), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_save_32_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Palettes list", _paletteSectionViewModel.PaletteListAction, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_color_24_regular.svg"));
    }
}
