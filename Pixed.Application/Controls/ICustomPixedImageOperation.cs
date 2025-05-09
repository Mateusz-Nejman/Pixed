using Avalonia;
using Avalonia.Rendering.SceneGraph;
using Pixed.Core.Models;

namespace Pixed.Application.Controls;
internal interface ICustomPixedImageOperation : ICustomDrawOperation
{
    public abstract new Rect Bounds { get; set; }
    public abstract void UpdateBitmap(PixelImage? source);
}