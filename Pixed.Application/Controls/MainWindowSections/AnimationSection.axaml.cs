using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Pixed.Common;
using Pixed.Core.Models;
using System;

namespace Pixed.Application.Controls.MainWindowSections;

public partial class AnimationSection : UserControl, IDisposable
{
    private bool _disposedValue;
    private readonly IDisposable _animationPreviewChanged;

    public new bool IsVisible
    {
        get => base.IsVisible;
        set
        {
            base.IsVisible = value;
            projectAnimation.IsVisible = value;
        }
    }
    public AnimationSection()
    {
        InitializeComponent();
        _animationPreviewChanged = Subjects.AnimationPreviewChanged.Subscribe(enabled =>
        {
            IsVisible = enabled;
        });

        var applicationData = App.ServiceProvider.Get<ApplicationData>();
        IsVisible = applicationData.UserSettings.AnimationPreviewVisible;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _animationPreviewChanged?.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}