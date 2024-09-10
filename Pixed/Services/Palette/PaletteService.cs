using Pixed.Models;
using Pixed.Services.Palette.Readers;
using Pixed.Services.Palette.Writers;
using System.IO;

namespace Pixed.Services.Palette
{
    internal class PaletteService
    {
        public List<PaletteModel> Palettes { get; }
        public PaletteModel CurrentColorsPalette => Palettes[0];
        public PaletteModel SelectedPalette => Palettes[PaletteIndex];
        public int PaletteIndex => 1;

        public PaletteService()
        {
            Palettes = [];
            Palettes.Add(new PaletteModel("default") { Name = "Current colors" });
            Palettes.Add(new PaletteModel("palette") { Name = "Palette" });
        }

        public void SetCurrentColors()
        {
            Palettes[0].Colors = Global.CurrentModel.GetAllColors();
        }

        public void AddColorsFromPalette(PaletteModel palette)
        {
            SelectedPalette.Colors.AddRange(palette.Colors.Where(c => !SelectedPalette.Colors.Contains(c)));
            SelectedPalette.Colors.Sort();
        }

        public void AddPrimaryColor()
        {
            if(!SelectedPalette.Colors.Contains(Global.PrimaryColor))
            {
                SelectedPalette.Colors.Add(Global.PrimaryColor);
                SelectedPalette.Colors.Sort();
            }
        }

        public void ClearPalette()
        {
            SelectedPalette.Colors.Clear();
        }

        public void Add(PaletteModel palette)
        {
            if (Palettes.FirstOrDefault(p => p.Id == palette.Id, null) == null)
            {
                Palettes.Add(palette);
                Subjects.PaletteAdded.OnNext(palette);
            }
        }

        public void Load(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);

            AbstractPaletteReader reader;

            if (fileInfo.Extension == ".json")
            {
                reader = new BasePaletteReader(filename);
            }
            else
            {
                reader = new GplPaletteReader(filename);
            }

            Palettes[1] = reader.Read();
            Subjects.PaletteSelected.OnNext(Palettes[1]);
            Subjects.PaletteAdded.OnNext(Palettes[1]);
        }

        public void Save(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);

            AbstractPaletteWriter writer;

            if (fileInfo.Extension == ".json")
            {
                writer = new BasePaletteWriter();
            }
            else
            {
                writer = new GplPaletteWriter();
            }

            writer.Write(Palettes[1], filename);
        }

        public void LoadAll()
        {
            if (!Directory.Exists("Palettes"))
            {
                return;
            }

            var files = Directory.GetFiles("Palettes");

            foreach (var file in files)
            {
                try
                {
                    PaletteModel palette = PaletteModel.FromJson(File.ReadAllText(file));
                    Palettes.Add(palette);
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        public static void Save(PaletteModel palette)
        {
            if (!Directory.Exists("Palettes"))
            {
                Directory.CreateDirectory("Palettes");
            }

            File.WriteAllText("Palettes/" + palette.Id + ".json", palette.ToJson());
        }
    }
}
