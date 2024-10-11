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

    public static byte[] ToBytes(this uint[] array)
    {
        MemoryStream stream = new();
        
        foreach(var value in array)
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
