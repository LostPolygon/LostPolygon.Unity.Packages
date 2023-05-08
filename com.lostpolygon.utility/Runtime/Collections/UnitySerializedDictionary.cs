using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Pool;

namespace LostPolygon.Unity.Utility {
    /// <summary>
    /// A dictionary that can be serialized with Unity
    /// and supports extra key and value validation.
    /// </summary>
    [Serializable]
    public class UnitySerializedDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>,
        ISerializationCallbackReceiver {
        [SerializeField]
        private List<SerializableKeyValuePair<TKey, TValue>> _serializedKeyValuePairs = new();

        public UnitySerializedDictionary() {
        }

        protected UnitySerializedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) {
        }

        protected UnitySerializedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) {
        }

        protected UnitySerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) {
        }

        protected UnitySerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer) {
        }

        protected UnitySerializedDictionary(IEqualityComparer<TKey> comparer) : base(comparer) {
        }

        protected UnitySerializedDictionary(int capacity) : base(capacity) {
        }

        protected UnitySerializedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) {
        }

        protected virtual bool AreValuesValidated => false;

        protected virtual bool AdditionalSerializationValidationPredicate(KeyValuePair<TKey, TValue> pair) {
            return true;
        }

        protected virtual bool AdditionalDeserializationValidationPredicate(KeyValuePair<TKey, TValue> pair) {
            return true;
        }

        private void ValidateValues(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
            List<KeyValuePair<TKey, TValue>> invalidKvs = ListPool<KeyValuePair<TKey, TValue>>.Get();
            invalidKvs.Clear();

            try {
                foreach (KeyValuePair<TKey, TValue> kv in this) {
                    if (!predicate(kv)) {
                        invalidKvs.Add(kv);
                    }
                }

                if (invalidKvs.Count == 0)
                    return;

                foreach (KeyValuePair<TKey, TValue> pair in invalidKvs) {
                    Remove(pair.Key);
                }
            } finally {
                ListPool<KeyValuePair<TKey, TValue>>.Release(invalidKvs);
            }
        }

        private bool SerializeValidationPredicate(KeyValuePair<TKey, TValue> pair) {
            if (pair.Key == null || (pair.Key != null && pair.Key.Equals(null)))
                return false;

            if ((AreValuesValidated && (pair.Value == null || (pair.Value != null && pair.Value.Equals(null)))) || !AdditionalSerializationValidationPredicate(pair))
                return false;

            return true;
        }

        private bool DeserializeValidationPredicate(KeyValuePair<TKey, TValue> pair) {
            if (ReferenceEquals(pair.Key, null) || pair.Key.Equals(null))
                return false;

            if ((AreValuesValidated && (ReferenceEquals(pair.Value, null) || pair.Value.Equals(null))) || !AdditionalDeserializationValidationPredicate(pair))
                return false;

            return true;
        }

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() {
            _serializedKeyValuePairs.Clear();

            ValidateValues(SerializeValidationPredicate);

            foreach (KeyValuePair<TKey, TValue> pair in this) {
                _serializedKeyValuePairs.Add(new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
        }

        public void OnAfterDeserialize() {
            Clear();

            for (int i = 0; i < _serializedKeyValuePairs.Count; i++) {
                if (!DeserializeValidationPredicate(new KeyValuePair<TKey, TValue>(_serializedKeyValuePairs[i].Key, _serializedKeyValuePairs[i].Value)))
                    continue;

                Add(_serializedKeyValuePairs[i].Key, _serializedKeyValuePairs[i].Value);
            }

            _serializedKeyValuePairs.Clear();
        }

        [OnSerializing]
        internal void OnBeforeSerialize(StreamingContext context) {
            ValidateValues(SerializeValidationPredicate);
        }

        [OnDeserialized]
        internal void OnAfterDeserialize(StreamingContext context) {
            ValidateValues(DeserializeValidationPredicate);
        }

        #endregion
    }
}
