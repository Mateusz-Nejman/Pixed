﻿using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Core.Models;
public class PixedImage : IImage, IDisposable
{
    private record class DrawOperation : ICustomDrawOperation
    {
        private readonly PixedImage? _image;
        public Rect Bounds { get; set; }

        public DrawOperation(PixedImage? image)
        {
            _image = image;
        }

        public void Dispose()
        {
            //nop
        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Avalonia.Point p) => Bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            if (_image?._source is SKBitmap bitmap && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    lock (bitmap)
                    {
                        if (bitmap != null)
                        {
                            try
                            {
                                lease.SkCanvas.DrawBitmap(bitmap, SKRect.Create((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                //Ignore exception
                            }
                        }
                    }
                }
            }
        }
    }

    private SKBitmap? _source;
    private readonly bool _canDispose = false;
    private Size _size;

    public Size Size => _size;

    public SKBitmap? Source => _source;

    public PixedImage(SKBitmap? source)
    {
        UpdateBitmap(source);
    }

    public void UpdateBitmap(SKBitmap? source)
    {
        _source = source;
        if (source?.Info.Size is SKSizeI size)
        {
            _size = new(size.Width, size.Height);
        }
    }

    public PixedImage Clone()
    {
        return new PixedImage(_source?.Copy());
    }

    public PixedImage(uint[] colors, Point size) : this(SkiaUtils.FromArray(colors, size))
    {
        _canDispose = true;
    }

    public void Dispose()
    {
        if (_canDispose)
        {
            _source?.Dispose();
        }
    }

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        context.Custom(new DrawOperation(this) { Bounds = destRect });
    }
}