namespace Pixed.Core.Utils;

public static class ConversionUtils
{
    public static uint[] ToUInt(this byte[] array)
    {
        uint[] result = new uint[array.Length / sizeof(uint)];

        for (int a = 0; a < array.Length; a += sizeof(uint))
        {
            result[a / sizeof(uint)] = BitConverter.ToUInt32(array, a);
        }

        return result;
    }
}
