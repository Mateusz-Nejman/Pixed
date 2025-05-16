using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using System;

namespace Pixed.Application.Controls;
internal abstract class ExtendedControl<T> : UserControl
{
    protected readonly IMenuItemRegistry _menuItemRegistry;

    protected static IPixedServiceProvider Provider => App.ServiceProvider;

    public T ViewModel => (T)DataContext;

    public ExtendedControl()
    {
        _menuItemRegistry = Provider.Get<IMenuItemRegistry>();
        var serviceProvider = this.GetServiceProvider();

        var viewModel = serviceProvider.Get<T>();

        if (viewModel is ExtendedViewModel extended)
        {
            extended.Initialize(this);
        }

        this.DataContext = viewModel;
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

    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        RegisterMenuItems();
        Unloaded += PixedUserControl_Unloaded;
        Loaded += PixedUserControl_Loaded;
    }

    private void PixedUserControl_Loaded(object? sender, RoutedEventArgs e)
    {
        OnLoaded();
        if (DataContext is ExtendedViewModel model)
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

internal abstract class EmptyExtendedControl() : UserControl
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
