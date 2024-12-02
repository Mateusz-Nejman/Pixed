using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Pixed.Application.Controls;
using Pixed.Application.Input;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Platform;
using Pixed.Application.Services;
using Pixed.Application.ViewModels;
using Pixed.Common;
using Pixed.Common.Input;
using Pixed.Common.Platform;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Services.Palette;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.Windows;

internal partial class MainView : PixedPage<MainViewModel>, IDisposable
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
    private readonly ToolSelector _toolSelector;
    private readonly MenuBuilder _menuBuilder;
    private readonly IStorageProviderHandle _storageProviderHandle;
    private readonly IDisposable _newInstanceHandled;
    private readonly IApplicationLifecycle _lifecycle;
    private bool _disposedValue;

    public static ICommand? QuitCommand { get; private set; }
    public MainView() : base()
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
        _toolSelector = Get<ToolSelector>();
        _menuBuilder = Get<MenuBuilder>();
        _paletteService = Get<PaletteService>();
        _storageProviderHandle = Get<IStorageProviderHandle>();
        _lifecycle = Get<IApplicationLifecycle>();
        _newInstanceHandled = Subjects.NewInstanceHandled.Subscribe(args =>
        {
            foreach (var arg in args)
            {
                Dispatcher.UIThread.Invoke(() => _pixedProjectMethods.Open(arg));
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
        _menuBuilder.Build();

        await Initialize();
        _toolSelector.SelectTool("tool_pen");
        await _recentFilesService.Load();
        await _paletteService.LoadAll();

        Subjects.ProjectAdded.OnNext(_applicationData.CurrentModel);
        Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
        base.OnLoaded();
    }

    public void OpenFromArgs(string[] args)
    {
        foreach (var arg in args)
        {
            if (File.Exists(arg))
            {
                _pixedProjectMethods.Open(arg);
            }
        }
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
            int untitledIndex = 0;

            foreach (var model in _applicationData.Models)
            {
                if (model.UnsavedChanges)
                {
                    var name = model.FileName;

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Untitled-" + untitledIndex;
                        untitledIndex++;
                    }

                    var result = await RouterControl.Navigate<ButtonResult>("/unsavedChanges", name);

                    if (result.HasValue)
                    {
                        if (result.Value == ButtonResult.Cancel)
                        {
                            return;
                        }
                        else if (result.Value == ButtonResult.Yes)
                        {
                            await _pixedProjectMethods.Save(model, model.FilePath == null, _recentFilesService);
                        }
                    }
                }
            }

            _lifecycle.Close();
        });
    }

    private void Window_KeyUp(object? sender, KeyEventArgs e)
    {
        Keyboard.Modifiers = e.KeyModifiers;
        Keyboard.ProcessReleased(e.Key);
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        Keyboard.Modifiers = e.KeyModifiers;
        Keyboard.ProcessPressed(e.Key);
        Subjects.KeyState.OnNext(new KeyState(
                e.Key,
                Keyboard.Modifiers.HasFlag(KeyModifiers.Shift),
                Keyboard.Modifiers.HasFlag(KeyModifiers.Control),
                Keyboard.Modifiers.HasFlag(KeyModifiers.Alt)));
    }

    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        Mouse.ProcessPoint(point);
    }

    private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        Mouse.ProcessPoint(point);
    }

    private async Task InitializeDataFolder()
    {
        var pixedFolder = await _storageProviderHandle.GetPixedFolder();
        await _storageProviderHandle.GetPalettesFolder();

        if(_storageProviderHandle.StorageFolder.ExtensionsEnabled)
        {
            await _storageProviderHandle.GetExtensionsFolder();
        }
        _applicationData.Initialize(pixedFolder);
    }

    private async Task Initialize()
    {
        await InitializeDataFolder();
        InitializeBeforeUI();
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files != null)
        {
            foreach (var file in files)
            {
                _pixedProjectMethods.Open(file.Path.AbsolutePath);
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
        var result = await RouterControl.Navigate<double>("/changeOpacity", layer.Opacity);

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
            MainView.QuitCommand.Execute(null);
        }
        else
        {
            _applicationData.Models.Remove(model);
            Subjects.ProjectRemoved.OnNext(model);
            _applicationData.CurrentModelIndex = Math.Clamp(_applicationData.CurrentModelIndex, 0, _applicationData.Models.Count - 1);
        }
    }
}