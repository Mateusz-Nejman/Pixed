using Microsoft.Extensions.DependencyInjection;

namespace Pixed.Common.DependencyInjection;
public interface IDependencyRegister
{
    public void Register(ref IServiceCollection collection);
}
