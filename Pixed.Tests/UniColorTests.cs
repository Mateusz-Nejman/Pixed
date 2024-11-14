using Pixed.Core;

namespace Pixed.Tests;
public class UniColorTests
{
    private readonly UniColor CornflowerBlue = UniColor.CornflowerBlue;
    private readonly UniColor MagentaColor = new(255, 0, 255);
    [Test]
    public void ARGBConversionTest()
    {
        int argb = CornflowerBlue;
        Assert.That(argb, Is.EqualTo(-10185235));
    }

    [Test]
    public void FromARGBConversionTest()
    {
        int argb = -10185235;
        UniColor color = argb;
        Assert.That(color, Is.EqualTo(CornflowerBlue));
    }

    [Test]
    public void LightenTest()
    {
        var newColor = CornflowerBlue.Lighten(10);
        UniColor expectedColor = new(146, 180, 242);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void DarkenTest()
    {
        var newColor = CornflowerBlue.Darken(10);
        UniColor expectedColor = new(54, 118, 232);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void BlendTest()
    {
        var newColor = CornflowerBlue.Blend(MagentaColor, 50);
        UniColor expectedColor = new(185, 26, 123);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void HslConversionTest()
    {
        var hsl = CornflowerBlue.ToHsl();
        Assert.That(hsl, Is.EqualTo(new UniColor.Hsl(218.54015313795884, 0.7919075144508672, 0.6607843137254902)));
    }

    [Test]
    public void HslToRGBConversionTest()
    {
        UniColor.Hsl hsl = new(218.540150538969, 0.7919075255636313, 0.6607843158291835);
        UniColor newColor = new(hsl);
        Assert.That(newColor, Is.EqualTo(CornflowerBlue));
    }

    [Test]
    public void ArgbConversionTestSkia()
    {
        UniColor color = new(0xFF00FF00);
        Assert.That(color.ToUInt(), Is.EqualTo(0xFF00FF00));
    }
}