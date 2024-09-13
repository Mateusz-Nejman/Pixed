using Avalonia.Input;
using System.Threading.Tasks;

namespace Pixed
{
    internal static class Clipboard
    {
        public static Task<string?> GetTextAsync()
        {
            return MainWindow.Handle.Clipboard.GetTextAsync();
        }

        public static Task SetTextAsync(string? text)
        {
            return MainWindow.Handle.Clipboard.SetTextAsync(text);
        }

        public static Task ClearAsync()
        {
            return MainWindow.Handle.Clipboard.ClearAsync();
        }

        public static Task SetDataObjectAsync(IDataObject data)
        {
            return MainWindow.Handle.Clipboard.SetDataObjectAsync(data);
        }

        public static Task<string[]> GetFormatsAsync()
        {
            return MainWindow.Handle.Clipboard.GetFormatsAsync();
        }

        public static Task<object?> GetDataAsync(string format)
        {
            return MainWindow.Handle.Clipboard.GetDataAsync(format);
        }
    }
}
