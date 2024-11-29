using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;
internal class UnsavedChangesWindow : YesNoCancelWindow
{
    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        Title = "Unsaved changes";
        Text = "Project " + args.ToString() + " has unsaved changes. Save it now?";
        return Task.CompletedTask;
    }
}
