using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.DependencyInjection;
using Pixed.Common.Extensions;
using Pixed.Common.Menu;
using Pixed.Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pixed.Application.Extensions;
internal static class ExtensionsLoader
{
    private static readonly List<ExtensionDefinition> _extensions = [];
    public static void RegisterMenuItems(IMenuItemRegistry registry)
    {
        foreach (var extension in _extensions)
        {
            extension.RegisterMenuItems(registry);
        }
    }

    public static Dictionary<string, BaseTool> GetTools(IPixedServiceProvider serviceProvider)
    {
        Dictionary<string, BaseTool> tools = [];

        foreach (var extension in _extensions)
        {
            var extensionTools = extension.GetTools();

            foreach (var extensionTool in extensionTools)
            {
                tools.Add(extensionTool.Name, serviceProvider.Get(extensionTool.Type) as BaseTool);
            }
        }

        return tools;
    }

    public static void RegisterTools(ref IServiceCollection collection)
    {
        foreach (var extension in _extensions)
        {
            extension.RegisterTools(ref collection);
        }
    }
    public static void Load(string extensionsFolder)
    {
        Directory.CreateDirectory(extensionsFolder);
        var items = Directory.GetFiles(extensionsFolder);

        foreach (var item in items)
        {
            if (item.EndsWith(".dll"))
            {
                var dll = Assembly.LoadFile(item);
                var types = dll.GetExportedTypes();

                foreach (var type in types)
                {
                    if (typeof(ExtensionDefinition).IsAssignableFrom(type))
                    {
                        var instance = Activator.CreateInstance(type);

                        if (instance is ExtensionDefinition definition)
                        {
                            _extensions.Add(definition);
                        }
                    }
                }
            }
        }
    }
}