using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Controls;
using Pixed.Models;
using Pixed.ViewModels;
using Pixed.Windows;
using System;
using System.IO;

namespace Pixed;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        IServiceCollection collection = new ServiceCollection();
        DIRegister.RegisterAll(ref collection);
        ServiceProvider provider = collection.BuildServiceProvider();
        this.Resources[typeof(IServiceProvider)] = provider;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            InitializeBaseVariables();
            PixedWindow<MainViewModel> window = provider.GetService<PixedWindow<MainViewModel>>();
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void InitializeBaseVariables()
    {
        Global.DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pixed");
        Global.UserSettings = Settings.Load();
        Global.Models.Add(new PixedModel(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
        Global.CurrentModel.FileName = Global.NamingService.GenerateName();
        Global.CurrentModel.AddHistory(false);
    }
}