﻿using Avalonia.Media;
using Pixed.Application.Controls;
using Pixed.Application.Utils;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Common.Services.Palette;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class PaletteSectionViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly IMenuItemRegistry _menuItemRegistry;
    private readonly PaletteService _paletteService;

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

    public PaletteModel SelectedPalette => _paletteService.SelectedPalette;
    public ObservableCollection<UniColor> SelectedPaletteColors
    {
        get
        {
            return new ObservableCollection<UniColor>(SelectedPalette.Colors.Select(s => (UniColor)s));
        }
    }
    public ObservableCollection<UniColor> CurrentPaletteColors
    {
        get
        {
            return new ObservableCollection<UniColor>(_paletteService.CurrentColorsPalette.Colors.Select(s => (UniColor)s));
        }
    }

    public ICommand PaletteAddPrimaryCommand { get; }
    public ICommand PaletteAddCurrentCommand { get; }
    public ICommand PaletteListCommand { get; }
    public ICommand PaletteOpenCommand { get; }
    public ICommand PaletteSaveCommand { get; }
    public ICommand PaletteClearCommand { get; }

    public PaletteSectionViewModel(ApplicationData applicationData, IMenuItemRegistry menuItemRegistry, PaletteService paletteService)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
        _paletteService = paletteService;
        _primaryProjectChanged = Subjects.PrimaryColorChanged.Subscribe(c => _applicationData.PrimaryColor = c);
        _secondaryProjectChanged = Subjects.SecondaryColorChanged.Subscribe(c => _applicationData.SecondaryColor = c);
        _primaryProjectChange = Subjects.PrimaryColorChange.Subscribe(c => PrimaryColor = c);
        _secondaryProjectChange = Subjects.SecondaryColorChange.Subscribe(c => SecondaryColor = c);

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            _paletteService.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });

        _layerModified = Subjects.LayerModified.Subscribe(l =>
        {
            _paletteService.SetCurrentColors();
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
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Add Primary color to palette", PaletteAddPrimaryCommand);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Merge palette with current colors", PaletteAddCurrentCommand);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Clear palette", PaletteClearCommand);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Open palette from file", PaletteOpenCommand);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Save palette to file", PaletteSaveCommand);
        _menuItemRegistry.Register(BaseMenuItem.Palette, "Palettes list", PaletteListCommand);
    }

    private void PaletteAddPrimaryAction()
    {
        _paletteService.AddPrimaryColor();
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteAddCurrentAction()
    {
        _paletteService.AddColorsFromPalette(_paletteService.CurrentColorsPalette);
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    private void PaletteClearAction()
    {
        _paletteService.ClearPalette();
        OnPropertyChanged(nameof(SelectedPaletteColors));
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
        PaletteWindow window = new(_paletteService);
        window.ShowDialog(MainWindow.Handle);
    }
}
