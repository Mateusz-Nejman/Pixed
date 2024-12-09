using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Application.DependencyInjection;
using Pixed.Application.Extensions;
using Pixed.Application.Platform;
using Pixed.Application.Windows;
using Pixed.Common.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceProvider = Pixed.Application.DependencyInjection.ServiceProvider;

namespace Pixed.Application;

public partial class App : Avalonia.Application
{
    private Mutex? _mutex;
    internal static IPixedServiceProvider ServiceProvider { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        Platform.PlatformSettings.Lifecycle.ApplicationLifetime = ApplicationLifetime;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            SplashWindow splash = new();
            desktop.MainWindow = splash;
            splash.Show();
            await InitializeMainWindow(desktop, splash);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            InitializeServices();
            singleViewPlatform.MainView = new MainView();
        }
        base.OnFrameworkInitializationCompleted();
    }

    private void InitializeServices()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        IServiceCollection collection = new ServiceCollection();
        var registers = GetDependencyRegisters();

        foreach (var register in registers)
        {
            register.Register(ref collection);
        }

        collection.AddSingleton(Platform.PlatformSettings.Lifecycle);

        if (Platform.PlatformSettings.Lifecycle.ExtensionsEnabled)
        {
            ExtensionsLoader.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pixed", "Extensions"));
            ExtensionsLoader.RegisterTools(ref collection);
        }

        ServiceProvider provider = new(collection.BuildServiceProvider());
        this.Resources[typeof(IPixedServiceProvider)] = provider;
        ServiceProvider = provider;
    }

    private async Task InitializeMainWindow(IClassicDesktopStyleApplicationLifetime desktop, Window splash)
    {
        if (!HandleNewInstance(Dispatcher.UIThread, desktop))
        {
            splash.Close();
            return;
        }
        await Task.Delay(1500);
        InitializeServices();
        var provider = this.Resources[typeof(IPixedServiceProvider)] as IPixedServiceProvider;
        //TODO open from args
        desktop.MainWindow = provider.Get<MainWindow>();
        desktop.MainWindow.Show();
        await Task.Delay(100);
        splash.Close();
    }

    private bool HandleNewInstance(Dispatcher? dispatcher, IClassicDesktopStyleApplicationLifetime desktop)
    {
        _mutex = new(true, "pixed_mutex_name", out bool isOwned);
        var handle = new EventWaitHandle(false, EventResetMode.AutoReset, "pixed_mutex_event_name");

        GC.KeepAlive(_mutex);

        if (dispatcher == null)
            return true;

        if (isOwned)
        {
            var thread = new Thread(
                async () =>
                {
                    while (handle.WaitOne())
                    {
                        //TODO
                        //var pixed = await desktop.MainWindow.StorageProvider.GetPixedFolder();
                        //var filePath = Path.Combine(pixed.Path.AbsolutePath, "instance.lock");

                        //if (File.Exists(filePath))
                        //{
                        //    string[] args = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(filePath)) ?? [];

                        //    if (args.Length > 0)
                        //    {
                        //        Subjects.NewInstanceHandled.OnNext(args);
                        //    }
                        //}
                    }
                })
            {
                IsBackground = true
            };

            thread.Start();
            return true;
        }

        Task.Run(async () =>
        {
            //TODO
            //var pixed = await desktop.MainWindow.StorageProvider.GetPixedFolder();
            //File.WriteAllText(Path.Combine(pixed.Path.AbsolutePath, "instance.lock"), JsonConvert.SerializeObject(Environment.GetCommandLineArgs()));
        });
        handle.Set();

        desktop.Shutdown();
        return false;
    }

    private List<IDependencyRegister> GetDependencyRegisters()
    {
        return [new DependencyInjectionRegister(), new ApplicationDependencyRegister()];
    }
}