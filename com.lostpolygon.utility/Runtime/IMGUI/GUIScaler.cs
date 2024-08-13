#if LP_UNITY_MODULE_IMGUI_ENABLED

using UnityEngine;

namespace LostPolygon.Unity.Utility {
    /// <summary>
    /// Usage:
    ///
    /// (optional) Call GUIScaler.Initialize() in Start(), Awake() or OnEnable() (only needed once)
    /// Call GUIScaler.Begin() at the top of your OnGUI() methods
    /// Call GUIScaler.End() at the bottom of your OnGUI() methods
    ///
    /// WARNING: If you don't match Begin() and End() strange things will happen.
    /// </summary>
    public static class GUIScaler {
        // 160 is the 100% scale Windows DPI.
        public const float BASE_SCALE = 96f;
        private static bool initialized = false;
        private static bool scaling = false;
        private static Vector3 guiScale = Vector3.one;
        private static Matrix4x4 restoreMatrix = Matrix4x4.identity;

        public static float Scale => scaling ? guiScale.x : 1f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ReloadDomain() {
            initialized = false;
        }

        /// <summary>
        /// Initialize the gui scaler with a specific scale.
        /// </summary>
        public static void Initialize(float scale) {
            //if (initialized) return;
            initialized = true;

            // scale will be 0 on platforms that have unknown dpi (usually non-mobile)
            // if the scale is less than 10% don't bother, it just makes gui look bad.
            if (scale is 0 or < 1f) return;

            guiScale.Set(scale, scale, scale);
            scaling = true;
        }

        /// <summary>
        /// Initialize the gui scaler using the detected screen dpi.
        /// </summary>
        public static void Initialize() {
            Initialize(CalculateScale());
        }

        public static float CalculateScale() {
            return Screen.dpi / BASE_SCALE;
        }

        /// <summary>
        /// All gui elements drawn after this
        /// will be scaled.
        /// </summary>
        public static void Begin() {
            if (!initialized) Initialize();

            if (!scaling) return;

            restoreMatrix = GUI.matrix;

            GUI.matrix = GUI.matrix * Matrix4x4.Scale(guiScale);
        }

        /// <summary>
        /// Restores the default gui scale.
        /// All gui elements drawn after this
        /// will not be scaled.
        /// </summary>
        public static void End() {
            if (!scaling) return;

            GUI.matrix = restoreMatrix;
        }
    }
}

#endif
