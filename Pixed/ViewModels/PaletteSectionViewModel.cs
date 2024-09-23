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

internal class PaletteSectionViewModel : PixedViewModel, IDisposable
{
    private UniColor _primaryColor = UniColor.Black;
    private UniColor _secondaryColor = UniColor.White;
    private bool _disposedValue;

    private readonly IDisposable _layerModified;
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _primaryProjectChanged;
    private readonly IDisposable _secondaryProjectChanged;
    private readonly IDisposable _primaryProjectChange;
    private readonly IDisposable _secondaryProjectChange;
    private readonly IDisposable _paletteSelected;

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
        _primaryProjectChanged = Subjects.PrimaryColorChanged.Subscribe(c => Global.PrimaryColor = c);
        _secondaryProjectChanged = Subjects.SecondaryColorChanged.Subscribe(c => Global.SecondaryColor = c);
        _primaryProjectChange = Subjects.PrimaryColorChange.Subscribe(c => PrimaryColor = c);
        _secondaryProjectChange = Subjects.SecondaryColorChange.Subscribe(c => SecondaryColor = c);

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            Global.PaletteService?.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });

        _layerModified = Subjects.LayerModified.Subscribe(l =>
        {
            Global.PaletteService?.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });

        PrimaryColor = UniColor.Black;
        SecondaryColor = UniColor.White;

        _paletteSelected = Subjects.PaletteSelected.Subscribe(p =>
        {
            OnPropertyChanged(nameof(SelectedPaletteColors));
        });

        PaletteAddPrimaryCommand = new ActionCommand(PaletteAddPrimaryAction);
        PaletteAddCurrentCommand = new ActionCommand(PaletteAddCurrentAction);
        PaletteOpenCommand = new AsyncCommand(PaletteOpenAction);
        PaletteSaveCommand = new AsyncCommand(PaletteSaveAction);
        PaletteClearCommand = new ActionCommand(PaletteClearAction);
        PaletteListCommand = new ActionCommand(PaletteListAction);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _layerModified?.Dispose();
                _projectChanged?.Dispose();
                _paletteSelected?.Dispose();
                _primaryProjectChanged?.Dispose();
                _primaryProjectChange?.Dispose();
                _secondaryProjectChanged?.Dispose();
                _secondaryProjectChange?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override void RegisterMenuItems()
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
        var files = await IODialogs.OpenFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", Global.PaletteService.SelectedPalette.Name);

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

        var file = await IODialogs.SaveFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", Global.PaletteService.SelectedPalette.Name);

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
