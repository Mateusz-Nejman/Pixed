using Pixed.Models;
using Pixed.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixed.ViewModels
{
    internal class PaletteWindowViewModel : PropertyChangedBase
    {
        public struct PaletteData
        {
            public BitmapImage BitmapImage { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public PaletteModel Model { get; set; }
            public ICommand SelectCommand { get; set; }
            public ICommand RemoveCommand { get; set; }
        }

        private ObservableCollection<PaletteData> _palettes;

        public ObservableCollection<PaletteData> Palettes
        {
            get => _palettes;
            set
            {
                _palettes = value;
                OnPropertyChanged();
            }
        }

        public Action<bool, PaletteModel> PaletteAction { get; set; }

        public PaletteWindowViewModel()
        {
            _palettes = [];

            for(int a = 2; a < Global.PaletteService.Palettes.Count; a++)
            {
                PaletteModel model = Global.PaletteService.Palettes[a];

                if(model.Colors.Count == 0)
                {
                    continue;
                }

                _palettes.Add(new PaletteData
                {
                    Name = model.Name,
                    Path = model.Path,
                    Model = model,
                    BitmapImage = GeneratePaleteBitmap(model),
                    SelectCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(true, m)),
                    RemoveCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(false, m)),
                });
            }
        }

        private BitmapImage GeneratePaleteBitmap(PaletteModel model)
        {
            int mod = (model.Colors.Count % 10) == 0 ? 0 : 1;
            Bitmap bitmap = new Bitmap(200, ((model.Colors.Count / 10) + mod) * 20);
            Graphics graphics = Graphics.FromImage(bitmap);
            int x = 0;
            int y = 0;

            foreach (var color in model.Colors)
            {
                int rectX = x * 20;
                int rectY = y * 20;

                graphics.FillRectangle(new SolidBrush((UniColor)color), new Rectangle(rectX, rectY, 20, 20));
                x++;

                if(x == 10)
                {
                    x = 0;
                    y++;
                }
            }
            graphics.Dispose();
            return bitmap.ToBitmapImage();
        }
    }
}
