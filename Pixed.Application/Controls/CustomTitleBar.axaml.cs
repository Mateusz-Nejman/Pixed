using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Pixed.Application.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pixed.Application.Controls;

internal partial class CustomTitleBar : EmptyPixedUserControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public bool CanMinimize
    {
        get { return GetValue(CanMinimizeProperty); }
        set { SetValue(CanMinimizeProperty, value); }
    }

    public bool CanMaximize
    {
        get { return GetValue(CanMaximizeProperty); }
        set { SetValue(CanMaximizeProperty, value); }
    }

    public List<MenuItem>? Menu
    {
        get => GetValue(MenuProperty);
        set => SetValue(MenuProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<CustomTitleBar, string>("Title", "Title");
    public static readonly StyledProperty<bool> CanMinimizeProperty = AvaloniaProperty.Register<CustomTitleBar, bool>("CanMinimize", true);
    public static readonly StyledProperty<bool> CanMaximizeProperty = AvaloniaProperty.Register<CustomTitleBar, bool>("CanMaximize", true);
    public static readonly StyledProperty<List<MenuItem>?> MenuProperty = AvaloniaProperty.Register<CustomTitleBar, List<MenuItem>?>(nameof(Menu), null);
    public CustomTitleBar() : base()
    {
        InitializeComponent();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            minimizeButton.Click += MinimizeWindow;
            maximizeButton.Click += MaximizeWindow;
            closeButton.Click += CloseWindow;
            windowIcon.DoubleTapped += CloseWindow;

            SubscribeToWindowState();
        }
        else
        {
            windowButtons.IsVisible = false;
        }
    }

    private void CloseWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainPage.QuitCommand?.Execute(null);
    }

    private void MaximizeWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Window hostWindow = (Window)this.VisualRoot;

        if (hostWindow.WindowState == WindowState.Normal)
        {
            hostWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            hostWindow.WindowState = WindowState.Normal;
        }
    }

    private void MinimizeWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Window hostWindow = (Window)this.VisualRoot;
        hostWindow.WindowState = WindowState.Minimized;
    }

    private async void SubscribeToWindowState()
    {
        Window hostWindow = (Window)this.VisualRoot;

        while (hostWindow == null)
        {
            hostWindow = (Window)this.VisualRoot;
            await Task.Delay(50);
        }

        hostWindow.GetObservable(Window.WindowStateProperty).Subscribe(s =>
        {
            if (s != WindowState.Maximized)
            {
                maximizeIcon.Data = Geometry.Parse("M2048 2048v-2048h-2048v2048h2048zM1843 1843h-1638v-1638h1638v1638z");
                hostWindow.Padding = new Thickness(0, 0, 0, 0);
                maximizeToolTip.Content = "Maximize";
            }
            if (s == WindowState.Maximized)
            {
                maximizeIcon.Data = Geometry.Parse("M2048 1638h-410v410h-1638v-1638h410v-410h1638v1638zm-614-1024h-1229v1229h1229v-1229zm409-409h-1229v205h1024v1024h205v-1229z");
                hostWindow.Padding = new Thickness(7, 7, 7, 7);
                maximizeToolTip.Content = "Restore Down";
            }
        });
    }
}