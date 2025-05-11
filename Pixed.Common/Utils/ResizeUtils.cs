using Pixed.Common.Services;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Pixed.Common.Utils;

public static class ResizeUtils
{
    public enum Origin
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    };

    public static PixedModel ResizeModel(ApplicationData applicationData, PixedModel model, Point size, bool resizeContent, Origin origin)
    {
        ObservableCollection<Frame> frames = [];

        foreach (var frame in model.Frames)
        {
            frames.Add(ResizeFrame(frame, size, resizeContent, origin));
        }

        var resizedModel = PixedModel.FromFrames(frames, model.FileName, applicationData);
        resizedModel.CurrentFrameIndex = model.CurrentFrameIndex;
        return resizedModel;
    }

    public static Frame ResizeFrame(Frame frame, Point size, bool resizeContent, Origin origin)
    {
        ObservableCollection<Layer> layers = [];

        foreach (var layer in frame.Layers)
        {
            layers.Add(ResizeLayer(layer, size, resizeContent, origin));
        }

        var resizedFrame = Frame.FromLayers(layers);
        resizedFrame.SelectedLayer = frame.SelectedLayer;
        return resizedFrame;
    }

    public static Layer ResizeLayer(Layer layer, Point size, bool resizeContent, Origin origin)
    {
        if (resizeContent)
        {
            return LayerUtils.Resize(layer, size);
        }

        Layer resizedLayer = new(size.X, size.Y);
        List<Pixel> pixels = [];

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var translated = TranslateCoordinates(new Point(x, y), layer, resizedLayer, origin);

                if (resizedLayer.ContainsPixel(translated))
                {
                    pixels.Add(new Pixel(translated, layer.GetPixel(new Point(x, y))));
                }
            }
        }

        var canvas = resizedLayer.GetCanvas();
        canvas.DrawPixels(pixels);
        canvas.Dispose();

        return resizedLayer;
    }

    private static Point TranslateCoordinates(Point point, Layer layer, Layer resizedLayer, Origin origin)
    {
        return new Point(TranslateX(point.X, layer.Width, resizedLayer.Width, origin), TranslateY(point.Y, layer.Height, resizedLayer.Height, origin));
    }

    private static int TranslateX(int x, int width, int resizedWidth, Origin origin)
    {
        if (origin == Origin.Left || origin == Origin.BottomLeft || origin == Origin.TopLeft)
        {
            return x;
        }
        else if (origin == Origin.Right || origin == Origin.BottomRight || origin == Origin.TopRight)
        {
            return x - (width - resizedWidth);
        }

        return (int)(x - Math.Round((width - resizedWidth) / 2.0));
    }

    private static int TranslateY(int y, int height, int resizedHeight, Origin origin)
    {
        if (origin == Origin.TopRight || origin == Origin.Top || origin == Origin.TopLeft)
        {
            return y;
        }
        else if (origin == Origin.BottomRight || origin == Origin.Bottom || origin == Origin.BottomLeft)
        {
            return y - (height - resizedHeight);
        }

        return (int)(y - Math.Round((height - resizedHeight) / 2.0));
    }
}
