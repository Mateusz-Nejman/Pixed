// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace BigGustave.Tests
{
    using Pixed.BigGustave;
    using Pixed.Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class PngTests
    {
        private static readonly UniColor Tr = new UniColor(0, 0, 0, 0);

        [Fact]
        public void FourByFourGrayscale()
        {
            var values = new List<byte[]>
            {
                new byte[] {176, 255, 255, 176},
                new byte[] {255, 0, 0, 255},
                new byte[] {133, 255, 255, 133},
                new byte[] {255, 176, 176, 255}
            };

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "4by4grayscale.png");

            using (var stream = File.OpenRead(path))
            {
                var img = Png.Open(stream);

                for (var row = 0; row < values.Count; row++)
                {
                    var expectedRow = values[row];

                    for (var col = 0; col < expectedRow.Length; col++)
                    {
                        var pixel = img.GetPixel(col, row);

                        Assert.Equal(values[row][col], pixel.R);
                        Assert.True(pixel.IsGrayscale);
                    }
                }

                Assert.Equal(4, img.Width);
                Assert.Equal(4, img.Height);
                Assert.False(img.HasAlphaChannel);
            }
        }

        [Fact]
        public void TenByTenRgbAWithAdam7()
        {
            var yellow = P(255, 255, 0);
            var green = P(0, 255, 6);
            var pink = P(255, 75, 240);
            var red = P(255, 0, 0);
            var blue = P(31, 57, 255);

            var values = new List<UniColor[]>
            {
                new [] { Tr, Tr, yellow, yellow, yellow, yellow, yellow, yellow, yellow, yellow  },
                new [] { Tr, Tr, yellow, green, green, green, green, pink, yellow, yellow  },
                new [] { Tr, Tr, yellow, yellow, yellow, pink, red, pink, yellow, yellow  },
                new [] { Tr, Tr, yellow, yellow, yellow, pink, yellow, pink, yellow, yellow  },
                new [] { Tr, Tr, yellow, blue, pink, pink, red, pink, yellow, yellow  },
                new [] { Tr, Tr, yellow, blue, green, green, green, green, pink, yellow  },
                new [] { Tr, Tr, yellow, blue, pink, yellow, yellow, yellow, yellow, yellow  },
                new [] { Tr, Tr, yellow, yellow, yellow, yellow, yellow, yellow, yellow, yellow  },
                new [] { Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr  },
                new [] { Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr, Tr  }
            };

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "10by10rgbaadam7.png");

            using (var stream = File.OpenRead(path))
            {
                var img = Png.Open(stream);

                Assert.Equal(10, img.Width);
                Assert.Equal(10, img.Height);

                Assert.True(img.HasAlphaChannel);

                for (var row = 0; row < values.Count; row++)
                {
                    var expectedRow = values[row];

                    for (var col = 0; col < expectedRow.Length; col++)
                    {
                        var pixel = img.GetPixel(col, row);

                        Assert.True(pixel.Equals(expectedRow[col]), $"Expected {expectedRow[col]} but got {pixel}.");
                    }
                }
            }
        }

        [Fact]
        public void TwelveBySevenRgbWithAdam7()
        {
            var green = P(0, 201, 52);
            var yellowGreen = P(81, 184, 50);
            var darkGreen = P(12, 140, 45);
            var nostrilGreen = P(54, 71, 52);
            var red = P(255, 37, 37);
            var blue = P(37, 83, 255);
            var white = P(255, 255, 255);
            var black = P(0, 0, 0);

            var values = new List<UniColor[]>
            {
                new [] { blue, blue, blue, blue, blue, blue, blue, blue, red, red, red, red },
                new [] { blue, blue, blue, yellowGreen, green, green, blue, blue, red, red, red, red },
                new [] { blue, blue, green, white, yellowGreen, white, green, blue, blue, blue, blue, blue },
                new [] { blue, blue, green, black, green, black, green, green, blue, blue, blue, blue },
                new [] { blue, blue, green, green, yellowGreen, green, yellowGreen, green, green, green, blue, blue },
                new [] { blue, blue, darkGreen, darkGreen, darkGreen, green, green, nostrilGreen, green, nostrilGreen, blue, blue },
                new [] { blue, blue, blue, blue, darkGreen, darkGreen, yellowGreen, green, green, green, blue, blue }
            };

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "12by7rgbadam7.png");

            using (var stream = File.OpenRead(path))
            {
                var img = Png.Open(stream);

                Assert.Equal(12, img.Width);
                Assert.Equal(7, img.Height);

                Assert.False(img.HasAlphaChannel);

                for (var row = 0; row < values.Count; row++)
                {
                    var expectedRow = values[row];

                    for (var col = 0; col < expectedRow.Length; col++)
                    {
                        var pixel = img.GetPixel(col, row);

                        Assert.True(pixel.Equals(expectedRow[col]), $"Expected {expectedRow[col]} but got {pixel}.");
                    }
                }
            }
        }

        [Fact]
        public void LargerImage()
        {
            CheckFile("piggies", 676, 573, true);
        }

        [Fact]
        public void PdfPigLogo()
        {
            CheckFile("pdfpig", 294, 294, true);
        }

        [Fact]
        public void FinnishOpinionPollGraph()
        {
            // https://commons.wikimedia.org/wiki/File:Finnish_Opinion_Polling,_30_Day_Moving_Average,_2015-2019.png
            CheckFile("finnish-opinion-polling", 1181, 500, true);
        }

        [Fact]
        public void TwoFiveSixByTwoFiveSixPalette()
        {
            CheckFile("256by2568bppwithplte", 256, 256, false);
        }

        [Fact]
        public void SevenTwentyByFiveSixtySixteenBitPerChannelRgb()
        {
            CheckFile("720by560spookycave", 720, 560, false);
        }

        [Fact]
        public void SixteenBySixteenGrayAlphaSixteenBitPerChannel()
        {
            CheckFile("16by16graya16bit", 16, 16, true, true);
        }

        [Fact]
        public void SixteenBySixteenGraySixteenBitPerChannel()
        {
            CheckFile("16by16gray16bit", 16, 16, false, true);
        }

        [Fact]
        public void TwelveByTwentyFourRgbaSixteenBitPerChannel()
        {
            CheckFile("12by24rgba16bit", 12, 24, true);
        }

        [Fact]
        public void TenByNinePixelsWithPaletteSameAsNonPalette()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"10by9pixelscompressedrgb8bpp.png");
            var pathRaw = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"10by9pixelsrgb8bpp.png");

            var png = Png.Open(File.ReadAllBytes(path));
            var pngRaw = Png.Open(File.ReadAllBytes(pathRaw));

            for (int y = 0; y < png.Height; y++)
            {
                for (int x = 0; x < png.Width; x++)
                {
                    var pix = png.GetPixel(x, y);
                    var pixRaw = pngRaw.GetPixel(x, y);

                    Assert.Equal(pix, pixRaw);
                }
            }
        }

        [Fact]
        public void TenByNinePixelsWithPaletteAdditionalColorsSameAsNonPalette()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"10by9pixelscompressedrgbadditionalcolors.png");
            var pathRaw = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"10by9pixelsrgbadditionalcolors.png");

            var png = Png.Open(File.ReadAllBytes(path));
            var pngRaw = Png.Open(File.ReadAllBytes(pathRaw));

            for (int y = 0; y < png.Height; y++)
            {
                for (int x = 0; x < png.Width; x++)
                {
                    var pix = png.GetPixel(x, y);
                    var pixRaw = pngRaw.GetPixel(x, y);

                    Assert.Equal(pix, pixRaw);
                }
            }
        }

        private static void CheckFile(string imageName, int width, int height, bool hasAlpha, bool grayscale = false)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"{imageName}.png");

            using (var stream = File.OpenRead(path))
            {
                var img = Png.Open(stream);

                Assert.Equal(width, img.Width);
                Assert.Equal(height, img.Height);

                Assert.Equal(hasAlpha, img.HasAlphaChannel);

                var data = GetImageData(imageName, grayscale);

                foreach (var (x, y, pixel) in data)
                {
                    Assert.Equal(pixel, img.GetPixel(x, y));
                }
            }
        }

        private static UniColor P(byte r, byte g, byte b)
        {
            return new UniColor(255, r, g, b);
        }

        private static IEnumerable<(int x, int y, UniColor pixel)> GetImageData(string filename, bool grayscale)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", $"{filename}.txt");

            foreach (var line in File.ReadLines(file))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var x = int.Parse(parts[0]);
                var y = int.Parse(parts[1]);

                var r = byte.Parse(parts[2]);
                var g = byte.Parse(parts[3]);
                var b = byte.Parse(parts[4]);
                var a = byte.Parse(parts[5]);

                yield return (x, y, new UniColor(a, r, g, b));
            }
        }
    }
}
