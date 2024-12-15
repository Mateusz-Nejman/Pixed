using Pixed.Common.Models;
using Pixed.Core;
using Pixed.Core.Utils;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Pixed.Common.Services.Palette;
internal class GplPaletteSerializer : AbstractPaletteSerializer
{
    public override PaletteModel Deserialize(Stream stream, string filename)
    {
        string lineRegex = @"^(\s*\d{1,3})(\s*\d{1,3})(\s*\d{1,3})";
        string nameRegex = @"name\s*\:\s*(.*)$";

        PaletteModel model = new(filename)
        {
            Path = filename
        };

        var lines = stream.ReadAllLines();

        foreach (var line in lines)
        {
            if (model.Name == string.Empty)
            {
                Match nameMatch = Regex.Match(line, nameRegex, RegexOptions.IgnoreCase);

                if (nameMatch.Success)
                {
                    model.Name = nameMatch.Groups[1].Value;
                    continue;
                }
            }

            Match lineMatch = Regex.Match(line, lineRegex);

            if (lineMatch.Success)
            {
                int r = int.Parse(lineMatch.Groups[1].Value);
                int g = int.Parse(lineMatch.Groups[2].Value);
                int b = int.Parse(lineMatch.Groups[3].Value);
                model.Colors.Add(new UniColor(r, g, b));
            }
        }

        if (model.Name == string.Empty)
        {
            model.Name = filename;
        }

        return model;
    }

    public override void Serialize(Stream stream, PaletteModel model)
    {
        StringBuilder builder = new();
        builder.AppendLine("GIMP Palette");
        builder.AppendLine("Name: " + model.Name);
        builder.AppendLine("Description: Palette created using Pixed Editor by Mateusz Nejman");
        builder.AppendLine("Colors: " + model.Colors.Count);

        foreach (var colorInt in model.Colors)
        {
            UniColor color = (UniColor)colorInt;
            builder.AppendLine(color.R.ToString().PadRight(3, ' ') + " " + color.G.ToString().PadRight(3, ' ') + " " + color.B.ToString().PadRight(3, ' ') + " " + colorInt);
        }

        stream.Write(builder.ToString());
    }
}
