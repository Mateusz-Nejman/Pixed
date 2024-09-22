using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pixed.StaticMenuBuilder;
using System.Windows.Input;

namespace Pixed.Controls;
internal class PixedWindow : Window
{
    public virtual void RegisterMenuItems()
    {
        if (DataContext is PixedViewModel model)
        {
            model.RegisterMenuItems();
        }
    }

    public virtual void OnLoaded()
    {

    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        RegisterMenuItems();
        Unloaded += PixedUserControl_Unloaded;
        Loaded += PixedWindow_Loaded;

        if (DataContext is PixedViewModel model)
        {
            model.OnInitialized();
        }
    }

    private void PixedWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is PixedViewModel model)
        {
            model.OnLoaded();
        }
    }

    private void PixedUserControl_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }

    public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, Action action)
    {
        RegisterMenuItem(baseMenu, text, new ActionCommand(action));
    }

    public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
    {
        RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
    }

    public static void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
    {
        StaticMenuBuilder.AddEntry(baseMenu, menuItem);
    }
}
