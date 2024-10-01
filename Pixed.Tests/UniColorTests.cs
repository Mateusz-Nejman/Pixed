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
        UniColor expectedColor = new(145, 180, 242);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void DarkenTest()
    {
        var newColor = CornflowerBlue.Darken(10);
        UniColor expectedColor = new(54, 117, 231);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void BlendTest()
    {
        UniColor color = CornflowerBlue;
        var newColor = color.Blend(MagentaColor, 50);
        UniColor expectedColor = new(185, 26, 123);
        Assert.That(newColor, Is.EqualTo(expectedColor));
    }
}