using Pixed.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixed.Models
{
    internal class Frame : PropertyChangedBase
    {
        private readonly ObservableCollection<Layer> _layers;
        private readonly int _width;
        private readonly int _height;
        private int _selectedLayer = 0;
        private string _id;
        private BitmapImage _renderSource;

        public int Width => _width;
        public int Height => _height;
        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = Math.Clamp(value, 0, Layers.Count - 1);
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public BitmapImage RenderSource
        {
            get => _renderSource;
            set
            {
                _renderSource = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Layer> Layers => _layers;

        public string Id => _id;

        public Frame(int width, int height)
        {
            _id = Guid.NewGuid().ToString();
            _layers = [];
            _width = width;
            _height = height;
            AddLayer(new Layer(width, height));
        }

        public void SetPixel(int x, int y, int color)
        {
            _layers[SelectedLayer].SetPixel(x, y, color);
        }

        public int GetPixel(int x, int y)
        {
            return _layers[SelectedLayer].GetPixel(x, y);
        }

        public void AddLayer(Layer layer)
        {
            string name = "Layer " + _layers.Count;
            layer.Name = name;
            layer.RefreshRenderSource();
            _layers.Add(layer);
            OnPropertyChanged(nameof(Layers));
        }

        public void RefreshRenderSource()
        {
            RenderSource = Render().ToBitmapImage();
        }

        public Bitmap RenderTransparent()
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

        public Bitmap Render()
        {
            Bitmap render = new(Width, Height);
            Graphics g = Graphics.FromImage(render);
            for (int a = 0; a < _layers.Count; a++)
            {
                g.DrawImage(_layers[a].Render(), 0, 0);
            }

            return render;
        }

        public bool PointInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public void MoveLayerUp(bool toTop)
        {
            if (_selectedLayer == 0)
            {
                return;
            }

            int oldIndex = _selectedLayer;
            int newIndex = toTop ? 0 : oldIndex - 1;
            Layer layer = Layers[oldIndex];
            _layers.RemoveAt(oldIndex);
            _layers.Insert(newIndex, layer);
            SelectedLayer = newIndex;
            OnPropertyChanged(nameof(Layers));
        }

        public void MoveLayerDown(bool toBottom)
        {
            if (_selectedLayer == Layers.Count - 1)
            {
                return;
            }

            int oldIndex = _selectedLayer;
            Layer layer = _layers[oldIndex];

            if(toBottom)
            {
                _layers.Add(layer);
                _layers.RemoveAt(oldIndex);
                SelectedLayer = _layers.Count - 1;
            }
            else
            {
                _layers.Insert(oldIndex + 2, layer);
                _layers.RemoveAt(oldIndex);
                SelectedLayer = oldIndex + 1;
            }
            OnPropertyChanged(nameof(Layers));
        }

        public void MergeLayerBelow()
        {
            if(SelectedLayer >= Layers.Count - 1)
            {
                return;
            }

            int index = SelectedLayer;
            Layer layer = _layers[index];
            Layer layer2 = _layers[index + 1];
            layer.MergeLayers(layer2);
            Layers.RemoveAt(index + 1);
            Layers[index].RefreshRenderSource();
            SelectedLayer = index;
        }
    }
}
