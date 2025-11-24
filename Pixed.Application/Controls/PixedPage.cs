using Avalonia.Interactivity;
using AvaloniaInside.Shell;
using Pixed.Application.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class PixedPage<T> : Page
{
    protected readonly IMenuItemRegistry? _menuItemRegistry;

    protected static IPixedServiceProvider? Provider => App.ServiceProvider;

    public T? ViewModel => (T?)DataContext;

    public PixedPage()
    {
        if (Provider != null)
        {
            _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
            var serviceProvider = this.GetServiceProvider();

            if (serviceProvider != null)
            {
                var viewModel = serviceProvider.Get<T>();

                if (viewModel is ExtendedViewModel extended)
                {
                    extended.Initialize(this);
                }

                this.DataContext = viewModel;
                Unloaded += PixedPage_Unloaded;
                Loaded += PixedPage_Loaded;
            }
        }
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

    private void PixedPage_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is ExtendedViewModel model)
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

internal abstract class EmptyPixedPage : Page
{
    private readonly IMenuItemRegistry? _menuItemRegistry;
    protected static IPixedServiceProvider? Provider => App.ServiceProvider;
    public EmptyPixedPage() : base()
    {
        if (Provider == null) return;
        
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        Unloaded += EmptyPixedUserControl_Unloaded;
        Loaded += EmptyPixedUserControl_Loaded;
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

    private void EmptyPixedUserControl_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is ExtendedViewModel model)
        {
            model.OnLoaded();
        }
    }

    private void EmptyPixedUserControl_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IDisposable viewModel) return;
        DataContext = null;
        viewModel.Dispose();
    }
}
