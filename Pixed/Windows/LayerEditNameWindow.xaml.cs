using System.Windows;

namespace Pixed.Windows
{
    /// <summary>
    /// Interaction logic for LayerEditNameWindow.xaml
    /// </summary>
    public partial class LayerEditNameWindow : Window
    {
        public string NewName
        {
            get { return name.Text; }
            set { name.Text = value; }
        }
        public LayerEditNameWindow(string name)
        {
            InitializeComponent();
            NewName = name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
