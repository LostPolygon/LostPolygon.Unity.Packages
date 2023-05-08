using System;
using System.Collections.Generic;
using System.Linq;

namespace LostPolygon.Unity.Utility {
    public static class LinqExtensions {
        public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : struct {
            foreach (TSource item in source) {
                if (predicate(item))
                    return item;
            }

            return null;
        }

        public static bool IsUnique<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> predicate) {
            IEnumerable<TSource> enumerable = source as TSource[] ?? source.ToArray();
            return enumerable.GroupBy(predicate, (key, group) => group.First()).Count() == enumerable.Count();
        }
    }
}
