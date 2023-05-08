
namespace LostPolygon.Unity.Preferences {
    /// <summary>
    /// A persistent preference that attempts to use the backend repository as less as possible
    /// by implementing value cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Preference<T> {
        public string Key { get; }
        public T DefaultValue{ get; }
        protected IPreferencesRepository PreferencesRepository { get; }

        private T _value;
        private bool _valueSet;

        protected Preference(IPreferencesRepository preferencesRepository, string key, T defaultValue) {
            PreferencesRepository = preferencesRepository;
            Key = key;
            DefaultValue = defaultValue;
        }

        protected abstract T PersistentValue { get; set; }

        public T Value {
            get {
                if (!_valueSet) {
                    _value = PersistentValue;
                    _valueSet = true;
                }

                return _value;
            }
            set {
                if (Equals(_value, value))
                    return;

                _value = value;
                _valueSet = true;
                PersistentValue = value;
            }
        }

        public static implicit operator T(Preference<T> pref) {
            return pref.Value;
        }
    }
}
