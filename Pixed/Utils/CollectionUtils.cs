using System.Collections.Generic;
using System.Linq;

namespace Pixed.Utils;
internal static class CollectionUtils
{
    public static T Pop<T>(this IList<T> list)
    {
        T last = list.Last();
        list.Remove(last);
        return last;
    }

    public static void AddRange<T>(this IList<T> collection, IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            collection.Add(value);
        }
    }
}
