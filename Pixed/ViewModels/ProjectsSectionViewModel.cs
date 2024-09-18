using Pixed.Models;
using System;
using System.Collections.ObjectModel;

namespace Pixed.ViewModels
{
    internal class ProjectsSectionViewModel : PropertyChangedBase
    {
        public static ObservableCollection<PixedModel> Projects => Global.Models;

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
                Subjects.FrameChanged.OnNext(p.Frames[0]);
                Subjects.LayerChanged.OnNext(p.Frames[0].Layers[p.Frames[0].SelectedLayer]); //TODO
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
