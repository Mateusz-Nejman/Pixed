using System;
using System.Drawing;
using System.IO;

namespace Pixed.Utils;

internal static class ConversionUtils
{
    public static Point ToSystemPoint(this Avalonia.Point point)
    {
        return new Point((int)point.X, (int)point.Y);
    }

    public static uint[] ToUInt(this byte[] array)
    {
        uint[] result = new uint[array.Length / sizeof(uint)];

        for (int a = 0; a < array.Length; a += sizeof(uint))
        {
            result[a / sizeof(uint)] = BitConverter.ToUInt32(array, a);
        }

        return result;
    }

    public static byte[] ToBytes(this uint[] array)
    {
        MemoryStream stream = new();

        foreach (var value in array)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        var bytes = stream.ToArray();
        stream.Dispose();
        return bytes;
    }

    public static byte[] ToBytes(this int[] array)
    {
        MemoryStream stream = new();

        foreach (var value in array)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        var bytes = stream.ToArray();
        stream.Dispose();
        return bytes;
    }
}
