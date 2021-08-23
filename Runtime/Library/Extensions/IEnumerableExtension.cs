using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HRYooba.Library
{
    public static class IEnumerableExtension
    {
        public static string ToResult<T>(this IEnumerable<T> source)
        {
            return "{" + string.Join(", ", source) + "}";
        }

        public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public class IndexedItem<T>
        {
            public T Element { get; private set; }
            public int Index { get; private set; }
            public IndexedItem(T element, int index)
            {
                Element = element;
                Index = index;
            }
        }

        public static IEnumerable<IndexedItem<T>> Indexed<T>(this IEnumerable<T> source)
        {
            return source.Select((x, i) => new IndexedItem<T>(x, i));
        }
    }
}