using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;
internal class ChangePaletteNameWindow : Prompt
{
    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        Title = "Rename Palette";
        Text = "New name: ";
        DefaultValue = args.ToString();
        textBox.Text = DefaultValue;
        return Task.CompletedTask;
    }
}
