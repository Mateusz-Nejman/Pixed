using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
using static Pixed.MenuBuilder;

namespace Pixed.Controls;
internal abstract class PixedWindow<T> : Window
{
    protected readonly MenuBuilder _menuBuilder;
    public PixedWindow(MenuBuilder menuBuilder)
    {
        this.DataContext = this.CreateInstance<T>();
        _menuBuilder = menuBuilder;
    }
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

    public void RegisterMenuItem(BaseMenuItem baseMenu, string text, Action action)
    {
        RegisterMenuItem(baseMenu, text, new ActionCommand(action));
    }

    public void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
    {
        RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
    }

    public void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
    {
        _menuBuilder.AddEntry(baseMenu, menuItem);
    }
}
