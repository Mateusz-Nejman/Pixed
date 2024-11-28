using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pixed.Application.DependencyInjection;
using Pixed.Application.Extensions;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceProvider = Pixed.Application.DependencyInjection.ServiceProvider;

namespace Pixed.Application;

public partial class App : Avalonia.Application
{
    private Mutex _mutex;
    internal static IPixedServiceProvider ServiceProvider { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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
        if (!HandleNewInstance(Dispatcher.UIThread, desktop))
        {
            splash.Close();
            return;
        }
        await Task.Delay(1500);
        BindingPlugins.DataValidators.RemoveAt(0);

        IServiceCollection collection = new ServiceCollection();
        var registers = GetDependencyRegisters();

        foreach (var register in registers)
        {
            register.Register(ref collection);
        }

        ExtensionsLoader.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pixed", "Extensions"));
        ExtensionsLoader.RegisterTools(ref collection);
        ServiceProvider provider = new(collection.BuildServiceProvider());
        this.Resources[typeof(IPixedServiceProvider)] = provider;
        ServiceProvider = provider;
        MainWindow window = provider.Get<MainWindow>();
        MainView mainView = provider.Get<MainView>();
        mainView.OpenFromArgs(desktop.Args);
        window.Content = mainView;
        desktop.MainWindow = window;
        window.Show();
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
                        var pixed = await desktop.MainWindow.StorageProvider.GetPixedFolder();
                        var filePath = Path.Combine(pixed.Path.AbsolutePath, "instance.lock");

                        if (File.Exists(filePath))
                        {
                            string[] args = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(filePath)) ?? [];

                            if (args.Length > 0)
                            {
                                Subjects.NewInstanceHandled.OnNext(args);
                            }
                        }
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
            var pixed = await desktop.MainWindow.StorageProvider.GetPixedFolder();
            File.WriteAllText(Path.Combine(pixed.Path.AbsolutePath, "instance.lock"), JsonConvert.SerializeObject(Environment.GetCommandLineArgs()));
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