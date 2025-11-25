using Pixed.Core.Models;
using SevenZip;
using System;
using System.IO;
using System.Text;

namespace Pixed.Application.IO;
public sealed class PixedProjectSerializer : IPixedProjectSerializer
{
    #region LZMASettings

    /// <summary>
    /// 2 MB of memory will be reserved for dictionary.
    /// </summary>
    private const int LzmaDictionary = 1 << 21; // 2 MB

    private const int LzmaPosStateBits = 2;
    private const int LzmaLitContextBits = 3;
    private const int LzmaLitPosBits = 0;
    private const int LzmaAlgorithm = 2;

    /// <summary>
    /// Incerease numFastBytes to get better compression ratios.
    /// 32 - moderate compression
    /// 128 - extreme compression
    /// </summary>
    private const int LzmaNumFastBytes = 32;

    private const bool LzmaEos = false;

    private static readonly CoderPropID[] LzmaPropIds =
    [
        CoderPropID.DictionarySize,
        CoderPropID.PosStateBits,
        CoderPropID.LitContextBits,
        CoderPropID.LitPosBits,
        CoderPropID.Algorithm,
        CoderPropID.NumFastBytes,
        CoderPropID.MatchFinder,
        CoderPropID.EndMarker
    ];

    private static readonly object[] LzmaProperties =
    [
        LzmaDictionary,
        LzmaPosStateBits,
        LzmaLitContextBits,
        LzmaLitPosBits,
        LzmaAlgorithm,
        LzmaNumFastBytes,
        "bt4",
        LzmaEos
    ];

    #endregion

    public bool CanSerialize => true;
    public bool CanDeserialize => true;

    public string FormatExtension => ".pixed";

    public string FormatName => "Pixed project";

    public void Serialize(Stream stream, PixedModel model, bool close = false)
    {
        MemoryStream memoryStream = new();
        model.Serialize(memoryStream);
        memoryStream.Position = 0;
        Compress(memoryStream, stream);
        memoryStream.Dispose();
        if (close)
        {
            stream.Dispose();
        }
    }
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        PixedModel model = new(applicationData);
        MemoryStream memoryStream = new();
        Decompress(stream, memoryStream);
        memoryStream.Position = 0;
        model.Deserialize(memoryStream);
        memoryStream.Dispose();
        return model;
    }

    public static void Compress(Stream inStream, Stream outStream)
    {
        SevenZip.Compression.LZMA.Encoder encoder = new();

        encoder.SetCoderProperties(LzmaPropIds, LzmaProperties);
        encoder.WriteCoderProperties(outStream);

        var writer = new BinaryWriter(outStream, Encoding.UTF8);
        // Write original size.
        writer.Write(inStream.Length - inStream.Position);

        // Save position with compressed size.
        long positionForCompressedSize = outStream.Position;
        // Leave placeholder for size after compression.
        writer.Write((Int64)0);

        long positionForCompressedDataStart = outStream.Position;
        encoder.Code(inStream, outStream, -1, -1, null);
        long positionAfterCompression = outStream.Position;

        // Seek back to the placeholder for compressed size.
        outStream.Position = positionForCompressedSize;

        // Write size after compression.
        writer.Write(positionAfterCompression - positionForCompressedDataStart);

        // Restore position.
        outStream.Position = positionAfterCompression;
    }

    /// <summary>
    /// Decompress LZMA compressed stream into outStream.
    /// Remeber to set desirable positions in input and output streams.
    /// </summary>
    /// <param name="inStream">Input stream</param>
    /// <param name="outStream">Output stream</param>
    public static void Decompress(Stream inStream, Stream outStream)
    {
        byte[] properties = new byte[5];
        if (inStream.Read(properties, 0, 5) != 5)
            throw (new Exception("Input stream is too short."));

        SevenZip.Compression.LZMA.Decoder decoder = new();
        decoder.SetDecoderProperties(properties);

        var br = new BinaryReader(inStream, Encoding.UTF8);
        long decompressedSize = br.ReadInt64();
        long compressedSize = br.ReadInt64();
        decoder.Code(inStream, outStream, compressedSize, decompressedSize, null);
    }
}
