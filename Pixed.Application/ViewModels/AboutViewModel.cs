using Pixed.Application.Controls;
using Pixed.Common;
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

    private static string GetVersion()
    {
        var assemblyName = Process.GetCurrentProcess().MainModule.FileName;
        var versionInfo = FileVersionInfo.GetVersionInfo(assemblyName);
        return versionInfo.FileVersion;
    }
}