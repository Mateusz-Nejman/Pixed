using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Controls;
using Pixed.IO;
using Pixed.Models;
using Pixed.Selection;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Tools;
using Pixed.Tools.Selection;
using Pixed.Tools.Transform;
using Pixed.ViewModels;
using Pixed.Windows;
using System;

namespace Pixed;
internal static class DIRegister
{
    public static void RegisterAll(ref IServiceCollection collection)
    {
        collection.AddSingleton<ApplicationData>();
        collection.AddSingleton<SelectionManager>();
        collection.AddSingleton<ToolSelector>();
        collection.AddSingleton<MenuBuilder>();
        collection.AddSingleton<RecentFilesService>();
        collection.AddSingleton<ShortcutService>();
        collection.AddSingleton<PaletteService>();

        collection.AddSingleton<FramesSectionViewModel>();
        collection.AddSingleton<LayersSectionViewModel>();
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<PaintCanvasViewModel>();
        collection.AddSingleton<PaletteSectionViewModel>();
        collection.AddSingleton<ProjectsSectionViewModel>();
        collection.AddSingleton<ToolsSectionViewModel>();

        collection.AddSingleton<PixedWindow<MainViewModel>, MainWindow>();

        collection.AddScoped<ToolBucket>();
        collection.AddScoped<ToolCircle>();
        collection.AddScoped<ToolColorPicker>();
        collection.AddScoped<ToolColorSwap>();
        collection.AddScoped<ToolDithering>();
        collection.AddScoped<ToolEraser>();
        collection.AddScoped<ToolLighten>();
        collection.AddScoped<ToolMove>();
        collection.AddScoped<ToolNoise>();
        collection.AddScoped<ToolNoiseFill>();
        collection.AddScoped<ToolOutliner>();
        collection.AddScoped<ToolPen>();
        collection.AddScoped<ToolRectangle>();
        collection.AddScoped<ToolStroke>();
        collection.AddScoped<ToolVerticalPen>();

        collection.AddScoped<LassoSelect>();
        collection.AddScoped<RectangleSelect>();
        collection.AddScoped<ShapeSelect>();

        collection.AddScoped<PixedProjectMethods>();
        collection.AddSingleton<TransformToolsMenuRegister>();
    }

    public static IServiceProvider GetServiceProvider(this IResourceHost control)
    {
        var resource = control.FindResource(typeof(IServiceProvider));

        if (resource is UnsetValueType unset)
        {
            return App.ServiceProvider;
        }

        return (IServiceProvider)resource;
    }

    public static T CreateInstance<T>(this IResourceHost control)
    {
        return ActivatorUtilities.CreateInstance<T>(control.GetServiceProvider());
    }
}
