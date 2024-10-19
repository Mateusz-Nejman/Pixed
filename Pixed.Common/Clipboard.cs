using Avalonia.Input;
using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace Pixed.Common;

public static class Clipboard
{
    public static IClipboard ClipboardHandle { get; set; }
    public static Task<string?> GetTextAsync()
    {
        return ClipboardHandle.GetTextAsync();
    }

    public static Task SetTextAsync(string? text)
    {
        return ClipboardHandle.SetTextAsync(text);
    }

    public static Task ClearAsync()
    {
        return ClipboardHandle.ClearAsync();
    }

    public static Task SetDataObjectAsync(IDataObject data)
    {
        return ClipboardHandle.SetDataObjectAsync(data);
    }

    public static Task<string[]> GetFormatsAsync()
    {
        return ClipboardHandle.GetFormatsAsync();
    }

    public static Task<object?> GetDataAsync(string format)
    {
        return ClipboardHandle.GetDataAsync(format);
    }
}
