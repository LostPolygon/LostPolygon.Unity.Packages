using UnityEngine;

namespace LostPolygon.Unity.Log4net {
    /// <summary>
    /// Detects whether we are running inside Editor in a thread-safe way.
    /// </summary>
    internal static class IsEditorProvider {
        public static bool IsEditor { get; }

        static IsEditorProvider() {
            IsEditor = Application.isEditor;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init() {
        }
    }
}
