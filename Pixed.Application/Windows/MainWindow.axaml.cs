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
using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Common.Utils;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.Windows;

internal partial class MainWindow : PixedWindow<MainViewModel>, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly TransformMenuRegister _transformToolsMenuRegister;
    private readonly CopyPasteMenuRegister _copyPasteMenuRegister;
    private readonly ViewMenuRegister _viewMenuRegister;
    private readonly RecentFilesService _recentFilesService;
    private readonly ToolSelector _toolSelector;
    private readonly MenuBuilder _menuBuilder;
    private readonly IDisposable _newInstanceHandled;
    private bool _disposedValue;

    public static Window? Handle { get; private set; }
    public static ICommand? QuitCommand { get; private set; }
    public MainWindow(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, MenuBuilder builder, IMenuItemRegistry menuItemRegistry,
        TransformMenuRegister transformToolsMenuRegister, RecentFilesService recentFilesService, ToolSelector toolSelector, CopyPasteMenuRegister copyPasteMenuRegister,
        ViewMenuRegister viewMenuRegister) : base(menuItemRegistry)
    {
        InjectMethods();
        _pixedProjectMethods = pixedProjectMethods;
        _applicationData = applicationData;
        _transformToolsMenuRegister = transformToolsMenuRegister;
        _copyPasteMenuRegister = copyPasteMenuRegister;
        _viewMenuRegister = viewMenuRegister;
        _recentFilesService = recentFilesService;
        _toolSelector = toolSelector;
        _menuBuilder = builder;
        _newInstanceHandled = Subjects.NewInstanceHandled.Subscribe(args =>
        {
            foreach (var arg in args)
            {
                Dispatcher.UIThread.Invoke(() => _pixedProjectMethods.Open(arg));
            }
        });
        InitializeWindow();
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
        _viewMenuRegister.Register();
        _transformToolsMenuRegister.Register();
        _copyPasteMenuRegister.Register();
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
        Handle = this;
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

                    YesNoCancelWindow window = new()
                    {
                        Title = "Unsaved changes",
                        Text = "Project " + name + " has unsaved changes. Save it now?"
                    };

                    var result = await window.ShowDialog<ButtonResult>(this);

                    if (result == ButtonResult.Cancel)
                    {
                        return;
                    }
                    else if (result == ButtonResult.Yes)
                    {
                        await _pixedProjectMethods.Save(model, model.FilePath == null, _recentFilesService);
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
        var pixedFolder = await StorageProvider.GetPixedFolder();
        await pixedFolder.CreateFolderAsync("Palettes");
        await pixedFolder.CreateFolderAsync("Extensions");
        _applicationData.Initialize(pixedFolder);
    }

    private async Task InitializeWindow()
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
        NumericPrompt numeric = new(Opacity)
        {
            Text = "Enter new opacity (%):"
        };
        var success = await numeric.ShowDialog<bool>(Handle);

        if (success)
        {
            Opacity = numeric.Value;
            layer.RefreshRenderSource();
        }
    }

    private void CloseCommandAction(PixedModel model)
    {
        if (_applicationData.Models.Count == 1)
        {
            MainWindow.QuitCommand.Execute(null);
        }
        else
        {
            _applicationData.Models.Remove(model);
            Subjects.ProjectRemoved.OnNext(model);
            _applicationData.CurrentModelIndex = Math.Clamp(_applicationData.CurrentModelIndex, 0, _applicationData.Models.Count - 1);
        }
    }
}