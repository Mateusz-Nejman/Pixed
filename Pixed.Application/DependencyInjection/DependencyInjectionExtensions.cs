using Avalonia;
using Avalonia.Controls;
using Pixed.Common.DependencyInjection;

namespace Pixed.Application.DependencyInjection;
internal static class DependencyInjectionExtensions
{
    public static IPixedServiceProvider? GetServiceProvider(this IResourceHost control)
    {
        var resource = control.FindResource(typeof(IPixedServiceProvider));

        if (resource is UnsetValueType unset)
        {
            return App.ServiceProvider;
        }

        return (IPixedServiceProvider?)resource;
    }
}
