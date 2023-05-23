using System;
using System.Collections.Generic;

namespace LostPolygon.Unity.Utility {
    public static class CollectionExtensions {
        public static IReadOnlyList<T> ForEach<T>(this IReadOnlyList<T> source, Action<T> action) {
            for (int i = 0; i < source.Count; i++) {
                action(source[i]);
            }

            return source;
        }

        public static List<T> GetRange<T>(this IReadOnlyList<T> list, int index, int count) {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Must be non-negative");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Must be non-negative");

            if (list.Count - index < count)
                throw new ArgumentException("Invalid offset for given length");

            List<T> rangeList = new(count);
            for (int i = 0; i < count; i++) {
                rangeList.Add(list[index + i]);
            }

            return rangeList;
        }

        public static TValue? GetValueOrNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;

            return null;
        }
    }
}
