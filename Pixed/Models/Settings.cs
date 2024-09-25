using Newtonsoft.Json;
using System.IO;

namespace Pixed.Models;

internal class Settings
{
    public int UserWidth { get; set; } = 16;
    public int UserHeight { get; set; } = 16;
    public UniColor GridColor { get; set; } = UniColor.Black;
    public int GridWidth { get; set; } = 1;
    public int GridHeight { get; set; } = 1;
    public bool GridEnabled { get; set; } = true;

    public void Save(string dataFolder)
    {
        string path = Path.Combine(dataFolder, "settings.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(this));
    }

    public static Settings Load(string dataFolder)
    {
        string path = Path.Combine(dataFolder, "settings.json");

        if (File.Exists(path))
        {
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
        }

        return new Settings();
    }
}
