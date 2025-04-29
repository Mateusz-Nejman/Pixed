using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;
using Newtonsoft.Json;
using Pixed.Application;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
#if DEBUG
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
#else
    public static void Main(string[] args)
    {
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log(e.Exception);
    }

    private static void Log(Exception e)
    {
        File.AppendAllText("exceptions.txt", JsonConvert.SerializeObject(e) + Environment.NewLine);
    }
#endif

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .UseShell();
}
