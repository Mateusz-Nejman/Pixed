using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;

namespace Pixed.Application.Zoom;
internal partial class ZoomBorder
{
    internal class PanGestureRecognizer(ZoomBorder container) : GestureRecognizer
    {
        private readonly ZoomBorder _container = container;

        public bool IsEnabled { get; set; } = true;

        protected override void PointerCaptureLost(IPointer pointer)
        {
            if (!IsEnabled) return;
            ReleasePan();
        }

        protected override void PointerMoved(PointerEventArgs e)
        {
            if (!IsEnabled) return;
            if (_container._element == null || _container._captured != true || _container._isPanning != true)
            {
                return;
            }
            var point = e.GetPosition(_container._element);
            _container.ContinuePanTo(point.X, point.Y, true);
            e.Handled = true;
        }

        protected override void PointerPressed(PointerPressedEventArgs e)
        {
            if (!IsEnabled) return;
            var properties = e.GetCurrentPoint(_container).Properties;
            if (!properties.IsMiddleButtonPressed || (_container.GestureZoomEnabled && !properties.IsLeftButtonPressed))
            {
                return;
            }
            if (_container._element != null && _container._captured == false && _container._isPanning == false)
            {
                var point = e.GetPosition(_container._element);
                _container.BeginPanTo(point.X, point.Y);
                _container._captured = true;
                _container._isPanning = true;
                _container.SetPseudoClass(":isPanning", _container._isPanning);
                e.Handled = true;
            }
        }

        protected override void PointerReleased(PointerReleasedEventArgs e)
        {
            if (!IsEnabled) return;
            if (ReleasePan())
            {
                e.Handled = true;
            }
        }

        private bool ReleasePan()
        {
            if (_container._element == null || _container._captured != true || _container._isPanning != true)
            {
                return false;
            }
            _container._captured = false;
            _container._isPanning = false;
            _container.SetPseudoClass(":isPanning", _container._isPanning);
            return true;
        }
    }
}