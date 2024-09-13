using Pixed.Models;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
        }
    }
}
