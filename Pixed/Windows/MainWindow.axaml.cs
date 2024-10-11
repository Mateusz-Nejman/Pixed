using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Pixed.Controls;
using Pixed.Input;
using Pixed.IO;
using Pixed.Menu;
using Pixed.Models;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Tools;
using Pixed.Utils;
using Pixed.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Windows;

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
    private IDisposable _newInstanceHandled;
    private bool _disposedValue;
    private IDisposable[] _debugDisposables;

    public static Window? Handle { get; private set; }
    public static ICommand? QuitCommand { get; private set; }
    public MainWindow(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, MenuBuilder builder, MenuItemRegistry menuItemRegistry,
        TransformMenuRegister transformToolsMenuRegister, RecentFilesService recentFilesService, ToolSelector toolSelector, CopyPasteMenuRegister copyPasteMenuRegister,
        ViewMenuRegister viewMenuRegister) : base(menuItemRegistry)
    {
        _debugDisposables = Subjects.InitDebug();
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
                foreach(var disposable in _debugDisposables)
                {
                    disposable.Dispose();
                }
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
}