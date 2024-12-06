using AvaloniaInside.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pixed.Application.Routing.RouterControl;

namespace Pixed.Application.Routing
{
    public readonly struct NavigatorResult<T>(bool hasValue, T value)
    {
        public bool HasValue { get; } = hasValue;
        public T Value { get; } = value;
    }
    internal static class Router
    {
        private static INavigator _navigator;
        public static void Initialize(INavigator navigator)
        {
            _navigator = navigator;
        }
        public static async Task<NavigatorResult<T>> Navigate<T>(string path, object? arg = null)
        {
            var navigateResult = await _navigator.NavigateAndWaitAsync(path, arg);

            if (navigateResult.Argument is T t)
            {
                return new NavigatorResult<T>(true, t);
            }

            return new NavigatorResult<T>(false, default);
        }

        public static Task Navigate(string path)
        {
            return _navigator.NavigateAsync(path);
        }
    }
}
