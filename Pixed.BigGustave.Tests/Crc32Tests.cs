using System.Text;

namespace Pixed.BigGustave.Tests;

public class Crc32Tests
{
    [Test]
    public void CalculatesCorrectCrc32ForRosettaCodeExample()
    {
        var input = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");

        var result = Crc32.Calculate(input);
        Assert.That(result.ToString("X"), Is.EqualTo("414FA339"));
    }

    [Test]
    public void ByteValueCorrect()
    {
        const uint expected = 2711477844;

        var input = new byte[] { 0, 0, 177, 143 };

        Assert.That(Crc32.Calculate(input), Is.EqualTo(expected));
    }

    [Test]
    public void FromTwoPartsCorrect()
    {
        const uint expected = 2711477844;

        var input1 = new byte[] { 0, 0 };
        var input2 = new byte[] { 177, 143 };

        Assert.That(Crc32.Calculate(input1, input2), Is.EqualTo(expected));
    }

    [Test]
    public void SingleByteValueCorrect()
    {
        const uint expected = 3523407757;

        var input = new byte[] { 0 };
        Assert.That(Crc32.Calculate(input), Is.EqualTo(expected));
    }
}