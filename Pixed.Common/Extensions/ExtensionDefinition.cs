using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.Menu;
using System.Collections.Generic;

namespace Pixed.Common.Extensions;
public abstract class ExtensionDefinition
{
    private readonly List<ExtensionToolEntry> _toolEntries = [];
    public virtual void RegisterMenuItems(IMenuItemRegistry registry)
    {

    }

    public List<ExtensionToolEntry> GetTools()
    {
        return _toolEntries;
    }

    public virtual void RegisterTools(ref IServiceCollection collection)
    {

    }

    protected void RegisterTool<T>(ref IServiceCollection collection, string name)
    {
        collection.AddSingleton(typeof(T));
        _toolEntries.Add(new ExtensionToolEntry(name, typeof(T)));
    }
}
