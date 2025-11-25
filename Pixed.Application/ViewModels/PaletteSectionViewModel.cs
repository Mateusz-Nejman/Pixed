using Avalonia.Media;
using Avalonia.Platform.Storage;
using Pixed.Application.Controls;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Common.Models;
using Pixed.Common.Services.Palette;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class PaletteSectionViewModel : ExtendedViewModel, IDisposable
{
    private readonly PaletteService _paletteService;
    private readonly DialogUtils _dialogUtils;
    private readonly IPlatformFolder _platformFolder;

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
            return [.. SelectedPalette.Colors.Take(256).Select(s => (UniColor)s)];
        }
    }
    public ObservableCollection<UniColor> CurrentPaletteColors
    {
        get
        {
            return [.. _paletteService.CurrentColorsPalette.Colors.Take(256).Select(s => (UniColor)s)];
        }
    }

    public ICommand PaletteAddPrimaryCommand { get; }
    public ICommand PaletteAddCurrentCommand { get; }
    public ICommand PaletteListCommand { get; }
    public ICommand PaletteOpenCommand { get; }
    public ICommand PaletteSaveCommand { get; }
    public ICommand PaletteClearCommand { get; }

    public PaletteSectionViewModel(ApplicationData applicationData, ToolsManager toolsManager, PaletteService paletteService, DialogUtils dialogUtils, IPlatformFolder platformFolder)
    {
        _paletteService = paletteService;
        _dialogUtils = dialogUtils;
        _platformFolder = platformFolder;
        _primaryProjectChanged = Subjects.PrimaryColorChanged.Subscribe(c =>
        {
            applicationData.PrimaryColor = c;

            if (toolsManager.SelectedTool is ToolEraser)
            {
                toolsManager.SelectTool("tool_pen");
            }
        });
        _secondaryProjectChanged = Subjects.SecondaryColorChanged.Subscribe(c =>
        {
            applicationData.SecondaryColor = c;

            if (toolsManager.SelectedTool is ToolEraser)
            {
                toolsManager.SelectTool("tool_pen");
            }
        });
        _primaryProjectChange = Subjects.PrimaryColorChange.Subscribe(c => PrimaryColor = c);
        _secondaryProjectChange = Subjects.SecondaryColorChange.Subscribe(c => SecondaryColor = c);

        _projectChanged = Subjects.ProjectChanged.Subscribe(_ =>
        {
            _paletteService.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });

        _layerModified = Subjects.LayerModified.Subscribe(_ =>
        {
            _paletteService.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });
        _frameModified = Subjects.FrameModified.Subscribe(_ =>
        {
            _paletteService.SetCurrentColors();
            OnPropertyChanged(nameof(CurrentPaletteColors));
        });

        PrimaryColor = UniColor.Black;
        SecondaryColor = UniColor.White;

        _paletteSelected = Subjects.PaletteSelected.Subscribe(_ =>
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
                _frameModified.Dispose();
                _layerModified.Dispose();
                _projectChanged.Dispose();
                _paletteSelected.Dispose();
                _primaryProjectChanged.Dispose();
                _primaryProjectChange.Dispose();
                _secondaryProjectChanged.Dispose();
                _secondaryProjectChange.Dispose();
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
        var files = await _dialogUtils.OpenFileDialog("All Supported (.json;.gpl;.png)|*.json;*.gpl;*.png|Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl|PNG Palettes (*.png)|*.png", _paletteService.SelectedPalette.Name);

        if (files.Count == 0)
        {
            return;
        }

        await Load(files[0]);
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
            await Save(file);
        }
    }

    public void PaletteListAction()
    {
        Router.Navigate("/palettes");
    }

    public async Task Rename(PaletteModel model, string newName)
    {
        int index = _paletteService.Palettes.IndexOf(model);
        var palette = _paletteService.Palettes[index];
        palette.Name = newName;

        var file = await _platformFolder.GetFile(palette.Path, FolderType.Palettes);

        if (file != null)
        {
            await Save(palette, file);
        }
    }

    public async Task LoadAll()
    {
        var files = _platformFolder.GetFiles(FolderType.Palettes);
        await foreach (var file in files)
        {
            if (file == null)
            {
                continue;
            }
            
            Stream? stream = null;
            try
            {
                stream = await file.OpenRead();

                if (stream == null)
                {
                    continue;
                }
                
                var serializer = AbstractPaletteSerializer.GetFromExtension(file.Extension);
                var palette = serializer.Deserialize(stream, file.Name);
                _paletteService.Palettes.Add(palette);
            }
            catch (Exception)
            {
                if (stream != null)
                {
                    await stream.DisposeAsync();
                }
            }
        }
    }

    private async Task Load(IStorageFile file)
    {
        string name = file.Name;
        var paletteFile = await _platformFolder.GetFile(name, FolderType.Palettes);

        if (paletteFile != null)
        {
            await file.CopyTo(paletteFile);
        }

        var stream = await file.OpenRead();

        if (stream == null)
        {
            return;
        }
        
        var serializer = AbstractPaletteSerializer.GetFromExtension(file.GetExtension());

        PaletteModel model;
        try
        {
            model = serializer.Deserialize(stream, file.Name);
            await stream.DisposeAsync();
        }
        catch (Exception)
        {
            await Router.Message("Opening error", "Invalid format");
            await stream.DisposeAsync();
            return;
        }
        _paletteService.Palettes[1] = model.ToCurrentPalette();

        if (_paletteService.Palettes.FirstOrDefault(p => p != null && p.Id == model.Id, null) == null)
        {
            _paletteService.Palettes.Add(model);
        }
        else
        {
            _paletteService.Palettes[_paletteService.Palettes.FindIndex(p => p.Id == model.Id)] = model;
        }
        Subjects.PaletteSelected.OnNext(_paletteService.Palettes[1]);
        Subjects.PaletteAdded.OnNext(_paletteService.Palettes[1]);
    }

    private async Task Save(IStorageFile file)
    {
        var model = _paletteService.Palettes[1].ToCurrentPalette();
        model.Path = file.Name;
        await Save(model, file);

        var paletteFile = await _platformFolder.GetFile(file.Name, FolderType.Palettes);

        if (paletteFile != null)
        {
            await Save(model, paletteFile);
        }
    }

    private async static Task Save(PaletteModel model, IStorageFile file)
    {
        FileInfo fileInfo = new(model.Path);

        AbstractPaletteSerializer serializer = AbstractPaletteSerializer.GetFromExtension(fileInfo.Extension);

        var stream = await file.OpenWrite();

        if (stream == null)
        {
            return;
        }
        
        serializer.Serialize(stream, model);
        await stream.DisposeAsync();
    }

    private static async Task Save(PaletteModel model, IStorageContainerFile file)
    {
        AbstractPaletteSerializer serializer = AbstractPaletteSerializer.GetFromExtension(file.Extension);

        var stream = await file.OpenWrite();

        if (stream == null)
        {
            return;
        }
        
        serializer.Serialize(stream, model);
        await stream.DisposeAsync();
    }
}
