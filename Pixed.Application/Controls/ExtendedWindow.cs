using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class ExtendedWindow<T> : Window
{
    protected readonly IMenuItemRegistry _menuItemRegistry;

    protected static IPixedServiceProvider Provider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public ExtendedWindow()
    {
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        var serviceProvider = this.GetServiceProvider();
        var viewModel = serviceProvider.Get<T>();

        if (viewModel is ExtendedViewModel extended)
        {
            extended.Initialize(this);
        }

        this.DataContext = viewModel;
        Unloaded += PixedWindow_Unloaded;
        Loaded += PixedWindow_Loaded;
    }

    public TResult Get<TResult>()
    {
        return Provider.Get<TResult>();
    }

    public virtual void RegisterMenuItems()
    {
        if (DataContext is ExtendedViewModel model)
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
        if (DataContext is ExtendedViewModel model)
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

internal abstract class EmptyExtendedWindow : Window
{
    protected readonly IMenuItemRegistry _menuItemRegistry;
    protected static IPixedServiceProvider Provider => App.ServiceProvider;
    public EmptyExtendedWindow() : base()
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
        if (DataContext is ExtendedViewModel model)
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
        if (DataContext is ExtendedViewModel model)
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
