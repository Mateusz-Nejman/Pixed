using Pixed.Models;
using System;
using System.Collections.ObjectModel;

namespace Pixed.ViewModels
{
    internal class ProjectsSectionViewModel : PropertyChangedBase
    {
        private int _selectedProject = 0;
        public static ObservableCollection<PixedModel> Projects => Global.Models;

        public int SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                Global.CurrentModelIndex = value;
                Subjects.ProjectChanged.OnNext(Global.CurrentModel);
            }
        }

        public ProjectsSectionViewModel()
        {
            Subjects.FrameChanged.Subscribe(FrameSubjects);
            Subjects.FrameModified.Subscribe(FrameSubjects);
            Subjects.LayerChanged.Subscribe(LayerSubjects);

            Subjects.ProjectModified.Subscribe(p =>
            {
                p.UpdatePreview();
                OnPropertyChanged(nameof(Projects));
            });

            Subjects.ProjectChanged.Subscribe(p =>
            {
                _selectedProject = Global.Models.IndexOf(p);
                OnPropertyChanged(nameof(SelectedProject));
                Subjects.FrameChanged.OnNext(p.Frames[0]);
                Subjects.LayerChanged.OnNext(p.Frames[0].Layers[p.Frames[0].SelectedLayer]); //TODO
            });

            Subjects.ProjectAdded.Subscribe(p =>
            {
                int index = Global.Models.IndexOf(p);
                SelectedProject = index;
            });
        }

        private void FrameSubjects(Frame _)
        {
            Subjects.ProjectModified.OnNext(Global.CurrentModel);
        }

        private void LayerSubjects(Layer _)
        {
            Subjects.ProjectModified?.OnNext(Global.CurrentModel);
        }
    }
}
