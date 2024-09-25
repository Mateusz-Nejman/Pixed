using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Controls;
using Pixed.Input;
using Pixed.IO;
using Pixed.Menu;
using Pixed.Models;
using Pixed.Selection;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Tools;
using Pixed.Tools.Transform;
using Pixed.ViewModels;
using System;
using System.IO;
using System.Windows.Input;

namespace Pixed.Windows;

internal partial class MainWindow : PixedWindow<MainViewModel>
{
    private readonly SelectionManager _selectionManager;
    private readonly ApplicationData _applicationData;
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly TransformToolsMenuRegister _menuRegister;
    private readonly RecentFilesService _recentFilesService;
    private readonly ToolSelector _toolSelector;
    private readonly MenuBuilder _menuBuilder;
    public static Window? Handle { get; private set; }
    public static ICommand? QuitCommand { get; private set; }
    public MainWindow(SelectionManager selectionManager, ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, MenuBuilder builder, MenuItemRegistry menuItemRegistry,
        TransformToolsMenuRegister transformToolsMenuRegister, RecentFilesService recentFilesService, ToolSelector toolSelector) : base(menuItemRegistry)
    {
        _selectionManager = selectionManager;
        _pixedProjectMethods = pixedProjectMethods;
        _applicationData = applicationData;
        _menuRegister = transformToolsMenuRegister;
        _recentFilesService = recentFilesService;
        _toolSelector = toolSelector;
        _menuBuilder = builder;

        InitializeBeforeUI();
        InitializeComponent();
    }

    public override void OnLoaded()
    {
        _toolSelector.SelectTool("tool_pen");

        Subjects.ProjectAdded.OnNext(_applicationData.CurrentModel);
        Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
        Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _menuRegister.Register();
        _menuBuilder.Build();
    }

    private void InitializeBeforeUI()
    {
        Handle = this;
        QuitCommand = new ActionCommand(async () =>
        {
            int untitledIndex = 0;

            foreach (var model in _applicationData.Models)
            {
                if (!model.HistoryEmpty)
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
}