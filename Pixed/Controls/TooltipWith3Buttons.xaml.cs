using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for TooltipWith3Buttons.xaml
    /// </summary>
    public partial class TooltipWith3Buttons : UserControl
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

        public string ButtonText3
        {
            get { return (string)GetValue(ButtonText3Property); }
            set { SetValue(ButtonText3Property, value); }
        }

        public string ButtonTextHelper3
        {
            get { return (string)GetValue(ButtonTextHelper3Property); }
            set { SetValue(ButtonTextHelper3Property, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("Title"));
        public static readonly DependencyProperty ButtonText1Property = DependencyProperty.Register("ButtonText1", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonText1"));
        public static readonly DependencyProperty ButtonTextHelper1Property = DependencyProperty.Register("ButtonTextHelper1", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonTextHelper1"));
        public static readonly DependencyProperty ButtonText2Property = DependencyProperty.Register("ButtonText2", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonText2"));
        public static readonly DependencyProperty ButtonTextHelper2Property = DependencyProperty.Register("ButtonTextHelper2", typeof(string), typeof(TooltipWith2Buttons), new PropertyMetadata("ButtonTextHelper2"));
        public static readonly DependencyProperty ButtonText3Property = DependencyProperty.Register("ButtonText3", typeof(string), typeof(TooltipWith3Buttons), new PropertyMetadata("ButtonText3"));
        public static readonly DependencyProperty ButtonTextHelper3Property = DependencyProperty.Register("ButtonTextHelper3", typeof(string), typeof(TooltipWith3Buttons), new PropertyMetadata("ButtonTextHelper3"));
        public TooltipWith3Buttons()
        {
            InitializeComponent();
        }
    }
}
