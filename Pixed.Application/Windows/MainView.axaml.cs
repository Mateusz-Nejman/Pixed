using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Pixed.Application.Controls;
using Pixed.Application.Input;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Services;
using Pixed.Application.ViewModels;
using Pixed.Common;
using Pixed.Common.Input;
using Pixed.Common.Menu;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Common.Utils;
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
    private readonly ToolSelector _toolSelector;
    private readonly MenuBuilder _menuBuilder;
    private readonly IDisposable _newInstanceHandled;
    private bool _disposedValue;

    public static Window? Handle { get; private set; }
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
        _newInstanceHandled = Subjects.NewInstanceHandled.Subscribe(args =>
        {
            foreach (var arg in args)
            {
                Dispatcher.UIThread.Invoke(() => _pixedProjectMethods.Open(arg));
            }
        });
        Initialize();
    }

    public override void OnLoaded()
    {
        _toolSelector.SelectTool("tool_pen");

        Subjects.ProjectAdded.OnNext(_applicationData.CurrentModel);
        Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.OnInitialized();
        _viewMenuRegister.Register();
        _transformToolsMenuRegister.Register();
        _copyPasteMenuRegister.Register();
        _paletteMenuRegister.Register();
        _toolsMenuRegister.Register();
        _menuBuilder.Build();
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
        Handle = MainWindow.Handle;
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

                    if(result.HasValue)
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

            Handle.Close();
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
        var pixedFolder = await MainWindow.Handle.StorageProvider.GetPixedFolder();
        await pixedFolder.CreateFolderAsync("Palettes");
        await pixedFolder.CreateFolderAsync("Extensions");
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

        if(result.HasValue)
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