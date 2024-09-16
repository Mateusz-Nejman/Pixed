using System.Collections.Generic;
using System.Linq;

namespace Pixed.Utils
{
    internal static class CollectionUtils
    {
        public static T Pop<T>(this IList<T> list)
        {
            T last = list.Last();
            list.Remove(last);
            return last;
        }
    }
}
