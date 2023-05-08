namespace LostPolygon.Unity.Preferences {
    public class IntPreference : Preference<int> {
        public IntPreference(IPreferencesRepository preferencesRepository, string key, int defaultValue)
            : base(preferencesRepository, key, defaultValue) {
        }

        protected override int PersistentValue {
            get => PreferencesRepository.GetIntPreference(Key, DefaultValue);
            set => PreferencesRepository.SetIntPreference(Key, value);
        }
    }
}
