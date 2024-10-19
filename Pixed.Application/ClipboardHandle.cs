using Avalonia.Input;
using Pixed.Application.Windows;
using Pixed.Common.Platform;
using System.Threading.Tasks;

namespace Pixed.Application;
internal class ClipboardHandle : IClipboardHandle
{
    public async Task ClearAsync()
    {
        await MainWindow.Handle.Clipboard.ClearAsync();
    }

    public async Task<object?> GetDataAsync(string format)
    {
        return await MainWindow.Handle.Clipboard.GetDataAsync(format);
    }

    public async Task<string[]> GetFormatsAsync()
    {
        return await MainWindow.Handle.Clipboard.GetFormatsAsync();
    }

    public async Task SetDataObjectAsync(IDataObject data)
    {
        await MainWindow.Handle.Clipboard.SetDataObjectAsync(data);
    }
}
