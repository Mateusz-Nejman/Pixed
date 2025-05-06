namespace Pixed.Core.Utils;
public static class ColorUtils
{
    public static uint MultiplyAlpha(uint color, double multiply)
    {
        var alpha = (uint)(multiply *(byte)(color >> 24 & 0xFFu));

        uint newColor = color & 0xFFu;
        newColor |= (color >> 8 & 0xFFu) << 8;
        newColor |= (color >> 16 & 0xFFu) << 16;
        newColor |= alpha << 24;
        return newColor;
    }
}