using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LostPolygon.Unity.Utility {
    public class CircleGestureDetector : MonoBehaviour {
        private readonly List<Vector2> _gesturePoints = new List<Vector2>();
        private int _gestureCount;

        [SerializeField]
        [Min(2)]
        private int _requiredGestureCount = 3;

        [SerializeField]
        private UnityEvent _circleGestureTriggered = new UnityEvent();

        public UnityEvent CircleGestureTriggered => _circleGestureTriggered;

        private void Update() {
            if (UpdateCircleGesture()) {
                _circleGestureTriggered.Invoke();
            }
        }

        private bool UpdateCircleGesture() {
            int pointsCount = _gesturePoints.Count;

            if (Input.GetMouseButton(0)) {
                Vector2 mousePosition = Input.mousePosition;
                if (pointsCount == 0 || (mousePosition - _gesturePoints[pointsCount - 1]).magnitude > 10) {
                    _gesturePoints.Add(mousePosition);
                    pointsCount++;
                }
            } else if (Input.GetMouseButtonUp(0)) {
                pointsCount = 0;
                _gestureCount = 0;
                _gesturePoints.Clear();
            }

            if (pointsCount < 10)
                return false;

            float finalDeltaLength = 0;

            Vector2 finalDelta = Vector2.zero;
            Vector2 previousPointsDelta = Vector2.zero;

            for (int i = 0; i < pointsCount - 2; i++) {
                Vector2 pointsDelta = _gesturePoints[i + 1] - _gesturePoints[i];
                finalDelta += pointsDelta;

                float pointsDeltaLength = pointsDelta.magnitude;
                finalDeltaLength += pointsDeltaLength;

                float dotProduct = Vector2.Dot(pointsDelta, previousPointsDelta);
                if (dotProduct < 0f) {
                    _gesturePoints.Clear();
                    _gestureCount = 0;
                    return false;
                }

                previousPointsDelta = pointsDelta;
            }

            bool result = false;
            int gestureBase = (Screen.width + Screen.height) / 4;

            if (finalDeltaLength > gestureBase && finalDelta.magnitude < gestureBase / 2f) {
                _gesturePoints.Clear();
                _gestureCount++;

                if (_gestureCount >= _requiredGestureCount) {
                    _gestureCount = 0;
                    result = true;
                }
            }

            return result;
        }
    }
}
