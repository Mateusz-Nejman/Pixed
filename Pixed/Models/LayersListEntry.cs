using System.Windows.Media.Imaging;

namespace Pixed.Models
{
    internal class LayersListEntry : PropertyChangedBase
    {
        private string _name;
        private BitmapImage _thumbnail;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Thumbnail
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
