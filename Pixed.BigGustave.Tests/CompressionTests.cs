namespace Pixed.BigGustave.Tests;

public class CompressionTests
{
    private static string GetImageFile(string fileName) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", fileName);

    private static readonly PngBuilder.SaveOptions SaveCompressed = new()
    {
        AttemptCompression = true
    };

    [Test]
    public void CanCompressImageWithAlphaChannel()
    {
        var file = GetImageFile("pdfpig.png");
        var png = Png.Open(file);

        var builder = PngBuilder.Create(png.Width, png.Height, png.HasAlphaChannel);

        CopyPngToBuilder(png, builder);

        var sizeRaw = builder.Save().Length;

        var compressed = builder.Save(SaveCompressed);
        Assert.That(compressed, Has.Length.LessThan(sizeRaw), $"Compressed size {compressed.Length} bytes was not smaller than raw size {sizeRaw}.");
    }

    [Test]
    public void CanCompressImageUsingPalette()
    {
        var rawBytes = File.ReadAllBytes(GetImageFile("10by9pixelsrgb8bpp.png"));
        var png = Png.Open(rawBytes);
        var builder = PngBuilder.FromPng(png);

        var compressed = builder.Save(SaveCompressed);

        Assert.That(compressed, Has.Length.LessThan(rawBytes.Length), $"Compressed size {compressed.Length} bytes was not smaller than raw size {rawBytes.Length}.");
    }

    private static void CopyPngToBuilder(Png png, PngBuilder builder)
    {
        for (int y = 0; y < png.Height; y++)
        {
            for (int x = 0; x < png.Width; x++)
            {
                var pixel = png.GetPixel(x, y);
                builder.SetPixel(pixel, x, y);
            }
        }
    }
}