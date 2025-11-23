using Avalonia.Input;
using Avalonia.Input.Platform;
using Pixed.Common.Platform;
using System.Threading.Tasks;

namespace Pixed.Application;
internal class ClipboardHandle : IClipboardHandle //TODO implement new clipboard features
{
    private IClipboard? _clipboard;
    public void Initialize(IClipboard? clipboard)
    {
        _clipboard = clipboard;
    }
    public async Task ClearAsync()
    {
        if(_clipboard == null)
        {
            return;
        }

        await _clipboard.ClearAsync();
    }

    public async Task<object?> GetDataAsync(string format)
    {

        return await _clipboard.GetDataAsync(format);
    }

    public async Task<string[]> GetFormatsAsync()
    {

        return await _clipboard.GetFormatsAsync();
    }

    public async Task SetDataObjectAsync(IDataObject data)
    {
        await _clipboard.SetDataObjectAsync(data);
    }
}
