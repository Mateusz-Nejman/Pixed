﻿using Pixed.Application.Controls;
using Pixed.Common;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace Pixed.Application.ViewModels;
internal class ProjectsSectionViewModel : ExtendedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private int _selectedProject = 0;
    private bool _disposedValue;
    private readonly IDisposable _projectAdded;
    private readonly IDisposable _projectRemoved;
    private readonly IDisposable _projectModified;

    public ObservableCollection<PixedModel> Projects => _applicationData.Models;

    public int SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            _applicationData.CurrentModelIndex = value;
            OnPropertyChanged();
            Subjects.ProjectChanged.OnNext(_applicationData.CurrentModel);
            Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
            Subjects.LayerChanged.OnNext(_applicationData.CurrentLayer);

            foreach (var frame in _applicationData.CurrentModel.Frames)
            {
                foreach (var layer in frame.Layers)
                {
                    Subjects.LayerModified.OnNext(layer);
                }
                Subjects.FrameModified.OnNext(frame);
            }
        }
    }

    public ProjectsSectionViewModel(ApplicationData applicationData)
    {
        _applicationData = applicationData;
        _projectAdded = Subjects.ProjectAdded.Subscribe(p =>
        {
            int index = _applicationData.Models.IndexOf(p);
            SelectedProject = index;
        });
        _projectRemoved = Subjects.ProjectRemoved.Subscribe(p =>
        {
            SelectedProject = Math.Clamp(SelectedProject, 0, _applicationData.Models.Count - 1);
        });
        _projectModified = Subjects.ProjectModified.Subscribe(p =>
        {
            foreach (var frame in p.Frames)
            {
                Subjects.FrameModified.OnNext(frame);

                foreach (var layer in frame.Layers)
                {
                    Subjects.LayerModified.OnNext(layer);
                }
            }

            Subjects.FrameChanged.OnNext(p.CurrentFrame);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _projectAdded?.Dispose();
                _projectRemoved?.Dispose();
                _projectModified?.Dispose();
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
