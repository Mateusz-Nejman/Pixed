using Avalonia.Media;
using Pixed.Application.Controls;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Common.Services.Palette;
using Pixed.Core;
using Pixed.Core.Models;
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
    private readonly DialogUtils _dialogUtils;

    private UniColor _primaryColor = UniColor.Black;
    private UniColor _secondaryColor = UniColor.White;
    private bool _disposedValue;

    private readonly IDisposable _frameModified;
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

    public PaletteSectionViewModel(ApplicationData applicationData, IMenuItemRegistry menuItemRegistry, PaletteService paletteService, DialogUtils dialogUtils)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
        _paletteService = paletteService;
        _dialogUtils = dialogUtils;
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
        _frameModified = Subjects.FrameModified.Subscribe(f =>
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
                _frameModified?.Dispose();
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
    }

    public void PaletteAddPrimaryAction()
    {
        _paletteService.AddPrimaryColor();
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    public void PaletteAddCurrentAction()
    {
        _paletteService.AddColorsFromPalette(_paletteService.CurrentColorsPalette);
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    public void PaletteClearAction()
    {
        _paletteService.ClearPalette();
        OnPropertyChanged(nameof(SelectedPaletteColors));
    }

    public async Task PaletteOpenAction()
    {
        var files = await _dialogUtils.OpenFileDialog("All Supported (.json;.gpl)|*.json;*.gpl|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];
        await _paletteService.Load(file.Path.AbsolutePath);
    }

    public async Task PaletteSaveAction()
    {
        if (_paletteService.SelectedPalette.Colors.Count == 0)
        {
            return;
        }

        var file = await _dialogUtils.SaveFileDialog("Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl", _paletteService.SelectedPalette.Name);

        if (file != null)
        {
            await _paletteService.Save(file.Path.AbsolutePath);
        }
    }

    public void PaletteListAction()
    {
        Router.Navigate("/palettes");
    }
}
