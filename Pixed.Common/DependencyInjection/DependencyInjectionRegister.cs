﻿using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Services.Palette;
using Pixed.Common.Tools;
using Pixed.Common.Tools.Selection;

namespace Pixed.Common.DependencyInjection;
public class DependencyInjectionRegister : IDependencyRegister
{
    public void Register(ref IServiceCollection collection)
    {
        collection.AddSingleton<ApplicationData>();
        collection.AddSingleton<SelectionManager>();
        collection.AddSingleton<ToolSelector>();
        collection.AddSingleton<ShortcutService>();
        collection.AddSingleton<PaletteService>();

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
    }
}