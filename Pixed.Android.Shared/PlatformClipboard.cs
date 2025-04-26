using Avalonia.Input;
using Avalonia.Input.Platform;
using Pixed.Common.Platform;

namespace Pixed.Android;
internal class PlatformClipboard : IClipboardHandle
{
    private byte[]? _copiedBuffer;
    public Task ClearAsync()
    {
        _copiedBuffer = null;
        return Task.CompletedTask;
    }

    public Task<object?> GetDataAsync(string format)
    {
        if(format != "PNG" || _copiedBuffer == null)
        {
            return Task.FromResult<object?>(null);
        }

        return Task.FromResult<object?>(_copiedBuffer);
    }

    public Task<string[]> GetFormatsAsync()
    {
        return Task.FromResult<string[]>(_copiedBuffer == null ? [] : ["PNG"]);
    }

    public void Initialize(IClipboard clipboard)
    {
    }

    public Task SetDataObjectAsync(IDataObject data)
    {
        var png = data.Get("PNG");

        if(png != null && png is byte[] array)
        {
            _copiedBuffer = array;
        }

        return Task.CompletedTask;
    }
}