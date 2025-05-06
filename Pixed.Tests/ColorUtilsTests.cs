using Pixed.Core;
using Pixed.Core.Utils;

namespace Pixed.Tests;
public class ColorUtilsTests
{
    [Test]
    public void MultiplyAlphaTest ()
    {
        double multiply = 0.25d;
        UniColor color1 = UniColor.CornflowerBlue;
        color1.A = (byte)(multiply * color1.A);

        uint color2 = ColorUtils.MultiplyAlpha(UniColor.CornflowerBlue, multiply);
        Assert.That(color2, Is.EqualTo(color1.ToUInt()));
    }
}