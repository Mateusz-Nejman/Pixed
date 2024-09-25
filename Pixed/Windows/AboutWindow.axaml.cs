using Avalonia.Controls;
using System.Reflection;

namespace Pixed.Windows;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        versionText.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}