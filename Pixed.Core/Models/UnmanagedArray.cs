using System.Runtime.InteropServices;

namespace Pixed.Core.Models;

public unsafe class UnmanagedArray(int length) : IDisposable
{
    private readonly byte* _ptr = (byte*)Marshal.AllocHGlobal(length);
    private readonly int _length = length;
    private bool _disposedValue;

    public byte this[int i]
    {
        get { return *(_ptr + i); }
        set { *(_ptr + i) = value; }
    }

    public long Length => _disposedValue ? 0 : _length;
    public IntPtr Ptr => (IntPtr) _ptr;

    ~UnmanagedArray()
    {
        Dispose(disposing: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }

            Marshal.FreeHGlobal((IntPtr)_ptr);
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}