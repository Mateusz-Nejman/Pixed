using System;

namespace Pixed.Common.DependencyInjection;
public interface IPixedServiceProvider
{
    public T? Get<T>();
    public object? Get(Type type);
    public IServiceProvider GetNativeProvider();
}