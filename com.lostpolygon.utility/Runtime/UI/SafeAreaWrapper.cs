using UnityEngine;

namespace LostPolygon.Unity.Utility {
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaWrapper : MonoBehaviour {
        [SerializeField]
        private RectTransform _panel;

        [SerializeField]
        private bool _ignoreX;

        [SerializeField]
        private bool _ignoreY;

        private void OnEnable() {
            UnityGlobalEvents.Instance.ScreenGeometryChanged += ApplySafeArea;
            ApplySafeArea();
        }

        private void OnDisable() {
            UnityGlobalEvents.Instance.ScreenGeometryChanged -= ApplySafeArea;
        }

        private void ApplySafeArea() {
            ApplySafeArea(Screen.safeArea, new Vector2Int(Screen.width, Screen.height));
        }

        private void ApplySafeArea(Rect safeArea, Vector2Int screenSize) {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= screenSize.x;
            anchorMax.x /= screenSize.x;
            anchorMin.y /= screenSize.y;
            anchorMax.y /= screenSize.y;

            _panel.anchorMin = new Vector2(_ignoreX ? _panel.anchorMin.x : anchorMin.x, _ignoreY ? _panel.anchorMin.y : anchorMin.y);
            _panel.anchorMax = new Vector2(_ignoreX ? _panel.anchorMax.x : anchorMax.x, _ignoreY ? _panel.anchorMax.y : anchorMax.y);
        }

        private void Reset() {
            _panel = GetComponent<RectTransform>();
        }
    }
}
