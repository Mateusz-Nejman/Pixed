using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Controls;
using Pixed.Input;
using Pixed.IO;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.ViewModels;
using System;
using System.IO;
using System.Windows.Input;

namespace Pixed.Windows;

internal partial class MainWindow : PixedWindow<MainViewModel>
{
    public static Window? Handle { get; private set; }
    public static ICommand? QuitCommand { get; private set; }
    public MainWindow()
    {
        InitializeBeforeUI();
        InitializeComponent();
    }

    public override void OnLoaded()
    {
        Global.ToolSelector.SelectTool("tool_pen");
        toolsSection.PaintCanvas = paintCanvas.ViewModel;
        ViewModel.PaintCanvasMouseWheelCommand = paintCanvas.ViewModel.MouseWheel;

        Subjects.ProjectAdded.OnNext(Global.CurrentModel);
        Subjects.ProjectChanged.OnNext(Global.CurrentModel);
        Subjects.FrameChanged.OnNext(Global.CurrentFrame);

        Global.SelectionManager = new Selection.SelectionManager(ov =>
        {
            paintCanvas.ViewModel.Overlay = ov;
        });
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        StaticMenuBuilder.Build();
    }

    private void InitializeBeforeUI()
    {
        Handle = this;
        QuitCommand = new ActionCommand(async () =>
        {
            int untitledIndex = 0;

            foreach (var model in Global.Models)
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
                        await PixedProjectMethods.Save(model, model.FilePath == null);
                    }
                }
            }

            Handle.Close();
        });
        Global.DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pixed");
        Global.UserSettings = Settings.Load();
        Global.Models.Add(new PixedModel(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
        Global.CurrentModel.FileName = Global.NamingService.GenerateName();
        Global.CurrentModel.AddHistory(false);
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