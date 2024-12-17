using Pixed.Common.Models;
using Pixed.Core.Utils;
using System.IO;

namespace Pixed.Common.Services.Palette;
internal class BasePaletteSerializer : AbstractPaletteSerializer
{
    public override PaletteModel Deserialize(Stream stream, string filename)
    {
        string json = stream.ReadAllText();
        PaletteModel model = PaletteModel.FromJson(json, filename);
        model.Path = filename;
        return model;
    }

    public override void Serialize(Stream stream, PaletteModel model)
    {
        stream.Write(model.ToJson());
    }
}
