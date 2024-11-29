using AvaloniaInside.Shell;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;
internal static class NavigatorUtils
{
    public readonly struct NavigatorResult<T>(bool hasValue, T value)
    {
        public bool HasValue { get; } = hasValue;
        public T Value { get; } = value;
    }
    public static async Task<NavigatorResult<T>> Navigate<T>(this INavigator navigator, string path)
    {
        var navigateResult = await navigator.NavigateAndWaitAsync(path);

        if(navigateResult.Argument is T)
        {
            return new NavigatorResult<T>(true, (T)navigateResult.Argument);
        }

        return new NavigatorResult<T>(false, default);
    }

    public static Task Navigate(this INavigator navigator, string path)
    {
        return navigator.NavigateAsync(path);
    }
}
