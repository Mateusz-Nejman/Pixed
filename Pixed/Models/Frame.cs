using Pixed.Utils;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Pixed.Models
{
    internal class Frame : PropertyChangedBase
    {
        private readonly ObservableCollection<Layer> _layers;
        private readonly int _width;
        private readonly int _height;
        private int _selectedLayer = 0;

        public int Width => _width;
        public int Height => _height;
        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }
        public ObservableCollection<Layer> Layers => _layers;

        public Frame(int width, int height)
        {
            _layers = [];
            _width = width;
            _height = height;
            AddLayer(new Layer(width, height));
        }

        public void SetPixel(int x, int y, int color)
        {
            _layers[SelectedLayer].SetPixel(x, y, color);
            OnPropertyChanged(nameof(Layers));
        }

        public void AddLayer(Layer layer)
        {
            string name = "Layer " + _layers.Count;
            layer.Name = name;
            _layers.Add(layer);
        }

        public Bitmap Render()
        {
            Bitmap render = new(Width, Height);
            Graphics g = Graphics.FromImage(render);
            for (int a = 0; a < _layers.Count; a++)
            {
                if (a == SelectedLayer)
                {
                    continue;
                }

                g.DrawImage(_layers[a].Render().OpacityImage(0.5f), 0, 0);
            }

            g.DrawImage(_layers[_selectedLayer].Render(), 0, 0);

            return render;
        }
    }
}
