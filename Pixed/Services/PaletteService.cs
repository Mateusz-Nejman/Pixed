using Pixed.Models;
using System.IO;

namespace Pixed.Services
{
    internal class PaletteService
    {
        public List<Palette> Palettes { get; }
        public Palette CurrentColorsPalette => Palettes[0];
        public Palette SelectedPalette => Palettes[PaletteIndex];
        public int PaletteIndex => 1;

        public PaletteService()
        {
            Palettes = [];
            Palettes.Add(new Palette("default") { Name = "Current colors" });
            Palettes.Add(new Palette("palette") { Name = "Palette" });
        }

        public void SetCurrentColors()
        {
            Palettes[0].Colors = Global.CurrentModel.GetAllColors();
        }

        public void AddColorsFromPalette(Palette palette)
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

        public void Add(Palette palette)
        {
            if (Palettes.FirstOrDefault(p => p.Id == palette.Id, null) == null)
            {
                Palettes.Add(palette);
                Subjects.PaletteAdded.OnNext(palette);
            }
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
                    Palette palette = Palette.FromJson(File.ReadAllText(file));
                    Palettes.Add(palette);
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        public static void Save(Palette palette)
        {
            if (!Directory.Exists("Palettes"))
            {
                Directory.CreateDirectory("Palettes");
            }

            File.WriteAllText("Palettes/" + palette.Id + ".json", palette.ToJson());
        }
    }
}
