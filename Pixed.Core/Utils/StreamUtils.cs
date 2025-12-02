using System.Text;

namespace Pixed.Core.Utils;
public static class StreamUtils
{
    public static int ReadInt(this Stream stream)
    {
        return BitConverter.ToInt32(stream.Read(sizeof(int)));
    }

    public static uint ReadUInt(this Stream stream)
    {
        return BitConverter.ToUInt32(stream.Read(sizeof(uint)));
    }

    public static double ReadDouble(this Stream stream)
    {
        return BitConverter.ToDouble(stream.Read(sizeof(double)));
    }

    public static bool ReadBool(this Stream stream)
    {
        return BitConverter.ToBoolean(stream.Read(sizeof(bool)));
    }

    public static float ReadFloat(this Stream stream)
    {
        return BitConverter.ToSingle(stream.Read(sizeof(float)));
    }

    public static string ReadString(this Stream stream)
    {
        int size = stream.ReadInt();
        byte[] buffer = stream.Read(size);
        return Encoding.UTF8.GetString(buffer);
    }

    public static void WriteInt(this Stream stream, int value)
    {
        IList<byte> buffer = BitConverter.GetBytes(value);
        stream.Write(buffer.ToArray());
    }

    public static void WriteUInt(this Stream stream, uint value)
    {
        IList<byte> buffer = BitConverter.GetBytes(value);
        stream.Write(buffer.ToArray());
    }

    public static void WriteIntArray(this Stream stream, int[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            stream.WriteInt(value[i]);
        }
    }

    public static void WriteUIntArray(this Stream stream, uint[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            stream.WriteUInt(value[i]);
        }
    }

    public static void WriteDouble(this Stream stream, double value)
    {
        stream.Write(BitConverter.GetBytes(value));
    }

    public static void WriteBool(this Stream stream, bool value)
    {
        stream.Write(BitConverter.GetBytes(value));
    }

    public static void WriteFloat(this Stream stream, float value)
    {
        stream.Write(BitConverter.GetBytes(value));
    }

    public static void WriteString(this Stream stream, string value)
    {
        stream.WriteInt(value.Length);
        stream.Write(Encoding.UTF8.GetBytes(value));
    }

    public static void Write(this Stream stream, string value)
    {
        stream.Write(Encoding.UTF8.GetBytes(value));
    }

    public static string ReadAllText(this Stream stream)
    {
        return Encoding.UTF8.GetString(ReadAllBytes(stream));
    }

    public static string[] ReadAllLines(this Stream stream)
    {
        return ReadAllText(stream).Split('\n');
    }

    public static byte[] ReadAllBytes(this Stream stream)
    {
        return stream.Read(stream.Length);
    }

    private static byte[] Read(this Stream stream, int size)
    {
        byte[] buffer = new byte[size];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        if (bytesRead != buffer.Length)
        {
            Console.WriteLine("Invalid buffer size");
        }
        return buffer;
    }

    private static byte[] Read(this Stream stream, long size)
    {
        byte[] buffer = new byte[size];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        if (bytesRead != buffer.Length)
        {
            Console.WriteLine("Invalid buffer size");
        }
        return buffer;
    }
}
