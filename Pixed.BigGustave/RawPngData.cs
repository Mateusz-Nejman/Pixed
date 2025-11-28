using Pixed.Core;

namespace Pixed.BigGustave;

/// <summary>
/// Provides convenience methods for indexing into a raw byte array to extract pixel values.
/// </summary>
internal class RawPngData
{
    private readonly byte[] _data;
    private readonly int _bytesPerPixel;
    private readonly int _width;
    private readonly Palette? _palette;
    private readonly ColorType _colorType;
    private readonly int _rowOffset;
    private readonly int _bitDepth;

    /// <summary>
    /// Create a new <see cref="RawPngData"/>.
    /// </summary>
    /// <param name="data">The decoded pixel data as bytes.</param>
    /// <param name="bytesPerPixel">The number of bytes in each pixel.</param>
    /// <param name="palette">The palette for the image.</param>
    /// <param name="imageHeader">The image header.</param>
    public RawPngData(byte[] data, int bytesPerPixel, Palette? palette, ImageHeader imageHeader)
    {
        if (_width < 0)
        {
            throw new ArgumentOutOfRangeException($"Width must be greater than or equal to 0, got {_width}.");
        }

        this._data = data ?? throw new ArgumentNullException(nameof(data));
        this._bytesPerPixel = bytesPerPixel;
        this._palette = palette;

        _width = imageHeader.Width;
        _colorType = imageHeader.ColorType;
        _rowOffset = imageHeader.InterlaceMethod == InterlaceMethod.Adam7 ? 0 : 1;
        _bitDepth = imageHeader.BitDepth;
    }

    public byte[] GetBytes(int height)
    {
        byte[] pixels = new byte[_width * height * _bytesPerPixel];

        //TODO palette case

        for (int y = 0; y < height; y++)
        {
            var rowStartPixel = (_rowOffset + (_rowOffset * y)) + (_bytesPerPixel * _width * y);
            Array.Copy(_data, rowStartPixel, pixels, _width * y * _bytesPerPixel, _width * _bytesPerPixel);
        }

        return pixels;
    }

    public UniColor GetPixel(int x, int y)
    {
        if (_palette != null)
        {
            var pixelsPerByte = (8 / _bitDepth);

            var bytesInRow = (1 + (_width / pixelsPerByte));

            var byteIndexInRow = x / pixelsPerByte;
            var paletteIndex = (1 + (y * bytesInRow)) + byteIndexInRow;

            var b = _data[paletteIndex];

            if (_bitDepth == 8)
            {
                return _palette.GetPixel(b);
            }

            var withinByteIndex = x % pixelsPerByte;
            var rightShift = 8 - ((withinByteIndex + 1) * _bitDepth);
            var indexActual = (b >> rightShift) & ((1 << _bitDepth) - 1);

            return _palette.GetPixel(indexActual);
        }

        var rowStartPixel = (_rowOffset + (_rowOffset * y)) + (_bytesPerPixel * _width * y);

        var pixelStartIndex = rowStartPixel + (_bytesPerPixel * x);

        var first = _data[pixelStartIndex];

        switch (_bytesPerPixel)
        {
            case 1:
                return new UniColor(255, first, first, first);
            case 2:
                switch (_colorType)
                {
                    case ColorType.None:
                        {
                            byte second = _data[pixelStartIndex + 1];
                            var value = ToSingleByte(first, second);
                            return new UniColor(255, value, value, value);

                        }
                    default:
                        return new UniColor(_data[pixelStartIndex + 1], first, first, first);
                }

            case 3:
                return new UniColor(255, first, _data[pixelStartIndex + 1], _data[pixelStartIndex + 2]);
            case 4:
                switch (_colorType)
                {
                    case ColorType.None | ColorType.AlphaChannelUsed:
                        {
                            var second = _data[pixelStartIndex + 1];
                            var firstAlpha = _data[pixelStartIndex + 2];
                            var secondAlpha = _data[pixelStartIndex + 3];
                            var gray = ToSingleByte(first, second);
                            var alpha = ToSingleByte(firstAlpha, secondAlpha);
                            return new UniColor(alpha, gray, gray, gray);
                        }
                    default:
                        return new UniColor(_data[pixelStartIndex + 3], first, _data[pixelStartIndex + 1], _data[pixelStartIndex + 2]);
                }
            case 6:
                return new UniColor(255, first, _data[pixelStartIndex + 2], _data[pixelStartIndex + 4]);
            case 8:
                return new UniColor(_data[pixelStartIndex + 6], first, _data[pixelStartIndex + 2], _data[pixelStartIndex + 4]);
            default:
                throw new InvalidOperationException($"Unreconized number of bytes per pixel: {_bytesPerPixel}.");
        }
    }

    private static byte ToSingleByte(byte first, byte second)
    {
        var us = (first << 8) + second;
        var result = (byte)Math.Round((255 * us) / (double)ushort.MaxValue);
        return result;
    }
}