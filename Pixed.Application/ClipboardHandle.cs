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

    public async Task<byte[]?> GetDataAsync()
    {
        if (_clipboard == null)
        {
            return null;
        }
        
        var data = await _clipboard.TryGetDataAsync();

        if (data == null)
        {
            return null;
        }
        return await data.TryGetValueAsync(DataFormat.CreateBytesPlatformFormat("PNG"));
    }

    public async Task SetDataAsync(byte[] data)
    {
        if (_clipboard == null)
        {
            return;
        }
        
        DataTransfer dataTransfer = new();
        DataTransferItem dataTransferItem = new();
        dataTransferItem.Set(DataFormat.CreateBytesPlatformFormat("PNG"), data);
        dataTransfer.Add(dataTransferItem);
        await _clipboard.SetDataAsync(dataTransfer);
    }
}
