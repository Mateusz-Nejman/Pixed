using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Models;

internal class PaletteModel(string id)
{
    [JsonIgnore]
    public string Id { get; } = id;
    public string Name { get; set; } = string.Empty;
    public List<int> Colors { get; set; } = [];
    [JsonIgnore]
    public string Path { get; set; } = string.Empty;

    public void Sort()
    {
        Colors.Sort();
    }

    public PaletteModel ToCurrentPalette()
    {
        return new PaletteModel("palette")
        {
            Colors = [.. Colors],
            Name = Name,
        };
    }

    public UniColor[] ToColors()
    {
        return Colors.Select(i => (UniColor)i).ToArray();
    }

    public static PaletteModel FromJson(string json, string id)
    {
        PaletteModel model = new(id);
        JsonConvert.PopulateObject(json, model);
        return model;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
