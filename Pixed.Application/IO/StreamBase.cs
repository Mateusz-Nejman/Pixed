using System;
using System.IO;

namespace Pixed.Application.IO;
public abstract class StreamBase(Stream stream) : Stream
{
    public static Type StreamReadImpl = typeof(DefaultStream);
    public static Type StreamWriteImpl = typeof(DefaultStream);
    protected Stream _stream = stream;

    public static StreamBase CreateWrite(Stream stream)
    {
        return (StreamBase)Activator.CreateInstance(StreamWriteImpl, stream);
    }

    public static StreamBase CreateRead(Stream stream)
    {
        return (StreamBase)Activator.CreateInstance(StreamReadImpl, stream);
    }
}