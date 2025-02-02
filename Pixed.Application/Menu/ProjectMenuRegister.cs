using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.BigGustave;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Utils;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;

internal class ProjectMenuRegister(IMenuItemRegistry menuItemRegistry, DialogUtils dialogUtils, ApplicationData applicationData)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly DialogUtils _dialogUtils = dialogUtils;
    private readonly ApplicationData _applicationData = applicationData;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Project, "Load layer from file", new AsyncCommand(LoadLayerFromFile), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_folder_open_28_regular.svg"));
    }

    private async Task LoadLayerFromFile()
    {
        var files = await _dialogUtils.OpenFileDialog("PNG images (*.png)|*.png", "Open file");

        if (files.Count != 1)
        {
            return;
        }

        var file = files[0];
        var result = await Router.Navigate<bool>("/loadLayer");

        if (result.HasValue)
        {
            bool isSingle = result.Value;

            var stream = await file.OpenRead();

            Png png = Png.Open(stream);
            var colors = png.GetPixelsUInt();

            stream.Dispose();

            var currentSize = new Point(_applicationData.CurrentFrame.Width, _applicationData.CurrentFrame.Height);
            SKBitmap? bitmap;

            if (png.Width > currentSize.X || png.Height > currentSize.Y)
            {
                colors = colors.GetRectangleColors(new Point(png.Width, png.Height), new Point(), new Point(Math.Min(png.Width, currentSize.X), Math.Min(png.Height, currentSize.Y)));
                bitmap = SkiaUtils.FromArray(colors, new Point(Math.Min(png.Width, currentSize.X), Math.Min(png.Height, currentSize.Y)));
            }
            else
            {
                bitmap = SkiaUtils.FromArray(colors, new Point(png.Width, png.Height));
            }

            var bitmapSize = new Point(bitmap.Width, bitmap.Height);
            SKBitmap? layerBitmap = new(currentSize.X, currentSize.Y, true);
            SKCanvas canvas = new(layerBitmap);

            if (isSingle)
            {
                canvas.DrawBitmap(bitmap, new SKPoint());
            }
            else
            {
                for (int x = 0; x < layerBitmap.Width; x += bitmapSize.X)
                {
                    for (int y = 0; y < layerBitmap.Height; y += bitmapSize.Y)
                    {
                        canvas.DrawBitmap(bitmap, new SKPoint(x, y));
                    }
                }
            }

            canvas.Dispose();
            var pixels = layerBitmap.ToArray();
            layerBitmap.Dispose();

            var layer = Layer.FromColors(pixels, currentSize.X, currentSize.Y, "");
            _applicationData.CurrentFrame.AddLayer(layer);
            Subjects.LayerAdded.OnNext(layer);
            Subjects.FrameModified.OnNext(_applicationData.CurrentFrame);
            _applicationData.CurrentModel.AddHistory();
        }
    }
}
