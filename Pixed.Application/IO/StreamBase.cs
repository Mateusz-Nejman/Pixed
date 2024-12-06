using System;
using System.IO;

namespace Pixed.Application.IO;
public abstract class StreamBase(Stream stream) : Stream
{
    public static Type StreamBaseImpl = typeof(DefaultStream);
    protected Stream _stream = stream;

    public static StreamBase Create(Stream stream)
    {
        return (StreamBase)Activator.CreateInstance(StreamBaseImpl, stream);
    }
}