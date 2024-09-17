using Avalonia.Media;
using Pixed.Controls;
using Pixed.Models;
using Pixed.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class PaletteSectionViewModel : PropertyChangedBase, IPixedViewModel
{
    private UniColor _primaryColor = UniColor.Black;
    private UniColor _secondaryColor = UniColor.White;

    public Color PrimaryColor
    {
        get => _primaryColor;
        set
        {
            _primaryColor = value;
            OnPropertyChanged();
            Subjects.PrimaryColorChanged.OnNext(value);
        }
    }

    public Color SecondaryColor
    {
        get => _secondaryColor;
        set
        {
            _secondaryColor = value;
            OnPropertyChanged();
            Subjects.SecondaryColorChanged.OnNext(value);
        }
    }

    public static PaletteModel SelectedPalette => Global.PaletteService == null ? null : Global.PaletteService.SelectedPalette;
    public static ObservableCollection<UniColor> SelectedPaletteColors
    {
        get
        {
            if (SelectedPalette == null)
            {
                return null;
            }

            return new ObservableCollection<UniColor>(SelectedPalette.Colors.Select(s => (UniColor)s));
        }
    }
    public static ObservableCollection<UniColor> CurrentPaletteColors
    {
        get
        {
            if (Global.PaletteService == null)
            {
                return null;
            }
            return new ObservableCollection<UniColor>(Global.PaletteService.CurrentColorsPalette.Colors.Select(s => (UniColor)s));
        }
    }

    public ICommand PaletteAddPrimaryCommand { get; }
    public ICommand PaletteAddCurrentCommand { get; }
    public ICommand PaletteListCommand { get; }
    public ICommand PaletteOpenCommand { get; }
    public ICommand PaletteSaveCommand { get; }
    public ICommand PaletteClearCommand { get; }

    public PaletteSectionViewModel()
    {
        Subjects.PrimaryColorChanged.Subscribe(c => Global.PrimaryColor = c);
        Subjects.SecondaryColorChanged.Subscribe(c => Global.SecondaryColor = c);
        Subjects.PrimaryColorChange.Subscribe(c => PrimaryColor = c);
        Subjects.SecondaryColorChange.Subscribe(c => SecondaryColor = c);

        PrimaryColor = UniColor.Black;
        SecondaryColor = UniColor.White;

        Subjects.PaletteSelected.Subscribe(p =>
        {
            OnPropertyChanged(nameof(ViewModels.PaletteSectionViewModel.SelectedPaletteColors));
        });

        PaletteAddPrimaryCommand = new ActionCommand(PaletteAddPrimaryAction);
        PaletteAddCurrentCommand = new ActionCommand(PaletteAddCurrentAction);
        PaletteOpenCommand = new AsyncCommand(PaletteOpenAction);
        PaletteSaveCommand = new AsyncCommand(PaletteSaveAction);
        PaletteClearCommand = new ActionCommand(PaletteClearAction);
        PaletteListCommand = new ActionCommand(PaletteListAction);

        Subjects.RefreshCanvas.Subscribe(_ =>
        {
            Global.PaletteService.SetCurrentColors();
            OnPropertyChanged(nameof(ViewModels.PaletteSectionViewModel.CurrentPaletteColors));
        });
    }

    public void RegisterMenuItems()
    {
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Add Primary color to palette", PaletteAddPrimaryCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Merge palette with current colors", PaletteAddCurrentCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Clear palette", PaletteClearCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Open palette from file", PaletteOpenCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Save palette to file", PaletteSaveCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Palette, "Palettes list", PaletteListCommand);
    }

    private void PaletteAddPrimaryAction()
    {
        Global.PaletteService.AddPrimaryColor();
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteAddCurrentAction()
    {
        Global.PaletteService.AddColorsFromPalette(Global.PaletteService.CurrentColorsPalette);
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteClearAction()
    {
        Global.PaletteService.ClearPalette();
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private async Task PaletteOpenAction()
    {
        var files = await IODialogs.OpenFileDialog("Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl|All Supported (.json;.gpl)|*.json;*.gpl", Global.PaletteService.SelectedPalette.Name);

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];
        Global.PaletteService.Load(file.Path.AbsolutePath);
    }

    private async Task PaletteSaveAction()
    {
        if (Global.PaletteService.SelectedPalette.Colors.Count == 0)
        {
            return;
        }

        var file = await IODialogs.SaveFileDialog("Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl|All Supported (.json;.gpl)|*.json;*.gpl", Global.PaletteService.SelectedPalette.Name);

        if (file != null)
        {
            Global.PaletteService.Save(file.Path.AbsolutePath);
        }
    }

    private void PaletteListAction()
    {
        PaletteWindow window = new();
        window.ShowDialog(MainWindow.Handle);
    }
}
