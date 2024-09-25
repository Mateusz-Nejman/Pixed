using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pixed.DependencyInjection
{
    internal interface IPixedServiceProvider
    {
        public T Get<T>();
        public IServiceProvider GetNativeProvider();
    }
    internal class ServiceProvider : IPixedServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Get<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public IServiceProvider GetNativeProvider()
        {
            return _serviceProvider;
        }
    }
}
