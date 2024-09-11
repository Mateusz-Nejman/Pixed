using ReactiveUI;
using System.Runtime.CompilerServices;

namespace Pixed
{
    internal class PropertyChangedBase : ReactiveObject
    {
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }
    }
}