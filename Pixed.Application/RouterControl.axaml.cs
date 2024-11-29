using Avalonia.Controls;
using AvaloniaInside.Shell;
using System.Threading.Tasks;

namespace Pixed.Application;

public partial class RouterControl : UserControl
{
    public readonly struct NavigatorResult<T>(bool hasValue, T value)
    {
        public bool HasValue { get; } = hasValue;
        public T Value { get; } = value;
    }

    public static INavigator Navigator { get; private set; }
    public RouterControl()
    {
        InitializeComponent();
        Navigator = ShellViewMain.Navigator;
    }

    public static async Task<NavigatorResult<T>> Navigate<T>(string path, object? arg = null)
    {
        var navigateResult = await Navigator.NavigateAndWaitAsync(path, arg);

        if (navigateResult.Argument is T t)
        {
            return new NavigatorResult<T>(true, t);
        }

        return new NavigatorResult<T>(false, default);
    }

    public static Task Navigate(string path)
    {
        return Navigator.NavigateAsync(path);
    }
}