using UnityEngine;
using UnityEngine.Scripting;

namespace LostPolygon.Unity.Preferences {
    [Preserve]
    public class PlayerPrefsPreferencesRepository : IPreferencesRepository {
        public string GetStringPreference(string key, string defaultValue) {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public int GetIntPreference(string key, int defaultValue) {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetIntPreference(string key, int value) {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public void SetStringPreference(string key, string value) {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }
}
