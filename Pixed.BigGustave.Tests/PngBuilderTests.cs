using Pixed.Core;

namespace Pixed.BigGustave.Tests;

public class PngBuilderTests
{
    [Test]
    public void SimpleCheckerboard()
    {
        var builder = PngBuilder.Create(2, 2, false);

        var red = new UniColor(255, 0, 12);
        var black = new UniColor(255, 0, 0, 0);

        builder.SetPixel(new UniColor(255, 0, 12), 0, 0);
        builder.SetPixel(255, 0, 12, 1, 1);

        using var memory = new MemoryStream();
        builder.Save(memory);

        memory.Seek(0, SeekOrigin.Begin);

        var read = Png.Open(memory);

        var left = read.GetPixel(0, 0);
        Assert.That(left, Is.EqualTo(red));
        var right = read.GetPixel(1, 0);
        Assert.That(right, Is.EqualTo(black));
        var bottomLeft = read.GetPixel(0, 1);
        Assert.That(bottomLeft, Is.EqualTo(black));
        var bottomRight = read.GetPixel(1, 1);
        Assert.That(bottomRight, Is.EqualTo(red));
    }

    [Test]
    public void SimpleCheckerboardWithArbitraryTextData()
    {
        var builder = PngBuilder.Create(2, 2, false);

        builder.SetPixel(new UniColor(255, 0, 12), 0, 0);
        builder.SetPixel(255, 0, 12, 1, 1);

        builder.StoreText("Title", "Checkerboard");
        builder.StoreText("another-data", "bərd that's good and other\r\nstuff");

        using var memory = new MemoryStream();
        builder.Save(memory);

        memory.Seek(0, SeekOrigin.Begin);

        var visitor = new MyChunkVisitor();

        var read = Png.Open(memory, visitor);

        Assert.That(read, Is.Not.Null);

        var textChunks = visitor.Visited.Where(x => x.header.Name == "iTXt").ToList();
        Assert.That(textChunks, Has.Count.EqualTo(2));
    }

    [Test]
    public void BiggerImage()
    {
        var builder = PngBuilder.Create(10, 10, false);

        var green = new UniColor(255, 0, 255, 25);
        var color1 = new UniColor(255, 60, 201, 32);
        var color2 = new UniColor(255, 100, 5, 250);

        builder.SetPixel(green, 1, 1).SetPixel(green, 2, 1).SetPixel(green, 3, 1).SetPixel(green, 4, 1).SetPixel(green, 5, 1);

        builder.SetPixel(color1, 5, 7).SetPixel(color1, 5, 8)
            .SetPixel(color1, 6, 7).SetPixel(color1, 6, 8)
            .SetPixel(color1, 7, 7).SetPixel(color1, 7, 8);

        builder.SetPixel(color2, 9, 9).SetPixel(color2, 8, 8);

        using var memoryStream = new MemoryStream();
        builder.Save(memoryStream);
    }

    private class MyChunkVisitor : IChunkVisitor
    {
        private readonly List<(ChunkHeader header, byte[] data)> visited = [];
        public IReadOnlyList<(ChunkHeader header, byte[] data)> Visited => visited;

        public void Visit(Stream stream, ImageHeader header, ChunkHeader chunkHeader, byte[] data, byte[] crc)
        {
            visited.Add((chunkHeader, data));
        }
    }
}