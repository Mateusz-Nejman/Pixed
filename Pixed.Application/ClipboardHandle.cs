using Avalonia.Input;
using Pixed.Application.Windows;
using Pixed.BigGustave;
using Pixed.Common;
using Pixed.Common.Platform;
using Pixed.Common.Selection;
using SkiaSharp;
using System.IO;
using System.Linq;
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

    public async Task CopySelectionAsync(BaseSelection selection)
    {
        var minX = selection.Pixels.Min(p => p.Position.X);
        var minY = selection.Pixels.Min(p => p.Position.Y);
        var maxX = selection.Pixels.Max(p => p.Position.X);
        var maxY = selection.Pixels.Max(p => p.Position.Y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        var builder = PngBuilder.Create(width, height, true);

        foreach (var pixel in selection.Pixels)
        {
            builder.SetPixel(new UniColor(pixel.Color), pixel.Position.X - minX, pixel.Position.Y - minY);
        }

        MemoryStream memoryStream = new();
        builder.Save(memoryStream);
        DataObject clipboardObject = new();
        clipboardObject.Set("PNG", memoryStream.ToArray());
        await ClearAsync();
        await SetDataObjectAsync(clipboardObject);
        memoryStream.Dispose();
    }
}
