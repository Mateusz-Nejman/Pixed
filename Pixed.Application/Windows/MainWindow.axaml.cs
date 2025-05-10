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
using Pixed.Common.Services;
using Pixed.Common.Services.Keyboard;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class MainWindow : ExtendedWindow<MainViewModel>
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly IHistoryService _historyService;
    private bool _closeStarted = false;
    public MainWindow(PixedProjectMethods pixedProjectMethods, IHistoryService historyService) : base()
    {
        InitializeComponent();
        _pixedProjectMethods = pixedProjectMethods;
        _historyService = historyService;
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
            await _historyService.ClearTempFiles();
            IPlatformSettings.Instance.Close();
        }
        _closeStarted = false;
    }

    private void Window_KeyUp(object? sender, KeyEventArgs e)
    {
#if DEBUG
        if (e.Key == Key.D)
        {
            var skia = AppDomain.CurrentDomain.GetAssemblies().First(a => a.ToString() == "SkiaSharp, Version=3.119.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
            if (skia != null)
            {
                Type handleDictionary = skia.GetType("SkiaSharp.HandleDictionary");
                var fields = handleDictionary.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                var instanceField = fields.First(f => f.Name == "instances");
                Dictionary<IntPtr, WeakReference> instances = (Dictionary<nint, WeakReference>)instanceField.GetValue(null);
                var values = instances.Values.ToArray();
                var targetBitmaps = values.Select(v => v.Target).Where(t => t is SKBitmap).ToList();
                var targetImages = values.Select(v => v.Target).Where(t => t is SKImage).ToList();
                skia.ToString();
            }
        }
#endif
        Keyboard.Modifiers = e.KeyModifiers;
        Keyboard.ProcessReleased(e.Key);
        Subjects.KeyState.OnNext(new KeyState(
                e.Key,
                false,
                Keyboard.Modifiers.HasFlag(KeyModifiers.Shift),
                Keyboard.Modifiers.HasFlag(KeyModifiers.Control),
                Keyboard.Modifiers.HasFlag(KeyModifiers.Alt)));
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        Keyboard.Modifiers = e.KeyModifiers;
        Keyboard.ProcessPressed(e.Key);
        Subjects.KeyState.OnNext(new KeyState(
                e.Key,
                true,
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