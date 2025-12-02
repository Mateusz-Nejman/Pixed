using Pixed.Core.IO;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Text;
using System.Windows.Input;
namespace Pixed.Core.Models;

public class Layer : PixelImage, IPixedSerializer, IDisposable
{
    private int _width;
    private int _height;
    private SKBitmap _bitmap;
    private double _opacity = 100.0d;
    private string _name = string.Empty;
    private bool _disposedValue;
    private readonly string _id;

    public double Opacity
    {
        get => _opacity;
        set
        {
            _opacity = value;
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
    public override int Width => _width;
    public override int Height => _height;
    public ICommand? ChangeOpacityCommand { get; }

    public static Func<Layer, Task>? ChangeOpacityAction { get; set; }

    public Layer(int width, int height)
    {
        ChangeOpacityCommand = new AsyncCommand(async () =>
        {
            if(ChangeOpacityAction != null)
            {
                await ChangeOpacityAction(this);
            }
        });
        _id = Guid.NewGuid().ToString();
        _width = width;
        _height = height;
        _bitmap = SkiaUtils.GetBitmap(width, height);
    }

    private Layer(int width, int height, SKBitmap bitmap)
    {
        ChangeOpacityCommand = new AsyncCommand(async () =>
        {
            if(ChangeOpacityAction != null)
            {
                await ChangeOpacityAction(this);
            }
        });
        _id = Guid.NewGuid().ToString();
        _width = width;
        _height = height;
        _bitmap = bitmap.Copy();
    }

    public Layer Clone()
    {
        Layer layer = new(_width, _height, _bitmap)
        {
            Name = Name,
        };
        return layer;
    }

    public uint[] GetDistinctPixels()
    {
        return [.. _bitmap.ToArray().Distinct()];
    }

    public SKCanvas GetCanvas()
    {
        return new SKCanvas(_bitmap);
    }

    public BitmapHandle GetHandle()
    {
        return new BitmapHandle(_bitmap);
    }

    public void MergeLayers(Layer layer2)
    {
        var bitmap = Render();
        var layer2Bitmap = layer2.Render();
        SKCanvas canvas = new(bitmap);
        canvas.DrawBitmap(layer2Bitmap, new SKPoint(0, 0));
        canvas.Dispose();
        _bitmap.Dispose();
        _bitmap = bitmap;
    }

    [Obsolete]
    public uint GetPixel(Point point)
    {
        if (!ContainsPixel(point))
        {
            return 0;
        }

        IntPtr pixelsPtr = _bitmap.GetPixels();

        unsafe
        {
            var unsafePtr = (uint*)pixelsPtr;
            return (UniColor)unsafePtr[point.Y * Width + point.X];
        }
    }

    public unsafe uint* GetPixels()
    {
        IntPtr pixelsPtr = _bitmap.GetPixels();
        return (uint*)pixelsPtr;
    }

    public override SKBitmap Render()
    {
        if (Opacity == 100)
        {
            return _bitmap.FastCopy();
        }

        byte newAlpha = (byte)((Opacity / 100d) * 255d);
        SKPaint paint = new() { Color = new SKColor(255, 255, 255, newAlpha) };
        SKBitmap opacityBitmap = SkiaUtils.GetBitmap(Width, Height);
        SKCanvas canvas = new(opacityBitmap);

        if (!SkiaUtils.IsNull(_bitmap))
        {
            canvas.DrawBitmap(_bitmap, SKPoint.Empty, paint);
        }
        canvas.Dispose();
        paint.Dispose();

        return opacityBitmap;
    }

    public bool ContainsPixel(Point point)
    {
        return point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height;
    }

    public long CalculateStreamSize()
    {
        var name = Encoding.UTF8.GetBytes(Name);
        return sizeof(double) + (sizeof(int) * 4) + name.LongLength + (_bitmap.Width * _bitmap.Height * sizeof(uint));
    }

    public void Serialize(Stream stream)
    {
        stream.WriteDouble(Opacity);
        stream.WriteInt(Width);
        stream.WriteInt(Height);
        stream.WriteString(Name);
        stream.WriteInt(Width * Height);
        var array = _bitmap.ToByteArray();
        stream.Write(array);
    }

    public void Deserialize(Stream stream)
    {
        Opacity = stream.ReadDouble();
        _width = stream.ReadInt();
        _height = stream.ReadInt();
        _name = stream.ReadString();
        int pixelsSize = stream.ReadInt();
        byte[] bytes = new byte[pixelsSize * sizeof(uint)];
        int bytesRead = stream.Read(bytes, 0, bytes.Length);

        if (bytesRead != bytes.Length)
        {
            return;
        }
        var colors = bytes.ToUInt();

        _bitmap?.Dispose();
        _bitmap = SkiaUtils.FromArray(colors, new Point(_width, _height));
    }

    public static Layer FromBitmap(SKBitmap bitmap, string name)
    {
        Layer layer = new(bitmap.Width, bitmap.Height)
        {
            _bitmap = bitmap,
            _name = name
        };
        return layer;
    }

    public static Layer FromColors(uint[] colors, int width, int height, string name)
    {
        Layer layer = new(width, height)
        {
            _bitmap = SkiaUtils.FromArray(colors, new Point(width, height)),
            _name = name
        };
        return layer;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _bitmap.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
