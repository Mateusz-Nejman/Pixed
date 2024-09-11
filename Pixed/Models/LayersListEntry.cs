using Avalonia.Media.Imaging;

namespace Pixed.Models
{
    internal class LayersListEntry : PropertyChangedBase
    {
        private string _name;
        private Bitmap _thumbnail;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Bitmap Thumbnail
        {
            get => _thumbnail;
            set
            {
                _thumbnail = value;
                OnPropertyChanged();
            }
        }
    }
}
