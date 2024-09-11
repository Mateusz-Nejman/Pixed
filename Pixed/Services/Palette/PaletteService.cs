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

        public void Select(PaletteModel model)
        {
            int index = Palettes.IndexOf(model);
            Palettes[1] = Palettes[index].ToCurrentPalette();
            Subjects.PaletteSelected.OnNext(model);
        }

        public void Remove(PaletteModel model)
        {
            int index = Palettes.IndexOf(model);
            Palettes.RemoveAt(index);

            if(File.Exists(model.Path))
            {
                File.Delete(model.Path);
            }
            Subjects.PaletteSelected.OnNext(Palettes[1]);
        }

        public void SetCurrentColors()
        {
            Palettes[0].Colors = Global.CurrentModel.GetAllColors();
        }

        public void ReplaceSecondPalette(PaletteModel palette)
        {
            Palettes[PaletteIndex] = palette;
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
            Subjects.PaletteSelected.OnNext(Palettes[1]);
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

            string paletteFileName = Path.Combine(Global.DataFolder, "Palettes", fileInfo.Name);
            File.Copy(filename, paletteFileName, true);

            AbstractPaletteReader reader;

            if (fileInfo.Extension == ".json")
            {
                reader = new BasePaletteReader(filename);
            }
            else
            {
                reader = new GplPaletteReader(filename);
            }

            PaletteModel model = reader.Read();
            Palettes[1] = model.ToCurrentPalette();

            if(Palettes.FirstOrDefault(p => p.Id == model.Id, null) == null)
            {
                Palettes.Add(model);
            }
            else
            {
                Palettes[Palettes.FindIndex(p => p.Id == model.Id)] = model;
            }
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

            writer.Write(Palettes[PaletteIndex], filename);
            writer.Write(Palettes[PaletteIndex], Path.Combine(Global.DataFolder, "Palettes", fileInfo.Name));
        }

        public void LoadAll()
        {
            var files = Directory.GetFiles(Path.Combine(Global.DataFolder, "Palettes"));

            foreach (var file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);

                    AbstractPaletteReader reader;
                    if(fileInfo.Extension == ".json")
                    {
                        reader = new BasePaletteReader(file);
                    }
                    else
                    {
                        reader = new GplPaletteReader(file);
                    }
                    PaletteModel palette = reader.Read();
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
