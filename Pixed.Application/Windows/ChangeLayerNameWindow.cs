using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;
internal class ChangeLayerNameWindow : Prompt
{
    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        Title = "Enter new layer name";
        Text = "New layer name:";
        DefaultValue = args.ToString();
        textBox.Text = DefaultValue;
        return Task.CompletedTask;
    }
}
