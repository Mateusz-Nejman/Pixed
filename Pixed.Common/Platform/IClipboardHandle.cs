using Avalonia.Input;
using Pixed.Common.Selection;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IClipboardHandle
{
    public Task ClearAsync();

    public Task SetDataObjectAsync(IDataObject data);

    public Task<string[]> GetFormatsAsync();

    public Task<object?> GetDataAsync(string format);
    public Task CopySelectionAsync(BaseSelection selection);
}