namespace Pixed.BigGustave.Tests;

public class Adler32ChecksumTests
{
    [Test]
    public void CalculatesCorrectChecksum()
    {
        var data = new byte[]
        {
                0,
                255, 0, 0,
                0, 0, 0,
                0,
                0, 0, 0,
                255, 0, 0
        };

        var checksum = Adler32Checksum.Calculate(data);
        Assert.That(checksum, Is.EqualTo(268304895));
    }

    [Test]
    public void CalculatesCorrectChecksumWithLengthArgument()
    {
        var data = new byte[]
        {
                0,
                255, 0, 0,
                0, 0, 0,
                0,
                0, 0, 0,
                255, 0, 0,
                44, 12, 126, 200
        };

        var checksum = Adler32Checksum.Calculate(data, 14);

        Assert.That(checksum, Is.EqualTo(268304895));
    }
}