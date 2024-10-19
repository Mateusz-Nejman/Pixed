using Avalonia.Controls;
using Avalonia.Media;
using Pixed.Common;

namespace Pixed.Application.Controls.MainWindowSections;

internal class Section : Border
{
    public Section() : base()
    {
        Margin = new Avalonia.Thickness(8);
        BorderThickness = new Avalonia.Thickness(2);
        BorderBrush = new SolidColorBrush(UniColor.GetFromResources("ButtonBorder"));
    }
}
