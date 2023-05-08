using System;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace LostPolygon.Unity.Utility {
    public class UnityGlobalEvents : SingletonMonoBehaviour<UnityGlobalEvents> {
        private Rect? _lastScreenSafeArea;
        private Vector2Int? _lastScreenSize;

        public event Action<bool> ApplicationPauseStatusChanged;

        public event Action<Rect> ScreenSafeAreaChanged;
        public event Action<Vector2Int> ScreenSizeChanged;
        public event Action ScreenGeometryChanged;

        public Rect ScreenSafeArea => _lastScreenSafeArea!.Value;
        public Vector2Int ScreenSize => _lastScreenSize!.Value;

        private void OnApplicationPause(bool pauseStatus) {
            ApplicationPauseStatusChanged?.Invoke(pauseStatus);
        }

        protected override void Awake() {
            base.Awake();
            RefreshScreen();
        }

        private void Update() {
            RefreshScreen();
        }

        private void OnDisable() {
            _lastScreenSafeArea = null;
            _lastScreenSize = null;
        }

        private void RefreshScreen() {
            Rect safeArea = Screen.safeArea;
            Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);

            if (safeArea != _lastScreenSafeArea) {
                _lastScreenSafeArea = safeArea;

                ScreenSafeAreaChanged?.Invoke(safeArea);
                ScreenGeometryChanged?.Invoke();
            }

            if (screenSize != _lastScreenSize) {
                _lastScreenSize = screenSize;

                ScreenSizeChanged?.Invoke(screenSize);
                ScreenGeometryChanged?.Invoke();
            }
        }
    }
}
