using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pixed.DependencyInjection;
internal interface IPixedServiceProvider
{
    public T Get<T>();
    public IServiceProvider GetNativeProvider();
}
internal class ServiceProvider(IServiceProvider serviceProvider) : IPixedServiceProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public T Get<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    public IServiceProvider GetNativeProvider()
    {
        return _serviceProvider;
    }
}
