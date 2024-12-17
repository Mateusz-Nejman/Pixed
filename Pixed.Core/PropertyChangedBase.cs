using ReactiveUI;
using System.Runtime.CompilerServices;

namespace Pixed.Core;

public class PropertyChangedBase : ReactiveObject
{
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.RaisePropertyChanged(propertyName);
    }
}