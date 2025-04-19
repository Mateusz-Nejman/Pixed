using Pixed.Core.Models;
using Pixed.Core.Selection;

namespace Pixed.Core.Utils;

public static class PaintUtils
{
    public static List<Point> GetToolPoints(Point point, int toolSize)
    {
        List<Point> points = [];
        for (int y = 0; y < toolSize; y++)
        {
            for (int x = 0; x < toolSize; x++)
            {
                points.Add(new(point.X - (int)Math.Floor(toolSize / 2.0d) + x, point.Y - (int)Math.Floor(toolSize / 2.0d) + y));
            }
        }

        return points;
    }
    public static List<Pixel> GetSimiliarConnectedPixels(Frame frame, Point point)
    {
        return GetSimiliarConnectedPixels(frame.CurrentLayer, point);
    }

    public static List<Pixel> GetSimiliarConnectedPixels(Layer layer, Point point)
    {
        uint targetColor = layer.GetPixel(point);

        var points = MathUtils.FloodFill(point, new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos) == targetColor);
        var pixels = points.Select(point => new Pixel(point, targetColor)).ToList();
        return pixels;
    }

    public static void PaintSimiliarConnected(Layer layer, Point point, uint replacementColor, BaseSelection? selection)
    {
        uint targetColor = layer.GetPixel(point);

        if (targetColor == replacementColor)
        {
            return;
        }

        var points = MathUtils.FloodFill(point, new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos) == targetColor && (selection == null || selection.InSelection(pos)));
        var pixels = points.Select(p => new Pixel(p, replacementColor)).ToList();
        layer.SetPixels(pixels);
    }

    public static UniColor GetNoiseColor(UniColor primary, UniColor secondary)
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        double[] colors = [0, 0.25, 0.5, 0.75, 1];
        double color = colors[(int)Math.Floor(random.NextDouble() * colors.Length)];
        return primary.Blend(secondary, color);
    }

    public static void PaintNoiseSimiliarConnected(Layer layer, Point point, UniColor primaryColor, UniColor secondaryColor)
    {
        uint targetColor = layer.GetPixel(point);

        var points = MathUtils.FloodFill(point, new Point(layer.Width, layer.Height), pos => layer.GetPixel(pos) == targetColor);
        var pixels = points.Select(p => new Pixel(p, GetNoiseColor(primaryColor, secondaryColor))).ToList();
        layer.SetPixels(pixels);
    }

    public static void OutlineSimiliarConnectedPixels(Layer layer, Point point, uint replacementColor, bool fillCorners)
    {
        var targetColor = layer.GetPixel(point);

        if (targetColor == replacementColor)
        {
            return;
        }

        bool neighbourCheck(Pixel pixel)
        {
            for (int y1 = -1; y1 <= 1; y1++)
            {
                for (int x1 = -1; x1 <= 1; x1++)
                {
                    Point neighbourPoint = pixel.Position + new Point(x1, y1);
                    if (!layer.ContainsPixel(neighbourPoint))
                    {
                        continue;
                    }

                    if (fillCorners || x1 == 0 || y1 == 0)
                    {
                        var pixelColor = layer.GetPixel(neighbourPoint);

                        if (pixelColor != targetColor)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        var pixels = GetSimiliarConnectedPixels(layer, point);
        pixels = pixels.Where(neighbourCheck).Select(p => new Pixel(p.Position, replacementColor)).ToList();

        layer.SetPixels(pixels);
    }
}
