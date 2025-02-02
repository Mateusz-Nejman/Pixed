using Microsoft.Extensions.DependencyInjection;
using Pixed.Application.Platform;
using Pixed.Common.DependencyInjection;

namespace Pixed.Android;
internal class AndroidDependencyRegister : IDependencyRegister
{
    public void Register(ref IServiceCollection collection)
    {
        collection.AddSingleton<IPlatformFolder, PlatformFolder>();
    }
}