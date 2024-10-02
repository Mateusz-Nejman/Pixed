using Pixed.Utils;

namespace Pixed.Tests
{
    internal class CollectionUtilsTest
    {
        [Test]
        public void PopTest()
        {
            List<string> list = [];

            for (int a = 0; a < 5; a++)
            {
                list.Add("Element " + a);
            }

            Assert.That(list.Pop(), Is.EqualTo("Element 4"));
        }

        [Test]
        public void AddRangeTest()
        {
            List<string> list1 = [];

            for (int a = 0; a < 5; a++)
            {
                list1.Add("Element " + a);
            }

            List<string> list2 = [];

            for (int a = 0; a < 5; a++)
            {
                list2.Add("New Element " + a);
            }

            list1.AddRange(list2);
            Assert.That(list1[^1], Is.EqualTo("New Element 4"));
        }
    }
}
