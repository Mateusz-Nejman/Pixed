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
using Pixed.Common.Services;
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
    private readonly ApplicationData? _applicationData;
    private readonly PixedProjectMethods? _pixedProjectMethods;
    private readonly FileMenuRegister? _fileMenuRegister;
    private readonly TransformMenuRegister? _transformToolsMenuRegister;
    private readonly UndoRedoMenuRegister? _undoRedoMenuRegister;
    private readonly CopyPasteMenuRegister? _copyPasteMenuRegister;
    private readonly PaletteMenuRegister? _paletteMenuRegister;
    private readonly ProjectMenuRegister? _projectMenuRegister;
    private readonly ViewMenuRegister? _viewMenuRegister;
    private readonly ToolsMenuRegister? _toolsMenuRegister;
    private readonly IHistoryService? _historyService;
    private readonly RecentFilesService? _recentFilesService;
    private readonly ToolsManager? _toolSelector;
    private readonly MenuBuilder? _menuBuilder;
    private readonly IStorageProviderHandle? _storageProviderHandle;
    private readonly IPlatformFolder? _platformFolder;
    private readonly IDisposable _newInstanceHandled;
    private readonly PaletteSectionViewModel? _paletteSectionViewModel;
    private bool _disposedValue;

    public static ICommand? QuitCommand { get; private set; }
    public Main() : base()
    {
        InjectMethods();

        if (Provider != null)
        {
            _pixedProjectMethods = Provider.Get<PixedProjectMethods>();
            _applicationData = Provider.Get<ApplicationData>();
            _fileMenuRegister = Provider.Get<FileMenuRegister>();
            _transformToolsMenuRegister = Provider.Get<TransformMenuRegister>();
            _undoRedoMenuRegister = Provider.Get<UndoRedoMenuRegister>();
            _copyPasteMenuRegister = Provider.Get<CopyPasteMenuRegister>();
            _paletteMenuRegister = Provider.Get<PaletteMenuRegister>();
            _projectMenuRegister = Provider.Get<ProjectMenuRegister>();
            _viewMenuRegister = Provider.Get<ViewMenuRegister>();
            _toolsMenuRegister = Provider.Get<ToolsMenuRegister>();
            _historyService = Provider.Get<IHistoryService>();
            _recentFilesService = Provider.Get<RecentFilesService>();
            _toolSelector = Provider.Get<ToolsManager>();
            _menuBuilder = Provider.Get<MenuBuilder>();
            _storageProviderHandle = Provider.Get<IStorageProviderHandle>();
            _platformFolder = Provider.Get<IPlatformFolder>();
            _paletteSectionViewModel = Provider.Get<PaletteSectionViewModel>();
        }
        
        _newInstanceHandled = Subjects.NewInstanceHandled.Subscribe(args =>
        {
            foreach (var arg in args)
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    if (_pixedProjectMethods != null)
                    {
                        await _pixedProjectMethods.Open(arg);
                    }
                });
            }
        });
    }

    public override async void OnLoaded()
    {
        if (_fileMenuRegister == null || _menuBuilder == null || _paletteSectionViewModel == null ||
            _applicationData == null)
        {
            return;
        }
        
        var topLevel = TopLevel.GetTopLevel(this);
        _storageProviderHandle?.Initialize(topLevel?.StorageProvider);

        if (Provider != null)
        {
            var clipboard = Provider.Get<IClipboardHandle>();
            clipboard.Initialize(topLevel?.Clipboard);
        }
        _viewMenuRegister?.Register();
        _transformToolsMenuRegister?.Register();
        _undoRedoMenuRegister?.Register();
        _copyPasteMenuRegister?.Register();
        _paletteMenuRegister?.Register();
        _toolsMenuRegister?.Register();

        await Initialize();
        if (IPlatformSettings.Instance.RecentFilesEnabled && _recentFilesService != null)
        {
            await _recentFilesService.Load();
        }
        _projectMenuRegister?.Register();
        await _fileMenuRegister.Register();
        await _menuBuilder.Build();
        _toolSelector?.SelectTool("tool_pen");
        await _paletteSectionViewModel.LoadAll();

        Subjects.ProjectAdded.OnNext(_applicationData.CurrentModel);
        Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
        base.OnLoaded();
    }

    public async static Task<bool> Close()
    {
        if (Provider != null)
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

    private void InitializeBeforeUi()
    {
        QuitCommand = new ActionCommand(async () =>
        {
            var canQuit = await Close();

            if (canQuit && _historyService != null)
            {
                await _historyService.ClearTempFiles();
                IPlatformSettings.Instance.Close();
            }
        });
    }

    private async Task InitializeDataFolder()
    {
        if (_applicationData == null || _historyService == null || _platformFolder == null)
        {
            return;
        }
        
        _platformFolder.GetFiles(FolderType.Root);
        _platformFolder.GetFiles(FolderType.Palettes);

        if (IPlatformSettings.Instance.ExtensionsEnabled)
        {
            _platformFolder.GetFiles(FolderType.Extensions);
        }

        _applicationData.UserSettings = await SettingsUtils.Load(_platformFolder);
        _applicationData.Initialize();
        _historyService.Register(_applicationData.CurrentModel);
        await _historyService.AddToHistory(_applicationData.CurrentModel, true);
        _applicationData.CurrentModel.UnsavedChanges = false;
        Subjects.AnimationPreviewChanged.OnNext(_applicationData.UserSettings.AnimationPreviewVisible);
    }

    private async Task Initialize()
    {
        await InitializeDataFolder();
        InitializeBeforeUi();
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private async void Drop(object? sender, DragEventArgs e)
    {
        if (_pixedProjectMethods == null)
        {
            return;
        }
        
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
        if (_applicationData == null)
        {
            return;
        }
        
        var result = await Router.NumericPrompt("Change opacity", "Enter new opacity (%):", layer.Opacity);

        if (result.HasValue)
        {
            layer.Opacity = result.Value;
            Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
        }
    }

    private void CloseCommandAction(PixedModel model)
    {
        if (_applicationData == null)
        {
            return;
        }

        if (_applicationData.Models.Count == 1)
        {
            QuitCommand?.Execute(null);
        }
        else
        {
            _applicationData.Models.Remove(model);
            Subjects.ProjectRemoved.OnNext(model);
            _applicationData.CurrentModelIndex = Math.Clamp(_applicationData.CurrentModelIndex, 0, _applicationData.Models.Count - 1);
        }
    }
}