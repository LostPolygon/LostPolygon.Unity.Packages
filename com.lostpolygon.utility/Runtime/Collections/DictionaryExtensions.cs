using System.Collections.Generic;
using System.Linq;

namespace LostPolygon.Unity.Utility {
    public static class DictionaryExtensions {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue
        ) {
            if (dictionary == null)
                return defaultValue;

            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        /// <returns>true if dictionaries have equal contents, false otherwise</returns>
        public static bool Equal<TKey, TValue>(this IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2) {
            if (ReferenceEquals(dict1, dict2)) return true;
            if (dict1 == null || dict2 == null) return false;
            if (dict1.Count != dict2.Count) return false;

            EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
            foreach (KeyValuePair<TKey, TValue> kvp in dict1) {
                if (!dict2.TryGetValue(kvp.Key, out TValue value2)) return false;
                if (!valueComparer.Equals(kvp.Value, value2)) return false;
            }

            return true;
        }

        public static int GetItemsHashCode<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null)
                return 0;

            int hashCode = 0;
            foreach (KeyValuePair<TKey, TValue> pair in dictionary.OrderBy(pair => pair.Key.GetHashCode())) {
                hashCode ^= pair.Key.GetHashCode();
                hashCode ^= pair.Value != null ? pair.Value.GetHashCode() : 0;
            }

            return hashCode;
        }
    }
}
