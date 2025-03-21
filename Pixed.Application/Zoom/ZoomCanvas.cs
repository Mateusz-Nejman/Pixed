using Avalonia.Controls;
using Pixed.Application.Controls;
using System.Collections.Specialized;

namespace Pixed.Application.Zoom;
internal class ZoomCanvas : Canvas
{
    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if(Parent is ZoomBorder zoomBorder)
        {
            foreach (var child in Children)
            {
                if (child is OverlayControl control)
                {
                    control.AttachToZoomBorder(zoomBorder);
                }
            }
        }
        
        base.ChildrenChanged(sender, e);
    }
}