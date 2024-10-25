using Avalonia;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using System;

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
            if (!properties.IsMiddleButtonPressed && (!_container.GestureZoomEnabled && !properties.IsLeftButtonPressed))
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

    private class ZoomGestureRecognizer : GestureRecognizer
    {
        private float _initialDistance;
        private IPointer? _firstContact;
        private Point _firstPoint;
        private IPointer? _secondContact;
        private Point _secondPoint;
        private Point _origin;
        private double _previousAngle;

        public bool IsEnabled { get; set; } = true;

        protected override void PointerCaptureLost(IPointer pointer)
        {
            if (!IsEnabled) return;
            RemoveContact(pointer);
        }

        protected override void PointerMoved(PointerEventArgs e)
        {
            if (!IsEnabled) return;
            if (Target is Visual visual)
            {
                if (_firstContact == e.Pointer)
                {
                    _firstPoint = e.GetPosition(visual);
                }
                else if (_secondContact == e.Pointer)
                {
                    _secondPoint = e.GetPosition(visual);
                }
                else
                {
                    return;
                }

                if (_firstContact != null && _secondContact != null)
                {
                    var distance = GetDistance(_firstPoint, _secondPoint);

                    var scale = distance / _initialDistance;

                    var degree = GetAngleDegreeFromPoints(_firstPoint, _secondPoint);

                    var pinchEventArgs = new PinchEventArgs(scale, _origin, degree, _previousAngle - degree);
                    _previousAngle = degree;
                    Target?.RaiseEvent(pinchEventArgs);
                    e.Handled = pinchEventArgs.Handled;
                    e.PreventGestureRecognition();
                }
            }
        }

        protected override void PointerPressed(PointerPressedEventArgs e)
        {
            if (!IsEnabled) return;
            if (Target is Visual visual && (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen))
            {
                if (_firstContact == null)
                {
                    _firstContact = e.Pointer;
                    _firstPoint = e.GetPosition(visual);

                    return;
                }
                else if (_secondContact == null && _firstContact != e.Pointer)
                {
                    _secondContact = e.Pointer;
                    _secondPoint = e.GetPosition(visual);
                }
                else
                {
                    return;
                }

                if (_firstContact != null && _secondContact != null)
                {
                    _initialDistance = GetDistance(_firstPoint, _secondPoint);

                    _origin = new Point((Math.Max(_firstPoint.X, _secondPoint.X) - Math.Min(_firstPoint.X, _secondPoint.X)) / 2.0, (Math.Max(_firstPoint.Y, _secondPoint.Y) - Math.Min(_firstPoint.Y, _secondPoint.Y)) / 2.0);

                    _previousAngle = GetAngleDegreeFromPoints(_firstPoint, _secondPoint);

                    Capture(_firstContact);
                    Capture(_secondContact);
                    e.PreventGestureRecognition();
                }
            }
        }

        protected override void PointerReleased(PointerReleasedEventArgs e)
        {
            if (!IsEnabled) return;
            if (RemoveContact(e.Pointer))
            {
                e.PreventGestureRecognition();
            }
        }

        private bool RemoveContact(IPointer pointer)
        {
            if (_firstContact == pointer || _secondContact == pointer)
            {
                if (_secondContact == pointer)
                {
                    _secondContact = null;
                }

                if (_firstContact == pointer)
                {
                    _firstContact = _secondContact;

                    _secondContact = null;
                }

                Target?.RaiseEvent(new PinchEndedEventArgs());
                return true;
            }
            return false;
        }

        private static float GetDistance(Point a, Point b)
        {
            var length = b - a;
            return (float)new Vector(length.X, length.Y).Length;
        }

        private static double GetAngleDegreeFromPoints(Point a, Point b)
        {
            // https://stackoverflow.com/a/15994225/20894223

            var deltaX = a.X - b.X;
            var deltaY = -(a.Y - b.Y);                           // I reverse the sign, because on the screen the Y axes
                                                                 // are reversed with respect to the Cartesian plane.
            var rad = System.Math.Atan2(deltaX, deltaY);         // radians from -π to +π
            var degree = ((rad * (180 / System.Math.PI))) + 180; // Atan2 returns a radian value between -π to +π, in degrees -180 to +180.
                                                                 // To get the angle between 0 and 360 degrees you need to add 180 degrees.
            return degree;
        }
    }
}