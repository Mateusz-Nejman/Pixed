using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Pixed.Controls
{
    internal class ToolButton : Button
    {
        [Content]
        public IImage? Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly StyledProperty<IImage?> SourceProperty =
            AvaloniaProperty.Register<ToolButton, IImage?>(nameof(Source), coerce: (o, img) =>
            {
                if (o is ToolButton button)
                {
                    Image image = new Image();
                    image.Source = img;
                    button.Content = image;
                }

                return img;
            });
        public ToolButton()
        {
        }
    }
}
