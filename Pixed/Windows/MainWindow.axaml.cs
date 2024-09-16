using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using System.Windows.Input;

namespace Pixed.Windows;

public partial class MainWindow : Window
{
    public static Window? Handle { get; private set; }
    public static ICommand? QuitCommand { get; private set; }
    public MainWindow()
    {
        Handle = this;
        QuitCommand = new ActionCommand(() => Handle.Close());
        InitializeBeforeUI();
        InitializeComponent();

        var menu = NativeMenu.GetMenu(this);

        Global.ToolSelector.SelectTool("tool_pen");
        toolsSection.PaintCanvas = paintCanvas.ViewModel;
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

    private static void InitializeBeforeUI()
    {
        Global.Models.Add(new PixedModel());
        Global.CurrentModel.Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
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
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)));
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