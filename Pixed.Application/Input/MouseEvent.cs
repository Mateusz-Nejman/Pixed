using Pixed.Core.Models;

namespace Pixed.Application.Input
{
    internal readonly struct MouseEvent(Point point, bool touch)
    {
        public Point Point { get; } = point;
        public bool Touch { get; } = touch;
    }
}
