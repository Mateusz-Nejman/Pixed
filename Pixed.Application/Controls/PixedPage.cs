using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInside.Shell;
using Pixed.Application.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class PixedPage<T> : Page
{
    protected readonly IMenuItemRegistry _menuItemRegistry;

    protected static IPixedServiceProvider Provider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public PixedPage()
    {
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        var serviceProvider = this.GetServiceProvider();
        this.DataContext = serviceProvider.Get<T>();
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

    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        RegisterMenuItems();
        Unloaded += PixedPage_Unloaded;
        Loaded += PixedPage_Loaded;
    }

    private void PixedPage_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is PixedViewModel model)
        {
            model.OnLoaded();
        }
    }

    private void PixedPage_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }
}

internal abstract class EmptyPixedPage() : Page
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
