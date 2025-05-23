﻿using Avalonia;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Pixed.Application.Controls.Gestures;

namespace Pixed.Application.Zoom.Internals;

internal partial class BaseControl
{
    private class PanGestureRecognizer(BaseControl container) : GestureRecognizer
    {
        private readonly BaseControl _container = container;

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
            _container.ContinuePanTo(point, true);
            e.Handled = true;
        }

        protected override void PointerPressed(PointerPressedEventArgs e)
        {
            if (!IsEnabled) return;
            var properties = e.GetCurrentPoint(_container).Properties;

            if (_container.GestureZoomEnabled)
            {
                if (!properties.IsLeftButtonPressed)
                {
                    return;
                }
            }
            else if (!properties.IsMiddleButtonPressed)
            {
                return;
            }

            if (_container._element != null && _container._captured == false && _container._isPanning == false)
            {
                var point = e.GetPosition(_container._element);
                _container.BeginPanTo(point);
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

    public class PinchGestureRecognizer(ZoomControl zoomControl) : MultiTouchGestureRecognizer
    {
        private readonly ZoomControl _zoomControl = zoomControl;
        private double _initialDistance;
        private Point _origin;
        private double _previousAngle;
        private Matrix _initialMatrix;

        public bool IsEnabled { get; set; } = true;
        public override bool MultiTouchEnabled => true;

        protected override void PointersPressed(PointersPressedEventArgs e)
        {
            if (!IsEnabled) return;
            _initialMatrix = _zoomControl.ZoomMatrix;
            _initialDistance = GetDistance(FirstPointer.Position, SecondPointer.Position);

            _origin = new Point(FirstPointer.Position.X + ((SecondPointer.Position.X - FirstPointer.Position.X) / 2),
                FirstPointer.Position.Y + ((SecondPointer.Position.X - FirstPointer.Position.Y) / 2));

            _previousAngle = GetAngleDegreeFromPoints(FirstPointer.Position, SecondPointer.Position);

            Capture(FirstPointer.Pointer);
            Capture(SecondPointer.Pointer);
            e.PreventGestureRecognition();
        }

        protected override void PointersMoved(PointersMovedEventArgs e)
        {
            if (!IsEnabled) return;
            var distance = GetDistance(FirstPointer.Position, SecondPointer.Position);
            var scale = distance / _initialDistance;
            var degree = GetAngleDegreeFromPoints(FirstPointer.Position, SecondPointer.Position);
            var newZoom = _initialMatrix.M11 * scale;
            var newOffsetX = _initialMatrix.M31 * scale;
            var newOffsetY = _initialMatrix.M32 * scale;
            var pinchEventArgs = new PinchEventArgs(newZoom, new Point(newOffsetX, newOffsetY), degree, _previousAngle - degree);
            _previousAngle = degree;
            Target?.RaiseEvent(pinchEventArgs);
            e.Handled = pinchEventArgs.Handled;
            e.PreventGestureRecognition();
        }

        protected override void PointersReleased(PointersReleasedEventArgs e)
        {
            if (!IsEnabled) return;
            Target?.RaiseEvent(new PinchEndedEventArgs());
            e.PreventGestureRecognition();
        }

        private static double GetDistance(Point a, Point b)
        {
            var length = b - a;
            return new Vector(length.X, length.Y).Length;
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