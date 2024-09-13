using Avalonia.Controls;
using Avalonia.Media;

namespace Pixed.Controls.MainWindowSections
{
    internal class Section : Border
    {
        public Section() : base()
        {
            Margin = new Avalonia.Thickness(8);
            BorderThickness = new Avalonia.Thickness(2);
            BorderBrush = new SolidColorBrush(new Color(255, 136, 136, 136)); //#888
        }
    }
}
