using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Common.Algos;

internal class FloodFill(Point startPosition, Point size) : IDisposable
{
    private bool _disposedValue;
    private readonly Queue<Point> _queue = [];
    private readonly int _startX = startPosition.X;
    private readonly int _startY = startPosition.Y;
    private readonly int _sizeX = size.X - 1;
    private readonly int _sizeY = size.Y - 1;

    public void Execute(Func<int, int, bool> validator, Action<int, int> action)
    {
        int[,] points = new int[_sizeX + 1, _sizeY + 1];
        for (int x = 0; x <= _sizeX; x++)
        {
            for (int y = 0; y <= _sizeY; y++)
            {
                points[x, y] = int.MaxValue;
            }
        }

        ProcessNeighbourIfValid(_startX, _startY, 0, ref points, validator, action);

        while (_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            int value = 1 + points[current.X, current.Y];

            ProcessNeighbourIfValid(current.X, current.Y - 1, value, ref points, validator, action);
            ProcessNeighbourIfValid(current.X, current.Y + 1, value, ref points, validator, action);
            ProcessNeighbourIfValid(current.X - 1, current.Y, value, ref points, validator, action);
            ProcessNeighbourIfValid(current.X + 1, current.Y, value, ref points, validator, action);
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

    private void ProcessNeighbourIfValid(int neighbourX, int neighbourY, int value, ref int[,] points, Func<int, int, bool> validator, Action<int, int> action)
    {
        if (neighbourX >= 0 && neighbourX <= _sizeX && neighbourY >= 0 && neighbourY <= _sizeY
               && points[neighbourX, neighbourY] == int.MaxValue
               && validator(neighbourX, neighbourY))
        {
            points[neighbourX, neighbourY] = value;
            action.Invoke(neighbourX, neighbourY);
            _queue.Enqueue(new Point(neighbourX, neighbourY));
        }
    }
}