using Pixed.Models;
using SevenZip;
using System;
using System.IO;
using System.Text;

namespace Pixed.IO
{
    internal sealed class PixedProjectSerializer : IPixedProjectSerializer
    {
        #region LZMASettings

        /// <summary>
        /// 2 MB of memory will be reserved for dictionary.
        /// </summary>
        readonly static Int32 dictionary = 1 << 21; // 2 MB

        readonly static Int32 posStateBits = 2;
        readonly static Int32 litContextBits = 3;
        readonly static Int32 litPosBits = 0;
        readonly static Int32 algorithm = 2;

        /// <summary>
        /// Incerease numFastBytes to get better compression ratios.
        /// 32 - moderate compression
        /// 128 - extreme compression
        /// </summary>
        readonly static Int32 numFastBytes = 32;

        readonly static bool eos = false;

        readonly static CoderPropID[] _propIds =
        {
            CoderPropID.DictionarySize,
            CoderPropID.PosStateBits,
            CoderPropID.LitContextBits,
            CoderPropID.LitPosBits,
            CoderPropID.Algorithm,
            CoderPropID.NumFastBytes,
            CoderPropID.MatchFinder,
            CoderPropID.EndMarker
        };

        readonly static object[] _properties =
        {
            (Int32)(dictionary),
            (Int32)(posStateBits),
            (Int32)(litContextBits),
            (Int32)(litPosBits),
            (Int32)(algorithm),
            (Int32)(numFastBytes),
            "bt4",
            eos
        };

        #endregion

        public void Serialize(Stream stream, PixedModel model, bool close = false)
        {
            MemoryStream memoryStream = new();
            model.Serialize(memoryStream);

            Compress(memoryStream, stream);
            memoryStream.Close();

            if (close)
            {
                stream.Dispose();
            }
        }
        public PixedModel Deserialize(Stream stream)
        {
            PixedModel model = new();
            MemoryStream memoryStream = new MemoryStream();
            Decompress(stream, memoryStream);
            model.Deserialize(memoryStream);
            memoryStream.Close();

            return model;
        }

        public static void Compress(Stream inStream, Stream outStream)
        {
            SevenZip.Compression.LZMA.Encoder encoder = new();

            encoder.SetCoderProperties(_propIds, _properties);
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
            writer.Write((Int64)(positionAfterCompression - positionForCompressedDataStart));

            // Restore position.
            outStream.Position = positionAfterCompression;
        }

        /// <summary>
        /// Decompress LZMA compressed stream into outStream.
        /// Remeber to set desirable positions in input and output streams.
        /// </summary>
        /// <param name="inStream">Input stream</param>
        /// <param name="outStream">Output stream</param>
        private static void Decompress(Stream inStream, Stream outStream)
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
}
