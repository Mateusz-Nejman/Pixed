using Pixed.Common.Models;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Algorithms;

internal class FloodFill(Point startPosition, Point size) : IDisposable
{
    private bool _disposedValue;
    private readonly Queue<Point> _queue = [];
    private readonly Point _startPosition = startPosition;
    private readonly Point _size = size - new Point(1);

    public void Execute(Func<Point, bool> validator, Action<Point> action)
    {
        int[,] points = new int[_size.X + 1, _size.Y + 1];
        for (int x = 0; x <= _size.X; x++)
        {
            for (int y = 0; y <= _size.Y; y++)
            {
                points[x, y] = int.MaxValue;
            }
        }

        ProcessNeighbourIfValid(_startPosition, 0, ref points, validator, action);

        while (_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            int value = 1 + points[current.X, current.Y];

            ProcessNeighbourIfValid(new Point(current.X, current.Y - 1), value, ref points, validator, action);
            ProcessNeighbourIfValid(new Point(current.X, current.Y + 1), value, ref points, validator, action);
            ProcessNeighbourIfValid(new Point(current.X - 1, current.Y), value, ref points, validator, action);
            ProcessNeighbourIfValid(new Point(current.X + 1, current.Y), value, ref points, validator, action);
        }

        points = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _queue.Clear();
            }

            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void ProcessNeighbourIfValid(Point neighbourPoint, int value, ref int[,] points, Func<Point, bool> validator, Action<Point> action)
    {
        if (neighbourPoint.X >= 0 && neighbourPoint.X <= _size.X && neighbourPoint.Y >= 0 && neighbourPoint.Y <= _size.Y
               && points[neighbourPoint.X, neighbourPoint.Y] == int.MaxValue
               && validator(neighbourPoint))
        {
            points[neighbourPoint.X, neighbourPoint.Y] = value;
            action.Invoke(neighbourPoint);
            _queue.Enqueue(neighbourPoint);
        }
    }
}