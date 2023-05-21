using System.Collections.Generic;
using System.Linq;

namespace LostPolygon.Unity.Utility {
    public static class DictionaryUtility {
        public static DictionaryDiffResult<TKey, TValue> CalculateDiff<TKey, TValue>(
            IDictionary<TKey, TValue> dictionaryNew,
            IDictionary<TKey, TValue> dictionaryOld,
            IEqualityComparer<KeyValuePair<TKey, TValue>> comparer = null
        ) {
            return new DictionaryDiffResult<TKey, TValue>(
                dictionaryNew
                    .Except(dictionaryOld, comparer)
                    .ToList(),
                dictionaryOld
                    .Except(dictionaryNew, comparer)
                    .ToList(),
                dictionaryNew
                    .Intersect(dictionaryOld, comparer)
                    .ToList()
            );
        }
    }

    public readonly struct DictionaryDiffResult<TKey, TValue> {
        public readonly List<KeyValuePair<TKey, TValue>> Added;
        public readonly List<KeyValuePair<TKey, TValue>> Removed;
        public readonly List<KeyValuePair<TKey, TValue>> Unchanged;

        public DictionaryDiffResult(
            List<KeyValuePair<TKey, TValue>> added,
            List<KeyValuePair<TKey, TValue>> removed,
            List<KeyValuePair<TKey, TValue>> unchanged
        ) {
            Added = added;
            Removed = removed;
            Unchanged = unchanged;
        }

        public void Deconstruct(
            out List<KeyValuePair<TKey, TValue>> added,
            out List<KeyValuePair<TKey, TValue>> removed,
            out List<KeyValuePair<TKey, TValue>> unchanged
        ) => (added, removed, unchanged) = (Added, Removed, Unchanged);
    }
}
