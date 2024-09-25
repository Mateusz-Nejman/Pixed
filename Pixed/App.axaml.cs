using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Controls;
using Pixed.DependencyInjection;
using Pixed.ViewModels;

namespace Pixed;

public partial class App : Application
{
    internal static IPixedServiceProvider ServiceProvider { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        IServiceCollection collection = new ServiceCollection();
        DependencyInjectionRegister.Register(ref collection);
        IPixedServiceProvider provider = new DependencyInjection.ServiceProvider(collection.BuildServiceProvider());
        this.Resources[typeof(IPixedServiceProvider)] = provider;
        ServiceProvider = provider;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            PixedWindow<MainViewModel> window = provider.Get<PixedWindow<MainViewModel>>();
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}