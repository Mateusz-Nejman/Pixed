using Pixed.Core.Models;
using Pixed.Core.Utils;
using PixiEditor.Parser;
using PixiEditor.Parser.Helpers;
using PixiEditor.Parser.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Pixed.Application.IO;
internal class PixiProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => false;

    public bool CanDeserialize => true;

    public string FormatExtension => ".pixi";

    public string FormatName => "Pixi project";

    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        //TODO add support for V5, when nuget will be ready
        var document = PixiParser.Deserialize(stream);
        var pixiLayers = GetLayers(document);
        ObservableCollection<Layer> layers = [];

        foreach (var pixiLayer in pixiLayers)
        {
            string name = pixiLayer.Name;
            double opacity = pixiLayer.Opacity * 100d;
            SKBitmap? bitmap = pixiLayer.ToSKBitmap();

            if (bitmap == null)
            {
                continue;
            }

            Layer layer = Layer.FromColors(bitmap.ToArray(), bitmap.Width, bitmap.Height, name);
            layer.Opacity = opacity;
            layers.Add(layer);
        }

        return PixedModel.FromFrames([Frame.FromLayers(layers)], "Pixi project", applicationData); ;
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        throw new NotSupportedException();
    }

    private static List<ImageLayer> GetLayers(Document document)
    {
        List<ImageLayer> layers = [];

        foreach (ImageLayer layer in document.RootFolder.GetChildrenRecursive().OfType<ImageLayer>())
        {
            if (layer is not ISize<int> || layer.ImageBytes == null || layer.ImageBytes.Length == 0 || !layer.GetFinalVisibility(document))
            {
                continue;
            }

            layers.Add(layer);
        }

        return layers;
    }
}