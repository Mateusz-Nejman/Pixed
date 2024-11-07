namespace BigGustave.Tests
{
    using Pixed.BigGustave;
    using Xunit;

    public class Adler32ChecksumTests
    {
        [Fact]
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

            Assert.Equal(268304895, checksum);
        }

        [Fact]
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

            Assert.Equal(268304895, checksum);
        }
    }
}
