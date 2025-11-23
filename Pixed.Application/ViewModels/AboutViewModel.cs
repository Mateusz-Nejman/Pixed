using Pixed.Application.Controls;
using Pixed.Application.Platform;
using Pixed.Core;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;
internal class AboutViewModel : ExtendedViewModel
{
    public ICommand OpenUrlCommand { get; }
    public string AppVersion { get; }
    public AboutViewModel()
    {
        OpenUrlCommand = new ActionCommand<string>(IPlatformSettings.Instance.OpenUrl);
        AppVersion = "Version: " + GetVersion();
    }

    private static string GetVersion()
    {
        return IPlatformSettings.Instance.GetVersion();
    }
}