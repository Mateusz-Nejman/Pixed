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

        public enum Direction
        {
            Clockwise,
            CounterClockwise
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

        public static void Rotate(ref Layer layer, Direction direction)
        {
            Layer clone = layer.Clone();
            int w = clone.Width;
            int h = clone.Height;

            int max = Math.Max(w, h);
            double deltaX = Math.Ceiling((max - w) / 2.0);
            double deltaY = Math.Ceiling((max - h) / 2.0);

            for(int x = 0; x < layer.Width;x++)
            {
                for(int y =  0; y < layer.Height;y++)
                {
                    int oldX = x;
                    int oldY = y;

                    double newX = x + deltaX;
                    double newY = y + deltaY;

                    double tempX = newX;
                    double tempY = newY;

                    if(direction == Direction.Clockwise)
                    {
                        newX = tempY;
                        newY = max - 1.0 - tempX;
                    }
                    else if(direction == Direction.CounterClockwise)
                    {
                        newY = tempX;
                        newX = max - 1.0 - tempY;
                    }

                    newX = newX - deltaX;
                    newY = newY - deltaY;

                    if(clone.ContainsPixel((int)newX, (int)newY))
                    {
                        layer.SetPixel(oldX, oldY, clone.GetPixel((int)newX, (int)newY));
                    }
                    else
                    {
                        layer.SetPixel(oldX, oldY, Constants.TRANSPARENT_COLOR.ToArgb());
                    }
                }
            }
        }
    }
}