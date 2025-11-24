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

    public Task<byte[]?> GetDataAsync()
    {
        return Task.FromResult<byte[]?>(_copiedBuffer);
    }

    public void Initialize(IClipboard clipboard)
    {
    }

    public Task SetDataAsync(byte[] data)
    {
        _copiedBuffer = data;

        return Task.CompletedTask;
    }
}