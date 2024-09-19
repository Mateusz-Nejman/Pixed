using Pixed.Utils;
using System.Collections.Generic;

namespace Pixed.ViewModels
{
    internal class ResizeProjectWindowViewModel : PropertyChangedBase
    {
        private int _width;
        private int _height;
        private bool _maintainAspectRatio;
        private bool _resizeCanvasContent;
        private int _anchor;

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();

                if (MaintainAspectRatio)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();

                if (MaintainAspectRatio)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public bool MaintainAspectRatio
        {
            get => _maintainAspectRatio;
            set
            {
                _maintainAspectRatio = value;
                OnPropertyChanged();
            }
        }

        public bool ResizeCanvasContent
        {
            get => _resizeCanvasContent;
            set
            {
                _resizeCanvasContent = value;
                OnPropertyChanged();
            }
        }

        public int Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
            }
        }

        public ResizeUtils.Origin AnchorEnum
        {
            get
            {
                return (ResizeUtils.Origin)Anchor;
            }
        }

        public static List<string> AnchorItems { get; } = ["Top left", "Top", "Top right", "Left", "Center", "Right", "Bottom left", "Bottom", "Bottom right"];
    }
}
