﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Pixed.Application;
using Pixed.Application.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Application.Utils;

namespace Pixed.Android;

[Activity(
    Label = "Pixed - Pixelart Editor",
    Icon = "@mipmap/pixed_icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
        PlatformSettings.MainActivity = this;
        PlatformFolder.Context = this;
        StreamBase.StreamReadImpl = typeof(AndroidStreamRead);
        StreamBase.StreamWriteImpl = typeof(AndroidStreamWrite);
        PlatformDependencyRegister.Implementation = new AndroidDependencyRegister();
        IPlatformSettings.Instance = new PlatformSettings();
        OnBackPressedDispatcher.AddCallback(new BackPressedCallback());

        base.OnCreate(savedInstanceState);
    }
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder).GetDefault();
    }
}

