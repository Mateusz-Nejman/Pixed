using Avalonia;
using Pixed.Common.Tools;

namespace Pixed.Application.Controls;

internal partial class ToolTooltip : EmptyPixedUserControl
{
    public ToolTooltip(ToolTooltipProperties props)
    {
        InitializeComponent();
        title.Text = props.Title;

        if (!props.HasButtons())
        {
            title.Margin = new Thickness(0);
        }

        if (props.Button1 != null)
        {
            wrap1.IsVisible = true;
            button1.Text = props.Button1;
            button1Text.Text = props.Button1Text;
        }

        if (props.Button2 != null)
        {
            wrap2.IsVisible = true;
            button2.Text = props.Button2;
            button2Text.Text = props.Button2Text;
        }

        if (props.Button3 != null)
        {
            wrap3.IsVisible = true;
            button3.Text = props.Button3;
            button3Text.Text = props.Button3Text;
        }
    }
}