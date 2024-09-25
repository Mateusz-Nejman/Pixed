using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Controls;
using Pixed.ViewModels;
using Pixed.Windows;
using System;

namespace Pixed;
internal static class DIRegister
{
    public static void RegisterAll(ref IServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<PixedWindow<MainViewModel>, MainWindow>();
    }

    public static IServiceProvider GetServiceProvider(this IResourceHost control)
    {
        return (IServiceProvider)control.FindResource(typeof(IServiceProvider));
    }

    public static T CreateInstance<T>(this IResourceHost control)
    {
        return ActivatorUtilities.CreateInstance<T>(control.GetServiceProvider());
    }
}
