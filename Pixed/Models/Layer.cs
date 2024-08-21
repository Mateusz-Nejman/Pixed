using Pixed.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Pixed.Models
{
    internal class Layer : PropertyChangedBase
    {
        private readonly int[] _pixels;
        private readonly int _width;
        private readonly int _height;
        private Bitmap _renderedBitmap = null;
        private bool _needRerender = true;
        private float _opacity = 1.0f;
        private string _name;
        private BitmapImage _renderSource;

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

        public BitmapImage RenderSource
        {
            get => _renderSource;
            set
            {
                _renderSource = value;
                OnPropertyChanged();
            }
        }
        public Layer(int width, int height)
        {
            _width = width;
            _height = height;
            _pixels = new int[width * height];

            Array.Fill(_pixels, Constants.TRANSPARENT_COLOR.ToArgb());
        }

        private Layer(int width, int height, int[] pixels)
        {
            _width = width;
            _height = height;
            _pixels = pixels;
        }

        public Layer Clone()
        {
            return new Layer(_width, _height, _pixels);
        }

        public void SetPixel(int x, int y, int color)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return;
            }
            _pixels[y * _width + x] = color;
            _needRerender = true;
            RenderSource = Render().ToBitmapImage();
        }

        public Bitmap Render()
        {
            if (!_needRerender)
            {
                return _renderedBitmap;
            }

            int[] pixels = new int[_width * _height];
            _pixels.CopyTo(pixels, 0);

            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = Color.FromArgb(pixels[i]);

                if (color.A * _opacity > 0)
                {
                    int newColor = Color.FromArgb((int)(255 * _opacity), color).ToArgb();
                    pixels[i] = newColor;
                }
            }

            Bitmap bitmap = new(_width, _height);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            _renderedBitmap = bitmap;
            _needRerender = false;
            return bitmap;
        }
    }
}
