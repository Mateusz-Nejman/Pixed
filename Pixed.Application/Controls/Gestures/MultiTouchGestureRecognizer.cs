using Avalonia;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Application.Controls.Gestures;
internal abstract class MultiTouchGestureRecognizer(Visual visual) : GestureRecognizer
{
    public readonly struct PointerData(IPointer pointer, PointerPoint pointerPoint, Point position)
    {
        public IPointer Pointer { get; } = pointer;
        public PointerPoint PointerPoint { get; } = pointerPoint;
        public Point Position { get; } = position;
    }

    public abstract class BaseMultiTouchEventArgs(RoutedEvent? routedEvent, PointerData pointer, Action prevent) : RoutedEventArgs(routedEvent)
    {
        private readonly Action _preventGestureRecognition = prevent;
        public PointerData Pointer => pointer;

        public void PreventGestureRecognition()
        {
            _preventGestureRecognition?.Invoke();
        }
    }

    public class PointersMovedEventArgs(RoutedEvent? routedEvent, PointerData pointer, Action prevent) : BaseMultiTouchEventArgs(routedEvent, pointer, prevent)
    {
    }
    public class PointersPressedEventArgs(RoutedEvent? routedEvent, PointerData pointer, Action prevent) : BaseMultiTouchEventArgs(routedEvent, pointer, prevent)
    {
    }
    public class PointersReleasedEventArgs(RoutedEvent? routedEvent, PointerData pointer, Action prevent) : BaseMultiTouchEventArgs(routedEvent, pointer, prevent)
    {
    }

    private readonly Visual _visual = visual;
    private readonly IList<PointerData> _pointers = [];
    private const int FINGER_SIZE = 3;
    public abstract bool MultiTouchEnabled { get; }

    public IReadOnlyList<PointerData> Pointers => _pointers.AsReadOnly();

    public PointerData FirstPointer => _pointers[0];
    public PointerData SecondPointer => _pointers[1];

    protected override void PointerCaptureLost(IPointer pointer)
    {
        PointerData pointerData = _pointers.Where(p => p.Pointer.Id == pointer.Id).FirstOrDefault();
        _pointers.Remove(pointerData);
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (MultiTouchEnabled && _pointers.Count < 2)
        {
            return;
        }
        if (TryGetPointerData(e.Pointer.Id, out var pointerData))
        {
            var position = e.GetPosition(_visual);
            var point = e.GetCurrentPoint(_visual);

            if (!IsInFingerArea(pointerData.Position, position))
            {
                int indexOf = _pointers.IndexOf(pointerData);
                if (indexOf != -1)
                {
                    _pointers[indexOf] = new PointerData(pointerData.Pointer, point, position);
                    PointersMovedEventArgs eventArgs = new(e.RoutedEvent, _pointers[indexOf], e.PreventGestureRecognition);
                    PointersMoved(eventArgs);
                    e.Handled = eventArgs.Handled;
                }
            }
        }
    }

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        if (!MultiTouchEnabled && _pointers.Count == 1)
        {
            return;
        }
        PointerData data = new(e.Pointer, e.GetCurrentPoint(_visual), e.GetPosition(_visual));
        _pointers.Add(data);

        if (MultiTouchEnabled && _pointers.Count > 1)
        {
            PointersPressedEventArgs eventArgs = new(e.RoutedEvent, data, e.PreventGestureRecognition);
            PointersPressed(eventArgs);
            e.Handled = eventArgs.Handled;
        }
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (TryGetPointerData(e.Pointer.Id, out var pointerData))
        {
            _pointers.Remove(pointerData);
            if ((MultiTouchEnabled && _pointers.Count > 0) || !MultiTouchEnabled)
            {
                PointersReleasedEventArgs eventArgs = new(e.RoutedEvent, pointerData, e.PreventGestureRecognition);
                PointersReleased(eventArgs);
                e.Handled = eventArgs.Handled;
            }
        }
    }

    protected abstract void PointersPressed(PointersPressedEventArgs e);
    protected abstract void PointersMoved(PointersMovedEventArgs e);
    protected abstract void PointersReleased(PointersReleasedEventArgs e);

    private PointerData? GetPointerData(int pointerId)
    {
        var pointerData = _pointers.Where(p => p.Pointer.Id == pointerId).FirstOrDefault();

        if (_pointers.Contains(pointerData))
        {
            return pointerData;
        }

        return null;
    }

    private bool TryGetPointerData(int pointerId, out PointerData pointerData)
    {
        var data = GetPointerData(pointerId);

        if (data.HasValue)
        {
            pointerData = data.Value;
            return true;
        }

        pointerData = new PointerData();
        return false;
    }

    private static bool IsInFingerArea(Point fingerPoint, Point newPoint)
    {
        return newPoint.X >= fingerPoint.X - FINGER_SIZE && newPoint.X <= fingerPoint.X + FINGER_SIZE && newPoint.Y >= fingerPoint.Y - FINGER_SIZE && newPoint.Y <= fingerPoint.Y + FINGER_SIZE;
    }
}