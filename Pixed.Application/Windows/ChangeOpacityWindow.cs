using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;
internal class ChangeOpacityWindow : NumericPrompt
{
    public override Task InitialiseAsync(CancellationToken cancellationToken)
    {
        Text = "Enter new opacity (%):";
        return Task.CompletedTask;
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        DefaultValue = (double)args;
        numeric.Value = (decimal)DefaultValue;
        return Task.CompletedTask;
    }
}