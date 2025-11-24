using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IClipboardHandle
{
    public void Initialize(IClipboard? clipboard);
    public Task ClearAsync();

    public Task SetDataAsync(byte[] data);

    public Task<byte[]?> GetDataAsync();
}