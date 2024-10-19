using Microsoft.Extensions.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Services;
using Pixed.Application.ViewModels;
using Pixed.Application.Windows;
using Pixed.Common.DependencyInjection;

namespace Pixed.Application.DependencyInjection;
internal class ApplicationDependencyRegister : IDependencyRegister
{
    public void Register(ref IServiceCollection collection)
    {
        collection.AddSingleton<FramesSectionViewModel>();
        collection.AddSingleton<LayersSectionViewModel>();
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<PaintCanvasViewModel>();
        collection.AddSingleton<PaletteSectionViewModel>();
        collection.AddSingleton<ProjectsSectionViewModel>();
        collection.AddSingleton<ToolsSectionViewModel>();

        collection.AddSingleton<MainWindow>();
        collection.AddSingleton<RecentFilesService>();
        collection.AddScoped<PixedProjectMethods>();
        collection.AddSingleton<MenuBuilder>();
        collection.AddSingleton<MenuItemRegistry>();

        collection.AddSingleton<TransformMenuRegister>();
        collection.AddSingleton<CopyPasteMenuRegister>();
        collection.AddSingleton<ViewMenuRegister>();
    }
}