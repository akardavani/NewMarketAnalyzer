using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public static class IQueryableExtensions
    {
        public static async Task<IReadOnlyCollection<TSource>> ToReadOnlyCollectionAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            var list = new List<TSource>();
            await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                list.Add(element);
            }

            return list;
        }

        public static async Task<IReadOnlyDictionary<TKey, TElement>> ToReadOnlyDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken cancellationToken = default)
        {
            var dictionary = new Dictionary<TKey, TElement>();
            await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                dictionary.Add(keySelector(element), elementSelector(element));
            }

            return dictionary;
        }

        private static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source)
        {
            if (source is IAsyncEnumerable<TSource> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            var msg = $"The source 'IQueryable' doesn't implement 'IAsyncEnumerable<{typeof(TSource)}>'. Only sources that implement 'IAsyncEnumerable' can be used for Entity Framework asynchronous operations.";
            throw new InvalidOperationException(msg);
        }
    }
}
