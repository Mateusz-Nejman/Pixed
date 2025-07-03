﻿using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Avalonia.Controls.ApplicationLifetimes;
using Pixed.Application.Platform;
namespace Pixed.Android;
internal class PlatformSettings : IPlatformSettings
{
    public static Activity MainActivity { get; internal set; }
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled => false;
    public bool RecentFilesEnabled => false;
    public bool ExtensionsOnSave => true;

    public void Close()
    {
        Process.KillProcess(Process.MyPid());
    }

    public string GetVersion()
    {
        PackageInfo info = MainActivity.PackageManager.GetPackageInfo(MainActivity.PackageName, 0);
        return info.VersionName;
    }

    public void ProcessMinimumScreenSize(int minScreenSize)
    {
        var system = Resources.System;

        if(system == null)
        {
            return;
        }
        var metrics = system.DisplayMetrics;

        if(metrics == null)
        {
            return;
        }

        var width = metrics.WidthPixels / metrics.Density;

        if (width <= minScreenSize)
        {
            MainActivity.RequestedOrientation = ScreenOrientation.Landscape;
        }
    } 
}
