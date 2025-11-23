using AndroidX.Activity;
using Pixed.Application;

namespace Pixed.Android;
internal class BackPressedCallback() : OnBackPressedCallback(true)
{
    public override void HandleOnBackPressed()
    {
        App.CloseCommand?.Execute(this);
    }
}