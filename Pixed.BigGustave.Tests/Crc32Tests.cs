namespace BigGustave.Tests
{
    using Pixed.BigGustave;
    using System;
    using System.Text;
    using Xunit;

    public class Crc32Tests
    {
        [Fact]
        public void CalculatesCorrectCrc32ForRosettaCodeExample()
        {
            var input = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");

            var result = Crc32.Calculate(input);

            Assert.Equal("414fa339", result.ToString("X"), StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void ByteValueCorrect()
        {
            const uint expected = 2711477844;

            var input = new byte[] { 0, 0, 177, 143 };

            Assert.Equal(expected, Crc32.Calculate(input));
        }

        [Fact]
        public void FromTwoPartsCorrect()
        {
            const uint expected = 2711477844;

            var input1 = new byte[] { 0, 0 };
            var input2 = new byte[] { 177, 143 };

            Assert.Equal(expected, Crc32.Calculate(input1, input2));
        }

        [Fact]
        public void SingleByteValueCorrect()
        {
            const uint expected = 3523407757;

            var input = new byte[] { 0 };

            Assert.Equal(expected, Crc32.Calculate(input));
        }
    }
}
