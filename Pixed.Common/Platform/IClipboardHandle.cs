using Avalonia.Input;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IClipboardHandle
{
    public Task ClearAsync();

    public Task SetDataObjectAsync(IDataObject data);

    public Task<string[]> GetFormatsAsync();

    public Task<object?> GetDataAsync(string format);
}