using Microsoft.Extensions.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Pages;
using Pixed.Application.Platform;
using Pixed.Application.Services;
using Pixed.Application.Utils;
using Pixed.Application.ViewModels;
using Pixed.Application.Windows;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Menu;
using Pixed.Common.Platform;

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
        collection.AddSingleton<PropertiesSectionViewModel>();
        collection.AddSingleton<ToolsSectionViewModel>();

        collection.AddSingleton<MainWindow>();
        collection.AddSingleton<Main>();
        collection.AddSingleton<RecentFilesService>();
        collection.AddScoped<PixedProjectMethods>();
        collection.AddSingleton<MenuBuilder>();
        collection.AddSingleton<IMenuItemRegistry, MenuItemRegistry>();

        collection.AddSingleton<TransformMenuRegister>();
        collection.AddSingleton<UndoRedoMenuRegister>();
        collection.AddSingleton<CopyPasteMenuRegister>();
        collection.AddSingleton<PaletteMenuRegister>();
        collection.AddSingleton<ProjectMenuRegister>();
        collection.AddSingleton<ViewMenuRegister>();
        collection.AddSingleton<ToolsMenuRegister>();
        collection.AddSingleton<IClipboardHandle, ClipboardHandle>();
        collection.AddSingleton<IStorageProviderHandle, StorageProviderHandle>();
        collection.AddSingleton<DialogUtils>();
    }
}