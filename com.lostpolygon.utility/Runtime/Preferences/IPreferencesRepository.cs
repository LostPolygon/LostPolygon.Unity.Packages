namespace LostPolygon.Unity.Preferences {
    public interface IPreferencesRepository {
        string GetStringPreference(string key, string defaultValue);
        int GetIntPreference(string key, int defaultValue);
        void SetIntPreference(string key, int value);
        void SetStringPreference(string key, string value);
    }
}
