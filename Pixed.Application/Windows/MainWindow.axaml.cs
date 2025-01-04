using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Application.Controls;
using Pixed.Application.Input;
using Pixed.Application.IO;
using Pixed.Application.Pages;
using Pixed.Application.Platform;
using Pixed.Application.ViewModels;
using Pixed.Common;
using Pixed.Common.Input;
using Pixed.Common.Services.Keyboard;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class MainWindow : Modal<MainViewModel>
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly IPlatformSettings _lifecycle;
    private bool _closeStarted = false;
    public MainWindow(PixedProjectMethods pixedProjectMethods, IPlatformSettings lifecycle) : base()
    {
        InitializeComponent();
        _pixedProjectMethods = pixedProjectMethods;
        _lifecycle = lifecycle;
    }

    public async Task OpenFromArgs(string[] args)
    {
        foreach (var arg in args)
        {
            if (File.Exists(arg))
            {
                await _pixedProjectMethods.Open(arg);
            }
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        e.Cancel = true;

        if (_closeStarted)
        {
            return;
        }

        _closeStarted = true;
        var canQuit = await Main.Close();

        if (canQuit)
        {
            _lifecycle.Close();
        }
        _closeStarted = false;
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