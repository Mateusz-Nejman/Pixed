using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Menu;
using System;

namespace Pixed.Controls;
internal abstract class PixedUserControl<T> : UserControl
{
    protected readonly MenuItemRegistry _menuItemRegistry;

    protected IServiceProvider ServiceProvider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public PixedUserControl()
    {
        _menuItemRegistry = ServiceProvider.GetService<MenuItemRegistry>();
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
