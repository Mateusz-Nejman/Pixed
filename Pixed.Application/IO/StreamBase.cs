using System;
using System.IO;

namespace Pixed.Application.IO;
public abstract class StreamBase(Stream stream) : Stream
{
    public static Type StreamReadImpl = typeof(DefaultStream);
    public static Type StreamWriteImpl = typeof(DefaultStream);
    protected Stream Stream = stream;

    public static StreamBase? CreateWrite(Stream? stream)
    {
        if (stream == null)
        {
            return null;
        }

        return (StreamBase?)Activator.CreateInstance(StreamWriteImpl, stream);
    }

    public static StreamBase? CreateRead(Stream? stream)
    {
        if (stream == null)
        {
            return null;
        }
        
        return (StreamBase?)Activator.CreateInstance(StreamReadImpl, stream);
    }
}