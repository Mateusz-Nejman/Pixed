using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.Extensions;
using Pixed.Common.Menu;

namespace Pixed.SimpleExtension;
public class SimpleExtensionDefinition : ExtensionDefinition
{
    public override void RegisterMenuItems(IMenuItemRegistry registry)
    {
        registry.Register(BaseMenuItem.Help, "Test", () => throw new Exception("It works"));
    }

    public override void RegisterTools(ref IServiceCollection collection)
    {
        RegisterTool<SimpleTool>(ref collection, "pixed_simple_tool");
    }
}