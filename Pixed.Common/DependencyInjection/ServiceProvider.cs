using System;

namespace Pixed.Common.DependencyInjection;
public interface IPixedServiceProvider
{
    public T Get<T>();
    public IServiceProvider GetNativeProvider();
}