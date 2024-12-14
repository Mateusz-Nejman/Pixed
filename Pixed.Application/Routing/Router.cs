using AvaloniaInside.Shell;
using Pixed.Application.IO;
using Pixed.Application.Models;
using System.Threading.Tasks;

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

        public static async Task<NavigatorResult<ButtonResult>> Confirm(string title, string text)
        {
            return await Navigate<ButtonResult>("/confirm", new ConfirmModel(title, text));
        }

        public static async Task<NavigatorResult<string>> Prompt(string title, string text, string defaultValue)
        {
            return await Navigate<string>("/prompt", new PromptModel(title, text, defaultValue));
        }

        public static async Task<NavigatorResult<double>> NumericPrompt(string title, string text, double defaultValue)
        {
            return await Navigate<double>("/numericPrompt", new NumericPromptModel(title, text, defaultValue));
        }

        public static Task Navigate(string path)
        {
            return _navigator.NavigateAsync(path);
        }
    }
}
