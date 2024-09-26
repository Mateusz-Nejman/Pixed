using Pixed.Controls;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace Pixed.ViewModels;
internal class AboutViewModel : PixedViewModel
{
    public ICommand OpenUrlCommand { get; }
    public string AppVersion { get; }
    public AboutViewModel()
    {
        OpenUrlCommand = new ActionCommand<string>(OpenUrlAction);
        AppVersion = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
}