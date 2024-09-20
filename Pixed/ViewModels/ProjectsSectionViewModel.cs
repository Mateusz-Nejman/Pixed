using Pixed.Models;
using System;
using System.Collections.ObjectModel;

namespace Pixed.ViewModels
{
    internal class ProjectsSectionViewModel : PropertyChangedBase, IDisposable
    {
        private int _selectedProject = 0;
        private bool _disposedValue;
        private readonly IDisposable _frameModified;
        private readonly IDisposable _frameRemoved;
        private readonly IDisposable _layerModified;
        private readonly IDisposable _layerRemoved;
        private readonly IDisposable _projectAdded;
        private readonly IDisposable _projectRemoved;
        private readonly IDisposable _projectModified;

        public static ObservableCollection<PixedModel> Projects => Global.Models;

        public int SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                Global.CurrentModelIndex = value;
                OnPropertyChanged();
                Subjects.ProjectChanged.OnNext(Global.CurrentModel);
                Subjects.FrameChanged.OnNext(Global.CurrentFrame);
                Subjects.LayerChanged.OnNext(Global.CurrentLayer);

                foreach (var frame in Global.CurrentModel.Frames)
                {
                    foreach (var layer in frame.Layers)
                    {
                        Subjects.LayerModified.OnNext(layer);
                    }
                    Subjects.FrameModified.OnNext(frame);
                }
            }
        }

        public ProjectsSectionViewModel()
        {
            _frameRemoved = Subjects.FrameRemoved.Subscribe(f => Global.CurrentModel.UpdatePreview());
            _frameModified = Subjects.FrameModified.Subscribe(f => Global.CurrentModel.UpdatePreview());
            _layerModified = Subjects.LayerModified.Subscribe(f => Global.CurrentModel.UpdatePreview());
            _layerRemoved = Subjects.LayerRemoved.Subscribe(f => Global.CurrentModel.UpdatePreview());
            _projectAdded = Subjects.ProjectAdded.Subscribe(p =>
            {
                int index = Global.Models.IndexOf(p);
                SelectedProject = index;
            });
            _projectRemoved = Subjects.ProjectRemoved.Subscribe(p =>
            {
                SelectedProject = Math.Clamp(SelectedProject, 0, Global.Models.Count - 1);
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
                Subjects.LayerChanged.OnNext(p.CurrentFrame.CurrentLayer);
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _frameRemoved?.Dispose();
                    _frameModified?.Dispose();
                    _layerModified?.Dispose();
                    _layerRemoved?.Dispose();
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
}
