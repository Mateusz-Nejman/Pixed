using System.Collections.Concurrent;

namespace Pixed.Core.Utils;
public static class CollectionUtils
{
    public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if(predicate(item)) return true;
        }

        return false;
    }
    public static T Pop<T>(this IList<T> list)
    {
        T last = list.Last();
        list.Remove(last);
        return last;
    }

    public static void AddRange<T>(this ConcurrentBag<T> collection , IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            collection.Add(value);
        }
    }
}
