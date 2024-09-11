using Avalonia.Media.Imaging;
using Pixed.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Pixed.Models
{
    internal class Layer : PropertyChangedBase
    {
        private readonly int[] _pixels;
        private readonly int _width;
        private readonly int _height;
        private System.Drawing.Bitmap _renderedBitmap = null;
        private bool _needRerender = true;
        private float _opacity = 1.0f;
        private string _name = string.Empty;
        private Bitmap? _renderSource = null;
        private string _id;

        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                _needRerender = true;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Bitmap RenderSource
        {
            get => _renderSource;
            set
            {
                _renderSource = value;
                OnPropertyChanged();
            }
        }

        public string Id => _id;
        public int Width => _width;
        public int Height => _height;
        public Layer(int width, int height)
        {
            _id = Guid.NewGuid().ToString();
            _width = width;
            _height = height;
            _pixels = new int[width * height];

            Array.Fill(_pixels, UniColor.Transparent);
        }

        private Layer(int width, int height, int[] pixels)
        {
            _id = Guid.NewGuid().ToString();
            _width = width;
            _height = height;
            _pixels = pixels;
        }

        public Layer Clone()
        {
            int[] pixels = new int[_pixels.Length];
            _pixels.CopyTo(pixels, 0);
            Layer layer = new Layer(_width, _height, pixels);
            layer.Name = Name;
            layer.RenderSource = _renderedBitmap.ToAvaloniaBitmap();
            layer._needRerender = true;
            return layer;
        }

        public int[] GetPixels()
        {
            return _pixels;
        }

        public void SetPixel(int x, int y, int color)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return;
            }
            _pixels[y * _width + x] = color;
            _needRerender = true;
            RefreshRenderSource();
        }

        public void MergeLayers(Layer layer2)
        {
            int transparent = UniColor.Transparent;

            for (int a = 0; a < _pixels.Length; a++)
            {
                if (layer2._pixels[a] != transparent && _pixels[a] != layer2._pixels[a])
                {
                    _pixels[a] = layer2._pixels[a];
                }
            }

            _needRerender = true;
        }

        public void RefreshRenderSource()
        {
            RenderSource = Render().ToAvaloniaBitmap();
        }

        public int GetPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return 0;
            }

            return _pixels[y * _width + x];
        }

        public System.Drawing.Bitmap Render()
        {
            if (!_needRerender)
            {
                return _renderedBitmap;
            }

            int[] pixels = new int[_width * _height];
            _pixels.CopyTo(pixels, 0);

            for (int i = 0; i < pixels.Length; i++)
            {
                UniColor color = pixels[i];

                if (color.A * _opacity > 0)
                {
                    int newColor = UniColor.WithAlpha((byte)(255 * _opacity), color);
                    pixels[i] = newColor;
                }
            }

            System.Drawing.Bitmap bitmap = new(_width, _height);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            _renderedBitmap = bitmap;
            _needRerender = false;
            return bitmap;
        }

        public bool ContainsPixel(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
}
