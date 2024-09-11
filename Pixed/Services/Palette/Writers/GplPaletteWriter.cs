using Pixed.Models;
using System.IO;
using System.Text;

namespace Pixed.Services.Palette.Writers
{
    internal class GplPaletteWriter : AbstractPaletteWriter
    {
        public override void Write(PaletteModel model, string filename)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("GIMP Palette");
            builder.AppendLine("Name: " + model.Name);
            builder.AppendLine("Description: Palette created using Pixed Editor by Mateusz Nejman");
            builder.AppendLine("Colors: " + model.Colors.Count);

            foreach (var colorInt in model.Colors)
            {
                UniColor color = (UniColor)colorInt;
                builder.AppendLine(color.R.ToString().PadRight(3, ' ') + " " + color.G.ToString().PadRight(3, ' ') + " " + color.B.ToString().PadRight(3, ' ') + " " + colorInt);
            }

            File.WriteAllText(filename, builder.ToString());
        }
    }
}
