using Pixed.Core.IO;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Windows.Input;
namespace Pixed.Core.Models;

public class Layer : PixelImage, IPixedSerializer
{
    private uint[] _pixels;
    private int _width;
    private int _height;
    private double _opacity = 100.0d;
    private string _name = string.Empty;
    private readonly string _id;
    private static readonly object _lock = new();

    public double Opacity
    {
        get => _opacity;
        set
        {
            _opacity = value;
            UUID = GenerateUUID();
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
        UUID = GenerateUUID();
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
        UUID = GenerateUUID();
    }

    public Layer Clone()
    {
        uint[] pixels = new uint[_pixels.Length];
        _pixels.CopyTo(pixels, 0);
        Layer layer = new(_width, _height, pixels)
        {
            Name = Name,
        };
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
        UUID = GenerateUUID();
    }

    public void SetPixels(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            SetPixelPrivate(pixel.Position, pixel.Color);
        }
        UUID = GenerateUUID();
    }

    public void MergeLayers(Layer layer2)
    {
        var bitmap = Render();
        var layer2Bitmap = layer2.Render();
        SKCanvas canvas = new(bitmap);
        canvas.DrawBitmap(layer2Bitmap, new SKPoint(0, 0));
        canvas.Dispose();

        _pixels = bitmap.ToArray();
        UUID = GenerateUUID();
    }

    public uint GetPixel(Point point)
    {
        if (!ContainsPixel(point))
        {
            return 0;
        }

        return _pixels[point.Y * _width + point.X];
    }

    public override SKBitmap Render()
    {
        SKBitmap bitmap;
        lock(_lock)
        {
            List<uint> pixels = new(_pixels);

            if (ModifiedPixels.Count > 0)
            {
                foreach (var pixel in ModifiedPixels)
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

            bitmap = SkiaUtils.FromArray(pixels, new Point(_width, _height));
            pixels.Clear();
        }
        return bitmap;
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

    public override bool NeedRender(string uuid)
    {
        if(string.IsNullOrEmpty(uuid) || string.IsNullOrEmpty(UUID))
        {
            return true;
        }

        return !UUID.Equals(uuid);
    }

    public override string GenerateUUID()
    {
        return Guid.NewGuid().ToString();
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
    }
}
