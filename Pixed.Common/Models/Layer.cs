using Pixed.Common.IO;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Pixed.Common.Models;

public class Layer : PropertyChangedBase, IPixedSerializer
{
    private uint[] _pixels;
    private int _width;
    private int _height;
    private SKBitmap _renderedBitmap = null;
    private bool _needRerender = true;
    private double _opacity = 100.0d;
    private string _name = string.Empty;
    private PixedImage _renderSource = new(null);
    private readonly string _id;

    public double Opacity
    {
        get => _opacity;
        set
        {
            _opacity = value;
            _needRerender = true;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public PixedImage RenderSource
    {
        get => _renderSource;
        set
        {
            _renderSource = value;
            OnPropertyChanged();
        }
    }

    public string Id => _id;
    public int Width => _width;
    public int Height => _height;
    public ICommand? ChangeOpacityCommand { get; }

    public static Func<Layer, Task> ChangeOpacityAction { get; set; }
    public Layer(int width, int height)
    {
        ChangeOpacityCommand = new AsyncCommand(async () =>
        {
            await ChangeOpacityAction(this);
        });
        _id = Guid.NewGuid().ToString();
        _width = width;
        _height = height;
        _pixels = new uint[width * height];

        Array.Fill(_pixels, UniColor.Transparent.ToUInt());
    }

    private Layer(int width, int height, uint[] pixels)
    {
        ChangeOpacityCommand = new AsyncCommand(async () =>
        {
            await ChangeOpacityAction(this);
        });
        _id = Guid.NewGuid().ToString();
        _width = width;
        _height = height;
        _pixels = pixels;
    }

    public Layer Clone()
    {
        uint[] pixels = new uint[_pixels.Length];
        _pixels.CopyTo(pixels, 0);
        Layer layer = new(_width, _height, pixels)
        {
            Name = Name,
            _needRerender = true
        };
        layer.RenderSource.UpdateBitmap(_renderedBitmap);
        return layer;
    }

    public uint[] GetPixels()
    {
        return _pixels;
    }

    public uint[] GetDistinctPixels()
    {
        return _pixels.Distinct().ToArray();
    }

    public void SetPixel(Point point, uint color)
    {
        SetPixelPrivate(point, color);
    }

    public void SetPixels(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            SetPixelPrivate(pixel.Position, pixel.Color);
        }
    }

    public void MergeLayers(Layer layer2)
    {
        Render(out SKBitmap bitmap);
        layer2.Render(out SKBitmap layer2bitmap);
        SKCanvas canvas = new(bitmap);
        canvas.DrawBitmap(layer2bitmap, new SKPoint(0, 0));
        canvas.Dispose();

        _pixels = bitmap.ToArray();
        _needRerender = true;
    }

    public void Rerender(List<Pixel>? pixels = null)
    {
        _needRerender = true;
        RefreshRenderSource(pixels);
    }

    public void RefreshRenderSource(List<Pixel>? pixels = null)
    {
        if (Render(out SKBitmap bitmap, pixels))
        {
            RenderSource.UpdateBitmap(bitmap);
        }
    }

    public uint GetPixel(Point point)
    {
        if (!ContainsPixel(point))
        {
            return 0;
        }

        return _pixels[point.Y * _width + point.X];
    }

    public bool Render(out SKBitmap render, List<Pixel>? modifiedPixels = null)
    {
        render = _renderedBitmap;
        if (!_needRerender && (modifiedPixels == null || modifiedPixels.Count == 0))
        {
            return false;
        }

        List<uint> pixels = new(_pixels);

        if (modifiedPixels != null)
        {
            foreach (var pixel in modifiedPixels)
            {
                pixels[pixel.Position.Y * _width + pixel.Position.X] = pixel.Color;
            }
        }

        if (Opacity != 100)
        {
            for (int a = 0; a < pixels.Count; a++)
            {
                UniColor color = pixels[a];
                color.A = (byte)(Opacity / 100d * color.A);
                pixels[a] = color;
            }
        }

        var bitmap = SkiaUtils.FromArray(pixels, new Point(_width, _height));
        pixels.Clear();
        _renderedBitmap = bitmap;
        _needRerender = false;
        render = bitmap;
        return true;
    }

    public bool ContainsPixel(Point point)
    {
        return point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height;
    }

    public void Serialize(Stream stream)
    {
        stream.WriteDouble(Opacity);
        stream.WriteInt(Width);
        stream.WriteInt(Height);
        stream.WriteString(Name);
        stream.WriteInt(_pixels.Length);
        stream.WriteUIntArray(_pixels);
    }

    public void Deserialize(Stream stream)
    {
        Opacity = stream.ReadDouble();
        _width = stream.ReadInt();
        _height = stream.ReadInt();
        _name = stream.ReadString();
        int pixelsSize = stream.ReadInt();
        _pixels = new uint[pixelsSize];

        for (int i = 0; i < pixelsSize; i++)
        {
            _pixels[i] = stream.ReadUInt();
        }
    }

    public static Layer FromColors(uint[] colors, int width, int height, string name)
    {
        Layer layer = new(width, height)
        {
            _pixels = colors,
            _name = name
        };
        return layer;
    }

    private void SetPixelPrivate(Point point, uint color)
    {
        if (!ContainsPixel(point))
        {
            return;
        }

        _pixels[point.Y * _width + point.X] = color;
        _needRerender = true;
    }
}
