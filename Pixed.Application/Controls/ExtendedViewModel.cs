using Pixed.Common.DependencyInjection;
using Pixed.Core;

namespace Pixed.Application.Controls;
internal abstract class ExtendedViewModel : PropertyChangedBase
{
    public IPixedServiceProvider Provider => App.ServiceProvider;
    public virtual void RegisterMenuItems()
    {

    }

    public virtual void OnLoaded()
    {

    }
}
