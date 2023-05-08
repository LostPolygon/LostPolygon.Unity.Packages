namespace LostPolygon.Unity.Preferences {
    public class StringPreference : Preference<string> {
        public StringPreference(IPreferencesRepository preferencesRepository, string key, string defaultValue)
            : base(preferencesRepository, key, defaultValue) {
        }

        protected override string PersistentValue {
            get => PreferencesRepository.GetStringPreference(Key, DefaultValue);
            set => PreferencesRepository.SetStringPreference(Key, value);
        }
    }
}
