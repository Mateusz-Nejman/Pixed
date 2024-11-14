using Pixed.Core.Models;

namespace Pixed.Core.Utils;
public static class CollectionUtils
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

    public static void ForEach<T>(this T[,] array, Point start, Point size, Action<int, int, T> action)
    {
        for (int x = start.X; x < size.X; x++)
        {
            for (int y = start.Y; y < size.Y; y++)
            {
                action?.Invoke(x, y, array[x, y]);
            }
        }
    }

    public static void ForEach<T>(this T[,] array, Action<int, int, T> action)
    {
        array.ForEach(new Point(), new Point(array.GetLength(0), array.GetLength(1)), action);
    }
}
