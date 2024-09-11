using System.Windows;
using System.Windows.Controls;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for TooltipWithButton.xaml
    /// </summary>
    public partial class TooltipWithButton : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public string ButtonTextHelper
        {
            get { return (string)GetValue(ButtonTextHelperProperty); }
            set { SetValue(ButtonTextHelperProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TooltipWithButton), new PropertyMetadata("Title"));
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(TooltipWithButton), new PropertyMetadata("ButtonText"));
        public static readonly DependencyProperty ButtonTextHelperProperty = DependencyProperty.Register("ButtonTextHelper", typeof(string), typeof(TooltipWithButton), new PropertyMetadata("ButtonTextHelper"));
        public TooltipWithButton()
        {
            InitializeComponent();
        }
    }
}
