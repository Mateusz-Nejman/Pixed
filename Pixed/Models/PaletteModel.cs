using Newtonsoft.Json;

namespace Pixed.Models
{
    internal class PaletteModel
    {
        public string Id { get; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<int> Colors { get; set; } = [];

        public PaletteModel(string id)
        {
            Id = id;
        }

        public void Sort()
        {
            Colors.Sort();
        }

        public UniColor[] ToColors()
        {
            return Colors.Select(i => (UniColor)i).ToArray();
        }

        public static PaletteModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<PaletteModel>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
