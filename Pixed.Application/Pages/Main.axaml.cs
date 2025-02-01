using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Pixed.Application.Controls;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Services;
using Pixed.Application.Utils;
using Pixed.Application.ViewModels;
using Pixed.Common;
using Pixed.Common.Platform;
using Pixed.Common.Services.Palette;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.Pages;

internal partial class Main : EmptyPixedPage, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly TransformMenuRegister _transformToolsMenuRegister;
    private readonly CopyPasteMenuRegister _copyPasteMenuRegister;
    private readonly PaletteMenuRegister _paletteMenuRegister;
    private readonly ViewMenuRegister _viewMenuRegister;
    private readonly ToolsMenuRegister _toolsMenuRegister;
    private readonly RecentFilesService _recentFilesService;
    private readonly PaletteService _paletteService;
    private readonly ToolsManager _toolSelector;
    private readonly MenuBuilder _menuBuilder;
    private readonly IStorageProviderHandle _storageProviderHandle;
    private readonly IDisposable _newInstanceHandled;
    private readonly IPlatformSettings _lifecycle;
    private readonly PaletteSectionViewModel _paletteSectionViewModel;
    private bool _disposedValue;

    public static ICommand? QuitCommand { get; private set; }
    public Main() : base()
    {
        InjectMethods();
        _pixedProjectMethods = Get<PixedProjectMethods>();
        _applicationData = Get<ApplicationData>();
        _transformToolsMenuRegister = Get<TransformMenuRegister>();
        _copyPasteMenuRegister = Get<CopyPasteMenuRegister>();
        _paletteMenuRegister = Get<PaletteMenuRegister>();
        _viewMenuRegister = Get<ViewMenuRegister>();
        _toolsMenuRegister = Get<ToolsMenuRegister>();
        _recentFilesService = Get<RecentFilesService>();
        _toolSelector = Get<ToolsManager>();
        _menuBuilder = Get<MenuBuilder>();
        _paletteService = Get<PaletteService>();
        _storageProviderHandle = Get<IStorageProviderHandle>();
        _lifecycle = Get<IPlatformSettings>();
        _paletteSectionViewModel = Get<PaletteSectionViewModel>();
        _newInstanceHandled = Subjects.NewInstanceHandled.Subscribe(args =>
        {
            foreach (var arg in args)
            {
                Dispatcher.UIThread.Invoke(async () => await _pixedProjectMethods.Open(arg));
            }
        });
    }

    public override async void OnLoaded()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        _storageProviderHandle.Initialize(topLevel.StorageProvider);
        var clipboard = Get<IClipboardHandle>();
        clipboard.Initialize(topLevel.Clipboard);
        _viewMenuRegister.Register();
        _transformToolsMenuRegister.Register();
        _copyPasteMenuRegister.Register();
        _paletteMenuRegister.Register();
        _toolsMenuRegister.Register();

        await Initialize();
        if(_lifecycle.RecentFilesEnabled)
        {
            await _recentFilesService.Load();
        }
        await _menuBuilder.Build();
        _toolSelector.SelectTool("tool_pen");
        await _paletteSectionViewModel.LoadAll();

        Subjects.ProjectAdded.OnNext(_applicationData.CurrentModel);
        Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
        base.OnLoaded();
    }

    public async static Task<bool> Close()
    {
        var applicationData = Provider.Get<ApplicationData>();
        var pixedProjectMethods = Provider.Get<PixedProjectMethods>();
        var recentFilesService = Provider.Get<RecentFilesService>();

        int untitledIndex = 0;

        foreach (var model in applicationData.Models)
        {
            if (model.UnsavedChanges)
            {
                var name = model.FileName;

                if (string.IsNullOrEmpty(name))
                {
                    name = "Untitled-" + untitledIndex;
                    untitledIndex++;
                }

                var result = await Router.Confirm("Unsaved changes", "Project " + name + " has unsaved changes. Save it now?");

                if (result.HasValue)
                {
                    if (result.Value == ButtonResult.Cancel)
                    {
                        return false;
                    }
                    else if (result.Value == ButtonResult.Yes)
                    {
                        await pixedProjectMethods.Save(model, model.FilePath == null, recentFilesService);
                    }
                }
            }
        }

        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _newInstanceHandled?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void InitializeBeforeUI()
    {
        QuitCommand = new ActionCommand(async () =>
        {
            var canQuit = await Close();

            if (canQuit)
            {
                _lifecycle.Close();
            }
        });
    }

    private async Task InitializeDataFolder()
    {
        _storageProviderHandle.StorageFolder.GetFiles(FolderType.Root);
        _storageProviderHandle.StorageFolder.GetFiles(FolderType.Palettes);

        if (_lifecycle.ExtensionsEnabled)
        {
            _storageProviderHandle.StorageFolder.GetFiles(FolderType.Extensions);
        }
        _applicationData.UserSettings = await SettingsUtils.Load(_storageProviderHandle.StorageFolder);
        _applicationData.Initialize();
        Subjects.AnimationPreviewChanged.OnNext(_applicationData.UserSettings.AnimationPreviewVisible);
    }

    private async Task Initialize()
    {
        await InitializeDataFolder();
        InitializeBeforeUI();
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private async void Drop(object? sender, DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files != null)
        {
            foreach (var file in files)
            {
                await _pixedProjectMethods.Open(file.Path.AbsolutePath);
            }

            e.Handled = true;
        }
    }

    private void InjectMethods()
    {
        Layer.ChangeOpacityAction = ChangeOpacityAction;
        PixedModel.CloseCommandAction = CloseCommandAction;
    }

    private async Task ChangeOpacityAction(Layer layer)
    {
        var result = await Router.NumericPrompt("Change opacity", "Enter new opacity (%):", layer.Opacity);

        if (result.HasValue)
        {
            layer.Opacity = result.Value;
            layer.RefreshRenderSource();
        }
    }

    private void CloseCommandAction(PixedModel model)
    {
        if (_applicationData.Models.Count == 1)
        {
            QuitCommand.Execute(null);
        }
        else
        {
            _applicationData.Models.Remove(model);
            Subjects.ProjectRemoved.OnNext(model);
            _applicationData.CurrentModelIndex = Math.Clamp(_applicationData.CurrentModelIndex, 0, _applicationData.Models.Count - 1);
        }
    }
}