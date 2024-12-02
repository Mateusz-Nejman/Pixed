using Android.App;
using Avalonia.Controls.ApplicationLifetimes;
using Pixed.Application.Platform;
using System;
namespace Pixed.Android;
internal class PlatformLifecycle : IApplicationLifecycle
{
    public static Activity MainActivity { get; internal set; }
    public IApplicationLifetime ApplicationLifetime { get; set; }

    public void Close()
    {
        MainActivity?.FinishAffinity();
    }
}
