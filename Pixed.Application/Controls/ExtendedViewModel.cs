using Avalonia.Controls;
using Pixed.Common.DependencyInjection;
using Pixed.Core;

namespace Pixed.Application.Controls;
internal abstract class ExtendedViewModel : PropertyChangedBase
{
    public Control? View { get; private set; }
    public IPixedServiceProvider? Provider => App.ServiceProvider;
    public virtual void RegisterMenuItems()
    {

    }

    public virtual void OnLoaded()
    {

    }

    public void Initialize(Control view)
    {
        View = view;
    }
}
