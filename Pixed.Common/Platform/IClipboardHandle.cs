using Avalonia.Input;
using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IClipboardHandle
{
    public void Initialize(IClipboard? clipboard);
    public Task ClearAsync();

    public Task SetDataObjectAsync(IDataObject data);

    public Task<string[]> GetFormatsAsync();

    public Task<object?> GetDataAsync(string format);
}