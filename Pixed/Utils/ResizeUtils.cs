using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Pixed.Utils;

internal static class ResizeUtils
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

    public static PixedModel ResizeModel(PixedModel model, int width, int height, bool resizeContent, Origin origin)
    {
        ObservableCollection<Frame> frames = [];

        foreach (var frame in model.Frames)
        {
            frames.Add(ResizeFrame(frame, width, height, resizeContent, origin));
        }

        var resizedModel = PixedModel.FromFrames(frames);
        resizedModel.CurrentFrameIndex = model.CurrentFrameIndex;
        return resizedModel;
    }

    public static Frame ResizeFrame(Frame frame, int width, int height, bool resizeContent, Origin origin)
    {
        ObservableCollection<Layer> layers = [];

        foreach (var layer in frame.Layers)
        {
            layers.Add(ResizeLayer(layer, width, height, resizeContent, origin));
        }

        var resizedFrame = Frame.FromLayers(layers);
        resizedFrame.SelectedLayer = frame.SelectedLayer;
        return resizedFrame;
    }

    public static Layer ResizeLayer(Layer layer, int width, int height, bool resizeContent, Origin origin)
    {
        if (resizeContent)
        {
            return LayerUtils.Resize(layer, width, height);
        }

        Layer resizedLayer = new(width, height);

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var translated = TranslateCoordinates(x, y, layer, resizedLayer, origin);

                if (resizedLayer.ContainsPixel(translated.X, translated.Y))
                {
                    resizedLayer.SetPixel(translated.X, translated.Y, layer.GetPixel(x, y));
                }
            }
        }

        return resizedLayer;
    }

    private static Point TranslateCoordinates(int x, int y, Layer layer, Layer resizedLayer, Origin origin)
    {
        return new Point(TranslateX(x, layer.Width, resizedLayer.Width, origin), TranslateY(y, layer.Height, resizedLayer.Height, origin));
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
