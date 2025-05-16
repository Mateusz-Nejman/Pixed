using Avalonia.Input;
using Pixed.BigGustave;
using Pixed.Common.Platform;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Common.Services;
public class ClipboardService(IClipboardHandle clipboardHandle)
{
    private readonly IClipboardHandle _clipboardHandle = clipboardHandle;

    public async Task Copy(BaseSelection? selection)
    {
        if (selection == null)
        {
            return;
        }

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
        await _clipboardHandle.ClearAsync();
        await _clipboardHandle.SetDataObjectAsync(clipboardObject);
        memoryStream.Dispose();
    }

    public async Task<SKBitmap?> GetBitmap()
    {
        Png? png = await GetPng();

        if (png != null)
        {
            return SkiaUtils.FromArray(png.GetPixelsUInt(), new Point(png.Width, png.Height));
        }

        return null;
    }

    public async Task<Png?> GetPng()
    {
        var formats = await _clipboardHandle.GetFormatsAsync();

        if (formats.Contains("PNG"))
        {
            var data = await _clipboardHandle.GetDataAsync("PNG");

            if (data is byte[] array)
            {
                return Png.Open(array);
            }
        }

        return null;
    }
}