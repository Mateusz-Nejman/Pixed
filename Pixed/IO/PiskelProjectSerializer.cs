using Newtonsoft.Json.Linq;
using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pixed.IO;
internal partial class PiskelProjectSerializer : IPixedProjectSerializer
{
    private const string BASE64_REGEX = @"base64?\s*,[^\\""]+(?=,|$)?";
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        StreamReader reader = new(stream);
        string streamString = reader.ReadToEnd();
        reader.Dispose();

        JObject baseJson = JObject.Parse(streamString);
        var json = baseJson["piskel"];
        string name = json["name"].Value<string>();
        int width = json["width"].Value<int>();
        int height = json["height"].Value<int>();

        var layers = json["layers"].ToArray();

        ObservableCollection<Frame> frames = [];

        foreach (var layer in layers)
        {
            string layerString = layer.Value<string>();
            var layerObject = JObject.Parse(layerString);
            var layerName = layerObject["name"].Value<string>();
            FillIfNeeded(ref frames, layerObject["frameCount"].Value<int>(), width, height);

            var match = LayerRegex().Match(layerString);

            if (match.Success)
            {
                string str = match.Value[7..];
                byte[] data = Convert.FromBase64String(str);
                MemoryStream memoryStream = new(data);
                Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream);

                for (int f = 0; f < frames.Count; f++)
                {
                    uint[] frameLayerData = new uint[width * height];

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            UniColor pixel = bitmap.GetPixel(x + (f * width), y);
                            frameLayerData[y * width + x] = pixel;
                        }
                    }

                    Layer newLayer = Layer.FromColors(frameLayerData, width, height, layerName);
                    frames[f].Layers.Add(newLayer);
                }

                memoryStream.Dispose();
                bitmap.Dispose();
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

    [GeneratedRegex(BASE64_REGEX)]
    private static partial Regex LayerRegex();
}
