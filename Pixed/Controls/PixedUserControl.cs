using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
using static Pixed.MenuBuilder;

namespace Pixed.Controls;
internal abstract class PixedUserControl<T> : UserControl
{
    protected readonly MenuBuilder _menuBuilder;

    protected IServiceProvider ServiceProvider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public PixedUserControl()
    {
        _menuBuilder = ServiceProvider.GetService<MenuBuilder>();
        this.DataContext = this.CreateInstance<T>();
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

internal abstract class EmptyPixedUserControl() : UserControl
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Unloaded += EmptyPixedUserControl_Unloaded;
    }

    private void EmptyPixedUserControl_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }
}
