using System.Windows;
using System.Windows.Controls;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for TooltipWith2Buttons.xaml
    /// </summary>
    public partial class TooltipWith2Buttons : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string ButtonText1
        {
            get { return (string)GetValue(ButtonText1Property); }
            set { SetValue(ButtonText1Property, value); }
        }

        public string ButtonTextHelper1
        {
            get { return (string)GetValue(ButtonTextHelper1Property); }
            set { SetValue(ButtonTextHelper1Property, value); }
        }

        public string ButtonText2
        {
            get { return (string)GetValue(ButtonText2Property); }
            set { SetValue(ButtonText2Property, value); }
        }

        public string ButtonTextHelper2
        {
            get { return (string)GetValue(ButtonTextHelper2Property); }
            set { SetValue(ButtonTextHelper2Property, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("Title"));
        public static readonly DependencyProperty ButtonText1Property = DependencyProperty.Register("ButtonText1", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonText1"));
        public static readonly DependencyProperty ButtonTextHelper1Property = DependencyProperty.Register("ButtonTextHelper1", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonTextHelper1"));
        public static readonly DependencyProperty ButtonText2Property = DependencyProperty.Register("ButtonText2", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonText2"));
        public static readonly DependencyProperty ButtonTextHelper2Property = DependencyProperty.Register("ButtonTextHelper2", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonTextHelper2"));
        public TooltipWith2Buttons()
        {
            InitializeComponent();
        }
    }
}
