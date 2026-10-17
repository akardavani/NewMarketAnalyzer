using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class IEnumerableExtensions
    {
        public static IReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
        {
            return  source.ToList();
        }

        public static IEnumerable<TResult> Lag<TSource, TResult>(this IEnumerable<TSource> source, Func<TResult, TSource, TResult> selector)
        {
            var previousItem = default(TResult);

            foreach (var item in source)
            {
                var current = selector(previousItem, item);
                yield return current;
                previousItem = current;
            }
        }

        public static IEnumerable<TSource> Lag<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> selector)
        {
            var previousItem = default(TSource);

            foreach (var item in source)
            {
                var current = selector(previousItem, item);
                yield return current;
                previousItem = current;
            }
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static T Next<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default;
        }

        public static T Previous<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source.Reverse())
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default;
        }

        public static T Next<T>(this Random rnd, IReadOnlyList<T> collection)
        {
            return collection[rnd.Next(0, collection.Count)];
        }
    }
}
