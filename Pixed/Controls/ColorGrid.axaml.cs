using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;

namespace Pixed.Controls;

internal partial class ColorGrid : EmptyPixedUserControl
{
    public int Columns
    {
        get { return GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }

    internal ObservableCollection<UniColor> Colors
    {
        get { return GetValue(ColorsProperty); }
        set { SetValue(ColorsProperty, value); }
    }

    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<ColorGrid, int>("Columns", 1);
    public static readonly StyledProperty<ObservableCollection<UniColor>> ColorsProperty = AvaloniaProperty.Register<ColorGrid, ObservableCollection<UniColor>>("Colors", [], coerce: OnColorsChanged);
    public ColorGrid() : base()
    {
        InitializeComponent();
    }

    private void RefreshControls(ObservableCollection<UniColor> colors)
    {
        colorStack.Children.Clear();

        int gridCount = (int)Math.Ceiling((double)colors.Count / (double)Columns);
        int colorIndex = 0;

        for (int g = 0; g < gridCount; g++)
        {
            Grid grid = new()
            {
                Height = 30
            };

            for (int a = 0; a < Columns; a++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int a = 0; a < Columns; a++)
            {
                if (colorIndex >= colors.Count)
                {
                    break;
                }

                Button colorButton = new()
                {
                    BorderBrush = new SolidColorBrush(colors[colorIndex]),
                    Background = new SolidColorBrush(colors[colorIndex]),
                    Command = new ActionCommand<UniColor>(Subjects.PrimaryColorChange.OnNext),
                    CommandParameter = colors[colorIndex],
                    Width = 30,
                    Height = 30
                };
                Grid.SetColumn(colorButton, a);

                grid.Children.Add(colorButton);
                colorIndex++;
            }

            colorStack.Children.Add(grid);
        }
    }

    private static ObservableCollection<UniColor> OnColorsChanged(AvaloniaObject o, ObservableCollection<UniColor> colors)
    {
        var grid = (ColorGrid)o;

        colors ??= [];
        grid.RefreshControls(colors);
        return colors;
    }
}