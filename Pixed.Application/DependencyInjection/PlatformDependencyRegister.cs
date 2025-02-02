using Microsoft.Extensions.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Common.DependencyInjection;

namespace Pixed.Application.DependencyInjection;
public class PlatformDependencyRegister
{
    public static IDependencyRegister Implementation { get; set; } = new DefaultImpl();
    private class DefaultImpl : IDependencyRegister
    {
        public void Register(ref IServiceCollection collection)
        {
            collection.AddSingleton<IPlatformFolder, DefaultPlatformFolder>();
        }
    }
}