using Pixed.Application.Controls;
using Pixed.Application.Platform;
using Pixed.Core;
using System.Diagnostics;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;
internal class AboutViewModel : PixedViewModel
{
    public ICommand OpenUrlCommand { get; }
    public string AppVersion { get; }
    public AboutViewModel()
    {
        OpenUrlCommand = new ActionCommand<string>(OpenUrlAction);
        AppVersion = "Version: " + GetVersion();
    }

    private void OpenUrlAction(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private string GetVersion()
    {
        var lifecycle = Provider.Get<IPlatformSettings>();

        return lifecycle.GetVersion();
    }
}