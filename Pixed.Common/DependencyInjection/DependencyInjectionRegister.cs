using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.Services;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Services.Palette;
using Pixed.Common.Tools;
using Pixed.Common.Tools.Selection;
using Pixed.Core.Models;

namespace Pixed.Common.DependencyInjection;
public class DependencyInjectionRegister : IDependencyRegister
{
    public void Register(ref IServiceCollection collection)
    {
        collection.AddSingleton<ApplicationData>();
        collection.AddSingleton<SelectionManager>();
        collection.AddSingleton<ToolsManager>();
        collection.AddSingleton<ShortcutService>();
        collection.AddSingleton<PaletteService>();
        collection.AddSingleton<ClipboardService>();

        collection.AddScoped<ToolBucket>();
        collection.AddScoped<ToolCircle>();
        collection.AddScoped<ToolColorPicker>();
        collection.AddScoped<ToolDithering>();
        collection.AddScoped<ToolEraser>();
        collection.AddScoped<ToolLighten>();
        collection.AddScoped<ToolMove>();
        collection.AddSingleton<ToolMoveCanvas>();
        collection.AddScoped<ToolOutliner>();
        collection.AddScoped<ToolPen>();
        collection.AddScoped<ToolRectangle>();
        collection.AddScoped<ToolStroke>();

        collection.AddScoped<ToolSelectLasso>();
        collection.AddScoped<ToolSelectRectangle>();
        collection.AddScoped<ToolSelectShape>();
    }
}
