using Microsoft.Extensions.DependencyInjection;
using Pixed.Common.DependencyInjection;
using System;

namespace Pixed.Application.DependencyInjection;
public class ServiceProvider(IServiceProvider serviceProvider) : IPixedServiceProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public T? Get<T>()
    {
        return _serviceProvider.GetService<T?>();
    }

    public object? Get(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }

    public IServiceProvider GetNativeProvider()
    {
        return _serviceProvider;
    }
}
