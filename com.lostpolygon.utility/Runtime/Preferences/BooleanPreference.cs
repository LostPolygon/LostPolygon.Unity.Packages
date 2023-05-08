namespace LostPolygon.Unity.Preferences {
    public class BooleanPreference : Preference<bool> {
        public BooleanPreference(IPreferencesRepository preferencesRepository, string key, bool defaultValue)
            : base(preferencesRepository, key, defaultValue) {
        }

        protected override bool PersistentValue {
            get => PreferencesRepository.GetIntPreference(Key, DefaultValue ? 1 : 0) != 0;
            set => PreferencesRepository.SetIntPreference(Key, value ? 1 : 0);
        }
    }
}
