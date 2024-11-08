using Pixed.Common.Algos;

namespace Pixed.Tests;
internal class SelectionBorderTests
{
    [Test]
    public void SelectionBorderRectangleTest()
    {
        var minX = 0;
        var maxX = 10;
        var minY = 0;
        var maxY = 10;
        int width = Math.Max(1, (maxX - minX) + 1);
        int height = Math.Max(1, (maxY - minY) + 1);

        bool[,] map = new bool[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = true;
            }
        }

        var border = SelectionBorder.Get(map);

        Assert.Multiple(() =>
        {
            Assert.That(width, Is.EqualTo(11));
            Assert.That(height, Is.EqualTo(11));
            Assert.That(border, Has.Count.EqualTo(4));
            Assert.That(border[0].Item1.Y, Is.EqualTo(0));
            Assert.That(border[0].Item2.Y, Is.EqualTo(11));
            Assert.That(border[1].Item1.Y, Is.EqualTo(0));
            Assert.That(border[1].Item2.Y, Is.EqualTo(11));
            Assert.That(border[1].Item1.X, Is.EqualTo(11));
            Assert.That(border[1].Item2.X, Is.EqualTo(11));
        });
    }

    [Test]
    public void SelectionBorderCircleTest()
    {
        var minX = 0;
        var maxX = 10;
        var minY = 0;
        var maxY = 10;
        int width = Math.Max(1, (maxX - minX) + 1);
        int height = Math.Max(1, (maxY - minY) + 1);

        bool[] map1D = 
            [
            false, false, false, false, true, true, true, false, false, false, false,
            false, false, false, true, true, true, true, true, false, false, false,
            false, false, true, true, true, true, true, true, true, false, false,
            false, true, true, true, true,true, true, true, true, true, false,
            true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true,
            false, true, true, true, true,true, true, true, true, true, false,
            false, false, true, true, true, true, true, true, true, false, false,
            false, false, false, true, true, true, true, true, false, false, false,
            false, false, false, false, true, true, true, false, false, false, false
            ];
        bool[,] map = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = map1D[y * width + x];
            }
        }

        var border = SelectionBorder.Get(map);

        Assert.Multiple(() =>
        {
            Assert.That(width, Is.EqualTo(11));
            Assert.That(height, Is.EqualTo(11));
            Assert.That(border, Has.Count.EqualTo(36));
        });
    }
}