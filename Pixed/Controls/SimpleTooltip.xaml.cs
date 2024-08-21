using System.Windows;
using System.Windows.Controls;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for SimpleTooltip.xaml
    /// </summary>
    public partial class SimpleTooltip : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SimpleTooltip), new PropertyMetadata("Title"));
        public SimpleTooltip()
        {
            InitializeComponent();
        }
    }
}
