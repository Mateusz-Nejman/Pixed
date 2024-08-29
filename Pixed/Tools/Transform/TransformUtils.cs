using Pixed.Models;

namespace Pixed.Tools.Transform
{
    internal static class TransformUtils
    {
        public enum Axis
        {
            Vertical,
            Horizontal
        }

        public static void Flip(ref Layer layer, Axis axis)
        {
            Layer clone = layer.Clone();

            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    int pixelX = x;
                    int pixelY = y;

                    if (axis == Axis.Vertical)
                    {
                        pixelX = layer.Width - x - 1;
                    }
                    else if (axis == Axis.Horizontal)
                    {
                        pixelY = layer.Height - y - 1;
                    }

                    layer.SetPixel(pixelX, pixelY, clone.GetPixel(x, y));
                }
            }
        }
    }
}