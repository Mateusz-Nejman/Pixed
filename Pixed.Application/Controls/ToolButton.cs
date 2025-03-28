﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Pixed.Application.Controls;

internal class ToolButton : TouchButton
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
                Image image = new()
                {
                    Source = img
                };
                button.Content = image;
            }

            return img;
        });
    public ToolButton()
    {
    }
}
