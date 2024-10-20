using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class PixedWindow<T> : Window
{
    protected readonly IMenuItemRegistry _menuItemRegistry;
    public PixedWindow(IMenuItemRegistry menuItemRegistry)
    {
        var serviceProvider = this.GetServiceProvider();
        this.DataContext = serviceProvider.Get<T>();
        _menuItemRegistry = menuItemRegistry;
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
}
