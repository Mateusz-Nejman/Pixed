using Pixed.IO;
using Pixed.Utils;
using Pixed.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Pixed.Models;

internal class Layer : PropertyChangedBase, IPixedSerializer
{
    private uint[] _pixels;
    private int _width;
    private int _height;
    private System.Drawing.Bitmap _renderedBitmap = null;
    private bool _needRerender = true;
    private double _opacity = 100.0d;
    private string _name = string.Empty;
    private Bitmap? _renderSource = null;
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

    public Bitmap RenderSource
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
    public Layer(int width, int height)
    {
        ChangeOpacityCommand = new AsyncCommand(ChangeOpacityAction);
        _id = Guid.NewGuid().ToString();
        _width = width;
        _height = height;
        _pixels = new uint[width * height];

        Array.Fill(_pixels, UniColor.Transparent.ToUInt());
    }

    private Layer(int width, int height, uint[] pixels)
    {
        ChangeOpacityCommand = new AsyncCommand(ChangeOpacityAction);
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
            RenderSource = _renderedBitmap.ToAvaloniaBitmap(),
            _needRerender = true
        };
        return layer;
    }

    public uint[] GetPixels()
    {
        return _pixels;
    }

    public void SetPixel(int x, int y, uint color)
    {
        SetPixelPrivate(x, y, color);
    }

    public void SetPixels(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            SetPixelPrivate(pixel.X, pixel.Y, pixel.Color);
        }
    }

    public void MergeLayers(Layer layer2)
    {
        System.Drawing.Bitmap outputBitmap = new(Width, Height);
        Graphics graphics = Graphics.FromImage(outputBitmap);
        graphics.DrawImage(Render(), new Point(0, 0));
        graphics.DrawImage(layer2.Render(), new Point(0, 0));
        graphics.Dispose();

        _pixels = outputBitmap.ToPixelColors();

        _needRerender = true;
    }

    public void Rerender()
    {
        _needRerender = true;
        RefreshRenderSource();
    }

    public void RefreshRenderSource()
    {
        RenderSource = Render().ToAvaloniaBitmap();
    }

    public uint GetPixel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height)
        {
            return 0;
        }

        return _pixels[y * _width + x];
    }

    public System.Drawing.Bitmap Render(List<Pixel>? modifiedPixels = null)
    {
        if (!_needRerender)
        {
            return _renderedBitmap;
        }

        uint[] pixels = new uint[_width * _height];
        _pixels.CopyTo(pixels, 0);

        foreach (var pixel in modifiedPixels ?? [])
        {
            pixels[pixel.Y * _width + pixel.X] = pixel.Color;
        }

        if (Opacity != 100)
        {
            for (int a = 0; a < pixels.Length; a++)
            {
                UniColor color = pixels[a];
                color.A = (byte)((Opacity / 100d) * color.A);
                pixels[a] = color;
            }
        }

        System.Drawing.Bitmap bitmap = new(_width, _height);
        var pixelBytes = pixels.ToBytes();
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        Marshal.Copy(pixelBytes, 0, bitmapData.Scan0, pixelBytes.Length);
        bitmap.UnlockBits(bitmapData);
        _renderedBitmap = bitmap;
        _needRerender = false;
        return bitmap.Clone(new Rectangle(0, 0, _width, _height), bitmap.PixelFormat);
    }

    public bool ContainsPixel(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
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

    private void SetPixelPrivate(int x, int y, uint color)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height)
        {
            return;
        }
        _pixels[y * _width + x] = color;
        _needRerender = true;
    }

    private async Task ChangeOpacityAction()
    {
        NumericPrompt numeric = new(Opacity)
        {
            Text = "Enter new opacity (%):"
        };
        var success = await numeric.ShowDialog<bool>(MainWindow.Handle);

        if (success)
        {
            if (Opacity != numeric.Value)
            {
                _needRerender = true;
            }
            Opacity = numeric.Value;
            RefreshRenderSource();
            Subjects.LayerModified.OnNext(this);
        }
    }
}
