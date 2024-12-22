using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInside.Shell;
using Pixed.Application.DependencyInjection;
using Pixed.Application.Windows;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class PixedWindow<T> : Window
{
    protected readonly IMenuItemRegistry _menuItemRegistry;

    protected static IPixedServiceProvider Provider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public PixedWindow()
    {
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        var serviceProvider = this.GetServiceProvider();
        this.DataContext = serviceProvider.Get<T>();
        Unloaded += PixedWindow_Unloaded;
        Loaded += PixedWindow_Loaded;
    }

    public TResult Get<TResult>()
    {
        return Provider.Get<TResult>();
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
        RegisterMenuItems();
    }

    private void PixedWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is PixedViewModel model)
        {
            model.OnLoaded();
        }
    }

    private void PixedWindow_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }
}

internal abstract class EmptyPixedWindow : Window
{
    protected readonly IMenuItemRegistry _menuItemRegistry;
    protected static IPixedServiceProvider Provider => App.ServiceProvider;
    public EmptyPixedWindow() : base()
    {
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        Unloaded += PixedWindow_Unloaded;
        Loaded += PixedWindow_Loaded;
    }

    public TResult Get<TResult>()
    {
        return Provider.Get<TResult>();
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
        RegisterMenuItems();
    }

    private void PixedWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is PixedViewModel model)
        {
            model.OnLoaded();
        }
    }

    private void PixedWindow_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }
}
