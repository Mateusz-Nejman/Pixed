using Pixed.Core.Models;
using SkiaSharp;
using System;

namespace Pixed.Application.Models;
internal class RenderModel : PixelImage
{
    private Frame? _frame;
    private SKBitmap? _overlay;
    private string _overlayUUID = string.Empty;

    public Frame? Frame
    {
        get => _frame;
        set
        {
            bool firstAssign = _frame == null;
            _frame = value;

            if (firstAssign)
            {
                UUID = GenerateUUID();
            }
        }
    }
    public SKBitmap? Overlay
    {
        get => _overlay;
        set
        {
            _overlay = value;
            _overlayUUID = Guid.NewGuid().ToString();
        }
    }

    public override string GenerateUUID()
    {
        return _frame.UUID + ";" + _overlayUUID;
    }

    public override bool NeedRender(string uuid)
    {
        if (_frame == null)
        {
            return true;
        }

        string[] uids = UUID.Split(';');

        if (_frame.NeedRender(uids[0]))
        {
            return true;
        }

        if (_overlayUUID != uids[1])
        {
            return true;
        }

        return false;
    }

    public override SKBitmap Render()
    {
        SKBitmap image = new(_frame.Width, _frame.Height, true);
        SKCanvas canvas = new(image);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(_frame.Render(), SKPoint.Empty);
        canvas.DrawBitmap(_overlay, SKPoint.Empty);
        canvas.Dispose();
        _overlayUUID = Guid.NewGuid().ToString();
        UUID = GenerateUUID();
        return image;
    }
}
