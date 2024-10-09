using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Pixed.DependencyInjection;
using Pixed.Windows;
using System.Threading.Tasks;

namespace Pixed;

public partial class App : Application
{
    internal static IPixedServiceProvider ServiceProvider { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            SplashWindow splash = new();
            desktop.MainWindow = splash;
            splash.Show();
            await InitializeMainWindow(desktop, splash);
        }
        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeMainWindow(IClassicDesktopStyleApplicationLifetime desktop, Window splash)
    {
        await Task.Delay(1500);
        BindingPlugins.DataValidators.RemoveAt(0);

        IServiceCollection collection = new ServiceCollection();
        DependencyInjectionRegister.Register(ref collection);
        DependencyInjection.ServiceProvider provider = new(collection.BuildServiceProvider());
        this.Resources[typeof(IPixedServiceProvider)] = provider;
        ServiceProvider = provider;
        MainWindow window = provider.Get<MainWindow>();
        window.OpenFromArgs(desktop.Args);
        window.Show();
        desktop.MainWindow = window;
        await Task.Delay(100);
        splash.Close();
    }
}