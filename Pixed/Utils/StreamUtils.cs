using System;
using System.IO;
using System.Text;

namespace Pixed.Utils
{
    internal static class StreamUtils
    {
        public static int ReadInt(this Stream stream)
        {
            return BitConverter.ToInt32(stream.Read(sizeof(int)));
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
            stream.Write(BitConverter.GetBytes(value));
        }

        public static void WriteIntArray(this Stream stream, int[] value)
        {
            byte[] newValue = new byte[value.Length * sizeof(int)];

            for(int i = 0; i < value.Length; i++)
            {
                int arrayIndex = i * sizeof(int);
                byte[] byteValue = BitConverter.GetBytes(value[i]);
                Array.Copy(byteValue, 0, newValue, arrayIndex, byteValue.Length);
            }

            stream.Write(newValue);
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

        private static byte[] Read(this Stream stream, int size)
        {
            byte[] buffer = new byte[size];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
