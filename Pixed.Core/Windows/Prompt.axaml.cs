using Pixed.Controls;
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
using System.Windows.Shapes;

namespace Pixed.Windows
{
    /// <summary>
    /// Interaction logic for Prompt.xaml
    /// </summary>
    public partial class Prompt : Window
    {
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string DefaultValue
        {
            get => (string)GetValue(DefaultValueProperty);
            set { SetValue(DefaultValueProperty, value); }
        }

        public string Value { get; private set; } = string.Empty;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Prompt), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(string), typeof(Prompt), new PropertyMetadata(string.Empty));
        public Prompt()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Value = textBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
