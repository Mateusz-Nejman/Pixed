using Newtonsoft.Json.Linq;
using Pixed.BigGustave;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pixed.Application.IO;
internal partial class PiskelProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => false;
    public bool CanDeserialize => true;

    public string FormatExtension => ".piskel";

    public string FormatName => "Piskel project";

    private const string Base64Regex = @"base64?\s*,[^\\""]+(?=,|$)?";
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        StreamReader reader = new(stream);
        string streamString = reader.ReadToEnd();
        reader.Dispose();

        JObject baseJson = JObject.Parse(streamString);
        var json = baseJson["piskel"];

        if (json == null)
        {
            return new PixedModel(1, 1);
        }
        string name = json["name"]?.Value<string>() ?? string.Empty;
        int width = json["width"]?.Value<int>() ?? 1;
        int height = json["height"]?.Value<int>() ?? 1;

        var layers = json["layers"]?.ToArray();

        if (layers == null)
        {
            return new PixedModel(1, 1);
        }

        ObservableCollection<Frame> frames = [];

        foreach (var layer in layers)
        {
            string layerString = layer.Value<string>() ?? string.Empty;
            var layerObject = JObject.Parse(layerString);
            var layerName = layerObject["name"]?.Value<string>() ?? string.Empty;
            FillIfNeeded(ref frames, layerObject["frameCount"]?.Value<int>() ?? 0, width, height);

            var match = LayerRegex().Match(layerString);

            if (match.Success)
            {
                var str = match.Value[7..];
                var data = Convert.FromBase64String(str);
                var png = Png.Open(data);

                for (var f = 0; f < frames.Count; f++)
                {
                    var frameLayerData = new uint[width * height];

                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            frameLayerData[y * width + x] = png.GetPixel(x + (f * width), y);
                        }
                    }

                    var newLayer = Layer.FromColors(frameLayerData, width, height, layerName);
                    frames[f].Layers.Add(newLayer);
                }
            }
        }

        return PixedModel.FromFrames(frames, name, applicationData);
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        throw new NotSupportedException();
    }

    private void FillIfNeeded(ref ObservableCollection<Frame> frames, int count, int width, int height)
    {
        if (frames.Count == count)
        {
            return;
        }

        frames.Clear();

        for (int i = 0; i < count; i++)
        {
            Frame frame = new(width, height);
            frame.Layers.Clear();
            frames.Add(frame);
        }
    }

    [GeneratedRegex(Base64Regex)]
    private static partial Regex LayerRegex();
}
