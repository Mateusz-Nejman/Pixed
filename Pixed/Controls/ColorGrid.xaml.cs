using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for ColorGrid.xaml
    /// </summary>
    public partial class ColorGrid : UserControl
    {
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        internal ObservableCollection<UniColor> Colors
        {
            get { return (ObservableCollection<UniColor>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(ColorGrid), new PropertyMetadata(1));
        public static readonly DependencyProperty ColorsProperty = DependencyProperty.Register("Colors", typeof(ObservableCollection<UniColor>), typeof(ColorGrid), new PropertyMetadata(new ObservableCollection<UniColor>(), OnColorsChanged));
        public ColorGrid()
        {
            InitializeComponent();
        }

        private void RefreshControls()
        {
            colorStack.Children.Clear();

            int gridCount = (int)Math.Ceiling((double)Colors.Count / (double)Columns);
            int colorIndex = 0;

            for (int g = 0; g < gridCount; g++)
            {
                Grid grid = new();
                grid.Height = 30;

                for (int a = 0; a < Columns; a++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

                for (int a = 0; a < Columns; a++)
                {
                    if (colorIndex >= Colors.Count)
                    {
                        break;
                    }

                    Button colorButton = new Button();
                    colorButton.BorderBrush = new SolidColorBrush(Colors[colorIndex]);
                    colorButton.Background = new SolidColorBrush(Colors[colorIndex]);
                    colorButton.Command = new ActionCommand<UniColor>(Subjects.PrimaryColorChange.OnNext);
                    colorButton.CommandParameter = Colors[colorIndex];
                    colorButton.Width = 30;
                    colorButton.Height = 30;
                    Grid.SetColumn(colorButton, a);

                    grid.Children.Add(colorButton);
                    colorIndex++;
                }

                colorStack.Children.Add(grid);
            }
        }

        private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }
            var grid = (ColorGrid)d;
            grid.RefreshControls();
        }
    }
}
