using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Pixed.Controls;
using Pixed.IO;
using Pixed.Menu;
using Pixed.Models;
using Pixed.Selection;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Tools;
using Pixed.Tools.Selection;
using Pixed.ViewModels;
using Pixed.Windows;

namespace Pixed.DependencyInjection;
internal static class DependencyInjectionRegister
{
    public static void Register(ref IServiceCollection collection)
    {
        collection.AddSingleton<ApplicationData>();
        collection.AddSingleton<SelectionManager>();
        collection.AddSingleton<ToolSelector>();
        collection.AddSingleton<MenuBuilder>();
        collection.AddSingleton<MenuItemRegistry>();
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
        collection.AddSingleton<ToolMoveCanvas>();
        collection.AddScoped<ToolNoise>();
        collection.AddScoped<ToolNoiseFill>();
        collection.AddScoped<ToolOutliner>();
        collection.AddScoped<ToolPen>();
        collection.AddScoped<ToolRectangle>();
        collection.AddScoped<ToolStroke>();
        collection.AddScoped<ToolVerticalPen>();
        collection.AddSingleton<ToolZoom>();

        collection.AddScoped<LassoSelect>();
        collection.AddScoped<RectangleSelect>();
        collection.AddScoped<ShapeSelect>();

        collection.AddScoped<PixedProjectMethods>();
        collection.AddSingleton<TransformMenuRegister>();
    }

    public static IPixedServiceProvider GetServiceProvider(this IResourceHost control)
    {
        var resource = control.FindResource(typeof(IPixedServiceProvider));

        if (resource is UnsetValueType unset)
        {
            return App.ServiceProvider;
        }

        return (IPixedServiceProvider)resource;
    }

    public static T CreateInstance<T>(this IResourceHost control)
    {
        return ActivatorUtilities.CreateInstance<T>(control.GetServiceProvider().GetNativeProvider());
    }
}
