using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class ExtendedWindow<T> : Window
{
    private readonly IMenuItemRegistry? _menuItemRegistry;

    protected static IPixedServiceProvider? Provider => App.ServiceProvider;

    public T? ViewModel => (T?)DataContext;

    public ExtendedWindow()
    {
        if (Provider != null)
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
